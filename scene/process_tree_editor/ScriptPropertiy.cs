using Godot;

namespace Box.Scene.ProcessTreeEditor {
    public class ScriptPropertiy : PropertiyNode {
        public Button Button;
        public ScriptEditor ScriptEditor;

        public override void _AddToPanel(ProcessTreeEditor editor)
        {
            ScriptEditor = editor.ScriptEditor;
        }

        public override void _Ready()
        {
            Button = GetNode<Button>("Button");

            Button.Connect("button_down",this,nameof(_ButtonDown));
        }

        public void _ButtonDown() {
            ScriptEditor.Open(Value);
        }
    }
}