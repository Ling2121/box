using Godot;
using System;
using System.Collections.Generic;

namespace Box {
    public interface ISandboxRegion {
        public int X {get;}
        public int Y {get;}
        public int TileOriginX {get;}
        public int TileOriginY {get;}
        public int TileMaxX {get;}
        public int TileMaxY {get;}

        Dictionary<Node,Node> Objects {get;}
        Dictionary<SandboxLayer,int[,]> Layers {get;}
        Dictionary<int,IBlock> CellBindBlocks {get;}
    }   
}