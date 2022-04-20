using Godot;
using Box.Components;
using Box.Itmes;
using Box.Itmes.Tools;

namespace Box.Events {
    [Register(nameof(LongInterplayEvent))]
    public class LongInterplayEvent : Godot.Object,IEvent {
        [Signal]
        public delegate void long_interplay_start(InterplayType type,Node e1,Node e2,Node item);
        [Signal]
        public delegate void long_interplay_end(InterplayType type,Node e1,Node e2,Node item);

        public bool IsEnterEvent(params object[] args) {
            if(args.Length < 2) return false;
            return 
                args[0] is InterplayComponent.InterplayItem &&
                args[1] is bool             //是否是开始
                ;
        }
        public void Execute(params object[] args) {
            if((bool)args[1]) {
                EmitSignal(nameof(long_interplay_start),args[0]);
            } else {
                EmitSignal(nameof(long_interplay_end),args[0]);
            }
        }
    }
}