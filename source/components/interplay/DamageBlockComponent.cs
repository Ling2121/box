using System;
using System.Globalization;
using Godot;
using System.Collections.Generic;

namespace Box.Components {
    public class DamageBlockComponent : Node {
        public const float BaseDamageTime = 0.5f;

        [Signal]
        //破坏动作开始
        public delegate void damage_start(Node entity,IBlock block);
        [Signal]
        //破坏动作结束
        public delegate void damage_end(Node entity,IBlock block);
        [Signal]
        //完成破坏（破坏是可以被中断的）
        public delegate void damage_complete(Node entity,IBlock block);

        public class DamageTimer {
            //设置为true时直接中断计时和破坏
            public bool Stop = false;
            public float Time = 0;
            public float Timer = 0;
        } 

        public Dictionary<IBlock,DamageTimer> DamageTable = new Dictionary<IBlock, DamageTimer>();

        public void DamageStart(IBlock block) {
            DamageTimer timer = new DamageTimer();
            timer.Time = block.Hardness * BaseDamageTime;
            DamageTable[block] = timer;
        }

        public void DamageEnd(IBlock block) {
            var timer = DamageTable[block];
            DamageTable.Remove(block);
            EmitSignal(nameof(damage_end),GetParent(),block);
            if(timer.Timer >= timer.Time){
                EmitSignal(nameof(damage_complete),GetParent(),block);
            }
        }

        public override void _Process(float delta)
        {
            foreach(var item in DamageTable) {
                var timer = item.Value;
                timer.Timer += delta;
                if(timer.Timer >= timer.Time || timer.Stop){
                    DamageEnd(item.Key);
                }
            }
        }
    }
}