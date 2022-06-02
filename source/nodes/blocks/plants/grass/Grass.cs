using Godot;
using Box.Components;
using System.Collections.Generic;

namespace Box.Blocks.Plants {
    [
        Register(nameof(Grass)),    
        BindCell("grass",true),
        BindScene("res://source/nodes/blocks/plants/grass/Grass.tscn"),
    ]
    public class Grass : Node2D,ICellBlock,IGrow {
        //硬度
        public int Hardness {get;protected set;} = 1;
        public SandboxLayer Layer {get;set;}
        public int X {get;set;}
        public int Y {get;set;}

        public int Stage {get;set;} = 0;
        //每个阶段的时间间隔 分钟为单位 从第一个阶段开始
        public List<long> StageSetting {get;set;} = new List<long>{
            300,//TimeHelper.MinuteDay(3),
            300,//TimeHelper.MinuteDay(3),
            300//TimeHelper.MinuteMonth(3),
        };
        public void _EnterNextStage(int stage) {
            Sandbox sandbox = Game.Instance.Sandbox;
            GD.Print(stage);
            sandbox.SetCellTile(Layer,X,Y,$"grass_stage_{stage}");
        }        

         //被破坏时触发
        public void _DamageStart(Node entity) {}

        //破坏完毕时触发
        public void _DamageComplete(Node entity) {}
    }
}