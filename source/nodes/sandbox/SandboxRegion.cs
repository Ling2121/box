using System.Linq;
using System;
using Godot;
using System.Collections.Generic;

namespace Box {
    public enum SandboxRegionStatus {
        Loading,
        Stop,
        Unload,
        NotLoad,
    }

    public class SandboxRegion : Node2D,IStorage {
        public int X {get;protected set;}
        public int Y {get;protected set;}
        public int TileOriginX {get;protected set;}
        public int TileOriginY {get;protected set;}
        public int TileMaxX {get;protected set;}
        public int TileMaxY {get;protected set;}
        public SandboxRegionStatus Status = SandboxRegionStatus.NotLoad;
        public long UnloadTimestamp = 0;
        public int IndexCount = 0;
        public Dictionary<Node,Node> Objects {get;protected set;} = new Dictionary<Node, Node>();
        
        public Dictionary<SandboxLayer,int[,]> Layers = new Dictionary<SandboxLayer, int[,]>();
        public NumberIndexPool IndexPool = new NumberIndexPool();

        public Sandbox Sandbox;

        public SandboxRegion() : this(null,0,0) {}

        public SandboxRegion(Sandbox sandbox,int x,int y) {
            X = x;
            Y = y;
            TileOriginX = x * Sandbox.REGION_SIZE;
            TileOriginY = y * Sandbox.REGION_SIZE;
            TileMaxX = TileOriginX + Sandbox.REGION_SIZE;
            TileMaxY = TileOriginY + Sandbox.REGION_SIZE;
            Sandbox = sandbox;

            Layers[SandboxLayer.Land] = new int[Sandbox.REGION_SIZE,Sandbox.REGION_SIZE];
            Layers[SandboxLayer.Wall] = new int[Sandbox.REGION_SIZE,Sandbox.REGION_SIZE];

            for(y = 0;y < Sandbox.REGION_SIZE;y++) {
                for(x = 0;x < Sandbox.REGION_SIZE;x++) {
                    Layers[SandboxLayer.Land][x,y] = -1;
                    Layers[SandboxLayer.Wall][x,y] = -1;
                }
            }
        }

        public void SetCell(SandboxLayer layer,int x,int y,string tile_name) {
            if(x < 0 || x >= Sandbox.REGION_SIZE || y < 0 || y >= Sandbox.REGION_SIZE) return;
            int[,] lay = Layers[layer];
            int wx = TileOriginX + x;
            int wy = TileOriginY + y;
            TileMap map = Sandbox.LayerTileMaps[layer];
            if(tile_name != "") {
                int id = IndexPool.GetIndex(tile_name);
                lay[x,y] = id;
                int tile_id = map.TileSet.FindTileByName(tile_name);
                map.CallDeferred("set_cell",wx,wy,tile_id);
            } else {
                lay[x,y] = -1;
                map.CallDeferred("set_cell",wx,wy,-1);
            }
        }

