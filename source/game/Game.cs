namespace Box {
    public class Game : Singleton<Game> {
        public Sandbox Sandbox;
        public TimeSystem TimeSystem;
        public EventManager EventManager;
        public DiurnalCycleSystem DiurnalCycleSystem;
        public GrowSystem GrowSystem;
    }
}