using Godot;
using System;
using System.Collections.Generic;
using Box.VoronoiMap;

namespace Box.WorldBuilds.VoronoiPort {

    public class VoronoiWorldBuilderData : IBuildData {
        public int Width {get;set;}
        public int Height  {get;set;}
        public List<DataCanvas> Canvases  {get;} = new List<DataCanvas>();
        public NumberIndexPool TileIndexPool {get;} = new NumberIndexPool();
        public RandomNumberGenerator Random {get;} = new RandomNumberGenerator();
        
        //生成配置
        //用个屁英文
        public Voronoi Voronoi; 
        public int 顶点数生成数;
        public int 湖泊生成几率;
        public int 湖泊生成尝试数;
        public int 火山湖泊生成几率;
        public int 火山湖泊生成尝试数;
    }

    public class VoronoiWorldBuilder : WorldBuilder<VoronoiWorldBuilderData> {
        
    }
}