using Godot;
using System;
using Box.Components;
using Box.Events;
using System.Collections.Generic;

namespace Box.Blocks {
    [
        Register(nameof(Thorns),false),
        BindScene("res://source/nodes/blocks/thorns/Thorns.tscn"),
        BindTile("thorns"),
        BlockNodeScript()
    ]
    public class Thorns : Area2D,IBlock,IRegister {
        [Export]
        public float HurtSpeed = 1;

        protected float hurt_timer = 0;

        CollisionEventListener CollisionEventListener;
        HandComponent HandComponent;

        public Dictionary<Node,Node> hurt_table = new Dictionary<Node, Node>();

        public int X {get;set;}
        public int Y {get;set;}
        //耐久
        public int Durable {get;set;}
        //硬度
        public int Hardness {get;set;}
        //是否加入到场景中进行更新
        public bool IsAddToSandbox() {
            return true;
        }
        //绑定tile时触发
        public void _CellBind() {
            int size2 = (Sandbox.REGION_CELL_PIXEL_SIZE / 2);
            int world_x = X * Sandbox.REGION_CELL_PIXEL_SIZE + size2;
            int world_y = Y * Sandbox.REGION_CELL_PIXEL_SIZE + size2;
            Position = new Vector2(world_x,world_y);
            CollisionShape2D shape = GetNode<CollisionShape2D>("Shape");
            RectangleShape2D rect = shape.Shape as RectangleShape2D;
            rect.Extents = new Vector2(size2,size2);
        }
        //接触绑定tile时触发
        public void _CellUnbind() {}

        public void _Damage(Node entity){}

        public override void _Ready()
        {
            EventListeningComponent event_listening = GetNode<EventListeningComponent>(nameof(EventListeningComponent));

            CollisionEventListener = event_listening.GetListener<CollisionEventListener>();
            HandComponent = GetNode<HandComponent>(nameof(HandComponent));

            CollisionEventListener.Connect(nameof(CollisionEventListener.collision_entered),this,nameof(_CollisionEntered));
            CollisionEventListener.Connect(nameof(CollisionEventListener.collision_exited),this,nameof(_CollisionExited));
        }

        public void _CollisionEntered(Node self,Node collision) {
            hurt_table[collision] = collision;
        }

        public void _CollisionExited(Node self,Node collision) {
            hurt_table.Remove(collision);
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