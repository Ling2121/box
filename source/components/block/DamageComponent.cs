using Godot;
using System;
using System.Diagnostics;

namespace Box.Components {
    //只能放在IBlock继承类的下面
    [ClassName(nameof(DamageComponent))]
    public class DamageComponent : Node,IComponent {
        public static float DamageBaseTime = 0.5f;
        public class DamageComponentError : Exception {
            public DamageComponentError():base("DamageComponent只能放在Block下") {}
        }
        protected Node Block;
        protected Stopwatch timer = new Stopwatch();
        protected float time;

        InterplayComponent InterplayComponent;

        public override void _Ready()
        {
            Block = GetParent();
            if(!(Block is IBlock)) {
                throw new DamageComponentError();
            }
            InterplayComponent = EntityHelper.GetComponent<InterplayComponent>(Block);
            IBlock block = Block as IBlock;
            time = (block.Hardness * DamageBaseTime) * 1000;
            InterplayComponent.Connect(nameof(InterplayComponent.receive_long_interplay_start),this,nameof(_ReceiveLongInterplayStart));
            InterplayComponent.Connect(nameof(InterplayComponent.receive_long_interplay_end),this,nameof(_ReceiveLongInterplayEnd));
        }

        public void _ReceiveLongInterplayStart(InterplayComponent.InterplayItem item) {
            item.EndCondition = i => {
                return timer.ElapsedMilliseconds >= time;
            };
            timer.Start();
        }

        public void _ReceiveLongInterplayEnd(InterplayComponent.InterplayItem item) {
            if(timer.ElapsedMilliseconds >= time) {
                timer.Stop();
                timer.Reset();
                Block.QueueFree();
            }
        }
    }
}