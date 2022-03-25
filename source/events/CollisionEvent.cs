using Godot;
using Box.Components;

namespace Box.Events {
    [Register(nameof(CollisionEvent))]
    public class CollisionEvent : IEvent {
        public struct Pack {
            public Node collision;
            public bool is_enter;
        }
        public bool IsEnterEvent(object a,object b) {
            return a is Node && b is Pack;
        }
        public void _Execute(object self_,object pack_) {
            Node self = self_ as Node;
            Pack pack = (Pack)pack_;

            string event_name = nameof(CollisionEventComponent.CollisionExited);
            if(pack.is_enter) {
                event_name = nameof(CollisionEventComponent.CollisionEntered);
            }

            CollisionEventComponent a_collision_event = self?.GetNodeOrNull<CollisionEventComponent>(nameof(CollisionEventComponent));
            a_collision_event?.EmitSignal(event_name,self,pack.collision);

            CollisionEventComponent b_collision_event = pack.collision?.GetNodeOrNull<CollisionEventComponent>(nameof(CollisionEventComponent));
            b_collision_event?.EmitSignal(event_name,pack.collision,self);
        }
    }
}