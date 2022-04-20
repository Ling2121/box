using Godot;
using System;
using Box.Components;

namespace Box.Entities.Lifes {
    public class Player : KinematicBody2D {
        InterplayComponent InterplayComponent;

        public override void _Ready()
        {
            InterplayComponent = EntityHelper.GetComponent<InterplayComponent>(this);

            InterplayComponent.Connect(nameof(InterplayComponent.emit_interplay),this,nameof(_EmitInterplay));
            InterplayComponent.Connect(nameof(InterplayComponent.receive_interplay),this,nameof(_ReceiveInterplay));

            InterplayComponent.Connect(nameof(InterplayComponent.emit_long_interplay_start),this,nameof(_LongEmitInterplayStart));
            InterplayComponent.Connect(nameof(InterplayComponent.emit_long_interplay_end),this,nameof(_LongEmitInterplayEnd));
        }

        public void _EmitInterplay(InterplayComponent.InterplayItem item) {
            GD.Print($"emit {Name} :{item.Type} -> {item.ReceiveObject.Name} : {item.Item.GetType().Name}");
        }

        public void _ReceiveInterplay(InterplayComponent.InterplayItem item) {
            GD.Print($"receive {Name} :{item.Type} -> {item.ReceiveObject.Name} : {item.Item.GetType().Name}");
        }

        public void _LongEmitInterplayStart(InterplayComponent.InterplayItem item) {
            GD.Print($"emit_long_start {Name} :{item.Type} -> {item.ReceiveObject.Name} : {item.Item.GetType().Name}");
        }

        public void _LongEmitInterplayEnd(InterplayComponent.InterplayItem item) {
            GD.Print($"emit_long_end {Name} :{item.Type} -> {item.ReceiveObject.Name} : {item.Item.GetType().Name}");
        }
    }
}