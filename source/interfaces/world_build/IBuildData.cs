using System.Collections.Generic;
using Godot;

namespace Box {
    public interface IBuildData {
        ulong Seed {get;}
        int Width {get;}
        int Height  {get;}
        Dictionary<string,DataCanvas> Canvases  {get;}
        NumberIndexPool TileIndexPool {get;}
        RandomNumberGenerator Random {get;}
    }
}