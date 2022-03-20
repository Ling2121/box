using System.Collections.Generic;
using Godot;

namespace Box.VoronoiMap {
    public class Cell {
        public csDelaunay.Site BasicCell;
        public long Index {get;protected set;}
        public Vector2 Position {get;protected set;}
        public List<Cell> Neighbor = new List<Cell>();
        public List<Edge> Edges = new List<Edge>();
        public List<Vertex> Vertices = new List<Vertex>();

        public Cell(long index,float x,float y) {
            Index = index;
            Position = new Vector2(x,y);
        }

        public Vector2[] VerticesToVector2Array() {
            Vector2[] arr = new Vector2[Vertices.Count];
            for(int i = 0;i<Vertices.Count;i++) {
                arr[i] = new Vector2(Vertices[i].X,Vertices[i].Y);
            }
            return arr;
        }

        public int[] VerticesToIntArray() {
            int[] arr = new int[Vertices.Count * 2];
            
            int i = 0;
            foreach(Vertex vertex in Vertices) {
                arr[i] = (int)vertex.X;
                arr[i + 1] = (int)vertex.Y;
                i += 2;
            }

            return arr;
        }

        public List<Vector2[]> ShakeEdgesToVector2Array() {
            List<Vector2[]> edges = new List<Vector2[]>();
            foreach(Edge edge in Edges) {
                if(edge.IsShake) {
                    edges.Add(edge.GetShakeVertexesVectorArray());
                }
            }
            return edges;
        }

        
        public List<int[]> ShakeEdgesToIntArray() {
            List<int[]> edges = new List<int[]>();
            foreach(Edge edge in Edges) {
                if(edge.IsShake) {
                    edges.Add(edge.GetShakeVertexesIntArray());
                }
            }
            return edges;
        }

    }
}