using Box.VoronoiMap;
using Box.DataCanvas;

namespace Box {
    public interface IBiome {
        void Build(Table table,Cell cell,IDataCanvas<ushort> canvas,NumberIndexPool tile_index_pool);
    }
}