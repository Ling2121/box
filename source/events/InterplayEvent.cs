using Godot;
using Box.Components;
using Box.Itmes;
using Box.Itmes.Tools;

namespace Box.Events {
    public enum InterplayType {
        Attack,
        Interplay,
    }

    [Register(nameof(InterplayEvent))]
    public class InterplayEvent : Godot.Object,IEvent {
        [Signal]
        public delegate void interplay(InterplayType type,Node e1,Node e2,Node item);

        public bool IsEnterEvent(params object[] args) {
            if(args.Length < 0) return false;
            return 
                args[0] is InterplayComponent.InterplayItem
                ;
        }
        public void Execute(params object[] args) {
            EmitSignal(nameof(interplay),args[0]);
        }
    }
}