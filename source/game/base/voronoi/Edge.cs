using System;
using System.Collections.Generic;
using Godot;

namespace Box.VoronoiMap {
    public class Edge {
        public csDelaunay.Edge BasicEdge;
        //从 P1和P2 的中心点通过Voronoi.GetPointHash生成
        public long Index {get;protected set;}
        public Cell Cell1;
        public Cell Cell2;
        public Vertex P1;
        public Vertex P2;

        public Vertex ShakeP1;
        public Vertex ShakeP2;
        public bool IsShake = false;

        public Edge(long index) {
            Index = index;
        }

        public Vector2[] GetShakeVertexesVectorArray(){
            if(!IsShake) return null;
            List<Vector2> arr = new List<Vector2>();
            Vertex node = ShakeP1;
            
            while(node != ShakeP2) {
                arr.Add(new Vector2(node.X,node.Y));
                node = node.Next;
            }

            arr.Add(new Vector2(ShakeP2.X,ShakeP2.Y));

            return arr.ToArray();
        }

        public int[] GetShakeVertexesIntArray() {
            if(!IsShake) return null;
            Vector2[] vec_arr = GetShakeVertexesVectorArray();
            int[] arr = new int[vec_arr.Length * 2];
            int i = 0;
            foreach(Vector2 p in vec_arr) {
                arr[i] = (int)p.x;
                arr[i + 1] = (int)p.y;
                i += 2;
            }
            return arr;
        }
    }
}