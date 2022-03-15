using System;
using Godot;

namespace Box.VoronoiMap {
    public class Vertex {
        public long Index {get;protected set;}

        public Vertex Next;
        public Vertex Up;

        public Vector2 Vector = new Vector2();

        public float X {
            get {
                return Vector.x;
            }

            set {
                Vector.x = value;
            }
        }
        public float Y {
            get {
                return Vector.y;
            }

            set {
                Vector.y = value;
            }
        }
        public int CeilX {get {return Mathf.CeilToInt(X);}}
        public int CeilY {get {return Mathf.CeilToInt(Y);}}

        public Vertex() {
            X = 0;
            Y = 0;
        }

        public Vertex(Vector2 p) {
            this.X = p.x;
            this.Y = p.y;
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