using Godot;

namespace Box {
    public static class EntityHelper {
        public static T GetComponent<T>(Node entity) where T: Node,IComponent {
            T component = entity.GetNodeOrNull<T>(typeof(T).Name);
            if(component == null) return null;
            return component;
        }
    }
}