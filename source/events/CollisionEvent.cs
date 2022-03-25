using Godot;
using Box.Components;

namespace Box.Events {
    [Register(nameof(CollisionEvent))]
    public class CollisionEvent : IEvent {
        public bool IsEnterEvent(object a,object b) {
            return a is Node && b is Node;
        }
        public void _Execute(object a,object b) {
            Node a_node = a as Node;
            Node b_node = b as Node;

            CollisionEventComponent a_collision_event = a_node?.GetNodeOrNull<CollisionEventComponent>(nameof(CollisionEventComponent));
            a_collision_event?.EmitSignal(nameof(CollisionEventComponent.Collision),a,b);

            CollisionEventComponent b_collision_event = b_node?.GetNodeOrNull<CollisionEventComponent>(nameof(CollisionEventComponent));
            b_collision_event?.EmitSignal(nameof(CollisionEventComponent.Collision),b,a);
        }
    }
}