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

            

            foreach(Cell cell in land_cells) {
                BuildCellInfo info = cell_infos[cell.Index];
                IBiome biome_builder = Register.Instance.GetBiome(info.Biome);

            }

            return table;
        }
    }
}