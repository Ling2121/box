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
    public class DefaultWorldBuild : WorldBuild {
        public Table table;
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
            
            setting.GetValue<Table>("Voronoi图生成设置",new Table())
                .SetValueFromSelf<int>("顶点数",()=>{
                    return ((width + height) / 2) * 0.05;
                });


            //添加生成过程
            AddProcess("生成Voronoi图",new VoronoiMapBuildProcess());
            AddProcess("生成地形图",new TopographicMapBuildProcess());
            AddProcess("生成生态群落图",new BiomeMapBuildProcess());
            AddProcess("对生态群落进行特定生成",new BiomeMapBuildProcess());

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
    }
}