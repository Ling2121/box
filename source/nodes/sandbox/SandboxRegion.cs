using System;
using Godot;

namespace Box {
    public enum SandboxRegionStatus {
        Loading,
        Stop,
        Unload,
        NotLoad,
    }

    public class SandboxRegion : Node2D {

        public SandboxRegionStatus Status = SandboxRegionStatus.NotLoad;

        public override void _Process(float delta)
        {
            //对区块进行回收判断

        }
    }
}