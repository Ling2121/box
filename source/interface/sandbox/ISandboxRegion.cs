using Godot;
using System;
using System.Collections.Generic;

namespace Box {
    public interface ISandboxRegion {
        int X {get;}
        int Y {get;}
        Dictionary<Node,Node> Objects {get;}
        Dictionary<SandboxLayer,int[,]> Layers {get;}
        Dictionary<int,IBlock> TileInstances {get;}
    }   
}