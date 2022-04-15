using System;
using Godot;
using Box.Events;

namespace Box.Components {
    [ClassName(nameof(CollisionEventListener))]
    public class CollisionEventListener : Node2D,IEventListener {
        [Signal]
        public delegate void collision_entered(Node self,Node collision);
        [Signal]
        public delegate void collision_exited(Node self,Node collision);
        [Export]
        public NodePath DecisionArea = new NodePath();
        public Area2D CollisionDecisionArea;
        
        public Node Entity {get;set;} = null;

        public bool IsRemove()
        {
            return false;
        }

        public void _InitListener()
        {
            CollisionDecisionArea = GetNodeOrNull<Area2D>(DecisionArea);
            if(CollisionDecisionArea == null){
                //从组件本体搜索Area
                foreach(Node node in GetChildren()) {
                    if(node is Area2D) {
                        CollisionDecisionArea = node as Area2D;
                    }
                }
                if(CollisionDecisionArea == null){
                    CollisionDecisionArea = new Area2D();
                    //从父节点搜索（需要父节点继承自PhysicsBody2D）
                    if(Entity is PhysicsBody2D) {
                        foreach(Node node in Entity.GetChildren()) {
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
                if(CollisionDecisionArea != null){
                    AddChild(CollisionDecisionArea);
                }
            }

             if(CollisionDecisionArea != null) {
                CollisionDecisionArea.Connect("body_entered",this,nameof(_BodyEntered));
                CollisionDecisionArea.Connect("body_exited",this,nameof(_BodyExited));
            } else {
                GD.PrintErr($"{nameof(InterplayEventListener)}需要有Area2D节点为子节点(父类为继承自PhysicsBody2D时不需要)");
            }
        }

        public void _BodyEntered(Node body) {
            if(body != Entity) {
                Game.Instance.EventManager.RequestEvent(nameof(CollisionEvent),Entity,body,true);
            }
        }

        public void _BodyExited(Node body) {
            if(body != Entity) {
                Game.Instance.EventManager.RequestEvent(nameof(CollisionEvent),Entity,body,false);
            }
        }

    }
}