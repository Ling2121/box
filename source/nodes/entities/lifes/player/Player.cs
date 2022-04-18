using Godot;
using System;
using Box.Components;

namespace Box.Entities.Lifes {
    public class Player : KinematicBody2D
    {
        HandComponent HandComponent;
        HPComponent HPComponent;

        public override void _Ready()
        {
            HandComponent = GetNode<HandComponent>(nameof(HandComponent));
            HPComponent = GetNode<HPComponent>(nameof(HPComponent));

            HandComponent.Connect(nameof(HandComponent.emit_attack),this,nameof(_Attack));
            HandComponent.Connect(nameof(HandComponent.receive_attack),this,nameof(_ReceiveAttack));
            HPComponent.Connect(nameof(HPComponent.injured),this,nameof(_Injured));
            HPComponent.Connect(nameof(HPComponent.recovery),this,nameof(_Recovery));
        }

        public void _Attack(Node receive_object,Itmes.Tools.BaseTool item) {
            GD.Print($"打了{receive_object.Name}一下");
        }

        public void _ReceiveAttack(Node emit_object,Itmes.Tools.BaseTool item) {
            GD.Print($"你被{emit_object.Name}打了一下");
        }

        public void _Injured(HPComponent self,int value){
            GD.Print($"你受到了{value}点伤害");
        }

        public void _Recovery(HPComponent self,int value){
            GD.Print($"你恢复了{value}点血量");
        }
    }

}
