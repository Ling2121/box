using Godot;

namespace Box {
    //! 请务必从Godot.Object类型进行拓展
    //工具类
    public interface ITool : IItem {
        int Hurt{get;set;}
        int Durable{get;set;}
    }
}