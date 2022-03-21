using System.Collections.Generic;

namespace Box {
    public class NumberIndexPool {
        public int index = 0;
        public Dictionary<string,int> IndexHash = new Dictionary<string,int>();
        public Dictionary<int,string> KeyHash = new Dictionary<int,string>();
        
        public void SetIndex(string key,int value) {
            IndexHash[key] = value;
            KeyHash[value] = key;
        }

        public int GetIndex(string key) {
            if(!IndexHash.ContainsKey(key)) {
                int new_index = index;
                index++;
                IndexHash[key] = new_index;
                KeyHash[new_index] = key;
                return new_index;
            }
            return IndexHash[key];
        }

        public string GetKey(int index) {
            if(!KeyHash.ContainsKey(index)) return "";
            return KeyHash[index];
        }
    }
}