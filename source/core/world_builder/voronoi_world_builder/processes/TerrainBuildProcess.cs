using Godot;

namespace Box.WorldBuilds.VoronoiPort {
    public class TerrainBuildProcess : IWorldBuildProcess<VoronoiWorldBuilderData> { 
        
        public void BuildWaterAndLand(VoronoiWorldBuilderData data) {
            var Noise = data.Noise;
            foreach(var item in data.细胞数据) {
                var cell_index = item.Key;
                var cell_data = item.Value;
                var cell = data.Voronoi.Cells[cell_index];
                int noise = (int)(Noise.IslandNoise(cell.IndexPoint.Position,data.Width,data.Height) * 255);
                if(noise >= data.山脉高度){
                    cell_data.Type = VoronoiCellType.Mountain;
                } else if(noise >= data.陆地高度) {
                    cell_data.Type = VoronoiCellType.Land;
                } else {
                    cell_data.Type = VoronoiCellType.Water;
                }
            }

            DataCanvas canvas = null;
            if(data.Canvases.ContainsKey("terrain_canvas1")) {
                canvas = data.Canvases["terrain_canvas1"];
            } else {
                canvas = new DataCanvas(data.Width,data.Height);
                data.Canvases["terrain_canvas1"] = canvas;
            }
            foreach(var item in data.细胞数据) {
                var cell_index = item.Key;
                var cell_data = item.Value;
                var cell = data.Voronoi.Cells[cell_index];
                uint color = 0;
                if(cell_data.Type == VoronoiCellType.Land) {
                    color = 0xff6abe30;
                }
                if(cell_data.Type == VoronoiCellType.Water) {
                    color = 0xff5fcde4;
                }
                if(cell_data.Type == VoronoiCellType.Mountain) {
                    color = 0xff595652;
                }
                canvas.DrawPolygon(color,cell.VerticesToVectorArray(),true);
            }
        }
        
        public void Build(VoronoiWorldBuilderData data) {
            BuildWaterAndLand(data);
        }
    }
}