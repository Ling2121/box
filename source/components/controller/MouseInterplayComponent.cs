using Godot;
using Box.Events;

namespace Box.Components {

    [ClassName(nameof(MouseInterplayComponent))]
    public class MouseInterplayComponent : Node {
        
        public InterplayEventListener InterplayEventListener;

        public HandComponent HandComponent;

        public override void _Ready()
        {
            EventListeningComponent event_listening = GetParent().GetNodeOrNull<EventListeningComponent>(nameof(EventListeningComponent));

            InterplayEventListener = event_listening.GetListener<InterplayEventListener>();

            HandComponent = GetParent().GetNode<HandComponent>(nameof(HandComponent));
        }

        public override void _Input(InputEvent @event)
        {
            if(InterplayEventListener != null) {
                InputEventMouseButton mouse_event = @event as InputEventMouseButton;
                if(mouse_event != null){
                    if(mouse_event.IsPressed()) {
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
                            // if(interplay_type == InterplayType.Interplay) {
                            //     HandComponent.EmitUse();
                            // }
                        }
                    }
                }
            }
        }
    }
}