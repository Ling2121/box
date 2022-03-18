using Godot;
using System.Collections.Generic;
using Box.VoronoiMap;
using Box.DataCanvas;
using Box.DataCanvas.SystemDrawing;

namespace Box.WorldBuils.Default {
    public class BiomeMapBuildProcess : IWorldBuildProcess {
        
        public string RandomSelectBiome(RandomNumberGenerator random) {
            int count = Register.Instance.BiomeNameList.Count;
            int select = random.RandiRange(0,count - 1);
            return Register.Instance.BiomeNameList[select];
        }
        
        public Table Build(Table table) {
            Dictionary<ushort,string> biome_number_name_map = new Dictionary<ushort, string>();

            int width = table.GetValue<int>("地图宽度");
            int height = table.GetValue<int>("地图高度");

            List<Cell> land_cells = table.GetValue<List<Cell>>("陆地Voronoi细胞");
            Dictionary<long,BuildCellInfo> cell_infos = table.GetValue<Dictionary<long,BuildCellInfo>>("Voronoi细胞信息");
            RandomNumberGenerator random = table.GetValue<RandomNumberGenerator>("随机数生成器");

            IDataCanvas<ushort> biome_map = new DataCanvas16Bit(width,height);

            Dictionary<string,ushort> biome_string_name_map = new Dictionary<string,ushort>();

            ushort biome_number_name = 1;
            biome_map.DrawBegin();
            foreach(Cell cell in land_cells) {
                BuildCellInfo info = cell_infos[cell.Index];
                info.Biome = RandomSelectBiome(random);
                ushort index = biome_number_name;
                if(!biome_string_name_map.ContainsKey(info.Biome)) {
                    biome_string_name_map[info.Biome] = biome_number_name;
                    biome_number_name_map[biome_number_name] = info.Biome;
                    biome_number_name ++;
                } else {
                    index = biome_string_name_map[info.Biome];
                }

                biome_map.DrawPolygon(cell.VerticesToIntArray(),index,true);

                foreach(int[] shake_edge in cell.ShakeEdgesToIntArray()) {
                    biome_map.DrawPolygon(shake_edge,index,true);
                }
            }
            biome_map.DrawEnd();

            table.SetValue<IDataCanvas<ushort>>("生态群系分布图",biome_map); 
            table.SetValue<Dictionary<ushort,string>>("生态群系数字索引",biome_number_name_map);

            return table;
        }
    }
}