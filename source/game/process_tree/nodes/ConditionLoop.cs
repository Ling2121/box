using System;
using System.Collections.Generic;
using Godot;

namespace Box.ProcessTreeNodes {
    public class ConditionLoop : ProcessTreeNode {
        public class LoopNode : Node {
            public ConditionLoop Self;

            public LoopNode() {}

            public LoopNode(ConditionLoop self) {
                Self = self;
            }

            public override void _Process(float delta)
            {
                if(Self.Tree.IsInsideTree()) {
                    if(Self.ConditionFunction() && Self.Power) {
                        Self.LoopFunction();
                    }
                } else {
                    QueueFree();
                }
            }
        }

        LoopNode Node;


        public Func<bool> ConditionFunction = () => {return false;};
        public Action LoopFunction = ()=>{};
        public bool Power = false;

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
                {"Condition",ProcessTreeNodePropertyType.Script},
                {"Loop",ProcessTreeNodePropertyType.Script}
            };
        }

        public override void _Ready(params object[] args) {
            if(args.Length > 1) {
                _Setting("Condition",args[0]);
                _Setting("Loop",args[1]);
            }

            Node = new LoopNode(this);

            Tree.AddChild(Node);
        }
        public override void _InputHigh(string port) {
            OutputHigh("Q0");
            Power = true;
        }
        public override void _InputLow(string port){
            Power = false;
            OutputLow("Q0");
        }
        public override void _Setting(string key,object value){
            if(key == "Condition") {
                ConditionFunction = value as Func<bool>;
            }
        }
    }
}