using System.Diagnostics.Tracing;
using System.Drawing;
using System;
using System.Collections.Generic;
using Godot;

namespace Box.VoronoiMap {
    public class Voronoi {
        public csDelaunay.Voronoi BasicVoronoi {get;protected set;}
        public Dictionary<long,Cell> Cells {get;protected set;} = new Dictionary<long,Cell>();
        public Dictionary<long,Edge> Edges {get;protected set;} = new Dictionary<long,Edge>();
        public Dictionary<long,Vertex> Vertexs {get;protected set;} = new Dictionary<long, Vertex>();
        public float Width {get;protected set;}
        public float Height {get;protected set;}

        public Voronoi(List<Vector2> vertexs,int width,int height) {
            BasicVoronoi = new csDelaunay.Voronoi(GDVectorListToCSDVector2fList(vertexs),new csDelaunay.Rectf(0,0,width,height));
            Width = width;
            Height = height;

            BasicVoronoi.LloydRelaxation(3);
            
            Dictionary<long,(csDelaunay.Site site,int cell_index)> create_sites = new Dictionary<long,(csDelaunay.Site site,int cell_index)>();

            //对Edge进行扫描生成Cell
            foreach(csDelaunay.Edge csd_edge in BasicVoronoi.Edges) {
                if(csd_edge.Visible()) {
                    Vertex p1 = GetVertexOrNew(csd_edge.ClippedEnds[csDelaunay.LR.LEFT]);
                    Vertex p2 = GetVertexOrNew(csd_edge.ClippedEnds[csDelaunay.LR.RIGHT]);

                    if(GetPointHash(p1) != GetPointHash(p2)) {
                        long edge_index = GetPointHash((p1.X + p2.X) / 2,(p1.Y + p2.Y) / 2);
                        Edge edge = new Edge(edge_index);
                        edge.P1 = p1;
                        edge.P2 = p2;
                        edge.BasicEdge = csd_edge;
                        Edges[edge_index] = edge;
                        if(csd_edge.LeftSite  != null) {
                            edge.Cell1 = GetCellOrNew(csd_edge.LeftSite);
                            Cell left_cell = edge.Cell1;
                            left_cell.Edges.Add(edge);
                        }
                        if(csd_edge.RightSite != null) {
                            edge.Cell2 = GetCellOrNew(csd_edge.RightSite);
                            Cell right_cell = edge.Cell2;
                            right_cell.Edges.Add(edge);
                        }
                    }
                }
            }
            //获取Cell周围的Cell以及所有顶点
            foreach(Cell cell in Cells.Values) {
                List<csDelaunay.Vector2f> points = BasicVoronoi.Region(cell.BasicCell.Coord);
                foreach(csDelaunay.Vector2f p in points) {
                    cell.Vertices.Add(GetVertexOrNew(p));
                }

                foreach(Edge edge in cell.Edges) {
                    cell.Neighbor.Add(edge.Cell1 == cell ? edge.Cell2:edge.Cell1);
                }
            }

        }

        public void SortCellsEdge() {
            foreach(Cell cell in Cells.Values) {
                List<Edge> edges = new List<Edge>(cell.Edges);
                Edge edge = edges[0];

                cell.Edges.Clear();
                
                edges.Remove(edge);
                cell.Edges.Add(edge);

                while(edges.Count != 0) {
                    Edge next_edge = FindNextEdge(edge,edges);
                    if(next_edge == null) {
                        next_edge = FindNullEdge(edges);
                    }

                    if(next_edge != null) {
                        edge = next_edge;
                        edges.Remove(edge);
                        cell.Edges.Add(edge);
                    }
                }
            }
        }

        public List<csDelaunay.Vector2f> GDVectorListToCSDVector2fList(List<Vector2> vertexs){
            List<csDelaunay.Vector2f> list = new List<csDelaunay.Vector2f>();
            foreach(Vector2 p in vertexs) {
                list.Add(new csDelaunay.Vector2f(p.x,p.y));
            }
            return list;
        }

        public long GetPointHash(float x,float y) {
            int intx = Mathf.FloorToInt(x),inty =  Mathf.FloorToInt(y);
            return inty * ((int)Width) + intx;
        }

        public long GetPointHash(Vertex p) {
            return GetPointHash(p.X,p.Y);
        }

        public long GetPointHash(Vector2 p) {
            return GetPointHash(p.x,p.y);
        }

        public long GetPointHash(csDelaunay.Vector2f p) {
            return GetPointHash(p.x,p.y);
        }

        protected Vertex GetVertexOrNew(csDelaunay.Vector2f csd_vec) {
            long hash_code = GetPointHash(csd_vec.x,csd_vec.y);
            if(Vertexs.ContainsKey(hash_code))
            {
                return Vertexs[hash_code];
            }
            Vertex vertex = new Vertex(csd_vec.x,csd_vec.y);
            Vertexs[hash_code] = vertex;
            return vertex;
        }

        protected Cell GetCellOrNew(csDelaunay.Site site) {
            Cell cell = null;
            long hash_code = GetPointHash(site.Coord);
            if(Cells.ContainsKey(hash_code)) {
                cell = Cells[hash_code];
            } else {
                cell = new Cell(hash_code,site.Coord.x,site.Coord.y);
                cell.BasicCell = site;
                Cells[hash_code] = cell;
            }

            return cell;
        }

        protected Edge FindNextEdge(Edge edge,List<Edge> edges) {
            Edge next_edge = null;
            long edge_p1_hash = GetPointHash(edge.P1);
            long edge_p2_hash = GetPointHash(edge.P2);
            foreach(Edge e in edges) {
                long p1hash = GetPointHash(e.P1);
                long p2hash = GetPointHash(e.P2);
                
                if( p1hash == edge_p1_hash
                ||  p1hash == edge_p2_hash
                ||  p2hash == edge_p1_hash
                ||  p2hash == edge_p2_hash
                ) {
                    next_edge = e;
                    break;
                }
            }
            return next_edge;
        }


        protected bool IsOuterEdge(Edge edge) {
            var p1 = edge.P1;
            var p2 = edge.P2;
            
            return p1.CeilX == 0 || p1.CeilX == Width || p1.CeilY == 0 || p1.CeilY == Height
            || p2.CeilX == 0 || p2.CeilX == Width || p2.CeilY == 0 || p2.CeilY == Height;
        }
        protected Edge FindNullEdge(List<Edge> edges) {
            Edge edge = null;
            foreach(Edge e in edges) {
                if(IsOuterEdge(e)) {
                    edge = e;
                    break;
                }
            }
            return edge;
        }

    }
}