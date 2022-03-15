using Godot;
using Box.WorldBuils.Default;

namespace Box.Test {
    public class VoronoiBuildTest : WorldBuildTest {

        public override int _SettingWorldBuild(Table setting)
        {
            setting
                .SetValue<int>("地图宽度",Width)
                .SetValue<int>("地图高度",Height)
                .SetValue<ulong>("随机种子",Seed)
                ;
            return 1;
        }

        public override void _BuildEnd(Table table)
        {
            
            Sprite sprite = GetNodeOrCreate<Sprite>("Sprite");
            IDataCanvas<ushort> canvas = table.GetValue<IDataCanvas<ushort>>("缓存画布");
            
            //sprite.Centered = false;
            sprite.Texture = DataCanvas.DataCanvasUtil.ToImageTexture<ushort>(canvas);
        }
    }
}