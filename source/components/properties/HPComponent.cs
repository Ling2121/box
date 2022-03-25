using System;
using Godot;

namespace Box.Components {

    [ClassName(nameof(HPComponent))]
    public class HPComponent : Node {
        [Signal]
        public delegate void Death(HPComponent self);
        [Signal]
        public delegate void Injured(HPComponent self,int value);
        [Signal]
        public delegate void Recovery(HPComponent self,int value);
        [Signal]
        public delegate void Change(HPComponent self,int value);

        [Export]
        public int HP {
            get {return hp;}
            
            set {
                if(value != hp) {
                    EmitSignal(nameof(Change),this,value);
                    int s = value - hp;
                    hp = value;
                    if(s < 0) {
                        EmitSignal(nameof(Injured),this,Mathf.Abs(s));
                    }
                    if(s > 0) {
                        EmitSignal(nameof(Recovery),this,s);
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
                EmitSignal(nameof(Death),this);
                if(hp <= 0) {
                    GetParent().QueueFree();
                }
            }
        }
    }
}