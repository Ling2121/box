using Godot;
using System;
using System.Collections.Generic;

namespace Box.Components {
    [ClassName(nameof(EventListeningComponent))]
    public class EventListeningComponent : Node2D {
        public Dictionary<string,IEventListener> Listeners = new Dictionary<string, IEventListener>();

        public override void _EnterTree()
        {
            Node parent = GetParent();
            foreach(Node node in GetChildren()) {
                if(node is IEventListener) {
                    IEventListener listener = node as IEventListener;
                    listener.Entity = parent;
                    Listeners[node.Name] = listener;
                }
            }
        }

        public override void _Ready()
        {
            foreach(var listener in Listeners.Values) {
                if(listener.IsRemove()) {
                    RemoveChild((Node)listener);
                }
                listener._InitListener();
            }
        }

        public T GetListener<T>() where T : IEventListener {
            Type type = typeof(T);
            if(!Listeners.ContainsKey(type.Name)) return default(T);
            return (T)Listeners[type.Name];
        }
    }
}