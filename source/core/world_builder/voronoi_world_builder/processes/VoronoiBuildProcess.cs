using System.Collections.Generic;
using Godot;

using Box.VoronoiMap;

namespace Box.WorldBuilds.VoronoiPort {
    public class VoronoiBuildProcess : IWorldBuildProcess<VoronoiWorldBuilderData> { 
        public const string ProcessName = "Voronoi";
        public Vector2[] GenPoints(RandomNumberGenerator random,int max_x,int max_y,int number) {
            Vector2[] points = new Vector2[number];
            for(int i = 0;i<number;i++) {
                points[i] = new Vector2(
                    (int)random.RandiRange(0,max_x),
                    (int)random.RandiRange(0,max_y)
                );
            }

            return points;
        }

        public void Build(VoronoiWorldBuilderData data) {
            int width = data.Width;
            int height = data.Height;
            RandomNumberGenerator random = data.Random;
            int point_number = data.顶点生成数;// (int)((width + height) * 0.233);

            Voronoi voronoi = new Voronoi(GenPoints(random,width,height,point_number));

            data.Voronoi = voronoi;

            foreach(var item in voronoi.Cells) {
                data.细胞数据[item.Key] = new VoronoiCellBuildData();
            }
        }
    }
}