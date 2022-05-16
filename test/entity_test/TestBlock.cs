using Godot;
using System;


namespace Box.Test.EntityTest {
    public class TestBlock : Node2D,IBlock
    {
        //硬度
        public int Hardness {get;} = 2;
        public override void _Ready()
        {
            
        }
        //被破坏时触发
        public void _DamageStart(Node entity) {}
        //破坏完毕时触发
        public void _DamageComplete(Node entity) {}
    }
}
