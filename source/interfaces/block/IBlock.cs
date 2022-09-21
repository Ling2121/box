using Godot;

namespace Box {
    public interface IBlock {
        //硬度
        int Hardness {get;}

        //被破坏时触发
        void _DamageStart(Node entity); 
        //破坏完毕时触发
        void _DamageComplete(Node entity);
    }
}