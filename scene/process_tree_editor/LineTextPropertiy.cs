using Godot;

namespace Box.Scene.ProcessTreeEditor {
    public class LineTextPropertiy : PropertiyNode {
        public override void _Ready()
        {
            GetNode<LineEdit>("LineEdit").Connect("text_changed",this,nameof(_TextChanged));
        }

        public void _TextChanged(string text) {
            Value.String = text;
        }
    }
}