        public void StorageWrite(StorageFile file) {
            file.Write(X);
            file.Write(Y);
            file.Write(IndexPool.IndexHash.Count);
            foreach(string key in IndexPool.IndexHash.Keys.ToArray()) {
                file.Write(key);
                file.Write(IndexPool.IndexHash[key]);
            }
            int[,] land_layer = Layers[SandboxLayer.Land];
            int[,] wall_layer = Layers[SandboxLayer.Wall];
            for(int y = 0;y < Sandbox.REGION_SIZE;y++) {
                for(int x = 0;x < Sandbox.REGION_SIZE;x++) {
                    file.Store32((uint)land_layer[x,y]);
                }
            }
            for(int y = 0;y < Sandbox.REGION_SIZE;y++) {
                for(int x = 0;x < Sandbox.REGION_SIZE;x++) {
                    file.Store32((uint)wall_layer[x,y]);
                }
            }
            // foreach(Node obj in region.Objects.Values) {
                //     /*对象多时需要集成到一个文件中保存*/
                //     if(obj is IStorage) {
                //         RegisterAttribute reg_info = obj.GetType().GetCustomAttribute<RegisterAttribute>();
                //         if(reg_info != null) {
                //             string obj_file_name = $"{OBJECTS_DIRECTORY_PATH}/{obj.Name}.odat";
                //             StorageFile obj_file = new StorageFile();
                //             obj_file.Write(obj.Name);
                //             if(obj is IEntity) {
                //                 obj_file.Write((int)Register.RegisterType.Entity);
                //             }
                //             else if(obj is IItem) {
                //                 obj_file.Write((int)Register.RegisterType.Item);
                //             }
                //             else if(obj is IBlock) {
                //                 obj_file.Write((int)Register.RegisterType.Block);
                //             }
                //             obj_file.Write(reg_info.RegisterName);
                //             ((IStorage)obj).StorageWrite(obj_file);
                //             obj_file.Close();

                //             file.Write(obj.Name);
                //         }
                //     }
                // }
        }
        public void StorageRead(StorageFile file) {
            X = file.ReadInt();
            Y = file.ReadInt();
            int len = file.ReadInt();
            for(int i = 0;i<len;i++) {
                string key = file.ReadString();
                int index = file.ReadInt();
                IndexPool.SetIndex(key,index);
            }
            for(int y = 0;y < Sandbox.REGION_SIZE;y++) {
                for(int x = 0;x < Sandbox.REGION_SIZE;x++) {
                    int id = (int)file.Get32();
                    if(id != -1) {
                        string tile_name = IndexPool.GetKey(id);
                        SetCell(SandboxLayer.Land,x,y,tile_name);
                    }
                }
            }
            for(int y = 0;y < Sandbox.REGION_SIZE;y++) {
                for(int x = 0;x < Sandbox.REGION_SIZE;x++) {
                    int id = (int)file.Get32();
                    if(id != -1) {
                        string tile_name = IndexPool.GetKey(id);
                        SetCell(SandboxLayer.Wall,x,y,tile_name);
                    }
                }
            }
            // while(file.GetPosition() < file.GetLen()) {
                //     string obj_name = file.ReadString();
                //     string obj_file_name = $"{OBJECTS_DIRECTORY_PATH}/{obj_name}.odat";
                //     StorageFile obj_file = new StorageFile();
                //     obj_file.Open(obj_file_name,File.ModeFlags.Read);
                //     if(obj_file.IsOpen()){
                //         Register.RegisterType type_label = (Register.RegisterType)obj_file.ReadInt();
                //         string reg_name = obj_file.ReadString();
                //         Node node = null;
                //         Register register = Register.Instance;
                //         switch(type_label) {
                //             case Register.RegisterType.Entity: {
                //                 node = register.CreateEntity(reg_name);
                //             }break;
                //             case Register.RegisterType.Item: {
                //                 node = register.CreateItem(reg_name);
                //             }break;
                //             case Register.RegisterType.Block: {
                //                 node = register.CreateBlock(reg_name);
                //             }break;
                //         }
                //         if(node != null) {
                //             if(node is IStorage) {
                //                 ((IStorage)node).StorageRead(obj_file);
                //                 region.Objects[node] = node;
                //             }
                //         }
                //     }
                //     obj_file.Close();
                // }
        }

        public void _Load(Sandbox sandbox) {
            Status = SandboxRegionStatus.Loading;
        
            #if BOX_DEBUG
                
                try {
                    sandbox.Archive.ReadRegion(this);
                } catch (Exception e) {
                    GD.Print(e);
                }
                
                
                // string[] namelist = {"grass","sand","stone","water"};
                // for(int y = 0;y < Sandbox.REGION_SIZE;y++) {
                //     for(int x = 0;x < Sandbox.REGION_SIZE;x++) {
                //         SetCell(SandboxLayer.Land,x,y,namelist[(int)GD.RandRange(0,namelist.Length)]);
                //     }
                // }
                // _Save(sandbox);

            #endif
        }

        public void _Unload(Sandbox sandbox) {
            //_Save(sandbox);
            Status = SandboxRegionStatus.Unload;

            #if BOX_DEBUG
                //GD.Print($"卸载区块({X}:{Y})");
                for(int y = 0;y < Sandbox.REGION_SIZE;y++) {
                    for(int x = 0;x < Sandbox.REGION_SIZE;x++) {
                        SetCell(SandboxLayer.Land,x,y,"");
                    }
                }
            #endif
        }

        public void _Save(Sandbox sandbox) {
            try {
                sandbox.Archive.SaveRegion(this);
            } catch (Exception e) {
                GD.Print(e.StackTrace);
            }
        }
    }
}