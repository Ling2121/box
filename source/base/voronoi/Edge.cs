using Godot;
using System.Collections.Generic;

namespace Box.VoronoiMap {
    public class Edge {
        public Vertex IndexPoint;
        public Vertex P1;
        public Vertex P2;
        public Vertex ShakeP1;
        public Vertex ShakeP2;
        public Cell C1;
        public Cell C2;
        public bool IsShake = false;

        public static void Shake(Vertex P1,Vertex P2,Vector2 P3,Vector2 P4,float min,float max,int number,RandomNumberGenerator random) {
            if(number == 0) return; 
            Vertex ins_p = new Vertex(P3 + ((P4 - P3) * random.RandfRange(min,max)));
            P1.Next = ins_p;
            P2.Up = ins_p;
            ins_p.Up = P1;
            ins_p.Next = P2;

            Vector2 NP3_1 = (P1.Position + P3) / 2;
            Vector2 NP4_1 = (P1.Position + P4) / 2;
            Edge.Shake(P1,ins_p,NP3_1,NP4_1,min,max,number - 1,random);

            Vector2 NP3_2 = (P2.Position + P3) / 2;
            Vector2 NP4_2 = (P2.Position + P4) / 2;
            Edge.Shake(ins_p,P2,NP3_2,NP4_2,min,max,number - 1,random);
        }

        public void Shake(float min,float max,int number,RandomNumberGenerator random) {
            if(IsShake || (C1 == null && C2 == null)) return;
            IsShake = true;
            ShakeP1 = new Vertex(P1.Position);
            ShakeP2 = new Vertex(P2.Position);
            Vector2 P3 = Vector2.Zero;
            Vector2 P4 = Vector2.Zero;
            Vector2 C = (ShakeP1.Position + ShakeP2.Position) / 2;
            if(C1 == null) {    
                P4 = C2.IndexPoint.Position;
                P3 = C + (C - P4); 
            } else if(C2 == null) {
                P3 = C1.IndexPoint.Position;
                P4 = C + (C - P3);
            } else {
                P3 = C1.IndexPoint.Position;
                P4 = C2.IndexPoint.Position;
            }

            Shake(ShakeP1,ShakeP2,P3,P4,min,max,number,random);
        }

        public Vector2[] GetShakeVectorArray() {
            List<Vector2> arr = new List<Vector2>();
            Vertex node = ShakeP1;
            while(node != ShakeP2) {
                arr.Add(node.Position);
                node = node.Next;
            }
            arr.Add(ShakeP2.Position);
            return arr.ToArray();
        }


    }
}