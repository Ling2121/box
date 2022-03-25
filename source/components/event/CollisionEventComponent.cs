using System;
using Godot;
using Box.Events;

namespace Box.Components {
    [ClassName(nameof(CollisionEventComponent))]
    public class CollisionEventComponent : Node2D {
        [Signal]
        public delegate void Collision();
        public Area2D CollisionDecisionArea;
        Node parent;
        public override void _Ready()
        {
            parent = GetParent();

            //从组件本体搜索Area
            foreach(Node node in GetChildren()) {
                if(node is Area2D) {
                    CollisionDecisionArea = node as Area2D;
                }
            }
            if(CollisionDecisionArea == null){
                CollisionDecisionArea = new Area2D();
                //从父节点搜索（需要父节点继承自PhysicsBody2D）
                if(parent is PhysicsBody2D) {
                    foreach(Node node in parent.GetChildren()) {
                        if(node is CollisionShape2D){
                            CollisionShape2D node_shape = node as CollisionShape2D;
                            CollisionShape2D shape = new CollisionShape2D();
                            shape.Shape = node_shape.Shape;
                            shape.Scale = new Vector2(1.1f,1.1f);
                            CollisionDecisionArea.AddChild(shape);
                            break;
                        } 
                        else if(node is CollisionPolygon2D) {
                            CollisionPolygon2D node_shape = node as CollisionPolygon2D;
                            CollisionPolygon2D shape = new CollisionPolygon2D();
                            shape.Polygon = node_shape.Polygon;
                            shape.Scale = new Vector2(1.1f,1.1f);
                            CollisionDecisionArea.AddChild(shape);
                            break;
                        }
                    }
                }
            }

            if(CollisionDecisionArea != null) {
                AddChild(CollisionDecisionArea);
                CollisionDecisionArea.Connect("body_entered",this,nameof(_BodyEntered));
            } else {
                GD.PrintErr($"{nameof(InterplayEventComponent)}需要有Area2D节点为子节点(父类为继承自PhysicsBody2D时不需要)");
            }
        }

        public void _BodyEntered(Node body) {
            if(body != parent) {
                Game.Instance.EventManager.RequestEvent(nameof(CollisionEvent),parent,body);
            }
        }

    }
}