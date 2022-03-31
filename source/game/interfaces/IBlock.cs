using Godot;

namespace Box {
    /*
        方块也是实体的一种
        Tile可以绑定方块，当实体对其进行交互时才会创建其实例
    */
    public interface IBlock {
        int X {get;set;}
        int Y {get;set;}
        int Durable {get;set;}
        int Hardness {get;set;}
    }
}