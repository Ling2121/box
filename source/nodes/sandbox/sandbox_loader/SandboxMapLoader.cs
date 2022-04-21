using Godot;

namespace Box {

    [ClassName(nameof(SandboxMapLoader))]
    public class SandboxMapLoader : SanboxLoader {
        public override void LoadRegion(SandboxRegion region){
            region.Status = SandboxRegionStatus.Loading;
            Archive.ReadRegion(region);
        }

        public override void UnloadRegion(SandboxRegion region){
            region.Status = SandboxRegionStatus.Unload;
            Archive.SaveRegion(region);
        }

        public override void SaveRegion(SandboxRegion region){
            Archive.SaveRegion(region);
        }
    }
}