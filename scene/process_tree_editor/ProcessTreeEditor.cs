using System.Security.Cryptography.X509Certificates;
using System.Linq;
using System.Reflection;
using Godot;
using System;
using System.Collections.Generic;
using GDDictionary = Godot.Collections.Dictionary;
using GDArray = Godot.Collections.Array;

namespace Box.Scene.ProcessTreeEditor {

    public class ProcessTreeEditor : Control{
        public GraphEdit Editor;
        public ScriptEditor ScriptEditor;
        public Panel MaskPanel;
        public PlaceMenu PlaceMenu;
        public FileDialog SaveWindow;
        public FileDialog ReadWindow;
        public FileDialog BuildWindow;
        public MenuButton FileMenu;
        public LineEdit TreeNameEdit;
        public ProcessTreeEditorNode SelectNode;
        public Dictionary<ProcessTreeEditorNode,ProcessTreeEditorNode> SelectNodes = new Dictionary<ProcessTreeEditorNode, ProcessTreeEditorNode>(); 
        public Node Properties;
        Vector2 PlacePosition = Vector2.Zero;

        public ProcessTreeEditorNode Root;

        public Dictionary<string,ProcessTreeNode> NodeTypes = new Dictionary<string, ProcessTreeNode>();

        public ProcessTreeEditor() {
            Type type = typeof(ProcessTreeNode);
            foreach(Type t in Assembly.GetAssembly(type).GetTypes()) {
                if(type.IsAssignableFrom(t)) {
                    NodeTypes[t.Name] = Activator.CreateInstance(t) as ProcessTreeNode;
                }
            }
        }

        public ProcessTreeEditorNode CreateNode(string name) {
            if(!NodeTypes.ContainsKey(name)) return null;
            return new ProcessTreeEditorNode(NodeTypes[name]);
        }

        public override void _Ready() {
            Properties = GetNode("VBoxContainer/HSplitContainer/PanelContainer/VBoxContainer/Properties");
            MaskPanel = GetNode<Panel>("MaskPanel");
            ScriptEditor = GetNode<ScriptEditor>("ScriptEditor");
            PlaceMenu = GetNode<PlaceMenu>("PlaceMenu");
            Editor = GetNode<GraphEdit>("VBoxContainer/HSplitContainer/Editor");
            SaveWindow = GetNode<FileDialog>("SaveWindow");
            ReadWindow = GetNode<FileDialog>("ReadWindow");
            BuildWindow = GetNode<FileDialog>("BuildWindow");
            FileMenu = GetNode<MenuButton>("VBoxContainer/HBoxContainer/PanelContainer/MenuContainer/FileMenu");
            TreeNameEdit = GetNode<LineEdit>("VBoxContainer/HSplitContainer/PanelContainer/VBoxContainer/TreeName");

            Root = CreateNode("Root");
            Editor.AddChild(Root);

            Editor.Connect("connection_request",this,nameof(_ConnectionRequest));
            Editor.Connect("disconnection_request",this,nameof(_DisconnectionRequest));
            Editor.Connect("node_selected",this,nameof(_NodeSelected));
            Editor.Connect("node_unselected",this,nameof(_NodeUnselected));
            Editor.Connect("delete_nodes_request",this,nameof(_DeleteNodesRequest));
            Editor.Connect("popup_request",this,nameof(_PopupRequest));

            ScriptEditor.Connect("about_to_show",this,nameof(_WindowPopup));
            ScriptEditor.Connect("popup_hide",this,nameof(_WindowHide));

            PlaceMenu.Connect("id_pressed",this,nameof(_PlaceMenuIdPressed));
            PlaceMenu.Connect("mouse_exited",this,nameof(_PlaceMenuMouseExited));

            SaveWindow.Connect("file_selected",this,nameof(_SaveFile));
            ReadWindow.Connect("file_selected",this,nameof(_ReadFile));
            BuildWindow.Connect("file_selected",this,nameof(_BuildFile));

            SaveWindow.Connect("about_to_show",this,nameof(_WindowPopup));
            SaveWindow.Connect("popup_hide",this,nameof(_WindowHide));

            ReadWindow.Connect("about_to_show",this,nameof(_WindowPopup));
            ReadWindow.Connect("popup_hide",this,nameof(_WindowHide));

            BuildWindow.Connect("about_to_show",this,nameof(_WindowPopup));
            BuildWindow.Connect("popup_hide",this,nameof(_WindowHide));

            FileMenu.GetPopup().Connect("id_pressed",this,nameof(_FileMenuSelect));
        }

