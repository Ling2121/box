using Box.VoronoiMap;

namespace Box.Biomes {
    //森林
    [Register(nameof(ForestBiome))]
    public class ForestBiome : IBiome,IRegister {
        public void Build(Table table,Cell cell,IDataCanvas<ushort> canvas,NumberIndexPool tile_index_pool) {
            ushort grass_index = (ushort)tile_index_pool.GetIndex("grass");
            DataCanvas.DataCanvasUtil.FillCell(canvas,cell,grass_index);
        }
    }
}