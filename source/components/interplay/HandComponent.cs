using System.Collections.Generic;
using Godot;
using Box.Events;
using Box.Itmes.Tools;
using Box.Itmes;

namespace Box.Components
{
    /*
        HandComponent : HandComponent

    */
    [ClassName(nameof(HandComponent))]
    public class HandComponent : Node2D {
        [Signal]
        public delegate void emit_attack(Node receive_object,Node item);
        [Signal]
        public delegate void receive_attack(Node attack_object,Node item);
        
        [Signal]
        public delegate void emit_use(Node receive_object,Node item);
        [Signal]
        public delegate void receive_use(Node use_object,Node item);

        [Signal]
        public delegate void emit_damage_start(Node use_object,Node item);
        [Signal]
        public delegate void emit_damage_end(Node use_object,Node item);
        

        [Export]
        public int AttackValue = 2;
        [Export]
        public float AttackSpeed = 0.5f;

        public bool IsAllowAttack = true;
        
        protected float attack_timer = 0;

        public InterplayEventListener InterplayEventListener;
        public EventListeningComponent EventListeningComponent;

        protected Node parent;
        public override void _Ready()
        {
            parent = GetParent();

            EventListeningComponent = parent.GetNodeOrNull<EventListeningComponent>(nameof(EventListeningComponent));

            InterplayEventListener = EventListeningComponent.GetListener<InterplayEventListener>();

            InterplayEventListener.Connect(nameof(InterplayEventListener.emit_interplay),this,nameof(_EmitInterplay));
            InterplayEventListener.Connect(nameof(InterplayEventListener.receive_interplay),this,nameof(_ReceiveInterplay));
        }

        public bool IsEmptyHand() {
            foreach(Node node in GetChildren()) {
                if(node is IItem) {
                    return false;
                }
            }
            return true;
        }

        public void EmitUse(Node use_objecct) {
            foreach(Node node in GetChildren()){
                if(node is IItem) {
                    IItem item = node as IItem;
                    if(item.IsUse(GetParent(),use_objecct)) {
                        InterplayEventListener.EmitInterplayEvent(InterplayType.Interplay,use_objecct,node);
                    }
                }
            }
        }

        public void EmitAttack(Node attack_object) {
            int c = 0;
            foreach(Node node in GetChildren()){
                if(node is ITool) {
                    InterplayEventListener.EmitInterplayEvent(InterplayType.Attack,attack_object,node);
                    c++;
                }
            }
            //空手撸
            if(c == 0) {
                InterplayEventListener.EmitInterplayEvent(InterplayType.Attack,attack_object,this);
            }
        }

        public void _EmitInterplay(Node receive_object,InterplayType type,Node tool) {
            if(type == InterplayType.Attack) {
                if(IsAllowAttack) {
                    HPComponent hp = receive_object.GetNodeOrNull<HPComponent>(nameof(HPComponent));
                    if(hp != null) {
                        if(tool is HandComponent) {
                            EmitSignal(nameof(emit_attack),receive_object,this);
                            hp.HP -= AttackValue;
                        }
                        else {
                            BaseTool attack_tool = tool as BaseTool;
                            if(attack_tool != null) {
                                EmitSignal(nameof(emit_attack),receive_object,attack_tool);
                                hp.HP -= attack_tool.Attack(parent);
                            }
                        }
                    }
                    HandComponent hand =  receive_object.GetNodeOrNull<HandComponent>(nameof(HandComponent));
                    if(hand != null){
                        hand.EmitSignal(nameof(HandComponent.receive_attack),GetParent(),tool);
                    }
                    IsAllowAttack = false;
                }
            }
            
            if(type == InterplayType.Interplay) {
                if(tool is IItem){
                    IItem item = tool as IItem;
                    if(item.IsUse(GetParent(),receive_object)) {
                        EmitSignal(nameof(emit_use),receive_object,tool);
                        item.Use(receive_object);
                    }
                }
            }
        }

        public void _ReceiveInterplay(Node use_objecct,InterplayType type,Node tool) {
            if(type == InterplayType.Interplay) {
                if(tool is IItem){
                    IItem item = tool as IItem;
                    EmitSignal(nameof(receive_use),use_objecct,item);
                }
            }
        }

        public override void _Process(float delta)
        {
            if(IsAllowAttack == false) {
                attack_timer += delta;
                if(attack_timer >= AttackSpeed) {
                    IsAllowAttack = true;
                }
            }
        }

    }
}