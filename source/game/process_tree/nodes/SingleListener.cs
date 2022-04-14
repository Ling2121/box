using System.Security.Cryptography;
using System;
using System.Collections.Generic;

namespace Box.ProcessTreeNodes {
    public class SingleListener : ProcessTreeNode {
        public class ListenerObject : Godot.Node {
            public SingleListener Self;

            public void _Connect() {
                if(Self.Power) {
                    Self.OutputHigh("Q0");
                }
            }
        }

        public ListenerObject Listener = new ListenerObject();
        public Godot.Object SignalObject;
        public string SignalName;
        public bool Power = false;

        // ~SingleListener() {
        //     if(SignalObject != null) {
        //         SignalObject.Disconnect(SignalName,Listener,nameof(ListenerObject._Connect));
        //     }
        //     if(Listener != null) {
        //         Listener.Free();
        //     }
        // }


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
                {"SignalObject",ProcessTreeNodePropertyType.Script},
                {"SignalName",ProcessTreeNodePropertyType.Text}
            };
        }

        public override void _Ready(params object[] args) {
            if(args.Length > 1) {
                if(args[0] is Func<Godot.Object>) {
                    _Setting("SignalObject",args[0]);
                }
                else if (args[0] is Godot.Object) {
                    _Setting("SignalObject",args[0]);
                }
                _Setting("SignalName",args[1]);
            }
            
            Listener.Self = this;
            SignalObject.Connect(SignalName,Listener,nameof(ListenerObject._Connect));
        }
        public override void _InputHigh(string port) {
            Power = true;
        }
        public override void _InputLow(string port){
            Power = false;
            OutputLow("Q0");
        }
        public override void _Setting(string key,object value){
            if(key == "SignalObject") {
                if(value is Func<Godot.Object>) {
                    SignalObject = (value as Func<Godot.Object>)();
                } else {
                    SignalObject = value as Godot.Object;
                }
            }
            if(key == "SignalName") {
                SignalName = value as string;
            }
        }
    }
}