        public void _FileMenuSelect(int id) {
            if(id == 0) {
                //save
                ReadWindow.Popup_();
            }
            if(id == 1) {
                //open
                SaveWindow.Popup_();
            }
            if(id == 2) {
                //build
                BuildWindow.Popup_();
            }
        }


        public void _SaveFile(string file_name) {
            Save(file_name);
        }

        public void _ReadFile(string file_name) {
            Read(file_name);
        }

        public void _BuildFile(string file_name) {
            GD.Print(file_name);
            Build(file_name);
        }

        public void _PlaceMenuIdPressed(int id) {
            string name = PlaceMenu.GetItemText(id);
            ProcessTreeEditorNode node = CreateNode(name);
            if(node != null) {
                node.Offset = PlacePosition;
                Editor.AddChild(node);
            }
            PlaceMenu.Hide();
        }

        public void _PlaceMenuMouseExited() {
            PlaceMenu.Hide();
        }

        public void _WindowPopup() {
            MaskPanel.Visible = true;
        }

        public void _WindowHide() {
            MaskPanel.Visible = false;
        }

        public void _ConnectionRequest(string from,int from_slot,string to,int to_slot) {
            var from_node = Editor.GetNode<ProcessTreeEditorNode>(from);
            var to_node = Editor.GetNode<ProcessTreeEditorNode>(to);
            var from_port = from_node.GetPort(from_slot,false);
            var to_port = to_node.GetPort(to_slot,true);
            from_port.AddChild(to_port);

            from_node.OutputConnects.Add((from,from_slot,to,to_slot));
            to_node.InputConnects.Add((from,from_slot,to,to_slot));
            Editor.ConnectNode(from,from_slot,to,to_slot);
        }

        protected void RemoveNodeConnect(List<(string, int, string, int)> list,string from,int from_slot,string to,int to_slot) {
            list.Remove(list.Find(item => {
                return item.Item1 == from && item.Item2 == from_slot 
                && item.Item3 == to && item.Item4 == to_slot;
            }));
        }

        protected void RemoveNodeConnect(List<(string, int, string, int)> list,(string, int, string, int) v) {
            list.Remove(list.Find(item => {
                return item.Item1 == v.Item1 && item.Item2 == v.Item2 
                && item.Item3 == v.Item3 && item.Item4 == v.Item4;
            }));
        }

        public void _DisconnectionRequest(string from,int from_slot,string to,int to_slot) {
            var from_node = Editor.GetNode<ProcessTreeEditorNode>(from);
            var to_node = Editor.GetNode<ProcessTreeEditorNode>(to);
            var from_port = from_node.GetPort(from_slot,false);
            var to_port = to_node.GetPort(to_slot,true);
            from_port.AddChild(to_port);

            RemoveNodeConnect(from_node.OutputConnects,from,from_slot,to,to_slot);
            RemoveNodeConnect(to_node.InputConnects,from,from_slot,to,to_slot);
            Editor.DisconnectNode(from,from_slot,to,to_slot);
        }

        public void ClearProperties() {
            foreach(Node child in Properties.GetChildren()){
                Properties.RemoveChild(child);
            }
        }

        public void ChangeProperties(ProcessTreeEditorNode node) {
            ClearProperties();
            foreach(var propertiy in node.PropertiesNodes.Values) {
                propertiy._AddToPanel(this);
                Properties.AddChild(propertiy);
            }
        }

        public void _NodeSelected(Node node) {
            SelectNode = node as ProcessTreeEditorNode;
            SelectNodes[SelectNode] = SelectNode;
            ChangeProperties(SelectNode);
        }

        public void _NodeUnselected(Node node) {
            SelectNodes.Remove(node as ProcessTreeEditorNode);
            if(SelectNodes.Count > 1 || SelectNodes.Count == 0) {
                ClearProperties();
            }
        }

        public void _DeleteNodesRequest() {
            SelectNode = null;
            var arr = SelectNodes.Values.ToArray();
            foreach(ProcessTreeEditorNode node in arr) {
                SelectNodes.Remove(node);
                foreach(var port in node.Ports.Values) {
                    if(port.Parent != null) {
                        port.Parent.RemoveChild(port);
                    }
                }

                foreach(var item in node.OutputConnects) {
                    //item.Item1指向自己
                    ProcessTreeEditorNode output_node = Editor.GetNode<ProcessTreeEditorNode>(item.Item3);
                    RemoveNodeConnect(output_node.InputConnects,item);
                    Editor.DisconnectNode(item.Item1,item.Item2,item.Item3,item.Item4);
                }

                foreach(var item in node.InputConnects) {
                    ProcessTreeEditorNode input_node = Editor.GetNode<ProcessTreeEditorNode>(item.Item1);
                    RemoveNodeConnect(input_node.OutputConnects,item);
                    Editor.DisconnectNode(item.Item1,item.Item2,item.Item3,item.Item4);
                }
            }
            ClearProperties();
            //全部断开连接后再删除
            foreach(ProcessTreeEditorNode node in arr) {
                if(node != Root) {
                    node.QueueFree();
                }
            }

            Editor.Update();
        }

