using System.Collections.Generic;

namespace Box {
    public class NumberIndexPool {
        public int index = 0;
        public Dictionary<string,int> index_hash = new Dictionary<string,int>();
        public Dictionary<int,string> key_hash = new Dictionary<int,string>();
        public int GetIndex(string key) {
            if(!index_hash.ContainsKey(key)) {
                int new_key = index;
                index++;
                index_hash[key] = new_key;
                key_hash[new_key] = key;
                return new_key;
            }
            return index_hash[key];
        }

        public string GetKey(int index) {
            if(!key_hash.ContainsKey(index)) return "";
            return key_hash[index];
        }
    }
}