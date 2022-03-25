using System.Collections.Generic;
using Godot;
using System;
using Box.Components;
using Box.Events;

namespace Box.Entities.Lifes {
    public class SpineMan : KinematicBody2D
    {
        [Export]
        public float HurtSpeed = 1;

        protected float hurt_timer = 0;

        CollisionEventComponent CollisionEventComponent;
        AttackComponent AttackComponent;
        HPComponent HPComponent;

        public Dictionary<Node,Node> hurt_table = new Dictionary<Node, Node>();

        public override void _Ready()
        {
            CollisionEventComponent = GetNode<CollisionEventComponent>(nameof(CollisionEventComponent));
            HPComponent = GetNode<HPComponent>(nameof(HPComponent));
            AttackComponent = GetNode<AttackComponent>(nameof(AttackComponent));
            
            AttackComponent.Connect(nameof(AttackComponent.Attack),this,nameof(_Attack));

            CollisionEventComponent.Connect(nameof(CollisionEventComponent.CollisionEntered),this,nameof(_CollisionEntered));
            CollisionEventComponent.Connect(nameof(CollisionEventComponent.CollisionExited),this,nameof(_CollisionExited));
        
            HPComponent.Connect(nameof(HPComponent.Injured),this,nameof(_Injured));
        }

        public void _CollisionEntered(Node self,Node collision) {
            hurt_table[collision] = collision;
        }

        public void _CollisionExited(Node self,Node collision) {
            hurt_table.Remove(collision);
        }

        public void _Attack(Node receive_object) {
            GD.Print($"尖刺人 打了{receive_object.Name}一下");
        }

        public void _Injured(HPComponent self,int value){
            GD.Print($"尖刺人{Name} 你受到了{value}点伤害");
        }

        public override void _Process(float delta)
        {
            hurt_timer += delta;
            if(hurt_timer >= HurtSpeed){
                hurt_timer = 0;
                foreach(Node entity in hurt_table.Values) {
                    AttackComponent.EmitAttack(entity);
                }
            }
        }

    }
}
