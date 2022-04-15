using System;
using System.Collections.Generic;
using Godot;
using System.Linq;
using Box.Events;

namespace Box.Components {

    [ClassName(nameof(MoveComponent))]
    public class MoveComponent : Node {
        protected enum MoveType {
            KinematicBody2D,
            Node2D,
        }
        
        public Vector2 Direction = Vector2.Zero;

        protected KinematicBody2D kinematic_body = null;
        protected Node2D node_2d = null;

        protected MoveType move_type;

        //用于判断碰撞事件的Area
        public Area2D CollisionDecisionArea;

        protected void Move(Vector2 vec,float delta) {
            if(move_type == MoveType.KinematicBody2D) {
                kinematic_body.MoveAndSlide(vec);
            } 
            else if(move_type == MoveType.Node2D) {
                node_2d.Position += vec * delta;
            }            
        }
        public override void _Ready()
        {
            Node parent = GetParent();
            if(parent is KinematicBody2D) {
                move_type = MoveType.KinematicBody2D;
                kinematic_body = (KinematicBody2D)parent;
            }
            else if(parent is Node2D) {
                move_type = MoveType.Node2D;
                node_2d = (Node2D)parent;
            }
        }

        public override void _Process(float delta)
        {
            if(Direction != Vector2.Zero) {
                Move(Direction,delta);
                Direction = Vector2.Zero;
            }
        }
    }
}