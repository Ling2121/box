using System.Collections.Generic;
using Godot;

namespace Box.Scene.ProcessTreeEditor {
    public class ScriptEditor : WindowDialog {
        public StringLinker Code;
        public TextEdit Edit;
        public Dictionary<string,GraphNode> GraphNodes = new Dictionary<string, GraphNode>();

        public override void _Ready()
        {
            Edit = GetNode<TextEdit>("TextEdit");
            Connect("popup_hide",this,nameof(_Hide));
        }
        
        public void Open(StringLinker code) {
            Popup_();
            Code = code;
            Edit.Text = Code.String;
        }

        public void _Hide() {
            if(Code != null) {
                Code.String = Edit.Text;
                Edit.Text = "";
            }
        }
    }
}