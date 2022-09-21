using System.Collections.Generic;
using Godot;

namespace Box.VoronoiMap {
    public class Cell {

        public static bool operator==(Cell a,Cell b) {
            return a.IndexPoint.GetHashValue() == b.IndexPoint.GetHashValue();
        }

        public static bool operator!=(Cell a,Cell b) {
            return a.IndexPoint.GetHashValue() != b.IndexPoint.GetHashValue();
        }

        public override bool Equals(object obj)
        {
            if(!(obj is Cell)) return false;
            return (obj as Cell).IndexPoint.GetHashValue() == IndexPoint.GetHashValue();
        }

        public List<Vertex> Vertices = new List<Vertex>();
        public List<Edge> Edges = new List<Edge>();
        public List<Cell> Regions = new List<Cell>();
        public Vertex IndexPoint;

        public Vector2[] VerticesToVectorArray() {
            Vector2[] arr = new Vector2[Vertices.Count];
            int i = 0;
            foreach(Vertex vertex in Vertices) {
                arr[i] = vertex.Position;
                i++;
            }

            return arr;
        } 
    }
}