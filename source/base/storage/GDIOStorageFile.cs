using System.Collections;
using System.Text;
using System;

namespace Box {
    public class GDIOStorageFile : Godot.File,IStorageFile {

        public bool Open(string path,StorageFileMode mode) {
            bool ret = true;
            if(mode == StorageFileMode.Write) {
                ret = Open(path,ModeFlags.WriteRead) == Godot.Error.Ok;
            }
            if(mode == StorageFileMode.Read) {
                ret = Open(path,ModeFlags.Read) == Godot.Error.Ok;
            }

            return ret;
        }

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


        public void WriteItem(int value) {
            Store8((byte)StorageItemType.Int);
            Store32((uint)value);
        }

        public void WriteItem(double value) {
            Store8((byte)StorageItemType.Float);
            StoreDouble(value);
        }

        public void WriteItem(string value) {
            byte[] str = Encoding.UTF8.GetBytes(value);
            Store8((byte)StorageItemType.String);
            Store32((uint)str.Length);
            StoreBuffer(str);
        }

        public void WriteItem(object value) {
            if(value is int)
            {
                WriteItem((int)value);
            }
            else if(value is double || value is float)
            {
                WriteItem((double)value);
            }
            else if(value is string)
            {
                WriteItem((string)value);
            }
            else if(value is ArrayList)
            {
                WriteItem((ArrayList)value);
            }
            else if(value is Hashtable)
            {
                WriteItem((Hashtable)value);
            }
            else {
                throw new StorageFileWriteError($"无法写入\"{value.GetType().Name}\"类型的值");
            }
        }

        public void WriteItem(ArrayList values) {
            Store8((byte)StorageItemType.Array);
            //数组长度
            Store32((uint)values.Count);
            for(int i = 0;i < values.Count;i++)
            {
                WriteItem(values[i]);
            }
        }

        public void WriteItem(Hashtable values) {
            Store8((byte)StorageItemType.Hashtable);
            Store32((uint)values.Count);
            foreach(string key in values.Keys) 
            {
                WriteItem(key);
                WriteItem(values[key]);
            }
        }

        public void WriteItem(IStorage storage) {
            storage.StorageWrite(this);
        }

        public void TryReadStorageObjectItem(IStorage storage) {
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

        public int TryReadIntItem() {
            TryReadType(StorageItemType.Int);
            return  _ReadInt();
        }

        protected double _ReadFloat() {
            return GetDouble();
        }

        public double TryReadFloatItem() {
            TryReadType(StorageItemType.Float);
            return _ReadFloat();
        }


        protected string _ReadString() {
            long len = (int)Get32();
            byte[] str = GetBuffer(len);
            return Encoding.UTF8.GetString(str);
        }

        public string TryReadStringItem() {
            TryReadType(StorageItemType.String);
            return _ReadString();
        }

        protected object ReadNext() {
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
                string key = TryReadStringItem();
                object value = ReadNext();

                hashtable[key] = value;
            }
            return hashtable;
        }

        public Hashtable TryReadHashtableItem() {
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

        public ArrayList TryReadArrayItem() {
            TryReadType(StorageItemType.Array);
            return _ReadArray();
        }


        public void Write(int value) {
            Store32((uint)value);
        }
        public void Write(double value) {
            StoreDouble(value);
        }

        public void Write(string value) {
            byte[] str = Encoding.UTF8.GetBytes(value);
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
            //数组长度
            Store32((uint)values.Count);
            for(int i = 0;i < values.Count;i++)
            {
                WriteItem(values[i]);
            }
        }

        public void Write(Hashtable values) {
            Store32((uint)values.Count);
            foreach(string key in values.Keys) 
            {
                WriteItem(key);
                WriteItem(values[key]);
            }
        }

        public void Write(IStorage storage) {
            storage.StorageWrite(this);
        }
        public int ReadInt() {
            return _ReadInt();
        }
        public double ReadFloat() {
            return _ReadFloat();
        }
        public string ReadString() {
            return _ReadString();
        }
        public Hashtable ReadHashtable() {
            return _ReadHashtable();
        }
        public ArrayList ReadArray() {
            return _ReadArray();
        }
        public void ReadStorageObject(IStorage storage) {
            storage.StorageRead(this);
        }
    }
}