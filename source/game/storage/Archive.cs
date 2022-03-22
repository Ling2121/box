using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Godot;

namespace Box {
    /*
        root
            L archives //所有存档
                L archive_name
                    L regions //区块目录
                        L region_name.dat
                            x : int
                            y : int
                            land_tiles : array<int>[Sandbox.REGION_SIZE * Sandbox.REGION_SIZE]
                            wall_tiles : array<int>[Sandbox.REGION_SIZE * Sandbox.REGION_SIZE]
                            tile_index_map : Hashtable<int,string> //Tile索引
                            objects : array<string>[*]
                        L ...
                    L objects //对象目录
                        L object_name.dat
                        L ...
                    L sanbox.dat
                        L name : string //存档名称
                        L width : int //地图宽度
                        L height : int //地图高度

    */
    public class Archive {        
        public const string REGIONS_DIRECTORY_NAME = "regions";
        public const string OBJECTS_DIRECTORY_NAME = "objects";
        public const string SANDBOX_CONF_FILE_NAME = "sandbox.dat";

        public static string AllocArchiveName(string name) {
            Directory dir = new Directory();
            string new_name = name;
            int i = 1;
            while(dir.Open($"{FileSystem.ARCHIVE_DIRECTORY}/{name}") != Error.Failed) {
                new_name = $"{name}({i})";
                i++;
            }
            return new_name;
        }
    
        public string Name {get;}
        protected Sandbox sandbox;
        protected GDIOStorageFile sandbox_info_file = new GDIOStorageFile();
        
        protected readonly string REGIONS_DIRECTORY_PATH;
        protected readonly string OBJECTS_DIRECTORY_PATH;

        public Archive() {}
        public Archive(string name,Sandbox s) {
            Name = name;
            sandbox = s;

            Directory archives_directory = new Directory();
            archives_directory.Open(FileSystem.ARCHIVE_DIRECTORY);

            REGIONS_DIRECTORY_PATH = $"{FileSystem.ARCHIVE_DIRECTORY}/{name}/{REGIONS_DIRECTORY_NAME}";
            OBJECTS_DIRECTORY_PATH = $"{FileSystem.ARCHIVE_DIRECTORY}/{name}/{OBJECTS_DIRECTORY_NAME}";

            archives_directory.MakeDir($"{FileSystem.ARCHIVE_DIRECTORY}/{name}");
            archives_directory.MakeDir(REGIONS_DIRECTORY_PATH);
            archives_directory.MakeDir(OBJECTS_DIRECTORY_PATH);

            sandbox_info_file = new GDIOStorageFile();
            sandbox_info_file.Open($"{FileSystem.ARCHIVE_DIRECTORY}/{name}/{SANDBOX_CONF_FILE_NAME}",File.ModeFlags.WriteRead);
            sandbox_info_file.WriteItem(name);
            sandbox_info_file.WriteItem(sandbox.Width);
            sandbox_info_file.WriteItem(sandbox.Height);
            sandbox_info_file.Close();
        }

        public string GetRegionFileName(SandboxRegion region) {
            return $"r{region.X}_{region.Y}.rdat";
        }

        public void SaveRegion(SandboxRegion region) {
            string file_name = $"{REGIONS_DIRECTORY_PATH}/{GetRegionFileName(region)}";
            GDIOStorageFile file = new GDIOStorageFile();
            file.Open(file_name,File.ModeFlags.WriteRead);
            if(file.IsOpen()) {
                file.WriteItem(region);
            }
            file.Close();
        }

        public bool ReadRegion(SandboxRegion region) {
            string file_name = $"{REGIONS_DIRECTORY_PATH}/{GetRegionFileName(region)}";
            GDIOStorageFile file = new GDIOStorageFile();
            file.Open(file_name,File.ModeFlags.Read);
            if(!file.IsOpen()) return false;

            file.TryReadStorageObjectItem(region);
            file.Close();
            return true;
        }

        public void SaveObject(string key,IStorage obj) {
            
        }
    }
}