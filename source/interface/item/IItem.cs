using Godot;

namespace Box {
    //基本物品类
    public interface IItem {
        void Use(Node receive);
    }
}