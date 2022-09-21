using System.Linq;
using Godot;
using System;
using System.Collections.Generic;
using Box.VoronoiMap;

namespace Box.WorldBuilds.VoronoiPort
{
    public class TerrainBuildProcess : IWorldBuildProcess<VoronoiWorldBuilderData>
    {

        public void BuildWaterAndLand(VoronoiWorldBuilderData data)
        {
            var Noise = data.Noise;
            //生成高度图
            foreach (var item in data.VoronoiCellDatas)
            {
                var cell_index = item.Key;
                var cell_data = item.Value;
                var cell = data.Voronoi.Cells[cell_index];
                int noise = (int)(Noise.IslandNoise(cell.IndexPoint.Position, data.Width, data.Height) * 255);
                cell_data.Height = noise;
                if (noise >= data.MountainHeight)
                {
                    cell_data.Type = VoronoiCellType.Mountain;
                    data.LandVoronoiCells.Add(cell);
                    data.MountainVoronoiCells.Add(cell);
                }
                else if (noise >= data.LandHeight)
                {
                    cell_data.Type = VoronoiCellType.Land;
                    data.LandVoronoiCells.Add(cell);
                }
                else
                {
                    cell_data.Type = VoronoiCellType.Water;
                    data.WaterVoronoiCells.Add(cell);
                }

            }
            //判断以及设置细胞类型
            foreach (var cell in data.LandVoronoiCells)
            {
                var cell_index = cell.IndexPoint.GetHashValue();
                var cell_data = data.VoronoiCellDatas[cell_index];

                foreach (Cell region_cell in cell.Regions)
                {
                    var rergion_cell_data = data.VoronoiCellDatas[region_cell.IndexPoint.GetHashValue()];
                    if (cell_data.Type != VoronoiCellType.Water)
                    {
                        if (rergion_cell_data.Type == VoronoiCellType.Water)
                        {
                            cell_data.IsCoastline = true;
                        }
                        if (cell_data.Type != VoronoiCellType.Mountain)
                        {
                            if (rergion_cell_data.Type == VoronoiCellType.Mountain)
                            {
                                cell_data.IsFoot = true;
                            }
                        }
                    }
                }

                if (cell_data.Type != VoronoiCellType.Water && !cell_data.IsCoastline)
                {
                    cell_data.IsInland = true;
                }
            }
        }

        protected Cell RandomSelectCell(VoronoiWorldBuilderData data, List<Cell> list, Func<Cell, bool> is_func)
        {
            if (list.Count < 1) return null;
            int i = data.Random.RandiRange(0, list.Count - 1);
            for (; i < list.Count; i++)
            {
                Cell cell = list[i];
                if (is_func(cell))
                {
                    return cell;
                }
            }

            return null;
        }

        protected Cell FindCell(List<Cell> cells,Func<Cell,bool> f,Dictionary<long,bool> exclude_table = null) {
            if(exclude_table != null) {
                foreach(Cell cell in cells) {
                    long index = cell.IndexPoint.GetHashValue();
                    if((!exclude_table.ContainsKey(index)) && f(cell)) return cell;
                }
            } else {
                foreach(Cell cell in cells) {
                    if(f(cell)) return cell;
                }
            }

            return null;
        }

        protected Edge GetAdjacentEdge(Cell c1,Cell c2) {
            foreach(Edge c1e in c1.Edges) {
                if(c1e.C1 == c2 || c1e.C2 == c2) {
                    return c1e;
                }
            }
            return null;
        }

        public void TryBuildCell(VoronoiWorldBuilderData data,List<Cell> list,int prob, int try_number, Func<Cell, bool> is_func, Action<Cell> build)
        {
            for (int i = 0; i < try_number; i++)
            {
                if (data.Random.RandiRange(0, 100) <= prob)
                {
                    Cell cell = RandomSelectCell(data,list, is_func);
                    if (cell != null)
                    {
                        build(cell);
                    }
                }
            }
        }

