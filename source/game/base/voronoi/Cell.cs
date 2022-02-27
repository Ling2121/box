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

    }
}