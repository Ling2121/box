namespace Box{    
    public interface IEvent {
        void Execute(params object[] args);
    }
}