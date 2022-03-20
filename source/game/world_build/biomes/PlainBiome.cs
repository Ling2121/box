using Box.VoronoiMap;

namespace Box.Biomes {
    //平原
    [Register(nameof(PlainBiome))]
    public class PlainBiome : IBiome,IRegister {
        public void Build(Table table,Cell cell,IDataCanvas<ushort> canvas,NumberIndexPool tile_index_pool) {
            ushort grass_index = (ushort)tile_index_pool.GetIndex("grass");
            DataCanvas.DataCanvasUtil.FillCell(canvas,cell,grass_index);
        }
    }
}