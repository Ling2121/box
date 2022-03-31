using Godot;
using Box.Components;

namespace Box.Itmes.Tools {
    public class BaseTool : Node2D,ITool {
        [Signal]
        public delegate void Damage(BaseTool self);

        public const int INFINITE_DURABLE = -1;

        [Export]
        public int Hurt {get;set;} = 0;
        [Export]
        public int Durable {get;set;} = INFINITE_DURABLE;

        public bool IsUse(Node emit,Node receive) {
            return true;
        }

        public void Use(Node receive_node) {
            if(Durable != INFINITE_DURABLE){
                Durable--;
                if(Durable == 0) {
                    EmitSignal(nameof(Damage),this);
                    if(Durable == 0) {
                        QueueFree();
                    }
                }
            }
            _Use(receive_node);
        }

        public int Attack(Node receive_node) {
            return Hurt;
        }

        public virtual void _Use(Node node) {

        }
    }
}