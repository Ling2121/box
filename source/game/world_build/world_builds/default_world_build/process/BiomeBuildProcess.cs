using Godot;
using System.Collections.Generic;
using Box.VoronoiMap;

namespace Box.WorldBuils.Default {
    public class BiomeBuildProcess : IWorldBuildProcess {
        public Table Build(Table table) {
            Dictionary<ushort,string> biome_name_map = new Dictionary<ushort, string>();

            List<Cell> land_cells = table.GetValue<List<Cell>>("陆地Voronoi细胞");
            Dictionary<long,BuildCellInfo> cell_infos = table.GetValue<Dictionary<long,BuildCellInfo>>("Voronoi细胞信息");
            RandomNumberGenerator random = table.GetValue<RandomNumberGenerator>("随机数生成器");

            IDataCanvas<ushort> world_canvas = table.GetValue<IDataCanvas<ushort>>("世界画布");
            

            NumberIndexPool tile_index_pool = new NumberIndexPool();

            foreach(Cell cell in land_cells) {
                BuildCellInfo info = cell_infos[cell.Index];
                IBiome biome_builder = Register.Instance.GetBiome(info.Biome);
                
                biome_builder.Build(table,cell,world_canvas,tile_index_pool);
            }

            table.SetValue<NumberIndexPool>("Tile索引池",tile_index_pool);

            return table;
        }
    }
}