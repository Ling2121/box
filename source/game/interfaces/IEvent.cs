namespace Box{    
    public interface IEvent {
        bool IsEnterEvent(params object[] args);
        void Execute(params object[] args);
    }
}