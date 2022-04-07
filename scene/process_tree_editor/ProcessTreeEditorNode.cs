using System;
using Godot;
using System.Collections.Generic;

namespace Box.Scene.ProcessTreeEditor {
    public class ProcessTreeEditorNode : GraphNode{
        public class ConnectInfo {
            
        }
        
        public class Layer {
            public int LayerID;
            public ProcessTreeNodePort Input;
            public ProcessTreeNodePort Output;
        }


        public class Propertiy {
            public ProcessTreeNodePropertyType Type;
            public StringLinker Value = new StringLinker();
        }
        public Dictionary<string,Propertiy> Properties = new Dictionary<string, Propertiy>();
        public Dictionary<string,PropertiyNode> PropertiesNodes = new Dictionary<string, PropertiyNode>();
        public Dictionary<string,ProcessTreeNodePort> Ports = new Dictionary<string, ProcessTreeNodePort>();
        public SortedDictionary<int,Layer> Layers = new SortedDictionary<int, Layer>();
        public List<(string,int,string,int)> OutputConnects = new List<(string, int, string, int)>();
        public List<(string,int,string,int)> InputConnects = new List<(string, int, string, int)>();
        public bool IsHover = false;
        public string TypeName;
        public ProcessTreeEditorNode() {}

        public ProcessTreeEditorNode(ProcessTreeNode tree_node) {
            Title = tree_node.GetType().Name;
            TypeName = Title;
            RectSize = new Vector2(120,60);

            var ports = tree_node._Ports();
            var ports_layer = tree_node._PortsLayer();
            var properties = tree_node._Properties();

            foreach(string name in ports.Keys) {
                var type = ports[name];
                var layer_id = ports_layer[name];
                var port = new ProcessTreeNodePort();
                port.Type = type;
                port.Name = name;
                Layer layer;
                if(!Layers.ContainsKey(layer_id)) {
                    layer = new Layer();
                    layer.LayerID = layer_id;
                    Layers[layer_id] = layer;
                } else {
                    layer = Layers[layer_id];
                }
                if(port.Type == ProcessTreeNodePortType.Input) {
                    layer.Input = port;
                } else {
                    layer.Output = port;
                }
                Ports[name] = port;
            }

            foreach(int layer_id in Layers.Keys) {
                Layer layer = Layers[layer_id];
                SetSlot(
                    layer_id,
                    layer.Input != null,1,Colors.Green,
                    layer.Output!= null,1,Colors.Yellow
                );
                AddChild(CreateSlotLabel(layer.Input?.Name,layer.Output?.Name));
            }

            foreach(var item in properties) {
                var propertiy = new Propertiy();
                propertiy.Type = item.Value;
                propertiy.Value.String = "";
                Properties[item.Key] = propertiy;
                PropertiyNode node = null;
                if(propertiy.Type == ProcessTreeNodePropertyType.Script) {
                    node = GD.Load<PackedScene>("res://scene/process_tree_editor/ScriptPropertiy.tscn").Instance<PropertiyNode>();
                }
                else {
                    node = GD.Load<PackedScene>("res://scene/process_tree_editor/LineTextPropertiy.tscn").Instance<PropertiyNode>();
                }
                node.Value = propertiy.Value;
                Label label = node.GetNode<Label>("Label");
                label.Text = item.Key;
                PropertiesNodes[item.Key] = node;
            }
        }

        public override void _Ready()
        {
            Connect("mouse_entered",this,nameof(_MouseEntered));
            Connect("mouse_exited",this,nameof(_MouseExited));
        }

        public void _MouseEntered() {
            IsHover = true;
        }

        public void _MouseExited() {
            IsHover = false;
        }

        private HBoxContainer CreateSlotLabel(string input_name,string output_name) {
            HBoxContainer container = new HBoxContainer();
            int flags = (int)(SizeFlags.Expand | SizeFlags.Fill);
            container.SizeFlagsHorizontal = flags;

            if(input_name != "") {
                Label label = new Label();
                label.SizeFlagsHorizontal = flags;
                label.Text = input_name;
                label.Align = Label.AlignEnum.Left;
                container.AddChild(label);
            }

            if(output_name != "") {
                Label label = new Label();
                label.SizeFlagsHorizontal = flags;
                label.Text = output_name;
                label.Align = Label.AlignEnum.Right;
                container.AddChild(label);
            }

            return container;
        }

        public Layer GetLayer(int layer_id) {
            if(!Layers.ContainsKey(layer_id)) {
                return null;
            }
            return Layers[layer_id];
        }

        public ProcessTreeNodePort GetPort(int layer_id,bool is_input) {
            Layer layer = GetLayer(layer_id);
            if(layer == null) return null;
            if(is_input) {
                return layer.Input;
            }
            return layer.Output;
        }

    }
}