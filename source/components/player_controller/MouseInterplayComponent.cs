using Godot;
using Box.Events;

namespace Box.Components {

    [ClassName(nameof(MouseInterplayComponent))]
    public class MouseInterplayComponent : Node {
        
        public InterplayEventComponent InterplayEventComponent;

        public override void _Ready()
        {
            InterplayEventComponent = GetParent().GetNodeOrNull<InterplayEventComponent>(nameof(InterplayEventComponent));
        }

        public override void _Input(InputEvent @event)
        {
            if(InterplayEventComponent != null) {
                InputEventMouseButton mouse_event = @event as InputEventMouseButton;
                if(mouse_event != null){
                    if(mouse_event.IsPressed()) {
                        InterplayType interplay_type = InterplayType.MouseLeft;
                        if(mouse_event.ButtonIndex == (int)ButtonList.Left) {
                            interplay_type = InterplayType.MouseLeft;
                        }

                        if(mouse_event.ButtonIndex == (int)ButtonList.Right) {
                            interplay_type = InterplayType.MouseRight;
                        }

                        if(mouse_event.ButtonIndex == (int)ButtonList.WheelDown) {
                            interplay_type = InterplayType.MouseCenter;
                        }
                        if(InterplayEventComponent.Select != null) {
                            if(InterplayEventComponent.Select != InterplayEventComponent) {
                                GD.Print(interplay_type,"  ",InterplayEventComponent.Select);
                                InterplayEventComponent.EmitInterplayEvent(interplay_type,InterplayEventComponent.Select.GetParent());
                            }
                        }
                    }
                }
            }
        }
    }
}