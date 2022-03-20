using Godot;
using System;


namespace Box.Test {
    public class BiomeMapBuildTestScene : TopographicMapBuildTest
    {
        public override void _EnterTree()
        {
            Register.Instance.Init();
        }

        public override int _SettingWorldBuild(Table setting) {
            base._SettingWorldBuild(setting);
            return 3;
        }

        public override void _BuildEnd(Table table)
        {
            Sprite sprite = GetNode<Sprite>("Sprite");
            IDataCanvas<ushort> canvas1 = table.GetValue<IDataCanvas<ushort> >("生态群系分布图");
            sprite.Texture = DataCanvas.DataCanvasUtil.ToImageTexture<ushort>(canvas1);
        }
    }
}
