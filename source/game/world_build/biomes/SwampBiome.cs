using Box.VoronoiMap;

namespace Box.Biomes {
    //沼泽
    [Register(nameof(SwampBiome))]
    public class SwampBiome : IBiome,IRegister {
        public void Build(Table table,Cell cell,IDataCanvas<ushort> canvas,NumberIndexPool tile_index_pool) {
            ushort grass_swamp_index = (ushort)tile_index_pool.GetIndex("grass_swamp");
            DataCanvas.DataCanvasUtil.FillCell(canvas,cell,grass_swamp_index);
        }
    }
}