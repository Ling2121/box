using System.Collections;
using System;

namespace Box {
        public class StorageFileWriteError : Exception {
        public StorageFileWriteError(string msg):base(msg) {}
    }

    public class StorageFileReadError : Exception {
        public StorageFileReadError(string msg):base(msg) {}
    }

    public enum StorageFileMode {
        Write,
        Read,
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


    public interface IStorageFile {
        bool Open(string path,StorageFileMode mode);
        void Close();
        void Flush();

        /*单纯的写入和读取*/
        void Write(int value);
        void Write(double value);
        void Write(string value);
        void Write(ArrayList values);
        void Write(Hashtable values);
        void Write(IStorage storage);

        int ReadInt();
        double ReadFloat();
        string ReadString();
        Hashtable ReadHashtable();
        ArrayList ReadArray();
        void ReadStorageObject(IStorage storage);
        /*带标签标记的写入和读取*/
        void WriteItem(int value);
        void WriteItem(double value);
        void WriteItem(string value);
        void WriteItem(ArrayList values);
        void WriteItem(Hashtable values);
        void WriteItem(IStorage storage);
        int TryReadIntItem();
        double TryReadFloatItem();
        string TryReadStringItem();
        Hashtable TryReadHashtableItem();
        ArrayList TryReadArrayItem();
        void TryReadStorageObjectItem(IStorage storage);
    }
}