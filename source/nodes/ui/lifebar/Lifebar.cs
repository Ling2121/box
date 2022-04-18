using Godot;
using System;
using Box.Components;


namespace Box.UI {
    public class Lifebar : TextureProgress
    {
        [Export]
        public NodePath HPComponent; 

        protected HPComponent hpc;

        public override void _Ready()
        {
            Node node = GetNodeOrNull(HPComponent);

            if(node != null) {
                if(node is HPComponent) {
                    hpc = node as HPComponent;
                    MinValue = 0;
                    MaxValue = hpc.MaxHP;
                    Value = hpc.HP;
                    hpc.Connect(nameof(Components.HPComponent.change),this,nameof(_Change));
                }
            }
        }


        public void _Change(HPComponent self,int value){
            Value = value;
        }
    }
}
