using Godot;

namespace Box {
    public static class EntityHelper {
        public static T GetComponent<T>(Node entity) where T: Node,IComponent {
            T component = entity.GetNodeOrNull<T>(nameof(T));
            if(component == null) return null;
            return component;
        }
    }
}