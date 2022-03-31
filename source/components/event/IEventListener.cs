using Godot;

namespace Box.Components {
    public interface IEventListener {
        Node Entity {get;set;}
        //是否从场景中删除
        //如果需要让监听器常驻于场景返回false即可
        //默认为true
        bool IsRemove();
        void _InitListener();
    }
}