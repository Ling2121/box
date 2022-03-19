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

    public class SandboxRegion : Node2D {
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

        public SandboxRegion() : this(0,0) {}

        public SandboxRegion(int x,int y) {
            X = x;
            Y = y;
            TileOriginX = x * Sandbox.REGION_SIZE;
            TileOriginY = y * Sandbox.REGION_SIZE;
            TileMaxX = TileOriginX + Sandbox.REGION_SIZE;
            TileMaxY = TileOriginY + Sandbox.REGION_SIZE;
        }

        public void _Load(Sandbox sandbox) {
            Status = SandboxRegionStatus.Loading;
            #if BOX_DEBUG
                //GD.Print($"加载区块({X}:{Y})");
                int grass_index = sandbox.TileMap.TileSet.FindTileByName("grass");
                for(int y = TileOriginY;y < TileMaxY;y++) {
                    for(int x = TileOriginX;x < TileMaxX;x++) {
                        sandbox.TileMap.CallDeferred("set_cell",x,y,grass_index);
                    }
                }
            #endif
        }

        public void _Unload(Sandbox sandbox) {
            _Save(sandbox);
            Status = SandboxRegionStatus.Unload;
            #if BOX_DEBUG
                GD.Print($"卸载区块({X}:{Y})");
                for(int y = TileOriginY;y < TileMaxY;y++) {
                    for(int x = TileOriginX;x < TileMaxX;x++) {
                        sandbox.TileMap.CallDeferred("set_cell",x,y,-1);
                    }
                }
            #endif
        }

        public void _Save(Sandbox sandbox) {

        }
    }
}