using Godot;
using Box.Events;

namespace Box.Components {

    [ClassName(nameof(MouseInterplayComponent))]
    public class MouseInterplayComponent : Node {
        
        public InterplayEventListener InterplayEventListener;

        public HandComponent HandComponent;
        public DamageBlockComponent DamageBlockComponent;
        public BlockRef SelectBlock = null;

        public BlockRef GetSelectBlock() {
            BlockRef block_index = new BlockRef();
            if(InterplayEventListener.Select is IBlock){
                block_index.IsTile = false;
                block_index.Block = InterplayEventListener.Select as IBlock;
                return block_index;
            }
            
            Sandbox sandbox = Game.Instance.Sandbox;
            Vector2 mouse_p = sandbox.GetLocalMousePosition();
            Vector2 tile_p = Sandbox.WorldToCell(mouse_p);
            
            block_index.IsTile = true;
            block_index.Position = tile_p;
            
            int max_layer = (int)SandboxLayer.BG - 1;
            for(int i = max_layer;i >= 0;i--){
                IBlock block = sandbox.GetCellBindBlock((SandboxLayer)i,(int)tile_p.x,(int)tile_p.y);
                if(block != null){
                    block_index.Block = block;
                    block_index.Layer = (SandboxLayer)i;
                    return block_index;
                }
            }
            return null;
        }

        public override void _Ready()
        {
            EventListeningComponent event_listening = GetParent().GetNodeOrNull<EventListeningComponent>(nameof(EventListeningComponent));

            InterplayEventListener = event_listening.GetListener<InterplayEventListener>();

            HandComponent = GetParent().GetNode<HandComponent>(nameof(HandComponent));
            DamageBlockComponent = GetParent().GetNode<DamageBlockComponent>(nameof(DamageBlockComponent));
        }

        public override void _Input(InputEvent @event)
        {
            if(InterplayEventListener != null) {
                InputEventMouseButton mouse_event = @event as InputEventMouseButton;
                InputEventMouseMotion mouse_move =  @event as InputEventMouseMotion;
                if(mouse_move != null) {
                    var new_select = GetSelectBlock();
                    if(SelectBlock != null) {
                        if(new_select?.Block != SelectBlock.Block) {
                            DamageBlockComponent.DamageEnd(SelectBlock);
                            SelectBlock = null;
                        }
                    } else {
                        if(new_select != null) {
                            SelectBlock = new_select;
                            DamageBlockComponent.DamageStart(SelectBlock);
                        }
                    }
                }
                if(mouse_event != null){
                    if(mouse_event.IsPressed()) {
                        if(HandComponent != null){
                            InterplayType interplay_type = InterplayType.Attack;
                            if(mouse_event.ButtonIndex == (int)ButtonList.Left) {
                                interplay_type = InterplayType.Attack;
                            }

                            if(mouse_event.ButtonIndex == (int)ButtonList.Right) {
                                interplay_type = InterplayType.Interplay;
                            }

                            if(mouse_event.ButtonIndex == (int)ButtonList.WheelDown) {
                                interplay_type = InterplayType.MouseCenter;
                            }
                            if(InterplayEventListener.Select != null) {
                                if(interplay_type == InterplayType.Attack) {
                                    if(InterplayEventListener.Select != InterplayEventListener) {
                                        HandComponent.EmitAttack(InterplayEventListener.Select.Entity);
                                    }
                                }
                                if(interplay_type == InterplayType.Interplay) {
                                    HandComponent.EmitUse(InterplayEventListener.Select.Entity);
                                }
                            } else {
                                if(interplay_type == InterplayType.Interplay) {
                                    HandComponent.EmitUse(GetParent());
                                }
                            }
                        }
                        if(DamageBlockComponent != null) {
                            var select = GetSelectBlock();
                            SelectBlock = select;
                            if(SelectBlock != null){
                                DamageBlockComponent.DamageStart(SelectBlock);
                            }
                        } 
                    }
                    else {
                        if(DamageBlockComponent != null) {
                            if(SelectBlock != null) {
                                DamageBlockComponent.DamageEnd(SelectBlock);
                            }
                        }
                    }
                }
            }
        }
    }
}