using System;
using Godot;
using System.Collections.Generic;

namespace Box.Components {
    [ClassName(nameof(HandComponent))]
    public class HandComponent : Node2D,IComponent {
        [Signal]
        public delegate void task(string head,Node obj);
        [Signal]
        public delegate void discarded(string head,Node obj);

        public class Hand {
            public Node Take;
        }

        public int TakeCount {get;protected set;} = 0;


        public Dictionary<string,Hand> Hands = new Dictionary<string, Hand>();

        public void AddHand(string name) {
            Hands[name] = new Hand();
        }

        public bool IsEmpty() {
            return TakeCount == 0;
        }

        public bool IsEmpty(string hand_name) {
            if(!Hands.ContainsKey(hand_name)) return true;
            return Hands[hand_name].Take == null;
        }

        public void Take(string hand_name,Node obj) {
            if(!Hands.ContainsKey(hand_name)) return;
            Hand hand = Hands[hand_name];
            if(hand.Take != null) {
                Discarded(hand_name);
            }
            hand.Take = obj;
            TakeCount++;
            EmitSignal(nameof(task),hand_name,obj);
        }

        public void Discarded(string hand_name) {
            if(!Hands.ContainsKey(hand_name)) return;
            Hand hand = Hands[hand_name];
            if(hand.Take != null) {
                hand.Take = null;
                TakeCount--;
                EmitSignal(nameof(discarded),hand_name,hand.Take);
            }

        }

        public Node GetTask(string hand_name) {
            if(!Hands.ContainsKey(hand_name)) return null;
            Hand hand = Hands[hand_name];
            return hand.Take;
        }
    }
}