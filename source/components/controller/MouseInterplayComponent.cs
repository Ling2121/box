using System.Diagnostics;
using Godot;
using Box.Events;
using System.Collections.Generic;

namespace Box.Components
{
    [ClassName(nameof(MouseInterplayComponent))]
    public class MouseInterplayComponent : Node, IComponent
    {
        public const int LongInterplayTime = 200;//ms

        InterplayComponent InterplayComponent;
        HandComponent HandComponent;
        AttackComponent AttackComponent;
        Stopwatch long_attack_timer = new Stopwatch();
        Stopwatch long_interplay_timer = new Stopwatch();
        Node Parent;
        Node Select;
        List<InterplayComponent.InterplayItem> InterplayItems = new List<InterplayComponent.InterplayItem>();

        bool is_drag_attack = false;

        public Node GetSelect() {
            if(InterplayComponent.Select != null) return InterplayComponent.Select;
            Sandbox sandbox = Game.Instance.Sandbox;
            Vector2 mouse_p = sandbox.GetLocalMousePosition();
            Vector2 tile_p = Sandbox.WorldToCell(mouse_p);

            int max_layer = (int)SandboxLayer.BG - 1;
            for(int i = max_layer;i >= 0;i--){
                IBlock block = sandbox.GetCellBlockInstance((SandboxLayer)i,(int)tile_p.x,(int)tile_p.y);
                if(block != null){
                    return (block as Node);
                }
            }

            return null;
        }

        public override void _Ready()
        {
            Parent = GetParent();
            InterplayComponent = EntityHelper.GetComponent<InterplayComponent>(Parent);
            HandComponent = EntityHelper.GetComponent<HandComponent>(Parent);
            AttackComponent = EntityHelper.GetComponent<AttackComponent>(Parent);
        }

        public override void _Input(InputEvent @event)
        {
            if(@event is InputEventMouseMotion) {
                if(Input.IsActionPressed("attack")) {
                    Node new_select = GetSelect();
                    if(Select != null) {
                        
                        if(new_select != Select) {
                            foreach(var item in InterplayItems) {
                                InterplayComponent.LongInterplayEnd(item);
                            }
                            InterplayItems.Clear();
                            Select = null;
                        }
                    }
                    if(new_select != Select) {
                        if(new_select != null) {
                            Attack(new_select);
                        }
                    }
                }
            }
        }

        protected void Attack(Node select) {
            if(!HandComponent.IsEmpty()) {
                foreach (var hand in HandComponent.Hands.Values) {
                    var item = InterplayComponent.LongInterplayStart(InterplayType.Attack, select, hand.Take,null);
                    InterplayItems.Add(item);
                }
            } else {
                var item = InterplayComponent.LongInterplayStart(InterplayType.Attack, select, AttackComponent,null);
                InterplayItems.Add(item);

            }
            Select = select;
        }

        public override void _Process(float delta)
        {
            if(HandComponent != null) {
                if (Input.IsActionJustReleased("attack")) {
                    is_drag_attack = false;
                    Select = null;
                    foreach(var item in InterplayItems) {
                        InterplayComponent.LongInterplayEnd(item);
                    }
                    InterplayItems.Clear();
                    long_attack_timer.Stop();
                    long_attack_timer.Reset();
                    
                }
                if(Input.IsActionJustPressed("attack")) {
                    Node select = GetSelect();
                    if(select == null) return;
                    long_attack_timer.Start();
                    if(!HandComponent.IsEmpty()) {
                        foreach (var hand in HandComponent.Hands.Values) {
                            var item = InterplayComponent.Interplay(InterplayType.Attack, select, hand.Take);
                            InterplayItems.Add(item);
                        }
                    } else {
                        var item = InterplayComponent.Interplay(InterplayType.Attack, select, AttackComponent);
                        InterplayItems.Add(item);
                    }
                    Select = select;
                }
                if(long_attack_timer.IsRunning) {
                    if(long_attack_timer.ElapsedMilliseconds > LongInterplayTime) {
                        long_attack_timer.Stop();
                        long_attack_timer.Reset();

                        Node select = GetSelect();
                        if(select == null) return;

                        Attack(select);
                        long_attack_timer.Stop();
                    }
                }
            }
        }
    }

}