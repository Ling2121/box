using System.Collections;
using System.Text;
using System;
using Godot;
using System.Collections.Generic;

namespace Box {

    public class StorageFileWriteError : Exception {
        public StorageFileWriteError(string msg):base(msg) {}
    }

    public enum StorageItemType {
        Label = 0,
        Null,
        Int,
        Float,
        String,
        Array,
        Hashtable,
    }

    public class StorageFile : File {

         public StorageItemType GetValueType(object value) {
            if(value is int)
            {
                return StorageItemType.Int;
            }
            if((value is double) || (value is float))
            {
                return StorageItemType.Float;
            }
            if(value is string)
            {
                return StorageItemType.String;
            }
            if(value is ArrayList)
            {
                return StorageItemType.Array;
            }
            if(value is Hashtable)
            {
                return StorageItemType.Hashtable;
            }
            return StorageItemType.Null;
        }


        public void Write(int value) {
            Store8((byte)StorageItemType.Int);
            Store32((uint)value);
        }

        public void Write(double value) {
            Store8((byte)StorageItemType.Float);
            StoreDouble(value);
        }

        public void Write(string value) {
            byte[] str = Encoding.UTF8.GetBytes(value);
            Store8((byte)StorageItemType.String);
            Store32((uint)str.Length);
            StoreBuffer(str);
        }

        public void Write(object value) {
            if(value is int)
            {
                Write((int)value);
            }
            else if(value is double || value is float)
            {
                Write((double)value);
            }
            else if(value is string)
            {
                Write((string)value);
            }
            else if(value is ArrayList)
            {
                Write((ArrayList)value);
            }
            else if(value is Hashtable)
            {
                Write((Hashtable)value);
            }
            
            throw new StorageFileWriteError($"无法写入\"{value.GetType().Name}\"类型的值");
        }

        public void Write(ArrayList values) {
            Store8((byte)StorageItemType.Array);
            //数组类型
            if(values.Count == 0)
            {
                Store8((byte)StorageItemType.Null);
            }
            else
            {
                Store8((byte)GetValueType(values[0]));
            }
            //数组长度
            Store32((uint)values.Count);
            for(int i = 0;i < values.Count;i++)
            {
                Write(values[i]);
            }
        }

        public void Write(Hashtable values) {
            Store8((byte)StorageItemType.Hashtable);
            Store32((uint)values.Count);
            foreach(string key in values.Keys) 
            {
                Write(key);
                Write(values[key]);
            }
        }

    }
}