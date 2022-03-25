using Godot;
using Box.Components;

namespace Box.Events {
    public enum InterplayType {
        MouseLeft,
        MouseRight,
        MouseCenter,
    }

    [Register(nameof(InterplayEvent))]
    public class InterplayEvent : IEvent {
        public struct InterplayPack {
            public InterplayType type;
            public Node receive_object;
        }

        public bool IsEnterEvent(object a,object b) {
            return a is Node && b is InterplayPack;
        }
        public void _Execute(object interplay_object,object pack_) {
            Node obj = interplay_object as Node;
            InterplayPack pack = (InterplayPack)pack_;

            InterplayEventComponent interplay_event = obj?.GetNodeOrNull<InterplayEventComponent>(nameof(InterplayEventComponent));
            interplay_event?.EmitSignal(nameof(InterplayEventComponent.EmitInterplay),pack.receive_object,pack.type);
            interplay_event?.EmitSignal(nameof(InterplayEventComponent.ReceiveInterplay),interplay_object,pack.type);
        }
    }
}