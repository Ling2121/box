using System;
using Godot;

namespace Box {
    public class NoiseGenerator : OpenSimplexNoise {

        public NoiseGenerator(int seed = 233) {
            Seed = seed;
        } 

        /*
            妙啊
            public float IslandNoise(Vector2 p,int width,int height) {
            float cx = width/ 2,cy = height / 2;
            float dx = p.x - cx,dy = p.y - cy;
            float d = (float)(Math.Sqrt(dx * dx + dy * dy)) * .6f;
            float n =(1 + GetNoise2dv(p)) / 2;
            return n - d;
        }
        */

        public float IslandNoise(Vector2 p,int width,int height) {
            float cx = width/ 2,cy = height / 2;
            float dx = (p.x - cx) / width,dy = (p.y - cy) /height;
            float d = (float)(Mathf.Sqrt(dx * dx + dy * dy) / 0.6);
            float n = (1 + GetNoise2dv(p)) / 2;
            return n - d;
        }
    }
}