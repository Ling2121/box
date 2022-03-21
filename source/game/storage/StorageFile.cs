using System.Collections;
using System.Text;
using System;
using Godot;
using System.Collections.Generic;

namespace Box {

    public class StorageFileWriteError : Exception {
        public StorageFileWriteError(string msg):base(msg) {}
    }

    public class StorageFileReadError : Exception {
        public StorageFileReadError(string msg):base(msg) {}
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
            else {
                throw new StorageFileWriteError($"无法写入\"{value.GetType().Name}\"类型的值");
            }
        }

        public void Write(ArrayList values) {
            Store8((byte)StorageItemType.Array);
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

        public void Write(IStorage storage) {
            storage.StorageWrite(this);
        }

        public void ReadStorageObject(IStorage storage) {
            storage.StorageRead(this);
        }

        protected void TryReadType(StorageItemType type) {
            StorageItemType read_type = (StorageItemType)Get8();
            if(read_type != type) {
                throw new StorageFileReadError($"读取类型错误请检查文件格式及其内容是否正确 需要{type}，但读取的是{read_type}");
            }
        }

        protected int _ReadInt() {
            return (int)Get32();
        }

        public int ReadInt() {
            TryReadType(StorageItemType.Int);
            return  _ReadInt();
        }

        protected double _ReadFloat() {
            return GetDouble();
        }

        public double ReadFloat() {
            TryReadType(StorageItemType.Float);
            return _ReadFloat();
        }


        protected string _ReadString() {
            long len = (int)Get32();
            byte[] str = GetBuffer(len);
            return Encoding.UTF8.GetString(str);
        }

        public string ReadString() {
            TryReadType(StorageItemType.String);
            return _ReadString();
        }

        public object ReadNext() {
            StorageItemType read_type = (StorageItemType)Get8();
            switch(read_type) {
                case StorageItemType.Int : return (int)_ReadInt();
                case StorageItemType.Float : return (double)_ReadFloat();
                case StorageItemType.String : return (string)_ReadString();
                case StorageItemType.Array : return (ArrayList)_ReadArray();
                case StorageItemType.Hashtable : return (Hashtable)_ReadHashtable();
                default : {
                    throw new StorageFileReadError($"读取错误 -> 读取到的类型为{read_type}");
                }
            }
        }

        protected Hashtable _ReadHashtable() {
            Hashtable hashtable = new Hashtable();
            int len = (int)Get32();
            for(int i = 0;i<len;i++) {
                string key = ReadString();
                object value = ReadNext();

                hashtable[key] = value;
            }
            return hashtable;
        }

        public Hashtable ReadHashtable() {
            TryReadType(StorageItemType.Hashtable);
            return _ReadHashtable();
        }

        protected ArrayList _ReadArray() {
            ArrayList array = new ArrayList();
            int len = (int)Get32();
            for(int i = 0;i<len;i++) {
                object value = ReadNext();
                array.Add(value);
            }
            return array;
        }

        public ArrayList ReadArray() {
            TryReadType(StorageItemType.Array);
            return _ReadArray();
        }
    }
}