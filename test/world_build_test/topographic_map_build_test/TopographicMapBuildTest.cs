using Godot;
using System;
using Box.DataCanvas;

namespace Box.Test {
    public class TopographicMapBuildTest : VoronoiBuildTest
    {
        [Export]
        public float ShakeMin = 0.55f;
        [Export]
        public float ShakeMax = 0.65f;
        [Export]
        public int ShankeNumber = 1;


        public override int _SettingWorldBuild(Table setting)
        {
            base._SettingWorldBuild(setting);

            setting.GetValue<Table>("地形图生成设置",new Table())
                .SetValue<float>("最小边缘扭曲比例",ShakeMin)
                .SetValue<float>("最大边缘扭曲比例",ShakeMax)
                .SetValue<int>("边缘扭曲递归数",ShankeNumber)
                .SetValue<NoiseGenerator>("噪声生成器",()=>{
                    ulong seed = setting.GetValue<ulong>("随机种子");
                    NoiseGenerator noise = new NoiseGenerator((int)seed);
                    noise.Period = 75f;
                    noise.Octaves = 3;
                    noise.Persistence = 0.5f;
                    noise.Lacunarity = 2f;
                    return noise;   
                })
                ;

            return 2;

        }

        public override void _BuildEnd(Table table)
        {
            Sprite sprite = GetNode<Sprite>("Sprite");
            IDataCanvas<ushort> canvas1 = table.GetValue<IDataCanvas<ushort> >("地形图海陆画布2");
            sprite.Texture = DataCanvas.DataCanvasUtil.ToImageTexture<ushort>(canvas1);
        }
    }

}