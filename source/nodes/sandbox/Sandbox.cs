using System.Linq;
using System;
using System.Threading.Tasks;
using System.Collections;
using Godot;
using System.Collections.Generic;
using System.Collections.Concurrent;

namespace Box {
    [ClassName("Sandbox")]
    public class Sandbox : YSort,ISandbox {
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

        [Export]
        public int Width {get;set;}
        [Export]
        public int Height {get;set;}
        [Export]
        public TileSet TileSet;

        //已加载的区块
        public Dictionary<int,Dictionary<int,ISandboxRegion>> Regions {get;} = new Dictionary<int, Dictionary<int, ISandboxRegion>>();

        protected bool region_thread_run = true;
        //区块加载线程
        protected Task region_load_thread;
        //区块卸载线程
        protected Task region_unload_thread;
        //区块加载队列
        public ConcurrentQueue<(int x,int y)> RegionLoadInstructQueue {get;protected set;} = new ConcurrentQueue<(int x,int y)>();
        //区块卸载队列
        public ConcurrentQueue<(int x,int y)> RegionUnloadInstructQueue {get;protected set;} = new ConcurrentQueue<(int x,int y)>();
        public TileMap LandTileMap {get {
            return LayerTileMaps[SandboxLayer.Land];
        }}
        public TileMap WallTileMap {get {
            return LayerTileMaps[SandboxLayer.Wall];
        }}
        public Dictionary<SandboxLayer,TileMap> LayerTileMaps = new Dictionary<SandboxLayer, TileMap>();
        //地图加载器
        public SanboxLoader SanboxLoader; 
    

        public Sandbox():this(512,512) {}

        public Sandbox(int width,int height) {
            Width = width;
            Height = height;
        }
        public override void _EnterTree()
        {
            Game.Instance.Sandbox = this;
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
                        SandboxRegion region = GetRegion<SandboxRegion>(r.rx,r.ry);
                        SanboxLoader.LoadRegion(region);
                        region.IndexCount ++;
                    } else {
                        if (status == SandboxRegionStatus.Loading) {
                            SandboxRegion region = Regions[r.ry][r.rx] as SandboxRegion;
                            region.IndexCount ++;
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
                    SandboxRegion region = GetRegion<SandboxRegion>(r.rx,r.ry);
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

        public (SandboxRegion,int,int) CellOfLocal(int world_x,int world_y) {
            int region_x = Mathf.FloorToInt(world_x / REGION_SIZE);
            int region_y = Mathf.FloorToInt(world_y / REGION_SIZE);
            SandboxRegion region = GetRegion<SandboxRegion>(region_x,region_y);
            int region_cell_x_local = world_x - region.TileOriginX;
            int region_cell_y_local = world_y - region.TileOriginY;

            return (region,region_cell_x_local,region_cell_y_local);
        }

        public void SetCell(SandboxLayer layer,int x,int y,string tile_name) {
            TileMap map = LayerTileMaps[layer];
            int tile_id = map.TileSet.FindTileByName(tile_name);
            (SandboxRegion region,int region_cell_x,int region_cell_y) = CellOfLocal(x,y);
            
            map.CallDeferred("set_cell",x,y,tile_id);
            region.Layers[layer][region_cell_x,region_cell_y] = region.IndexPool.GetIndex(tile_name);
            CellBindBlock(layer,x,y);
        }

        public IBlock GetCellBindBlock(SandboxLayer layer,int x,int y) {
            (SandboxRegion region,int region_cell_x,int region_cell_y) = CellOfLocal(x,y);
            int index = region_cell_y * Sandbox.REGION_SIZE + region_cell_x;
            return region.CellBindBlocks[index];
        }

        public void CellBindBlock(SandboxLayer layer,int x,int y) {
            (SandboxRegion region,int region_cell_x,int region_cell_y) = CellOfLocal(x,y);
            TileMap map = LayerTileMaps[layer];

            int cell = map.GetCell(x,y);
            if(cell == TileMap.InvalidCell) return;
            string name = map.TileSet.TileGetName(cell);
            int index = region_cell_y * Sandbox.REGION_SIZE + region_cell_x;
            
            if(region.CellBindBlocks.ContainsKey(index)) {
                IBlock old_block = region.CellBindBlocks[index];
                if(old_block.IsAddToSandbox()) {
                    RemoveChild(old_block as Node);
                }
                old_block._CellUnbind();
                region.CellBindBlocks.Remove(index);
            }
            Node block_node =  Register.Instance.CreateTileBindBlock(name);
            if(block_node != null) {
                IBlock block = block_node as IBlock;
                if(block.IsAddToSandbox()){
                    block.X = x;
                    block.Y = y;
                    AddChild(block_node);
                }
                region.CellBindBlocks[index] = block;
                block._CellBind();
            }
        }

        public ISandboxRegion GetRegion(int rx,int ry) {
            if(!Regions.ContainsKey(ry)) {
                Regions[ry] = new Dictionary<int, ISandboxRegion>();
            }
            if(!Regions[ry].ContainsKey(rx)) { 
                Regions[ry][rx] = new SandboxRegion(this,rx,ry);
            }
            return Regions[ry][rx];
        }

        public T GetRegion<T>(int rx,int ry) where T : ISandboxRegion {
            ISandboxRegion region = GetRegion(rx,ry);
            return region != null?(T)region : default(T);
        }

        public SandboxRegionStatus GetRegionStatus(int rx,int ry) {
            if(!Regions.ContainsKey(ry)){ return SandboxRegionStatus.NotLoad; }
            if(!Regions[ry].ContainsKey(rx)){ return SandboxRegionStatus.NotLoad; }
            SandboxRegion region = GetRegion<SandboxRegion>(rx,ry);
            return region.Status;
        }

        public void AddRegion(ISandboxRegion region) {

        }

        public void RemoveRegion(ISandboxRegion region) {
            for(int y = region.TileOriginY;y < region.TileMaxY;y++) {
                for(int x = region.TileOriginX;x < region.TileMaxX;x++) {
                    LayerTileMaps[SandboxLayer.Land].CallDeferred("set_cell",x,y,TileMap.InvalidCell);
                    LayerTileMaps[SandboxLayer.Wall].CallDeferred("set_cell",x,y,TileMap.InvalidCell);
                }
            }
        }

        public SandboxRegion RemoveRegion(int rx,int ry) {
            if(!Regions.ContainsKey(ry)) {
                Regions[ry] = new Dictionary<int, ISandboxRegion>();
            }
            if(Regions[ry].ContainsKey(rx)) {
                SandboxRegion region = GetRegion<SandboxRegion>(rx,ry);
                Regions[ry].Remove(rx);

                RemoveRegion(region);

                return region;

            }
            return null;
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