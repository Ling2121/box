namespace Box {
    public interface IStorage {
        void StorageWrite(IStorageFile file);
        void StorageRead(IStorageFile file);
    }
}