using Box.VoronoiMap;
using Godot;
using System.Collections.Generic;
using Box.DataCanvas;
using Box.DataCanvas.SystemDrawing;

namespace Box.WorldBuils.Default {
    public enum TopographicType {
        Land,
        Water,
    }

    public struct CellTopographicInfo {
        public TopographicType Type;
        //是否是当前TopographicType类型的边缘 
        bool IsTypeEdge;
    }

    public class TopographicMapBuildProcess : IWorldBuildProcess {
        public Table Build(Table table) {
            Table setting = table.GetValue<Table>("地形图生成设置");

            
            Voronoi voronoi = table.GetValue<Voronoi>("Voronoi图");
            RandomNumberGenerator random = table.GetValue<RandomNumberGenerator>("随机数生成器");
            NoiseGenerator noise = setting.GetValue<NoiseGenerator>("噪声生成器");
            int width = table.GetValue<int>("地图宽度");
            int height = table.GetValue<int>("地图高度");
            int shake_number = setting.GetValue<int>("边缘扭曲递归数");
            float shake_min = setting.GetValue<float>("最小边缘扭曲比例");
            float shake_max = setting.GetValue<float>("最大边缘扭曲比例");

            Dictionary<long,CellTopographicInfo> cell_infos = new Dictionary<long, CellTopographicInfo>();
            List<Cell> land_cells = new List<Cell>();

            foreach(Cell cell in voronoi.Cells.Values) {
                int n = (int)(noise.IslandNoise(cell.Position,width,height) * 255);
                CellTopographicInfo info = new CellTopographicInfo();
                if(n > 2) {
                    info.Type = TopographicType.Land;
                    land_cells.Add(cell);
                } else {
                    info.Type = TopographicType.Water;
                }
                cell_infos[cell.Index] = info;
            }

            
            #if BOX_DEBUG
                IDataCanvas<ushort> canvas1 = new DataCanvas16Bit(width,height);
                foreach(Cell cell in voronoi.Cells.Values) {
                    CellTopographicInfo info = cell_infos[cell.Index];
                    ushort d = (ushort)(info.Type == TopographicType.Land ? 1:0);
                    canvas1.DrawPolygon(DataCanvasUtil.ToPointArray(cell.VerticesToVector2Array()),d,true);
                }
                table.SetValue<IDataCanvas<ushort>>("地形图海陆画布1",canvas1);
            #endif

            foreach(Cell land_cell in land_cells) {
                Vector2 p4 = land_cell.Position;
                foreach(Edge edge in land_cell.Edges) {
                    CellTopographicInfo cell1_info = cell_infos[edge.Cell1.Index];
                    CellTopographicInfo cell2_info = cell_infos[edge.Cell2.Index];
                    if(cell1_info.Type == TopographicType.Water || cell2_info.Type == TopographicType.Water) {
                        if(!edge.IsShake) {
                            edge.IsShake = true;
                            edge.ShakeP1 = new Vertex(edge.P1.X,edge.P1.Y);
                            edge.ShakeP2 = new Vertex(edge.P2.X,edge.P2.Y);
                            Cell rcell = edge.Cell1.Index != land_cell.Index ? edge.Cell1 :  edge.Cell2;
                            Vector2 p3 = rcell.Position;
                            Voronoi.ShakeEdge(edge.ShakeP1,edge.ShakeP2,p3,p4,shake_number,shake_min,shake_max,random);
                        }
                    }
                }
            }

            #if BOX_DEBUG
                /*
                    所以Cell的边和点都是只有一套的!
                */
                IDataCanvas<ushort> canvas2 = new DataCanvas16Bit(width,height);
                ushort c = 1;
                foreach(Cell cell in voronoi.Cells.Values) {
                    CellTopographicInfo info = cell_infos[cell.Index];
                    if(info.Type == TopographicType.Land) {
                        canvas2.DrawPolygon(DataCanvasUtil.ToPointArray(cell.VerticesToVector2Array()),1,true);
                        foreach(Edge edge in cell.Edges) {
                            if(edge.IsShake) {
                                List<int> point_arr = new List<int>();
                                Vertex node = edge.ShakeP1;
                                while(true) {
                                    if(node == null) {
                                        break;
                                    }

                                    point_arr.Add((int)node.X);
                                    point_arr.Add((int)node.Y);

                                    if(node == edge.ShakeP2) {
                                        break;
                                    }
                                    
                                    node = node.Next;
                                }
                                canvas2.DrawPolygon(point_arr.ToArray(),1,true);
                            }
                        }
                    }
                }
                table.SetValue<IDataCanvas<ushort>>("地形图海陆画布2",canvas2);
            #endif

            return table;
        }
    }
}