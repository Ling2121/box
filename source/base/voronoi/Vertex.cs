using System.Security.Cryptography.X509Certificates;
using System.Collections.Generic;
using Godot;

namespace Box.VoronoiMap {
    public class Vertex {
        public static long ToHashValue(int x,int y) {
            return (long)x | ((long)y << 32);
        }

        public static long ToHashValue(float x,float y) {
            return (long)(x - 0.5f) | ((long)(y - 0.5f) << 32);
        }

        public static bool operator==(Vertex a,Vertex b) {
            return a.GetHashValue() == b.GetHashValue();
        }

        public static bool operator!=(Vertex a,Vertex b) {
            return a.GetHashValue() != b.GetHashValue();
        }

        public override bool Equals(object obj)
        {
            if(!(obj is Vertex)) return false;
            return (obj as Vertex).GetHashValue() == GetHashValue();
        }


        public Vector2 Position = new Vector2();
        public Vertex Next;
        public Vertex Up;

        public Dictionary<long,Edge> Edges = new Dictionary<long, Edge>();
        public Dictionary<long,Cell> Cells = new Dictionary<long, Cell>();

        public int x {
            get {
                return (int)Position.x;
            }

            set {
                Position.x = value;
            }
        }

        public int y {
            get {
                return (int)Position.y;
            }

            set {
                Position.y = value;
            }
        }

        public Vertex(){}

        public Vertex(int x,int y) {
            Position.x = x;
            Position.y = y;
        }

        public Vertex(float x,float y) {
            Position.x = x;
            Position.y = y;
        }

        public Vertex(Vector2 p) {
            Position.x = p.x;
            Position.y = p.y;
        }

        public long GetHashValue()
        {
            return ToHashValue(x,y);
        }
    }
}