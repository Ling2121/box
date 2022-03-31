using Godot;
using Box.Components;

namespace Box.Events {
    [Register(nameof(CollisionEvent))]
    public class CollisionEvent : IEvent {
        public bool IsEnterEvent(params object[] args) {
            if(args.Length < 3) return false;
            return args[0] is Node && args[1] is Node && args[2] is bool;
        }
        public void Execute(params object[] args) {
            Node self = args[0] as Node;
            Node collision = args[1] as Node;
            bool is_enter = (bool)args[2];

            string event_name = nameof(CollisionEventListener.collision_exited);
            if(is_enter) {
                event_name = nameof(CollisionEventListener.collision_entered);
            }

            CollisionEventListener a_collision_event = self?.GetNodeOrNull<CollisionEventListener>(nameof(CollisionEventListener));
            a_collision_event?.EmitSignal(event_name,self,collision);

            CollisionEventListener b_collision_event = collision?.GetNodeOrNull<CollisionEventListener>(nameof(CollisionEventListener));
            b_collision_event?.EmitSignal(event_name,collision,self);
        }
    }
}