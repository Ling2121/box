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
            if(args.Length < 5) return false;
            return 
                args[0] is InterplayType && //互动类型
                args[1] is Node &&          //发出互动的对象
                args[2] is Node &&          //接收互动的对象
                args[3] is Node &&          //互动使用的物体
                args[4] is bool             //是否是开始
                ;
        }
        public void Execute(params object[] args) {
            var interplay_type = (InterplayType)args[0];
            var emit_object    = args[1] as Node;
            var receive_object = args[2] as Node;
            var interplay_item = args[3] as Node;
            var is_start        = (bool)args[4];
            if(is_start) {
                EmitSignal(nameof(long_interplay_start),interplay_type,emit_object,receive_object,interplay_item);
            } else {
                EmitSignal(nameof(long_interplay_end),interplay_type,emit_object,receive_object,interplay_item);
            }
        }
    }
}