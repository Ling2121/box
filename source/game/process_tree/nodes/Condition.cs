using System;
using System.Collections.Generic;

namespace Box.ProcessTreeNodes {
    public class Condition : ProcessTreeNode {
        Func<bool> ConditionFunction = () => {return false;};

        public override Dictionary<string, ProcessTreeNodePortType> _Ports()
        {
            return new Dictionary<string, ProcessTreeNodePortType>{
                {"I0",ProcessTreeNodePortType.Input},
                {"Q0",ProcessTreeNodePortType.Output},
                {"Else",ProcessTreeNodePortType.Output}
            };
        }

        public override Dictionary<string, int> _PortsLayer()
        {
            return new Dictionary<string, int> {
                {"I0",0},
                {"Q0",0},
                {"Else",1},
            };
        }

        public override Dictionary<string, ProcessTreeNodePropertyType> _Properties()
        {
            return new Dictionary<string, ProcessTreeNodePropertyType>{
                {"Condition",ProcessTreeNodePropertyType.Script}
            };
        }

        public override void _Ready(params object[] args) {
            if(args.Length > 0) {
                _Setting(nameof(ConditionFunction),args[0]);
            }
        }
        public override void _InputHigh(string port) {
            if(ConditionFunction()){
                OutputHigh("Q0");
            }
            else {
                OutputHigh("Else");
                OutputLow("Else");
            }
        }
        public override void _InputLow(string port){
            OutputLow("Q0");
            OutputLow("Else");
        }
        public override void _Setting(string key,object value){
            if(key == "Condition") {
                ConditionFunction = value as Func<bool>;
            }
        }
    }
}