using Godot;

namespace Box {
    /*
        地图加载器

        自定义的加载过程，可以是进行生成后加载或是直接加载现有的地图
    */
    public class SanboxLoader : Node {
        [Export]
        public string ArchiveName = "test";
        [Export]
        //为true时则遇到同名存档时分配新存档
        //为false时打开同名的存档
        public bool IsNewArchive = false;
        public Archive Archive {get; protected set;}

        public virtual void _Loader() {

        }

        public override void _Ready()
        {
            if(IsNewArchive) {
                ArchiveName = Archive.AllocArchiveName(ArchiveName);
            }
            Archive = new Archive(ArchiveName,GetParent<Sandbox>());
            _Loader();
        }

        public virtual void LoadRegion(SandboxRegion region){
            
        }

        public virtual void UnloadRegion(SandboxRegion region){
            
        }

        public virtual void SaveRegion(SandboxRegion region){
            
        }
    }
}