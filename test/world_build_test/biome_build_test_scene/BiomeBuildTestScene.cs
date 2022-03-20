using Godot;
using System;


namespace Box.Test {
    public class BiomeBuildTestScene : BiomeMapBuildTestScene
    {
        public override int _SettingWorldBuild(Table setting) {
            base._SettingWorldBuild(setting);
            return 4;
        }

        public override void _BuildEnd(Table table)
        {
            // Sprite sprite = GetNode<Sprite>("Sprite");
            IDataCanvas<ushort> canvas1 = table.GetValue<IDataCanvas<ushort> >("世界画布");
            // sprite.Texture = DataCanvas.DataCanvasUtil.ToImageTexture<ushort>(canvas1);
            
            NumberIndexPool tile_index_pool = table.GetValue<NumberIndexPool>("Tile索引池");
            TileMap tile_map = GetNode<TileMap>("TileMap");
            for(int y = 0;y < canvas1.Height;y++) {
                for(int x = 0;x < canvas1.Width;x++) {
                    ushort pixel = canvas1[x,y];
                    string tile_name = tile_index_pool.GetKey(pixel);
                    int index = tile_map.TileSet.FindTileByName("water");
                    if(tile_name != "") {
                        index = tile_map.TileSet.FindTileByName(tile_name);
                    }
                    tile_map.SetCell(x,y,index);
                }
            }

        }
    }
}
