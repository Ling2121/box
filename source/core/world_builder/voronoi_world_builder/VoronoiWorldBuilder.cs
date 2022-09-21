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
        public int Height;//高度
        public int Humidity;//湿度
        public int Temperature;//温度
        public bool IsInland = false;//是否是内陆
        public bool IsCoastline = false;//是否是海岸线
        public bool IsFoot = false;//是否在山脚下
        public bool IsLake = false;//是否是湖泊
        public bool IsLava = false;//是否是熔岩湖
        public bool IsRiverStart = false;//河流起始
        public bool IsRiverEnd = false;//河流终点
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

        public List<Cell> LandVoronoiCells = new List<Cell>();
        public List<Cell> WaterVoronoiCells = new List<Cell>();
        public List<Cell> MountainVoronoiCells = new List<Cell>();
        public List<Cell> LakeVoronoiCells = new List<Cell>();
        public List<Cell> LavaVoronoiCells = new List<Cell>();
        public Dictionary<long,VoronoiCellBuildData> VoronoiCellDatas = new Dictionary<long, VoronoiCellBuildData>();

        public List<List<Edge>> RiverVoronoiEdges = new List<List<Edge>>();

        

        public int PointGenerateNumber;

        public byte LandHeight = 230;
        public byte MountainHeight = 230;

        public int LakeGenerateProbability;
        public int LakeGenerateTry;
        public Interval<int> LakeSizeInterval;

        public int LavaGenerateProbability;
        public int LavaGenerateTry;
        public Interval<int> LavaSizeInterval;

        public int RiverGenerateProbability;
        public int RiverGenerateTry;
        public Interval<int> RiverSizeInterval;
    }

    public class VoronoiWorldBuilder : WorldBuilder<VoronoiWorldBuilderData> {
        public VoronoiWorldBuilder() {
            AddProcess("Voronoi",new VoronoiBuildProcess());
            AddProcess("Terrain",new TerrainBuildProcess());
        }
    }
}