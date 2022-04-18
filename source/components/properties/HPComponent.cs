using System;
using Godot;

namespace Box.Components {

    [ClassName(nameof(HPComponent))]
    public class HPComponent : Node,IComponent {
        [Signal]
        public delegate void death(HPComponent self);
        [Signal]
        public delegate void injured(HPComponent self,int value);
        [Signal]
        public delegate void recovery(HPComponent self,int value);
        [Signal]
        public delegate void change(HPComponent self,int value);

        [Export]
        public int HP {
            get {return hp;}
            
            set {
                if(value != hp) {
                    EmitSignal(nameof(change),this,value);
                    int s = value - hp;
                    hp = value;
                    if(s < 0) {
                        EmitSignal(nameof(injured),this,Mathf.Abs(s));
                    }
                    if(s > 0) {
                        EmitSignal(nameof(recovery),this,s);
                    }
                    if(hp > MaxHP) {
                        hp = MaxHP;
                    }
                }
            }
        }

        [Export]
        public int MaxHP = 20;

        protected int hp = 20;

        public override void _Process(float delta)
        {
            if(hp <= 0) {
                hp = 0;
                EmitSignal(nameof(death),this);
                if(hp <= 0) {
                    GetParent().QueueFree();
                }
            }
        }
    }
}