using System.Diagnostics;
using System.Collections.Generic;
using Godot;

namespace Box {
    public interface IWorldBuilder<BDType> where BDType : IBuildData {
        List<IWorldBuildProcess<BDType>> Processes {get;}
        IWorldBuildProcess<BDType> GetProcess(string name);
        IWorldBuildProcess<BDType> GetProcess(int index);
        void AddProcess(string name,IWorldBuildProcess<BDType> process);
        void Build(BDType data);
    }
    
}