using System;
using System.Collections.Generic;

namespace Box.ProcessTreeNodes {
    public class Execute : ProcessTreeNode {
        Action ExecuteFunction = () => {};

        public override Dictionary<string, ProcessTreeNodePortType> _Ports()
        {
            return new Dictionary<string, ProcessTreeNodePortType>{
                {"I0",ProcessTreeNodePortType.Input},
                {"Q0",ProcessTreeNodePortType.Output},
            };
        }

        public override Dictionary<string, int> _PortsLayer()
        {
            return new Dictionary<string, int> {
                {"I0",0},
                {"Q0",0},
            };
        }

        public override Dictionary<string, ProcessTreeNodePropertyType> _Properties()
        {
            return new Dictionary<string, ProcessTreeNodePropertyType>{
                {"Execute",ProcessTreeNodePropertyType.Script}
            };
        }

        public override void _Ready(params object[] args) {
            if(args.Length > 0) {
                _Setting("Execute",args[0]);
            }
        }
        public override void _InputHigh(string port) {
            OutputHigh("Q0");
            ExecuteFunction();
        }
        public override void _InputLow(string port){
            OutputLow("Q0");
        }
        public override void _Setting(string key,object value){
            if(key == "Execute") {
                ExecuteFunction = value as Action;
            }
        }
    }
}