using System.Reflection;
using Godot;
using System;
using System.Collections.Generic;

namespace Box {
    public class ProcessTreeConstructorNotNodeType : Exception {
        public ProcessTreeConstructorNotNodeType(string name):base(
            $"不存在 \"{name}\" 类型的节点"
        ){}
    }

    public class ProcessTreeConstructor {
        public ProcessTree Tree;
        protected Stack<ProcessTreeNode> ProcessStack = new Stack<ProcessTreeNode>();
        protected ProcessTreeNode CurrentProcess;
        public Dictionary<string,Type> NodeTypes = new Dictionary<string, Type>();
        public ProcessTreeConstructor(ProcessTree tree) {
            Tree = tree;
            Type type = typeof(ProcessTreeNode);
            Assembly assembly = Assembly.GetAssembly(type);
            foreach(Type t in assembly.GetTypes()){
                if(type.IsAssignableFrom(t)) {
                    NodeTypes[t.Name] = t;
                }
            }
        }

        public ProcessTreeNode CreateNode(string name) {
            if(!NodeTypes.ContainsKey(name)) return null;
            return (ProcessTreeNode)Activator.CreateInstance(NodeTypes[name]);
        }

        public ProcessTreeConstructor Node(string name,string out_port,string in_port) {
            var node = CreateNode(name);
            if(node == null) {
                throw new ProcessTreeConstructorNotNodeType(name);
            }
            CurrentProcess.Connect(out_port,node,in_port);
            node.Tree = Tree;
            CurrentProcess = node;
            return this;
        }

        public ProcessTreeConstructor Ready(params object[] args) {
            CurrentProcess._Ready(args);
            return this;
        }

        public ProcessTreeConstructor Setting(string key,object value) {
            CurrentProcess._Setting(key,value);
            return this;
        }

        public ProcessTreeConstructor Enter() {
            ProcessStack.Push(CurrentProcess);
            return this;
        }

        public ProcessTreeConstructor Exit() {
            CurrentProcess = ProcessStack.Pop();
            return this;
        }
    }


    public class ProcessTree : Node2D {
        public ProcessTreeNode Root = new ProcessTreeNode();
        public ProcessTreeConstructor Constructor;

        public ProcessTree() {
            Constructor = new ProcessTreeConstructor(this);
        }

        public void Start() {
            Root.Output(ProcessTreeNode.HIGH,"Root");
        }
    }
}