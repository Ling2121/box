using Godot;

namespace Box {
    [ClassName(nameof(EventManager))]
    public class EventManager : Node {
        Register register = Register.Instance;

        public override void _EnterTree()
        {
            Game.Instance.EventManager = this;
        }

        public bool PublishEvent(string event_name,params object[] args) {
            IEvent e = register.GetEvent(event_name);
            if(e == null) return false;
            if(e.IsEnterEvent(args)) {
                e.Execute(args);
            }
            return true;
        }
    }
}