using Godot;

namespace Box {
    //! 请务必从Godot.Object类型进行拓展
    //基本物品类
    public interface IItem {
        bool IsUse(Node emit,Node receive);
        void Use(Node receive);
    }
}