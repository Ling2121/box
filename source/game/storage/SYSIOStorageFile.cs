using System;
using System.Collections;
using System.Text;
using System.IO;

namespace Box {
    public class SYSIOStorageFile : IStorageFile {
        protected FileStream file_stream;
        protected BinaryWriter write;
        protected BinaryReader read;

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

        public SYSIOStorageFile() {}

        public SYSIOStorageFile(string path,StorageFileMode mode) {
            Open(path,mode);
        }

        public bool Open(string path,StorageFileMode mode) {
            try {
                file_stream = File.Open(path,System.IO.FileMode.OpenOrCreate);
                if(mode == StorageFileMode.Write) {
                    write = new BinaryWriter(file_stream);
                } else {
                    read = new BinaryReader(file_stream);
                }
            }catch(Exception) {
                return false;
            }
            return true;
        }

        public void Close() {
            if(write != null) write.Close();
            if(read != null) read.Close();
            file_stream.Close();
        }

        public void Flush() {
            if(write != null) {
                write.Flush();
            }
        }

        protected void WriteObjectValue(object value) {
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

        public void Write(int value) {
            write.Write((uint)value);
        }
        public void Write(double value) {
            write.Write(value);
        }

        public void Write(string value) {
            byte[] str = Encoding.UTF8.GetBytes(value);
            write.Write((uint)str.Length);
            write.Write(str);
        }

        public void Write(ArrayList values) {
            //数组长度
            write.Write((uint)values.Count);
            for(int i = 0;i < values.Count;i++)
            {
                WriteObjectValueItem(values[i]);
            }
        }

        public void Write(Hashtable values) {
            write.Write((uint)values.Count);
            foreach(string key in values.Keys) 
            {
                WriteItem(key);
                WriteObjectValueItem(values[key]);
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

        protected void WriteObjectValueItem(object value) {
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

        public void WriteItem(int value) {
            write.Write((byte)StorageItemType.Int);
            Write(value);
        }

        public void WriteItem(double value) {
            write.Write((byte)StorageItemType.Float);
            Write(value);
        }

        public void WriteItem(string value) {
            write.Write((byte)StorageItemType.String);

            Write(value);
        }

        public void WriteItem(ArrayList values) {
            write.Write((byte)StorageItemType.Array);
            Write(values);
        }

        public void WriteItem(Hashtable values) {
            write.Write((byte)StorageItemType.Hashtable);
            Write(values);
        }

        public void WriteItem(IStorage storage) {
            storage.StorageWrite(this);
        }

        public void TryReadStorageObjectItem(IStorage storage) {
            storage.StorageRead(this);
        }

        protected void TryReadType(StorageItemType type) {
            StorageItemType read_type = (StorageItemType)read.ReadByte();
            if(read_type != type) {
                throw new StorageFileReadError($"读取类型错误请检查文件格式及其内容是否正确 需要{type}，但读取的是{read_type}");
            }
        }

        protected int _ReadInt() {
            return read.ReadInt32();
        }

        public int TryReadIntItem() {
            TryReadType(StorageItemType.Int);
            return _ReadInt();
        }

        protected double _ReadFloat() {
            return read.ReadDouble();
        }

        public double TryReadFloatItem() {
            TryReadType(StorageItemType.Float);
            return _ReadFloat();
        }


        protected string _ReadString() {
            int len = read.ReadInt32();
            byte[] str = read.ReadBytes(len);
            return Encoding.UTF8.GetString(str);
        }

        public string TryReadStringItem() {
            TryReadType(StorageItemType.String);
            return _ReadString();
        }

        protected object ReadNext() {
            StorageItemType read_type = (StorageItemType)read.ReadByte();
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
            int len = read.ReadInt32();
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
            int len = read.ReadInt32();
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
    }
}