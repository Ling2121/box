using Godot;
using Box.WorldBuils.Default;

namespace Box.Test {
    public class WorldBuildTest : BaseTestScene {
        [Export]
        public int Width = 512;
        [Export]
        public int Height = 512;
        [Export]
        public ulong Seed = 233;

        protected DefaultWorldBuild WorldBuild;
        protected Table Table;

        public virtual int _SettingWorldBuild(Table setting) {
            return -1;
        }

        public override void _Ready()
        {
            #if BOX_DEBUG 

            Table = new Table();
            int step = _SettingWorldBuild(Table);
            WorldBuild = new DefaultWorldBuild(Table);
            _BuildEnd(WorldBuild.DebugBuild(step));

            #endif
        }

        public virtual void _BuildEnd(Table table) {

        }
    }
}