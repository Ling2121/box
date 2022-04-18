using System;
using Godot;


namespace Box.Events {

    [Register(nameof(DamageEvent))]
    public class DamageEvent : Godot.Object,IEvent,IRegister {
        public enum ExecuteType {
            Start,
            End,
            Complete,
        }

        [Signal]
        //破坏动作开始
        public delegate void damage_start(Node entity,BlockRef block);
        [Signal]
        //破坏动作结束
        public delegate void damage_end(Node entity,BlockRef  block);
        [Signal]
        //完成破坏（破坏是可以被中断的）
        public delegate void damage_complete(Node entity,BlockRef block);

        public bool IsEnterEvent(params object[] args) {
            if(args.Length < 3) return false;
            return 
                args[0] is ExecuteType &&
                args[1] is Node &&
                args[2] is BlockRef
            ;
        }
        public void Execute(params object[] args) {
            ExecuteType execute_type = (ExecuteType)args[0];
            Node entity = args[1] as Node;
            BlockRef block = args[2] as BlockRef;
            switch(execute_type){
                case ExecuteType.Start : {
                    EmitSignal(nameof(damage_start),entity,block);
                }break;
                case ExecuteType.End : {
                    EmitSignal(nameof(damage_end),entity,block);
                }break;
                case ExecuteType.Complete : {
                    EmitSignal(nameof(damage_complete),entity,block);
                }break;
            }
        }
    }
}