using Godot;
using Box.Components;

namespace Box.Blocks.Plants {
    [
        Register(nameof(Grass)),
        BindCell("grass",true),
        BindScene("res://source/nodes/blocks/plants/grass/Grass.tscn"),
    ]
    public class Grass : Node2D,ICellBlock {
        //硬度
        public int Hardness {get;protected set;} = 1;
        public SandboxLayer Layer {get;set;}
        public int X {get;set;}
        public int Y {get;set;}

         //被破坏时触发
        public void _DamageStart(Node entity) {}

        //破坏完毕时触发
        public void _DamageComplete(Node entity) {}
    }
}