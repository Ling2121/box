using System.Linq;
using Godot;
using System;
using System.Collections.Generic;
using Box.VoronoiMap;

namespace Box.WorldBuilds.VoronoiPort {
    public class TerrainBuildProcess : IWorldBuildProcess<VoronoiWorldBuilderData> { 
        
        public void BuildWaterAndLand(VoronoiWorldBuilderData data) {
            var Noise = data.Noise;
            foreach(var item in data.VoronoiCellDatas) {
                var cell_index = item.Key;
                var cell_data = item.Value;
                var cell = data.Voronoi.Cells[cell_index];
                int noise = (int)(Noise.IslandNoise(cell.IndexPoint.Position,data.Width,data.Height) * 255);
                cell_data.Height = noise;
                if(noise >= data.山脉高度){
                    cell_data.Type = VoronoiCellType.Mountain;
                    data.LandVoronoiCells[cell_index] = cell;
                    data.MountainVoronoiCells[cell_index] = cell;
                } else if(noise >= data.陆地高度) {
                    cell_data.Type = VoronoiCellType.Land;
                    data.LandVoronoiCells[cell_index] = cell;
                } else {
                    cell_data.Type = VoronoiCellType.Water;
                    data.WaterVoronoiCells[cell_index] = cell;
                }
            }

            foreach(var item in data.VoronoiCellDatas) {
                var cell_index = item.Key;
                var cell_data = item.Value;
                var cell = data.Voronoi.Cells[cell_index];

                foreach(Cell region_cell in cell.Regions) {
                    var rergion_cell_data = data.VoronoiCellDatas[region_cell.IndexPoint.GetHashValue()];
                    if(cell_data.Type != VoronoiCellType.Water) {
                        if(rergion_cell_data.Type == VoronoiCellType.Water) {
                            cell_data.IsCoastline = true;
                        }
                        if(cell_data.Type != VoronoiCellType.Mountain) {
                            if(rergion_cell_data.Type == VoronoiCellType.Mountain) {
                                cell_data.IsFoot = true;
                            }
                        }
                    }
                }

                if(!cell_data.IsCoastline) {
                    cell_data.IsInland = true;
                }
            }


            #if DEBUG

            DataCanvas canvas = null;
            if(data.Canvases.ContainsKey("terrain_canvas1")) {
                canvas = data.Canvases["terrain_canvas1"];
            } else {
                canvas = new DataCanvas(data.Width,data.Height);
                data.Canvases["terrain_canvas1"] = canvas;
            }
            foreach(var item in data.VoronoiCellDatas) {
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
                canvas.DrawCircle(0xffffffff,cell.IndexPoint.Position,2,true);
            }

            #endif
        }

        protected Cell RandomSelectCell(VoronoiWorldBuilderData data,Dictionary<long,Cell> table,Func<Cell,bool> is_func) {
            Cell[] list = table.Values.ToArray();
            if(list.Length < 1) return null;
            int i = data.Random.RandiRange(0,list.Length - 1);
            for(;i<list.Length;i++) {
                Cell cell = list[i];
                if(is_func(cell)) {
                    return cell;
                }
            }

            return null;
        }

        public void TryBuildLake(VoronoiWorldBuilderData data,int prob,int try_number,Func<Cell,bool> is_func,Action<Cell> build) {
            for(int i = 0;i < try_number;i++) {
                if(data.Random.RandiRange(0,100) <= prob) {
                    Cell cell = RandomSelectCell(data,data.LandVoronoiCells,is_func);
                    if(cell != null) {
                        build(cell);
                    }
                }
            }
        }

        public void BuildLakes(VoronoiWorldBuilderData data) {
            //生成火山湖
            //条件
            // 1.只会在内陆生成
            // 2.只会在山脉以及附近生成
            // 3.不会在湖泊旁边生成
            TryBuildLake(data,data.火山湖生成几率,data.火山湖生成尝试数,
                c => {
                    bool region_is_lake = false;
                    foreach(Cell rc in c.Regions) {
                        var rcd = data.VoronoiCellDatas[rc.IndexPoint.GetHashValue()];
                        if(rcd.IsLake) {
                            region_is_lake = true;
                            break;
                        }
                    }
                    if(region_is_lake) return false;

                    var cd = data.VoronoiCellDatas[c.IndexPoint.GetHashValue()];
                    return cd.IsInland && (cd.Type == VoronoiCellType.Mountain || cd.IsFoot);
                },
                c => {

                }
            );

            //生成湖泊
            //条件
            // 1.只会在内陆生成
            // 2.不会在岩浆湖旁边生成
            TryBuildLake(data,data.火山湖生成几率,data.火山湖生成尝试数,
                c => {
                    bool region_is_lava = false;
                    foreach(Cell rc in c.Regions) {
                        var rcd = data.VoronoiCellDatas[rc.IndexPoint.GetHashValue()];
                        if(rcd.IsLava) {
                            region_is_lava = true;
                            break;
                        }
                    }
                    if(region_is_lava) return false;

                    var cd = data.VoronoiCellDatas[c.IndexPoint.GetHashValue()];
                    return cd.IsInland;
                },
                c => {
                    
                }
            );


        }

        public void Build(VoronoiWorldBuilderData data) {
            BuildWaterAndLand(data);
        }
    }
}