using Godot;
using System;
using System.Collections.Generic;

namespace Box {
    public enum ProcessTreeNodePropertyType {
        Text,
        Script,
        Number,
        Boolean,
    }

    public class ProcessTreeNode {
        public const int LOW = 0;
        public const int HIGH = 1;
        public ProcessTree Tree;
        public Dictionary<string,ProcessTreeNodePort> Ports = new Dictionary<string, ProcessTreeNodePort>();
        public virtual Dictionary<string,ProcessTreeNodePortType> _Ports() {return new Dictionary<string, ProcessTreeNodePortType>();}
        //以下两个回调是用来生成GraphNode的
        //这个返回节点的所有属性信息
        public virtual Dictionary<string,ProcessTreeNodePropertyType> _Properties(){return new Dictionary<string, ProcessTreeNodePropertyType>();}
        //这个返回每个端口所在的层
        //GraphNode的节点分布是按照层来分布的
        // 如下所示
        //  ----------------
        //  0              -  <-第0层
        //  -              0  <-第1层
        //  0              -  <-第N层
        //  ----------------
        public virtual Dictionary<string,int> _PortsLayer(){return new Dictionary<string, int>();}
        public virtual void _Ready(params object[] args) {}
        public virtual void _InputHigh(string port) {}
        public virtual void _InputLow(string port){}
        public virtual void _Setting(string key,object value){}

        //为了保证通用性，请勿使用带参数的构造函数
        //以及非必要不要通过构造函数进行初始化，请使用_Ready函数进行
        public ProcessTreeNode() {
            var ports = _Ports();
            foreach(string port_name in ports.Keys) {
                var type = ports[port_name];
                var port = new ProcessTreeNodePort();
                port.Type = type;
                port.Name = port_name;
                port.Node = this;
                Ports[port_name] = port;
            }
        }

        public ProcessTreeNodePort GetPort(string name) {
            if(!Ports.ContainsKey(name)) return null;
            return Ports[name];
        }

        public void OutputLow(string port_name) {
            Output(LOW,port_name);
        }

        public void OutputHigh(string port_name) {
            Output(HIGH,port_name);
        }

        public void Output(int out_type,string port_name) {
            if(Ports.ContainsKey(port_name)) {
                var port = Ports[port_name];
                if(out_type == HIGH) {
                    foreach(var child in port.Children.Values) {
                        child.Node._InputHigh(child.Name);
                    }
                } else {
                    foreach(var child in port.Children.Values) {
                        child.Node._InputLow(child.Name);
                    }
                }
            }
        }

        public void Connect(string out_port_name,ProcessTreeNode node,string in_port_name) {
            var out_port = GetPort(out_port_name);
            var in_port = node.GetPort(in_port_name);
            if(out_port == null || in_port == null) return;
            out_port.AddChild(in_port);
        }

        public void Disconnect(string out_port_name,ProcessTreeNode node,string in_port_name) {
            var out_port = GetPort(out_port_name);
            var in_port = node.GetPort(in_port_name);
            if(out_port == null || in_port == null) return;
            out_port.RemoveChild(in_port);
        }
    }
}