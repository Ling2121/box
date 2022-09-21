using Godot;

namespace Box {
    public interface IContainer {
        int Capacity {get;}
        IItem GetItem(int position);
        void AddItem(int position,IItem item);
        IItem RemoveItem(int position);
    }
}