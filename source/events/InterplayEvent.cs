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
            if(args.Length < 4) return false;
            return 
                args[0] is InterplayType && //互动类型
                args[1] is Node &&          //发出互动的对象
                args[2] is Node &&          //接收互动的对象
                args[3] is Node             //互动使用的物体
                ;
        }
        public void Execute(params object[] args) {
            var interplay_type = (InterplayType)args[0];
            var emit_object    = args[1] as Node;
            var receive_object = args[2] as Node;
            var interplay_item = args[3] as Node;
            EmitSignal(nameof(interplay),emit_object,receive_object,interplay_item);
        }
    }
}