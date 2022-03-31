using Godot;

namespace Box {
    //工具类
    public interface ITool : IItem {
        int Hurt{get;set;}
        int Durable{get;set;}
    }
}