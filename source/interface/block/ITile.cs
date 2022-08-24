namespace Box {
    public interface ITile : IBlock {
        SandboxLayer Layer {get;set;}
        int X {get;set;}
        int Y {get;set;}
    }
}