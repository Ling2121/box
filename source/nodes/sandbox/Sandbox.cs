using System.Linq;
using System;
using System.Threading.Tasks;
using System.Collections;
using Godot;
using System.Collections.Generic;
using System.Collections.Concurrent;

namespace Box {
    public enum SandboxLayer {
        Land,
        Wall,
    }

    [ClassName("Sandbox")]
    public class Sandbox : YSort {
        public const int REGION_CELL_PIXEL_SIZE = 16;
        public const int REGION_SIZE = 16;
        public const int REGION_PIXEL_SIZE = REGION_CELL_PIXEL_SIZE * REGION_SIZE;

        public static (int,int) WorldToRegion(float x,float y) {
            return (Mathf.FloorToInt(x / REGION_PIXEL_SIZE),Mathf.FloorToInt(y / REGION_PIXEL_SIZE));
        }

        public static (int,int) WorldToRegion(Vector2 p) {
            return (Mathf.FloorToInt(p.x / REGION_PIXEL_SIZE),Mathf.FloorToInt(p.y / REGION_PIXEL_SIZE));
        }

        public static Vector2 RegionToWorld(float x,float y) {
            return new Vector2(x * REGION_PIXEL_SIZE,y * REGION_PIXEL_SIZE);
        }

        public static Vector2 RegionToWorld(Vector2 p) {
            return new Vector2(p.x * REGION_PIXEL_SIZE,p.y * REGION_PIXEL_SIZE);
        }

        protected bool region_thread_run = true;
        //区块加载线程
        protected Task region_load_thread;
        //区块卸载线程
        protected Task region_unload_thread;
        //区块加载队列
        public ConcurrentQueue<(int x,int y)> RegionLoadInstructQueue {get;protected set;} = new ConcurrentQueue<(int x,int y)>();
        //区块卸载队列
        public ConcurrentQueue<(int x,int y)> RegionUnloadInstructQueue {get;protected set;} = new ConcurrentQueue<(int x,int y)>();
        //已加载的区块
        public Dictionary<int,Dictionary<int,SandboxRegion>> Regions {get;protected set;} = new Dictionary<int, Dictionary<int, SandboxRegion>>();
        public TileMap LandTileMap {get {
            return LayerTileMaps[SandboxLayer.Land];
        }}
        public TileMap WallTileMap {get {
            return LayerTileMaps[SandboxLayer.Wall];
        }}
        public Dictionary<SandboxLayer,TileMap> LayerTileMaps = new Dictionary<SandboxLayer, TileMap>();
        //地图加载器
        public SanboxLoader SanboxLoader; 

        [Export]
        public int Width {get; protected set;}
        [Export]
        public int Height {get; protected set;}
        [Export]
        public TileSet TileSet;

        public Sandbox():this(512,512) {}

        public Sandbox(int width,int height) {
            Width = width;
            Height = height;
        }

        public void SetCell(SandboxLayer layer,int x,int y,string tile_name) {
            TileMap map = LayerTileMaps[layer];
            int tile_id = map.TileSet.FindTileByName(tile_name);
            map.CallDeferred("set_cell",x,y,tile_id);
            int region_x = Mathf.FloorToInt(x / REGION_SIZE);
            int region_y = Mathf.FloorToInt(y / REGION_SIZE);
            SandboxRegion region = Regions[region_y][region_x];
            int region_cell_x = x - region.TileOriginX;
            int region_cell_y = y - region.TileOriginY;
            region.Layers[layer][region_cell_x,region_cell_y] = region.IndexPool.GetIndex(tile_name);
        }

        public override void _EnterTree()
        {
            FileSystem.Instance.Init();
            Register.Instance.Init();
            Game.Instance.CurrentSandbox = this;
        }

        public override void _Ready()
        {
            base._Ready();

            foreach(Node node in GetChildren()) {
                if(node is SanboxLoader) {
                    SanboxLoader = (SanboxLoader)(node);
                    break;
                }
            }

            if(TileSet == null) {
                TileSet = GD.Load<TileSet>("res://resource/image/tilesets/default/default.tres");
            }

            TileMap land_tile_map = GetNodeOrNull<TileMap>("LandTileMap");
            TileMap wall_tile_map = GetNodeOrNull<TileMap>("WallTileMap");
            if(land_tile_map == null) {
                
                land_tile_map = new TileMap();
                land_tile_map.ZIndex = -100;
                land_tile_map.CellSize = new Vector2(REGION_CELL_PIXEL_SIZE,REGION_CELL_PIXEL_SIZE);
                land_tile_map.TileSet = TileSet;
                AddChild(land_tile_map);
            }

            if(wall_tile_map == null) {
                wall_tile_map = new TileMap();
                wall_tile_map.ZIndex = -99;
                wall_tile_map.CellSize = new Vector2(REGION_CELL_PIXEL_SIZE,REGION_CELL_PIXEL_SIZE);
                wall_tile_map.TileSet = TileSet;
                AddChild(wall_tile_map);
            }

            LayerTileMaps[SandboxLayer.Land] = land_tile_map;
            LayerTileMaps[SandboxLayer.Wall] = wall_tile_map;

            region_load_thread = Task.Run(_RegionLoadThread);
            region_unload_thread = Task.Run(_RegionUnLoadThread);
        }

