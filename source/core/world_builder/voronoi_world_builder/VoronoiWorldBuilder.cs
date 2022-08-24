using Godot;
using System;
using System.Collections.Generic;
using Box.VoronoiMap;

namespace Box.WorldBuilds.VoronoiPort {
    public enum VoronoiCellType {
        Land,
        Water,
        Mountain
    }

    
    public class VoronoiCellBuildData {
        public VoronoiCellType Type;
    }

    public class VoronoiWorldBuilderData : IBuildData {

        public ulong Seed {
            get {
                return Random.Seed;
            }
            set {
                Random.Seed = value;
                Noise.Seed = (int)value;
            }
        }
        public int Width {get;set;} = 512;
        public int Height  {get;set;} = 512;
        public Dictionary<string,DataCanvas> Canvases  {get;} = new Dictionary<string,DataCanvas>();
        public NumberIndexPool TileIndexPool {get;} = new NumberIndexPool();
        public RandomNumberGenerator Random {get;set;} = new RandomNumberGenerator();
        public NoiseGenerator Noise {get;set;} = new NoiseGenerator{
            Octaves = 3,
            Period = 120,
            Persistence = 0.6f,
            Lacunarity = 2.75f
        };

        //生成配置
        public Voronoi Voronoi; 

        public Dictionary<long,Cell> 陆地Voronoi细胞 = new Dictionary<long, Cell>();
        public Dictionary<long,Cell> 海洋Voronoi细胞 = new Dictionary<long, Cell>();
        public Dictionary<long,VoronoiCellBuildData> 细胞数据 = new Dictionary<long, VoronoiCellBuildData>();

        public int 顶点生成数;


        public byte 陆地高度 = 230;
        public byte 山脉高度 = 230;

        public int 湖泊生成几率;
        public int 湖泊生成尝试数;
        public Interval<int> 湖泊大小区间;

        public int 火山湖生成几率;
        public int 火山湖生成尝试数;
        public Interval<int> 火山湖大小区间;

        public int 河流生成几率;
        public int 河流生成尝试数;
        public Interval<int> 河流大小区间;
    }

    public class VoronoiWorldBuilder : WorldBuilder<VoronoiWorldBuilderData> {
        public VoronoiWorldBuilder() {
            AddProcess("Voronoi",new VoronoiBuildProcess());
            AddProcess("Terrain",new TerrainBuildProcess());
        }
    }
}