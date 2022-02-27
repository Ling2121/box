using System.Collections.Generic;

namespace Box.VoronoiMap {
    public class Edge {
        public csDelaunay.Edge BasicEdge;
        //从 P1和P2 的中心点通过Voronoi.GetPointHash生成
        public long Index {get;protected set;}
        public Cell Cell1;
        public Cell Cell2;
        public Vertex P1;
        public Vertex P2;

        public Edge(long index) {
            Index = index;
        }
    }
}