using Godot;
using Box.Events;

namespace Box.Components {
    public class CollisionComponent : Area2D,IComponent {
        [Signal]
        public delegate void collision_entered(Node collision);
        [Signal]
        public delegate void collision_exited(Node collision);
        protected Node Parent;

        public override void _Ready()
        {
            Parent = GetParent();
            Connect("body_entered",this,nameof(_BodyEntered));
            Connect("body_exited",this,nameof(_BodyExited));
        }

        public void _BodyEntered(Node body) {
            if(body != Parent) {
                Game.Instance.EventManager.PublishEvent(nameof(CollisionEvent),Parent,body,true);
                EmitSignal(nameof(collision_entered),body);
            }
        }

        public void _BodyExited(Node body) {
            if(body != Parent) {
                Game.Instance.EventManager.PublishEvent(nameof(CollisionEvent),Parent,body,false);
                EmitSignal(nameof(collision_exited),body);
            }
        }
    }
}