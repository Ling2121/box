using Godot;
using System;
using System.Collections.Generic;

namespace Box {
    public enum SandboxLayer {
        Land,
        Wall,
    }


    public interface ISandbox {
        int Width {get;}
        int Height {get;}
        Dictionary<int,Dictionary<int,ISandboxRegion>> Regions {get;}
        void SetCell(SandboxLayer layer,int x,int y,string tile_name);
        void CellBindBlock(SandboxLayer layer,int x,int y);
        IBlock GetCellBindBlock(SandboxLayer layer,int x,int y);
        ISandboxRegion GetRegion(int rx,int ry);
        T GetRegion<T>(int rx,int ry) where T : ISandboxRegion;
        void AddRegion(ISandboxRegion region);
        void RemoveRegion(ISandboxRegion region);
    }   
}