        public void _PopupRequest(Vector2 p) {
            PlaceMenu.Popup_();
            PlacePosition = Editor.GetLocalMousePosition() + Editor.ScrollOffset;
            PlaceMenu.RectPosition = p;
        }

        protected void SaveNode(File file,ProcessTreeEditorNode node) {
            GDArray connects = new GDArray();
            GDDictionary properties = new GDDictionary();
            GDDictionary dictionary = new GDDictionary{
                {"type",node.TypeName},
                {"connects",connects},
                {"properties",properties}
            };

            foreach(var connect in node.OutputConnects) {
                connects.Add(new GDArray{
                    connect.Item1,connect.Item2,
                    connect.Item3,connect.Item4
                });
            }

            foreach(var item in node.Properties) {
                GDArray property = new GDArray();
                property.Add((int)item.Value.Type);
                property.Add(item.Value.Value.String);
                properties.Add(item.Key,property);
            }

            file.StoreString($"\"{node.Name}\" : ");
            file.StoreString(JSON.Print(dictionary) + ",");
        }

        public void Save(string file_name) {
            File file = new File();

            if(file.Open(file_name,File.ModeFlags.WriteRead) == Error.Ok) {
                file.StoreString("{ ");
                foreach(Node node in Editor.GetChildren()) {
                    if(node is ProcessTreeEditorNode) {
                        SaveNode(file,node as ProcessTreeEditorNode);
                    }
                }
                file.StoreString(" }");

                file.Close();
            }
        }

        public void Read(string file_name) {

        }

        protected string GenSpace(int layer) {
            string str = "";
            for(int i = 0;i<layer;i++){
                str += "    ";
            }
            return str;
        }

        protected void BuildNode(File file,ProcessTreeEditorNode node,string space,int layer) {
            string add_space = GenSpace(layer);
            file.StoreString($"{space}{add_space}.Enter()");
            foreach(var item in node.OutputConnects) {
                ProcessTreeEditorNode input_node = Editor.GetNode<ProcessTreeEditorNode>(item.Item3);
                ProcessTreeNodePort output_port = node.GetLayer(item.Item2).Output;
                ProcessTreeNodePort input_port = input_node.GetLayer(item.Item4).Input;
                file.StoreString($"{space}{add_space}.Node({node.TypeName},{output_port.Name},{input_port.Name})");
                if(input_node.Properties.Count > 0) {
                    string add_space2 = GenSpace(layer + 1);
                    file.StoreString($"{space}{add_space}.Enter()");
                    foreach(var propertiy in input_node.Properties) {
                        file.StoreString($"{space}{add_space2}.Setting({propertiy.Key},{propertiy.Value.Value.String})");
                    }
                    file.StoreString($"{space}{add_space}.Exit()");
                }
                file.StoreString($"{space}{add_space}.Ready()");
                BuildNode(file,input_node,space,layer + 1);
            }
            file.StoreString($"{space}{add_space}.Exit()");
        }

        public void Build(string file_name) {
            File file = new File();
            string code = "";

            if(file.Open(file_name,File.ModeFlags.Read) == Error.Ok) {
                code = file.GetAsText();
                file.Close();
            }


            string start_str = "/**[ProcessTreeBuild].Start**/";
            string end_str = "/**[ProcessTreeBuild].End**/";
            int len = code.Length;

            int start_p = code.IndexOf(start_str);
            int end_p = code.IndexOf(end_str);
            file = new File();
            if(start_p != -1 && end_p != -1){
                if(file.Open(file_name,File.ModeFlags.Write) == Error.Ok){
                    string space = "";
                    int p = start_p - 1;
                    while(p > 0 && code[p] == ' ') {
                        space += ' ';
                        p--;
                    }


                    string up_str = code.Substr(0,start_p + start_str.Length);
                    string down_str = code.Substr(end_p,len);    
                    string tree_name = TreeNameEdit.Text;


                    file.StoreString(up_str);

                    file.StoreString($"\n{space}var {tree_name} = new {nameof(ProcessTree)}();");
                    
                    

                    file.StoreString($"{space}{down_str}");
                    file.Close();
                }
            }
        }
    }
}