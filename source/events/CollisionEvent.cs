using Godot;
using Box.Components;

namespace Box.Events {
    [Register(nameof(CollisionEvent))]
    public class CollisionEvent : Godot.Object,IEvent {
        [Signal]
        public delegate void collision_entered(Node e1,Node e2);
        [Signal]
        public delegate void collision_exited(Node e1,Node e2);

        public bool IsEnterEvent(params object[] args) {
            if(args.Length < 3) return false;
            return args[0] is Node && args[1] is Node && args[2] is bool;
        }
        public void Execute(params object[] args) {
            Node e1 = args[0] as Node;
            Node e2 = args[1] as Node;
            bool is_enter = (bool)args[2];

            if(is_enter) {
                EmitSignal(nameof(collision_entered),e1,e2);
            } else {
                EmitSignal(nameof(collision_exited),e1,e2);
            }
        }
    }
}