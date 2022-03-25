using Godot;
using System;
using Box.Components;

namespace Box.Entities.Lifes {
    public class Player : KinematicBody2D
    {
        AttackComponent AttackComponent;
        HPComponent HPComponent;

        public override void _Ready()
        {
            AttackComponent = GetNode<AttackComponent>(nameof(AttackComponent));
            HPComponent = GetNode<HPComponent>(nameof(HPComponent));

            AttackComponent.Connect(nameof(AttackComponent.Attack),this,nameof(_Attack));
            AttackComponent.Connect(nameof(AttackComponent.ReceiveAttack),this,nameof(_ReceiveAttack));
            HPComponent.Connect(nameof(HPComponent.Injured),this,nameof(_Injured));
            HPComponent.Connect(nameof(HPComponent.Recovery),this,nameof(_Recovery));
        }

        public void _Attack(Node receive_object) {
            GD.Print($"打了{receive_object.Name}一下");
        }

        public void _ReceiveAttack(Node emit_object) {
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
