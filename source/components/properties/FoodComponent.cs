using Godot;

namespace Box.Components {
    [ClassName(nameof(FoodComponent))]
    public class FoodComponent : Node,IComponent {
        [Signal]
        public delegate void hunger(FoodComponent self);
        [Signal]
        public delegate void veryHungry(FoodComponent self);
        [Signal]
        public delegate void change(HPComponent self,int value);

        [Export]
        //食物值
        public int Food {
            get {
                return food;
            }
            set {
                if(value <= 0) value = 0;
                if(food != value) {
                    EmitSignal(nameof(change),this,value);
                    food = value;
                    if(value > MaxFood) {
                        food = MaxFood;
                    }
                    if(value == 0) {
                        food = 0;
                        EmitSignal(nameof(veryHungry),this);
                    }
                }
            }
        }
        [Export]
        //最大食物值
        public int MaxFood = 20;

        [Export]
        //食物衰减时间
        public float WeakTime = 5;

        [Export]
        //饥饿阈值，当到达百分比时会发送Hunger信号
        public float HungerThresholdValue = 0.2f;

        [Export]
        //食物衰减值
        public int WeakValue = 1;

        public bool IsHunger {get;protected set;} = false;

        protected int food = 20;
        protected float weak_timer = 0;

        public override void _Process(float delta)
        {
            weak_timer += delta;
            if(weak_timer >= WeakTime) {
                weak_timer = 0;
                Food -= WeakValue;
            }

            if(!IsHunger) {
                if(Food <= (MaxFood * HungerThresholdValue)) {
                    IsHunger = true;
                    EmitSignal(nameof(hunger),this);
                }
            } else {
                if(Food > (MaxFood * HungerThresholdValue)) {
                    IsHunger = false;
                }
            }
        }
    }
} 