using Godot;

namespace Box.Components {
    [ClassName(nameof(ArmorComponent))]
    public class ArmorComponent : Node {
        [Export]
        public int Armor = 1;

        protected HPComponent HPComponent;

        public override void _Ready()
        {
            HPComponent = GetParent().GetNodeOrNull<HPComponent>(nameof(HPComponent));
            if(HPComponent != null) {
                HPComponent.Connect(nameof(HPComponent.Injured),this,nameof(_Injured));
            }
        }

        public void _Injured(HPComponent self,int value) {
            if(value > 0) {
                self.HP += Armor;
            }
        }
    }
} 