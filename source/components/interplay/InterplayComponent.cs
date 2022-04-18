using System.Linq;
using System.Collections.Generic;
using Godot;
using Box.Events;
using System;

namespace Box.Components {
    [ClassName(nameof(InterplayComponent))]
    public class InterplayComponent : Area2D,IComponent {
        public static InterplayComponent Select = null;

        [Signal]
        public delegate void emit_interplay(InterplayType type,Node receive_object,Node interplay_item);
        [Signal]
        public delegate void receive_interplay(InterplayType type,Node emit_object,Node interplay_item);
        [Signal]
        public delegate void emit_long_interplay_start(InterplayType type,Node receive_object,Node interplay_item);
        [Signal]
        public delegate void emit_long_interplay_end(InterplayType type,Node emit_object,Node interplay_item);
        [Signal]
        public delegate void receive_long_interplay_start(InterplayType type,Node receive_object,Node interplay_item);
        [Signal]
        public delegate void receive_long_interplay_end(InterplayType type,Node emit_object,Node interplay_item);

        public Dictionary<InterplayType,Dictionary<Node,(Func<bool>,Node,Node)>> LongInterplayTable = new Dictionary<InterplayType,Dictionary<Node,(Func<bool>,Node,Node)>> {
            {InterplayType.Attack,new Dictionary<Node, (Func<bool>,Node,Node)>()},
            {InterplayType.Interplay,new Dictionary<Node, (Func<bool>,Node,Node)>()},
        };

        public override void _Ready()
        {
            Connect("mouse_entered",this,nameof(_MouseEntered));
            Connect("mouse_entered",this,nameof(_MouseExited));
        }
        
        public override void _ExitTree() {
            if(Select == this) {
                Select = null;
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

        protected void Interplay(InterplayType type,Node interplay_entity,bool is_long,bool is_start) {
            if(type == InterplayType.Attack) {
                AttackComponent attack_component = EntityHelper.GetComponent<AttackComponent>(GetParent());
                if(attack_component != null) {
                    int c = 0;
                    foreach(Node node in GetChildren()){
                        if(node is ITool) {
                            Game.Instance.EventManager.PublishEvent(nameof(InterplayEvent),type,GetParent(),interplay_entity,node);
                            c++;
                        }
                    }
                    //空手撸
                    if(c == 0) {
                        Game.Instance.EventManager.PublishEvent(nameof(InterplayEvent),type,GetParent(),interplay_entity,attack_component);
                    }
                }
            }
            if(type == InterplayType.Interplay) {
                    foreach(Node node in GetChildren()){
                    if(node is IItem) {
                        IItem item = node as IItem;
                        if(item.IsUse(GetParent(),interplay_entity)) {
                            Game.Instance.EventManager.PublishEvent(nameof(InterplayEvent),type,GetParent(),interplay_entity,item);
                        }
                    }
                }
            }
        }

        public void Interplay(InterplayType type,Node interplay_entity,Node interplay_item) {
            if(type == InterplayType.Attack) {
                AttackComponent attack_component = EntityHelper.GetComponent<AttackComponent>(GetParent());
                if(attack_component != null) {
                    if(attack_component.IsAllowAttack) {
                        attack_component.IsAllowAttack = false;
                        Game.Instance.EventManager.PublishEvent(nameof(InterplayEvent),type,GetParent(),interplay_entity,interplay_item);
                        EmitSignal(nameof(emit_interplay),type,interplay_entity,interplay_item);

                        EntityHelper.GetComponent<InterplayComponent>(interplay_entity)
                            ?.EmitSignal(nameof(receive_interplay),type,GetParent(),interplay_item);
                    }
                }
            }
            if(type == InterplayType.Interplay) {
                Game.Instance.EventManager.PublishEvent(nameof(InterplayEvent),type,GetParent(),interplay_entity,interplay_item);
                EmitSignal(nameof(emit_interplay),type,interplay_entity,interplay_item);

                EntityHelper.GetComponent<InterplayComponent>(interplay_entity)
                    ?.EmitSignal(nameof(receive_interplay),type,GetParent(),interplay_item);
            }
        }

        public void LongInterplayStart(InterplayType type,Node interplay_entity,Node interplay_item,Func<bool> end_condition) {
            var table = LongInterplayTable[type];
            if(!table.ContainsKey(interplay_entity)){
                Interplay(type,interplay_entity,true,true);
                table[interplay_entity] = (end_condition,interplay_entity,interplay_item);
                
                Game.Instance.EventManager.PublishEvent(nameof(LongInterplayEvent),type,GetParent(),interplay_entity,interplay_item,true);
                EmitSignal(nameof(emit_long_interplay_start),type,interplay_entity,interplay_item);

                EntityHelper.GetComponent<InterplayComponent>(interplay_entity)
                    ?.EmitSignal(nameof(receive_long_interplay_start),type,GetParent(),interplay_item);
            }
        }

        public void LongInterplayEnd(InterplayType type,Node interplay_entity,Node interplay_item) {
            var table = LongInterplayTable[type];
            if(table.ContainsKey(interplay_entity)){
                table.Remove(interplay_entity);

                Game.Instance.EventManager.PublishEvent(nameof(LongInterplayEvent),type,GetParent(),interplay_entity,interplay_item,false);
                EmitSignal(nameof(emit_long_interplay_end),type,interplay_entity,interplay_item);

                EntityHelper.GetComponent<InterplayComponent>(interplay_entity)
                    ?.EmitSignal(nameof(receive_long_interplay_end),type,GetParent(),interplay_item);
            }
        }

        public override void _Process(float delta)
        {
            foreach(var item in LongInterplayTable) {
                foreach(var key in item.Value.Keys.ToArray()) {
                    var item2 = item.Value[key];
                    var end_condition = item2.Item1;
                    if(end_condition()){
                        LongInterplayEnd(item.Key,item2.Item2,item2.Item3);
                    }
                }
            }
        }
    }
}