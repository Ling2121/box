namespace Box {
    public interface ICellBlock : IBlock {
        SandboxLayer Layer {get;set;}
        int X {get;set;}
        int Y {get;set;}
    }
}