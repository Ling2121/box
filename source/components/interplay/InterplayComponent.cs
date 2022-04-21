using System.Linq;
using System.Collections.Generic;
using Godot;
using Box.Events;
using System;
using EndConditionFunc = System.Func<Box.Components.InterplayComponent.InterplayItem,bool>;

namespace Box.Components {
    [ClassName(nameof(InterplayComponent))]
    public class InterplayComponent : Area2D,IComponent {
        public class InterplayItem : Godot.Reference {
            public InterplayType Type;
            public Node EmitObject;
            public Node ReceiveObject;
            public Node Item;
            public EndConditionFunc EndCondition = item =>{return true;};
            public InterplayItem(){}

            public InterplayItem(InterplayType type,Node emit,Node receive,Node item) {
                Type = type;
                EmitObject = emit;
                ReceiveObject = receive;
                Item = item;
            }
        }

        public static Node Select = null;

        [Signal]
        public delegate void emit_interplay(InterplayItem info);
        [Signal]
        public delegate void receive_interplay(InterplayItem info);
        [Signal]
        public delegate void emit_long_interplay_start(InterplayItem info);
        [Signal]
        public delegate void emit_long_interplay_end(InterplayItem info);
        [Signal]
        public delegate void receive_long_interplay_start(InterplayItem info);
        [Signal]
        public delegate void receive_long_interplay_end(InterplayItem info);

        public Dictionary<InterplayItem,InterplayItem> LongInterplayTable = new Dictionary<InterplayItem, InterplayItem>();

        public Node Parent;

        public override void _Ready()
        {
            Parent = GetParent();
            Connect("mouse_entered",this,nameof(_MouseEntered));
            Connect("mouse_entered",this,nameof(_MouseExited));
        }
        
        public override void _ExitTree() {
            if(Select == Parent) {
                Select = null;
            }
        }
        
        public void _MouseEntered() {
            Select = Parent;
        }

        public void _MouseExited() {
            if(Select == Parent) {
                Select = null;
            }
        }

        public InterplayItem Interplay(InterplayType type,Node interplay_entity,Node interplay_item) {
            InterplayItem item = new InterplayItem(type,GetParent(),interplay_entity,interplay_item);
            if(type == InterplayType.Attack) {
                AttackComponent attack_component = EntityHelper.GetComponent<AttackComponent>(GetParent());
                if(attack_component != null) {
                    if(attack_component.IsAllowAttack) {
                        attack_component.IsAllowAttack = false;

                        Game.Instance.EventManager.PublishEvent(nameof(InterplayEvent),item);
                        EmitSignal(nameof(emit_interplay),item);

                        EntityHelper.GetComponent<InterplayComponent>(interplay_entity)
                            ?.EmitSignal(nameof(receive_interplay),item);
                    }
                }
            }
            if(type == InterplayType.Interplay) {
                Game.Instance.EventManager.PublishEvent(nameof(InterplayEvent),item);
                EmitSignal(nameof(emit_interplay),item);
                EntityHelper.GetComponent<InterplayComponent>(interplay_entity)
                    ?.EmitSignal(nameof(receive_interplay),item);
            }

            return item;
        }

        public InterplayItem LongInterplayStart(InterplayType type,Node interplay_entity,Node interplay_item,EndConditionFunc end_condition) {
            var item = Interplay(type,interplay_entity,interplay_item);
            if(end_condition != null) {
                item.EndCondition = end_condition;
            }
            
            LongInterplayTable[item] = item;
            
            Game.Instance.EventManager.PublishEvent(nameof(LongInterplayEvent),item,true);
            EmitSignal(nameof(emit_long_interplay_start),item);

            EntityHelper.GetComponent<InterplayComponent>(interplay_entity)
                ?.EmitSignal(nameof(receive_long_interplay_start),item);
            return item;
        }

        public void LongInterplayEnd(InterplayItem item) {
            if(LongInterplayTable.ContainsKey(item)){

                LongInterplayTable.Remove(item);

                Game.Instance.EventManager.PublishEvent(nameof(LongInterplayEvent),item);
                EmitSignal(nameof(emit_long_interplay_end),item);

                EntityHelper.GetComponent<InterplayComponent>(item.ReceiveObject)
                    ?.EmitSignal(nameof(receive_long_interplay_end),item);
            }
        }

        public override void _Process(float delta)
        {
            foreach(var item in LongInterplayTable.Values.ToArray()) {
                if(item.EndCondition(item)) {
                    LongInterplayEnd(item);
                }
            }
        }
    }
}