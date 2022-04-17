using Godot;

namespace Box {
    /*
        方块也是实体的一种
        Tile可以绑定方块，当实体对其进行交互时才会创建其实例
    */
    public interface IBlock {
        int X {get;set;}
        int Y {get;set;}
        //耐久
        int Durable {get;set;}
        //硬度
        int Hardness {get;set;}
        //绑定tile时触发
        void _CellBind();
        //接触绑定tile时触发
        void _CellUnbind();
        //被破坏时触发
        void _Damage(Node entity);
    }
}