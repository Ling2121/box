using Godot;

namespace Box {
    //基本物品类
    public interface IItem {
        bool IsUse(Node emit,Node receive);
        void Use(Node receive);
    }
}