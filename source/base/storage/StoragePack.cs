using System;
using System.Collections;
using System.Collections.Generic;

namespace Box {
    //这个类用于安全化数据的存储，主要用来存储游戏中的实体。IStorage接口的存储与读取方式存在
    //着较大的隐患，在编写不当时会造成写入如读取的错位与遗漏，从而造成整个区块的损坏。
    //这个类通过预先定义的方式进行数据整理和规范，从而避免了以上的问题。
    public class StorgaePack {
        public class StoragePackDefinedError : Exception {}
        public class StoragePackReadError : Exception {}

        public class DataItem {
            public StorageItemType Type;
            public object Data;
        }

        protected bool defined = false;

        protected List<DataItem> items = new List<DataItem>();

        protected Dictionary<string,int> items_index = new Dictionary<string, int>();

        public object this[string name]
        {
            get { 
                return items[items_index[name]].Data;
            }

            set {
                items[items_index[name]].Data = value;
            }
        }

        public StorgaePack DefineItem(string name,StorageItemType type,object data) {
            if(defined) throw new StoragePackDefinedError();
            
            int index = items.Count;
            items.Add(new DataItem{
                Type = type,
                Data = data,
            });
            return this;
        }

        public StorgaePack DefineEnd() {
            defined = true;
            return this;
        } 

        public void Write(IStorageFile file) {
            if(!defined) throw new StoragePackDefinedError();
            foreach(DataItem item in items) {
                object value = item.Data;
                if(value is int)
                {
                    file.Write((int)value);
                }
                else if(value is double || value is float)
                {
                    file.Write((double)value);
                }
                else if(value is string)
                {
                    file.Write((string)value);
                }
                else if(value is ArrayList)
                {
                    file.Write((ArrayList)value);
                }
                else if(value is Hashtable)
                {
                    file.Write((Hashtable)value);
                }
            }
        }
        public void Read(IStorageFile file) {
            if(!defined) throw new StoragePackDefinedError();
            foreach(DataItem item in items) {
                switch(item.Type) {
                    case StorageItemType.Int : {
                        item.Data = file.ReadInt();
                    }break;
                    case StorageItemType.Float : {
                        item.Data = file.ReadFloat();
                    }break;
                    case StorageItemType.String : {
                        item.Data = file.ReadString();
                    }break;
                    case StorageItemType.Array : {
                        item.Data = file.ReadArray();
                    }break;
                    case StorageItemType.Hashtable : {
                        item.Data = file.ReadHashtable();
                    }break;

                    default : {
                        throw new StoragePackReadError();
                    };
                }
            }
        }
    }
}