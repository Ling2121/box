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

        CollisionEventListener CollisionEventListener;
        HandComponent HandComponent;
        HPComponent HPComponent;

        public Dictionary<Node,Node> hurt_table = new Dictionary<Node, Node>();

        public override void _Ready()
        {
            EventListeningComponent event_listening = GetNode<EventListeningComponent>(nameof(EventListeningComponent));

            CollisionEventListener = event_listening.GetListener<CollisionEventListener>();
            HPComponent = GetNode<HPComponent>(nameof(HPComponent));
            HandComponent = GetNode<HandComponent>(nameof(HandComponent));
            
            HandComponent.Connect(nameof(HandComponent.emit_attack),this,nameof(_Attack));

            CollisionEventListener.Connect(nameof(CollisionEventListener.collision_entered),this,nameof(_CollisionEntered));
            CollisionEventListener.Connect(nameof(CollisionEventListener.collision_exited),this,nameof(_CollisionExited));
        
            HPComponent.Connect(nameof(HPComponent.injured),this,nameof(_Injured));
        }

        public void _CollisionEntered(Node self,Node collision) {
            GD.Print("enter:",collision.Name);
            hurt_table[collision] = collision;
        }

        public void _CollisionExited(Node self,Node collision) {
            GD.Print("exit:",collision.Name);
            hurt_table.Remove(collision);
        }

        public void _Attack(Node receive_object,Node tool) {
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
                    HandComponent.EmitAttack(entity);
                }
            }
        }

    }
}
