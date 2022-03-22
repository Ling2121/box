using Godot;

namespace Box {
    public class FileSystem : Singleton<FileSystem> {
        public const string ROOT_DIRECTORY = "user://";
        public const string ARCHIVE_DIRECTORY = "user://archives";
        public const string MAP_DIRECTORY = "user://maps";
        public const string MODE_DIRECTORY = "user://modes";
        public Directory RootDirectory = new Directory();

        protected void CreateDirectory(string path) {
            Directory directory = new Directory();
            if(directory.Open(path) != Error.Ok) {
                RootDirectory.MakeDir(path);
            }
        }

        public void Init() {
            RootDirectory = new Directory();
            RootDirectory.Open(ROOT_DIRECTORY);
            CreateDirectory(ARCHIVE_DIRECTORY);
            CreateDirectory(MAP_DIRECTORY);
            CreateDirectory(MODE_DIRECTORY);
        }
    }
}