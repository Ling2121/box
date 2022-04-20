using System.Diagnostics;
using Godot;
using Box.Events;
using System;

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

        public Node GetSelect() {
            if(InterplayComponent.Select != null) return InterplayComponent.Select;
            Sandbox sandbox = Game.Instance.Sandbox;
            Vector2 mouse_p = sandbox.GetLocalMousePosition();
            Vector2 tile_p = Sandbox.WorldToCell(mouse_p);
            
            int max_layer = (int)SandboxLayer.BG - 1;
            for(int i = max_layer;i >= 0;i--){
                IBlock block = sandbox.GetCellBlockInstance((SandboxLayer)i,(int)tile_p.x,(int)tile_p.y);
                if(block != null){
                    return block as Node;
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

        public override void _Process(float delta)
        {
            if(HandComponent != null) {
                if(Input.IsActionJustPressed("attack")) {
                    Node select = GetSelect();
                    if(select == null) return;

                    long_attack_timer.Start();
                    if(!HandComponent.IsEmpty()) {
                        foreach (var hand in HandComponent.Hands.Values) {
                            InterplayComponent.Interplay(InterplayType.Attack, select, hand.Take);
                        }
                    } else {
                        InterplayComponent.Interplay(InterplayType.Attack, select, AttackComponent);
                    }
                }
                if(long_attack_timer.IsRunning) {
                    if (Input.IsActionJustReleased("attack")) {
                        long_attack_timer.Stop();
                        long_attack_timer.Reset();
                    }
                    if(long_attack_timer.ElapsedMilliseconds > LongInterplayTime) {
                        Node select = GetSelect();
                        if(select == null) return;

                        if(!HandComponent.IsEmpty()) {
                        foreach (var hand in HandComponent.Hands.Values) {
                            InterplayComponent.LongInterplayStart(InterplayType.Attack, select, hand.Take,null);
                        }
                        } else {
                            InterplayComponent.LongInterplayStart(InterplayType.Attack, select, AttackComponent,null);
                        }
                        long_attack_timer.Stop();
                    }
                }
            }
        }
    }

}