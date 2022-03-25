using Godot;

namespace Box {
    public class GameStart : Node {
        public override void _EnterTree()
        {
            FileSystem.Instance.Init();
            Register.Instance.Init();
        }

        public override void _Ready()
        {
            GetParent().CallDeferred("remove_child",this);
        }
    }
}