        public void BuildLakes(VoronoiWorldBuilderData data)
        {
            //生成火山湖
            //条件
            // 1.只会在内陆生成
            // 2.只会在山脉以及附近生成
            // 3.不会在湖泊邻近生成
            TryBuildCell(data,data.LandVoronoiCells,data.LavaGenerateProbability, data.LavaGenerateTry,
                c =>
                {
                    bool region_is_lake = false;
                    foreach (Cell rc in c.Regions)
                    {
                        var rcd = data.VoronoiCellDatas[rc.IndexPoint.GetHashValue()];
                        if (rcd.IsLake || rcd.Type == VoronoiCellType.Water)
                        {
                            region_is_lake = true;
                            break;
                        }
                    }
                    if (region_is_lake) return false;

                    var cd = data.VoronoiCellDatas[c.IndexPoint.GetHashValue()];
                    return cd.IsInland && (cd.Type == VoronoiCellType.Mountain || cd.IsFoot);
                },
                c =>
                {
                    var cd = data.VoronoiCellDatas[c.IndexPoint.GetHashValue()];
                    cd.IsLava = true;
                    data.LavaVoronoiCells.Add(c);
                }
            );

            //生成湖泊
            //条件
            // 1.只会在内陆生成
            // 2.不会在岩浆湖邻近生成
            TryBuildCell(data,data.LandVoronoiCells,data.LakeGenerateProbability, data.LakeGenerateTry,
                c =>
                {
                    bool region_is_lava = false;
                    foreach (Cell rc in c.Regions)
                    {
                        var rcd = data.VoronoiCellDatas[rc.IndexPoint.GetHashValue()];
                        if (rcd.IsLava || rcd.Type == VoronoiCellType.Water)
                        {
                            region_is_lava = true;
                            break;
                        }
                    }
                    if (region_is_lava) return false;

                    var cd = data.VoronoiCellDatas[c.IndexPoint.GetHashValue()];
                    return cd.IsInland;
                },
                c =>
                {
                    var cd = data.VoronoiCellDatas[c.IndexPoint.GetHashValue()];
                    cd.IsLake = true;
                    data.LakeVoronoiCells.Add(c);
                }
            );


        }

        public void BuildRiver(VoronoiWorldBuilderData data)
        {
            //河流类型
            //  1.普通
            //      1.2湖泊为起始
            //  2.岩浆
            //      2.1以熔岩湖为起始

            for(int i = 0;i<data.RiverGenerateTry;i++) {
                var cell = RandomSelectCell(data,data.LakeVoronoiCells,c=> { return true; });
                TryBuildCell(data,data.LakeVoronoiCells,data.RiverGenerateProbability,data.RiverGenerateTry,
                    c => {
                        return true;
                    },
                    c => {
                        List<Edge> river = new List<Edge>();
                        Cell cc = data.LakeVoronoiCells[data.Random.RandiRange(0,data.LakeVoronoiCells.Count - 1)]; 
                        var ccd = data.VoronoiCellDatas[cc.IndexPoint.GetHashValue()];
                        Cell srcc = cc.Regions.Find(rcc => {
                            var rccd = data.VoronoiCellDatas[rcc.IndexPoint.GetHashValue()];
                            return rccd.Height <= ccd.Height;
                        });
                        if(srcc == null) {
                            srcc = cc.Regions[data.Random.RandiRange(0,cc.Regions.Count - 1)];
                        }
                        Edge e = GetAdjacentEdge(cc,srcc);
                        Vertex p = e.P1;
                        float height = data.Noise.IslandNoise(p.Position,data.Width,data.Height);

                        foreach(Edge pre in p.Edges.Values) {
                            if(pre != e) {
                                Vertex next_p = pre.P1 != p ? pre.P1 : pre.P2;
                                if(data.Noise.IslandNoise(next_p.Position,data.Width,data.Height) <= height) {
                                    p = next_p;
                                    e = pre;
                                    break;
                                }
                            }
                        }
                        river.Add(e);

                    }
                );
            }
        }

        public void Build(VoronoiWorldBuilderData data)
        {
            BuildWaterAndLand(data);
            BuildLakes(data);

#if DEBUG

            DataCanvas canvas = null;
            if (data.Canvases.ContainsKey("terrain_canvas1"))
            {
                canvas = data.Canvases["terrain_canvas1"];
            }
            else
            {
                canvas = new DataCanvas(data.Width, data.Height);
                data.Canvases["terrain_canvas1"] = canvas;
            }
            foreach (var item in data.VoronoiCellDatas)
            {
                var cell_index = item.Key;
                var cell_data = item.Value;
                var cell = data.Voronoi.Cells[cell_index];
                uint color = 0;
                if (cell_data.Type == VoronoiCellType.Land)
                {
                    color = 0xff6abe30;
                    if (cell_data.IsInland)
                    {
                        color = 0xff478819;
                    }
                }
                if (cell_data.Type == VoronoiCellType.Water)
                {
                    color = 0xff5fcde4;
                }
                if (cell_data.Type == VoronoiCellType.Mountain)
                {
                    color = 0xff595652;
                }
                if (cell_data.IsLake)
                {
                    color = 0xff5b6ee1;
                }
                if (cell_data.IsLava)
                {
                    color = 0xffff471c;
                }
                canvas.DrawPolygon(color, cell.VerticesToVectorArray(), true);
                canvas.DrawCircle(0xffffffff, cell.IndexPoint.Position, 2, true);
            }

#endif
        }
    }
}