using Godot;
using Box.Events;

namespace Box.Components {
    [ClassName(nameof(AttackComponent))]
    public class AttackComponent : Node {
        [Signal]
        public delegate void Attack(Node receive_object);
        [Signal]
        public delegate void ReceiveAttack(Node attack_object);

        [Export]
        public int AttackValue = 2;
        [Export]
        public float AttackSpeed = 0.5f;


        public bool IsAllowAttack = true;
        
        protected float attack_timer = 0;

        public InterplayEventComponent InterplayEventComponent;

        public override void _Ready()
        {
            InterplayEventComponent = GetParent().GetNodeOrNull<InterplayEventComponent>(nameof(InterplayEventComponent));
            if(InterplayEventComponent != null) {
                InterplayEventComponent.Connect(nameof(InterplayEventComponent.EmitInterplay),this,nameof(_EmitInterplay));
            }
        }

        public void EmitAttack(Node attack_object) {
            InterplayEventComponent.EmitInterplayEvent(InterplayType.MouseLeft,attack_object);
        }

        public void _EmitInterplay(Node receive_object,InterplayType type) {
            if(type == InterplayType.MouseLeft) {
                if(IsAllowAttack) {
                    HPComponent hp = receive_object.GetNodeOrNull<HPComponent>(nameof(HPComponent));
                    if(hp != null) {
                        hp.HP -= AttackValue;
                        EmitSignal(nameof(Attack),receive_object);
                    }
                    AttackComponent relobj_ac =  receive_object.GetNodeOrNull<AttackComponent>(nameof(AttackComponent));
                    if(relobj_ac != null){
                        relobj_ac.EmitSignal(nameof(AttackComponent.ReceiveAttack),GetParent());
                    }
                    IsAllowAttack = false;
                }
            }
        }

        public override void _Process(float delta)
        {
            if(IsAllowAttack == false) {
                attack_timer += delta;
                if(attack_timer >= AttackSpeed) {
                    IsAllowAttack = true;
                }
            }
        }

    }
}