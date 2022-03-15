using Godot;

namespace Box.Test {
    public class BaseTestScene : Node2D {
        public T GetNodeOrCreate<T>(NodePath path) where T : Node,new() {
            T node = GetNodeOrNull<T>(path);
            if(node == null) {
                node = new T();
            }
            AddChild(node);
            return  node;
        }
    }
}