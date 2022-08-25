using System.Collections.Generic;
using Godot;

using Box.VoronoiMap;

namespace Box.WorldBuilds.VoronoiPort {
    public class VoronoiBuildProcess : IWorldBuildProcess<VoronoiWorldBuilderData> { 
        public const string ProcessName = "Voronoi";
        public Vector2[] GenPoints(RandomNumberGenerator random,int max_x,int max_y,int number) {
            Vector2[] buffer_points = new Vector2[number];
            for(int i = 0;i<number;i++) {
                buffer_points[i] = new Vector2(
                    (int)random.RandiRange(0,max_x),
                    (int)random.RandiRange(0,max_y)
                );
            }
            List<Vector2> points = new List<Vector2>();
            for(int i = 0;i < number;i++) {
                Vector2 p0 = buffer_points[i];
                if(p0.x >= 0) {
                    points.Add(p0);
                    for(int n = i + 1;n < number;n++) {
                        Vector2 p1 =  buffer_points[n];
                        if(p1.x > 0) {
                            if(p0.DistanceTo(p1) < 10) {
                                buffer_points[n] = new Vector2(-1,-1);
                            }
                        }
                    }
                }
            }

            return points.ToArray();
        }

        public void Build(VoronoiWorldBuilderData data) {
            int width = data.Width;
            int height = data.Height;
            RandomNumberGenerator random = data.Random;
            int point_number = data.顶点生成数;// (int)((width + height) * 0.233);

            Voronoi voronoi = new Voronoi(GenPoints(random,width,height,point_number));

            data.Voronoi = voronoi;

            foreach(var item in voronoi.Cells) {
                data.VoronoiCellDatas[item.Key] = new VoronoiCellBuildData();
            }
        }
    }
}