using Godot;

namespace Box {
    [ClassName(nameof(EventManager))]
    public class EventManager : Node {
        Register register = Register.Instance;

        public override void _EnterTree()
        {
            Game.Instance.EventManager = this;
        }

        public bool RequestEvent(string event_name,object a,object b) {
            IEvent e = register.GetEvent(event_name);
            if(e == null) return false;
            if(e.IsEnterEvent(a,b)) {
                e._Execute(a,b);
            }
            return true;
        }
    }
}