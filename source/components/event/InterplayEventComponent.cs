using System;
using Godot;
using Box.Events;

namespace Box.Components {
    [ClassName(nameof(InterplayEventComponent))]
    public class InterplayEventComponent : Node2D {
        [Signal]
        public delegate void EmitInterplay(Node receive_object,InterplayType type);
        [Signal]
        public delegate void ReceiveInterplay(Node emit_object,InterplayType type);

        [Export]
        public Area2D ClickDecisionArea;
        Node parent;
        public override void _Ready()
        {
            parent = GetParent();

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
                    if(parent is PhysicsBody2D) {
                        foreach(Node node in parent.GetChildren()) {
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
            } else {
                GD.PrintErr($"{nameof(InterplayEventComponent)}需要有Area2D节点为子节点(父类为继承自PhysicsBody2D时不需要)");
            }
        }

        public void _MouseEntered() {
            if(Input.IsMouseButtonPressed((int)ButtonList.Left)) {
                Game.Instance.EventManager.RequestEvent(nameof(InterplayEvent),parent,null);
            }
        }

    }
}