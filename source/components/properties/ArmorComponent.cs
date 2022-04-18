using Godot;

namespace Box.Components {
    [ClassName(nameof(ArmorComponent))]
    public class ArmorComponent : Node,IComponent {
        [Export]
        public int Armor = 1;

        protected HPComponent HPComponent;

        public override void _Ready()
        {
            HPComponent = EntityHelper.GetComponent<HPComponent>(GetParent());
            if(HPComponent != null) {
                HPComponent.Connect(nameof(HPComponent.injured),this,nameof(_Injured));
            }
        }

        public void _Injured(HPComponent self,int value) {
            if(value > 0) {
                self.HP += Armor;
            }
        }
    }
} 