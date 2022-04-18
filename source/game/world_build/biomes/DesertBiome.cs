using Box.VoronoiMap;

namespace Box.Biomes {
    //沙漠
    [Register(nameof(DesertBiome))]
    public class DesertBiome : IBiome {
        public void Build(Table table,Cell cell,IDataCanvas<ushort> canvas,NumberIndexPool tile_index_pool) {
            ushort sand_index = (ushort)tile_index_pool.GetIndex("sand");
            DataCanvas.DataCanvasUtil.FillCell(canvas,cell,sand_index);
        }
    }
}