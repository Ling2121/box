using System.Linq;
using System;
using System.Globalization;
using Godot;
using System.Collections.Generic;
using Box.Events;

namespace Box.Components {
    [ClassName(nameof(DamageBlockComponent))]
    public class DamageBlockComponent : Node {
        public const float BaseDamageTime = 0.5f;

        //damage_start、damage_end和damage_complete的position参数只有block是Tile时才有意义
        [Signal]
        //破坏动作开始
        public delegate void damage_start(Node entity,Godot.Object block,Vector2 position);
        [Signal]
        //破坏动作结束
        public delegate void damage_end(Node entity,Godot.Object  block,Vector2 position);
        [Signal]
        //完成破坏（破坏是可以被中断的）
        public delegate void damage_complete(Node entity,Godot.Object block,Vector2 position);

        public class DamageData {
            //设置为true时直接中断计时和破坏
            public bool Stop = false;
            public float Time = 0;
            public float Timer = 0;
            public int Durable = 0;
        } 

        public Dictionary<BlockRef,DamageData> DamageTable = new Dictionary<BlockRef, DamageData>();
        protected DamageEvent DamageEvent;

        public override void _Ready()
        {
            DamageEvent = Register.Instance.GetEvent(nameof(DamageEvent)) as DamageEvent;
        }

        public void DamageStart(BlockRef block_ref) {
            DamageData damage = new DamageData();
            IBlock block = block_ref.Block;
            damage.Time = block.Hardness * BaseDamageTime;
            DamageTable[block_ref] = damage;
            EmitSignal(nameof(damage_start),GetParent(),block_ref);
            DamageEvent.Execute(DamageEvent.ExecuteType.Start,GetParent(),block_ref);
        }

        public void DamageEnd(BlockRef block_ref) {
            if(DamageTable.ContainsKey(block_ref)) {
                IBlock block = block_ref.Block;
                Node entity = GetParent();

                var damage = DamageTable[block_ref];
                DamageTable.Remove(block_ref);

                EmitSignal(nameof(damage_end),entity,block_ref);
                DamageEvent.Execute(DamageEvent.ExecuteType.End,GetParent(),block_ref);
                if(damage.Timer >= damage.Time){
                    block._Damage(entity);
                    EmitSignal(nameof(damage_complete),entity,block_ref);
                    DamageEvent.Execute(DamageEvent.ExecuteType.Complete,GetParent(),block_ref);
                    block_ref.Remove();
                }
            }
        }

        public override void _Process(float delta)
        {
            var damage_arr = DamageTable.Keys.ToArray();
            foreach(var block in damage_arr) {
                var timer = DamageTable[block];
                timer.Timer += delta;
                if(timer.Timer >= timer.Time || timer.Stop){
                    
                    DamageEnd(block);
                }
            }
        }
    }
}