        public override void _ExitTree()
        {
            region_thread_run = false;
            region_load_thread.Wait();
            region_unload_thread.Wait();
        }

        public void _RegionLoadThread() {
            GD.Print("区块加载线程启动");
            while(region_thread_run) {
                (int rx,int ry) r = (0,0);
                while(RegionLoadInstructQueue.TryDequeue(out r)) {
                    SandboxRegionStatus status = GetRegionStatus(r.rx,r.ry);
                    if(status != SandboxRegionStatus.Loading) {
                        //进行加载
                        SandboxRegion region = GetRegion(r.rx,r.ry);
                        SanboxLoader.LoadRegion(region);
                        region.IndexCount ++;
                    } else {
                        if (status == SandboxRegionStatus.Loading) {
                            Regions[r.ry][r.rx].IndexCount ++;
                        }
                    }
                }
            }
            GD.Print("区块加载线程关闭");
        }

        public void _RegionUnLoadThread() {
            GD.Print("区块卸载线程启动");
            while(region_thread_run) {
                (int rx,int ry) r = (0,0);
                while(RegionUnloadInstructQueue.TryDequeue(out r)) {
                    SandboxRegion region = GetRegion(r.rx,r.ry);
                    SandboxRegionStatus status = region.Status;
                    if(status == SandboxRegionStatus.Loading) {
                        region.IndexCount --;
                        if(region.IndexCount == 0) {
                            SanboxLoader.UnloadRegion(region);
                            RemoveRegion(region);
                        } else {
                            SanboxLoader.SaveRegion(region);
                        }
                    }
                }
            }
            GD.Print("区块卸载线程关闭");
        }


        public SandboxRegion GetRegion(int rx,int ry) {
            if(!Regions.ContainsKey(ry)) {
                Regions[ry] = new Dictionary<int, SandboxRegion>();
            }
            if(!Regions[ry].ContainsKey(rx)) {
                Regions[ry][rx] = new SandboxRegion(this,rx,ry);
            }
            return Regions[ry][rx];
        }

        public SandboxRegionStatus GetRegionStatus(int rx,int ry) {
            if(!Regions.ContainsKey(ry)){ return SandboxRegionStatus.NotLoad; }
            if(!Regions[ry].ContainsKey(rx)){ return SandboxRegionStatus.NotLoad; }
            return Regions[ry][rx].Status;
        }


        public SandboxRegion RemoveRegion(int rx,int ry) {
            if(!Regions.ContainsKey(ry)) {
                Regions[ry] = new Dictionary<int, SandboxRegion>();
            }
            if(Regions[ry].ContainsKey(rx)) {
                SandboxRegion region = Regions[ry][rx];
                Regions[ry].Remove(rx);

                for(int y = region.TileOriginY;y < region.TileMaxY;y++) {
                    for(int x = region.TileOriginX;x < region.TileMaxX;x++) {
                        LayerTileMaps[SandboxLayer.Land].CallDeferred("set_cell",x,y,TileMap.InvalidCell);
                        LayerTileMaps[SandboxLayer.Wall].CallDeferred("set_cell",x,y,TileMap.InvalidCell);
                    }
                }

                return region;

            }
            return null;
        }

        public void RemoveRegion(SandboxRegion region) {
            RemoveRegion(region.X,region.Y);
        }

        public override void _Draw()
        {
            // foreach(var a in Regions.Values.ToArray()) {
            //     if(a.Values.Count > 0) {
            //         var b = a.Values.ToArray<SandboxRegion>();
            //         foreach(var region in b) { 
            //             DrawRect(new Rect2(region.X * Sandbox.REGION_PIXEL_SIZE,region.Y * Sandbox.REGION_PIXEL_SIZE,Sandbox.REGION_PIXEL_SIZE,Sandbox.REGION_PIXEL_SIZE),Colors.Red,false);
            //         }
            //     }
            // }
        }
    }
}