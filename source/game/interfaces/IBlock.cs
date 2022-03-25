using Godot;

namespace Box {
    public interface IBlock {
        int X {get;set;}
        int Y {get;set;}

        void _Collision(CollisionObject2D collision);
    }
}