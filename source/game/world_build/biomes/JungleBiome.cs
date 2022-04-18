using Box.VoronoiMap;

namespace Box.Biomes {
    //丛林
    [Register(nameof(JungleBiome))]
    public class JungleBiome : IBiome {
        public void Build(Table table,Cell cell,IDataCanvas<ushort> canvas,NumberIndexPool tile_index_pool) {
            ushort grass_jungle_index = (ushort)tile_index_pool.GetIndex("grass_jungle");
            DataCanvas.DataCanvasUtil.FillCell(canvas,cell,grass_jungle_index);
        }
    }
}