namespace Box {
    public interface IStorage {
        void StorageWrite(StorageFile file);
        void StorageRead(StorageFile file);
    }
}