using Godot;

namespace Box.Components {
    [ClassName(nameof(AttackComponent))]
    public class AttackComponent : Node,IComponent {
        [Export]
        public int AttackValue = 2;
        [Export]
        public float AttackSpeed = 0.5f;

        public bool IsAllowAttack = true;
        
        protected float attack_timer = 0;

        public override void _Process(float delta)
        {
            if(!IsAllowAttack) {
                attack_timer += delta;
                if(attack_timer >= AttackSpeed){
                    attack_timer = 0;
                    IsAllowAttack = true;
                }
            }
        }
    }
}