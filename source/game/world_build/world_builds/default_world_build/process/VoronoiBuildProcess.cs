

using System;
using Box.DataCanvas;
using Box.VoronoiMap;
using Godot;
using System.Collections.Generic;

namespace Box.WorldBuils.Default {
    public class VoronoiMapBuildProcess : IWorldBuildProcess {
        public Table Build(Table table) {
            Table setting = table.GetValue<Table>("Voronoi图生成设置");
            
            int width = table.GetValue<int>("地图宽度");
            int height = table.GetValue<int>("地图高度");
            RandomNumberGenerator random = table.GetValue<RandomNumberGenerator>("随机数生成器");
            IDataCanvas<ushort> canvas = table.GetValue<IDataCanvas<ushort>>("缓存画布");
            
            int point_number = setting.GetValue<int>("顶点数");
            

            GD.Print("顶点数:",point_number);

            List<Vector2> points = new List<Vector2>();

            for(int i = 0;i<point_number;i++) {
                points.Add(new Vector2(random.RandfRange(10,width - 10),random.RandfRange(10,height - 10)));
            }

            Voronoi voronoi = new Voronoi(points,width,height);

            table.SetValue<Voronoi>("Voronoi图",voronoi);

            #warning 在DEBUG模式下VoronoiMapBuildProcess会对 “缓存画布” 进行绘制
            #if BOX_DEBUG
            canvas.DrawBegin();
                canvas.DrawRectangle(0,0,width,height,true,0);
                
                foreach(Cell cell in voronoi.Cells.Values) {    
                    ushort d = (ushort)random.RandiRange(0,0xffff);
                    canvas.DrawPolygon(DataCanvasUtil.ToPointArray(cell.VerticesToVector2Array()),d,true);
                }
            canvas.DrawEnd();
            #endif
            
            return table;
        }
    }
}