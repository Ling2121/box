using System.Collections.Generic;
using Godot;
using Box.Events;
using Box.Itmes;


namespace Box.Components {
    [ClassName(nameof(InterplayEventListener))]
    public class InterplayEventListener : Node2D,IEventListener {
        public static InterplayEventListener Select;

        [Signal]
        public delegate void emit_interplay(InterplayType type,Node receive_object,Node item);
        [Signal]
        public delegate void receive_interplay(InterplayType type,Node emit_object,Node item);

        [Export]
        public Area2D ClickDecisionArea;

        public Node Entity {get;set;} = null;

        public bool IsRemove()
        {
            return false;
        }

        public void _InitListener()
        {
            if(ClickDecisionArea == null){
                //从组件本体搜索Area
                foreach(Node node in GetChildren()) {
                    if(node is Area2D) {
                        ClickDecisionArea = node as Area2D;
                    }
                }
                if(ClickDecisionArea == null){
                    ClickDecisionArea = new Area2D();
                    //从父节点搜索（需要父节点继承自PhysicsBody2D）
                    if(Entity is PhysicsBody2D) {
                        foreach(Node node in Entity.GetChildren()) {
                            if(node is CollisionShape2D){
                                CollisionShape2D node_shape = node as CollisionShape2D;
                                CollisionShape2D shape = new CollisionShape2D();
                                shape.Shape = node_shape.Shape;
                                ClickDecisionArea.AddChild(shape);
                                break;
                            } 
                            else if(node is CollisionPolygon2D) {
                                CollisionPolygon2D node_shape = node as CollisionPolygon2D;
                                CollisionPolygon2D shape = new CollisionPolygon2D();
                                shape.Polygon = node_shape.Polygon;
                                ClickDecisionArea.AddChild(shape);
                                break;
                            }
                        }
                    }
                }
                AddChild(ClickDecisionArea);
            }

            if(ClickDecisionArea != null) {
                ClickDecisionArea.Connect("mouse_entered",this,nameof(_MouseEntered));
                ClickDecisionArea.Connect("mouse_exited",this,nameof(_MouseExited));
            } else {
                GD.PrintErr($"{nameof(InterplayEventListener)}需要有Area2D节点为子节点(父类为继承自PhysicsBody2D时不需要)");
            }
        }

        public void _MouseEntered() {
            Select = this;
        }

        public void _MouseExited() {
            if(Select == this) {
                Select = null;
            }
        }

        public override void _ExitTree()
        {
            if(Select == this) {
                Select = null;
            }
        }

        public void EmitInterplayEvent(InterplayType interplay_type,Node receive_object,Node item) {
            Game.Instance.EventManager.RequestEvent(nameof(InterplayEvent),interplay_type,Entity,receive_object,item);
        }
    }
}