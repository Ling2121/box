namespace Box{    
    public interface IEvent {
        bool IsEnterEvent(object a,object b);
        void _Execute(object a,object b);
    }
}