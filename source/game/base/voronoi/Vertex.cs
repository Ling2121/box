using System;
using Godot;

namespace Box.VoronoiMap {
    public class Vertex {
        public long Index {get;protected set;}

        public float X = 0;
        public float Y = 0;
        public int CeilX {get {return Mathf.CeilToInt(X);}}
        public int CeilY {get {return Mathf.CeilToInt(Y);}}

        public Vertex() {
            X = 0;
            Y = 0;
        }

        public Vertex(float x, float y) {
            this.X = x;
            this.Y = y;
        }

        public override int GetHashCode () {
			return X.GetHashCode () ^ Y.GetHashCode () << 2;
		}

        public override string ToString()
        {
            return $"Voronoi.Vertex({X},{Y})";
        }

        public Vector2 ToVector() {
            return new Vector2(X,Y);
        }
    }
}