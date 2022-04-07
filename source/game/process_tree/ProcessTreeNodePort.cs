using Godot;
using System;
using System.Collections.Generic;

namespace Box {
    public enum ProcessTreeNodePortType {
        Input,
        Output,
    }

    public class ProcessTreeNodePort {
        public string Name;
        public ProcessTreeNodePortType Type;
        public ProcessTreeNodePort Parent;
        public Dictionary<ProcessTreeNodePort,ProcessTreeNodePort> Children = new Dictionary<ProcessTreeNodePort, ProcessTreeNodePort>();
        public ProcessTreeNode Node;

        public void AddChild(ProcessTreeNodePort port) {
            port.Parent = this;
            Children[port] = port;
        }

        public void RemoveChild(ProcessTreeNodePort port) {
            port.Parent = null;
            Children.Remove(port);
        }
    }
}