using System.Collections.Generic;
using Godot;
using System;
using DelaunatorSharp;

namespace Box.VoronoiMap {
    public class Voronoi : Node2D
    {
        [Export]
        public int PointNumber = 100;

        IPoint[] points;
        Delaunator delaunator;

        public Dictionary<long,Vertex> Vertices {get;} = new Dictionary<long, Vertex>();
        public Dictionary<long,Edge> Edges {get;} = new Dictionary<long, Edge>();
        public Dictionary<long,Cell> Cells {get;} = new Dictionary<long, Cell>();

        public Voronoi() {}
        public Voronoi(Vector2[] points) {
            delaunator = new  Delaunator(VectorArrToPointArr(points));

            delaunator.ForEachVoronoiCell(c => {
                Cell cell = new Cell();
                
                IPoint current_point;
                Vertex current;
                Vertex up = GetVertexOrCreate((int)c.Points[0].X,(int)c.Points[0].Y);
                Edge edge;
                double cx = up.x;
                double cy = up.y;
                cell.Vertices.Add(up);
                for(int i = 1;i<c.Points.Length;i++) {
                    current_point = c.Points[i];
                    current = GetVertexOrCreate((int)current_point.X,(int)current_point.Y);

                    edge = GetEdgeOrCreate(up,current);
                    if(edge.C1 == null) {
                        edge.C1 = cell;
                    } else {
                        if(edge.C2 == null) {
                            edge.C2 = cell;
                        }
                    }
                    
                    cell.Edges.Add(edge);
                    cell.Vertices.Add(current);

                    up = current;
                    cx += current.x;
                    cy += current.y;
                }
                edge = GetEdgeOrCreate(up,GetVertexOrCreate((int)c.Points[0].X,(int)c.Points[0].Y));
                cell.Edges.Add(edge);

                cx = cx / c.Points.Length;
                cy = cy / c.Points.Length;

                cell.IndexPoint = GetVertexOrCreate((int)cx,(int)cy);
                long hash = cell.IndexPoint.GetHashValue();
                
                Cells[hash] = cell;
            });

            foreach(Cell cell in Cells.Values) {
                foreach(Edge edge in cell.Edges) {
                    Cell region_cell = edge.C1 == cell ? edge.C2 : edge.C1;
                    if(region_cell != null) {
                        cell.Regions.Add(region_cell);
                    }
                }
            }
        }

        protected Vertex GetVertexOrCreate(int x,int y) {
            long hash = Vertex.ToHashValue(x,y);
            if(!Vertices.ContainsKey(hash)) {
                Vertices[hash] = new Vertex(x,y);
            }
            return Vertices[hash];
        }

        protected Edge GetEdgeOrCreate(Vertex p1,Vertex p2) {
            Vector2 c = (p1.Position + p2.Position) / 2;
            Vertex edge_index_point = GetVertexOrCreate((int)c.x,(int)c.y);
            long hash = Vertex.ToHashValue(edge_index_point.x,edge_index_point.y);
            if(!Edges.ContainsKey(hash)) {
                Edge edge = new Edge();
                edge.P1 = p1;
                edge.P2 = p2;
                edge.IndexPoint = edge_index_point;
                Edges[hash] = edge;
            }
            return Edges[hash];
        }

        protected IPoint[] VectorArrToPointArr(Vector2[] points) {
            IPoint[] arr = new IPoint[points.Length];
            for(int i = 0;i<points.Length;i++) {
                arr[i] = new Point(points[i].x,points[i].y);
            }
            return arr;
        }

        protected Vector2[] PointArrToVectorArr(IPoint[] points) {
            Vector2[] arr = new Vector2[points.Length];
            for(int i = 0;i<points.Length;i++) {
                arr[i] = new Vector2((float)points[i].X,(float)points[i].Y);
            }
            return arr;
        }
    }

}