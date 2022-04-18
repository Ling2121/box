using Godot;
using System;
using Box.Components;

namespace Box.UI {
    public class Foodbar : TextureProgress
    {
        [Export]
        public NodePath FoodComponent; 

        FoodComponent fc;
        
        public override void _Ready()
        {
            Node node = GetNodeOrNull(FoodComponent);

            if(node != null) {
                if(node is FoodComponent) {
                    fc = node as FoodComponent;
                    MinValue = 0;
                    MaxValue = fc.MaxFood;
                    Value = fc.Food;

                    fc.Connect(nameof(Components.FoodComponent.change),this,nameof(_Change));

                    fc.Connect(nameof(Components.FoodComponent.hunger),this,nameof(_Hunger));
                }
            }
        }

        public void _Change(FoodComponent self,int value) {
            Value = value;
        }

        public void _Hunger(FoodComponent self) {
            TintProgress = TintProgress / 2;
        }

    }
}
