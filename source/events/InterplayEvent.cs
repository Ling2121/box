using Godot;
using Box.Components;
using Box.Itmes;
using Box.Itmes.Tools;

namespace Box.Events {
    public enum InterplayType {
        Attack,
        Interplay,
        MouseCenter,
    }

    [Register(nameof(InterplayEvent))]
    public class InterplayEvent : IEvent {

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

            EventListeningComponent emit_event_listening = emit_object.GetNodeOrNull<EventListeningComponent>(nameof(EventListeningComponent));
            InterplayEventListener emit_event = emit_event_listening?.GetListener<InterplayEventListener>();
            emit_event?.EmitSignal(nameof(InterplayEventListener.emit_interplay),receive_object,interplay_type,interplay_item);
            

            EventListeningComponent receive_event_listening = receive_object.GetNodeOrNull<EventListeningComponent>(nameof(EventListeningComponent));
            InterplayEventListener receive_event = receive_event_listening?.GetListener<InterplayEventListener>();
            receive_event?.EmitSignal(nameof(InterplayEventListener.receive_interplay),emit_object,interplay_type,interplay_item);
        }
    }
}