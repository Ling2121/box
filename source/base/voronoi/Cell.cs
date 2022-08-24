using System.Collections.Generic;
using Godot;

namespace Box.VoronoiMap {
    public class Cell {
        public List<Vertex> Vertices = new List<Vertex>();
        public List<Edge> Edges = new List<Edge>();
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