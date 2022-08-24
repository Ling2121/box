using System;
using System.Collections.Generic;

namespace Box {
    public class SpaceHash<T> {
        protected Dictionary<int,Dictionary<int,Dictionary<T,T>>> Spaces = new Dictionary<int, Dictionary<int, Dictionary<T, T>>>();

        public readonly int SpaceSize;

        public SpaceHash() {}
        public SpaceHash(int space_size) {
            SpaceSize = space_size;
        }

        public (int x,int y) ToSpaceIndex(int x,int y) {
            return ((int)(x / SpaceSize),(int)(y / SpaceSize));
        }

        public Dictionary<T,T> GetSpzeFromIndex(int x,int y) {
            if(!Spaces.ContainsKey(x)) {
                Spaces[x] = new Dictionary<int, Dictionary<T,T>>();
            }
            if(!Spaces[x].ContainsKey(y)) {
                Spaces[x][y] = new Dictionary<T,T>();
            }
            return Spaces[x][y];
        }

        public Dictionary<T,T> GetSpace(int x,int y) {
            x = (int)(x / SpaceSize);
            y = (int)(y / SpaceSize);
            return GetSpzeFromIndex(x,y);
        }

        public void Add(int x,int y,T obj) {
            var space = GetSpace(x,y);
            space[obj] = obj;
        }

        public void Remove(int x,int y,T obj) {
            var space = GetSpace(x,y);
            space.Remove(obj);
        }
    }
}