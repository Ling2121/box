using Godot;
using System;
using System.Reflection;

namespace Box.Scene.ProcessTreeEditor {
    public class PlaceMenu : PopupMenu
    {
        ProcessTreeEditor Editor;

        public override void _Ready()
        {
            Type type = typeof(ProcessTreeNode);
            foreach(Type t in Assembly.GetAssembly(type).GetTypes()) {
                if(type.IsAssignableFrom(t)) {
                    if(t.Name != "Root") {
                        if(t.Name != nameof(ProcessTreeNode)) {
                            AddItem(t.Name);
                        }
                    }
                }
            }
        }

        public void _IdPressed(int id) {
            
        }

    }
}
