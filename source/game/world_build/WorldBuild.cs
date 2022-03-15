using System;
using System.Collections.Generic;

namespace Box {
    public class SettingProcessError : Exception {
        public SettingProcessError(string msg):base(msg){}
    }

    public class WorldBuild {
        protected Dictionary<string,IWorldBuildProcess> build_process_ports = new Dictionary<string,IWorldBuildProcess>();
        protected List<IWorldBuildProcess> build_process = new List<IWorldBuildProcess>();

        public void AddProcess(string name,IWorldBuildProcess process) {
            build_process_ports[name] = process;
        }

        public void SettingProcess(string[] process_list){
            build_process.Clear();
            foreach(string process_name in process_list) {
                if(!build_process_ports.ContainsKey(process_name)) {
                    throw new SettingProcessError($"不存在\"{process_name}\"过程");
                }
                IWorldBuildProcess process = build_process_ports[process_name];
                build_process.Add(process);
            }
        }

        public Table Build(Table table) {
            foreach(IWorldBuildProcess process in build_process) {
                table = process.Build(table);   
            }
            return table; 
        }
    }
}