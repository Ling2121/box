using Godot;
using System;
using System.Collections.Generic;

namespace Box.WorldBuilds {
    public class WorldBuilder<T> : IWorldBuilder<T> where T :IBuildData {
        public List<IWorldBuildProcess<T>> Processes {get;} = new List<IWorldBuildProcess<T>>();
        protected Dictionary<string,int> ProcessIndexs {get;} = new Dictionary<string, int>();

        public IWorldBuildProcess<T> GetProcess(string name) {
            if(!ProcessIndexs.ContainsKey(name)) return null;
            return GetProcess(ProcessIndexs[name]);
        }

        public IWorldBuildProcess<T> GetProcess(int index) {
            if(index < 0 || index >= Processes.Count) return null;
            return Processes[index];
        }   

        public void AddProcess(string name,IWorldBuildProcess<T> process) {
            int index = Processes.Count;
            Processes.Add(process);
            ProcessIndexs[name] = index;
        }

        public void Build(T data) {
            foreach(IWorldBuildProcess<T> process in Processes) {
                process.Build(data);
            }
        }

    }
}