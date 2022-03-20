using Godot;
using System;
using Box.DataCanvas;
using Box.DataCanvas.SystemDrawing;

namespace Box.WorldBuils.Default {
    /*
        世界生成流程
        1.生成Voronoi图
        2.生成地形图
        3.生成生态群落图
        4.对各个生态群落进行生成
    */

    public enum TopographicType {
        Land,
        Water,
    }

    public class BuildCellInfo {
        public TopographicType Type;
        //是否是当前TopographicType类型的边缘 
        public bool IsEdge;
        public string Biome;
    }

    public class DefaultWorldBuild : WorldBuild {

        public Table Table {get {return table;}}

        public Table table = new Table();
        public DefaultWorldBuild(Table setting) {
            //配置
            table
                .SetValueFrom<int>(setting,"地图宽度",512)
                .SetValueFrom<int>(setting,"地图高度",512)
                .SetValueFrom<ulong>(setting,"随机种子",233)
                ; 

            int width = table.GetValue<int>("地图宽度");
            int height = table.GetValue<int>("地图高度");

            table 
                .SetValue<RandomNumberGenerator>("随机数生成器",()=>{
                    ulong seed = table.GetValue<ulong>("随机种子");
                    RandomNumberGenerator random = new RandomNumberGenerator();
                    random.Seed = seed;
                    return random;
                })
                .SetValue<IDataCanvas<ushort>>("缓存画布",()=>{
                    return new DataCanvas16Bit(width,height);
                })
                .SetValue<IDataCanvas<ushort>>("世界画布",()=>{
                    return new DataCanvas16Bit(width,height);
                })
                ;

            
            
            table.SetValueFrom<Table>(setting,"Voronoi图生成设置",new Table())
            .GetValue<Table>("Voronoi图生成设置")
                .SetValueFromSelf<int>("顶点数",(int)(((width + height) / 2) * 0.05))
                ;

            table.SetValueFrom<Table>(setting,"地形图生成设置",new Table())
            .GetValue<Table>("地形图生成设置")
                .SetValueFromSelf<float>("最小边缘扭曲比例",0.6f)
                .SetValueFromSelf<float>("最大边缘扭曲比例",0.7f)
                .SetValueFromSelf<int>("边缘扭曲递归数",4)
                .SetValueFromSelf<NoiseGenerator>("噪声生成器",()=>{
                    ulong seed = table.GetValue<ulong>("随机种子");
                    NoiseGenerator noise = new NoiseGenerator((int)seed);
                    noise.Period = 75f;
                    noise.Octaves = 3;
                    noise.Persistence = 0.5f;
                    noise.Lacunarity = 2f;
                    return noise;   
                })
                ;


            //添加生成过程
            AddProcess("生成Voronoi图",new VoronoiMapBuildProcess());
            AddProcess("生成地形图",new TopographicMapBuildProcess());
            AddProcess("生成生态群落图",new BiomeMapBuildProcess());
            AddProcess("对生态群落进行特定生成",new BiomeBuildProcess());

            //配置生成过程
            SettingProcess(new string[]{
            /*1*/    "生成Voronoi图",
            /*2*/    "生成地形图",
            /*3*/    "生成生态群落图",
            /*4*/    "对生态群落进行特定生成"
            });
        }

        public Table Build() {
            return Build(table);
        }

        #if BOX_DEBUG
        
        public Table DebugBuild(int step = -1) {
            if(step == -1) {//处理全部过程
                foreach(IWorldBuildProcess process in build_process) {
                    process.Build(table);
                }
            } else {
                int s = 0;
                foreach(IWorldBuildProcess process in build_process) {
                    if(s >= step) break;
                    process.Build(table);
                    s++;
                }
            }
            return table;
        }
        
        #endif
    }
}