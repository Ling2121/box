using Godot;

namespace Box.Components {

    public enum ControllerMode {
        Keyboard,
        Gamepad,
    }


    [ClassName(nameof(MoveControllerComponent))]
    public class MoveControllerComponent : Node {

        [Export]
        ControllerMode ControllerMode = ControllerMode.Keyboard;
        [Export]
        public float Speed = 100;

        protected MoveComponent move_component;

        public override void _Ready()
        {
            move_component = EntityHelper.GetComponent<MoveComponent>(GetParent());
            if(move_component == null) {
                GD.PushError($"{nameof(MoveControllerComponent)} : 父类需要有 {nameof(MoveComponent)} 组件");
            }
        }

        public override void _Process(float delta)
        {
            Vector2 dir = new Vector2(0,0);
            if(this.ControllerMode == ControllerMode.Keyboard) {
                if(Input.IsActionPressed("move_up")) {
                    dir.y = -Speed;
                }
                if(Input.IsActionPressed("move_down")) {
                    dir.y = Speed;
                }
                if(Input.IsActionPressed("move_left")) {
                    dir.x = -Speed;
                }
                if(Input.IsActionPressed("move_right")) {
                    dir.x = Speed;
                }
            }
            if(this.ControllerMode == ControllerMode.Gamepad) {
                dir.x = Input.GetJoyAxis(0,(int)JoystickList.Axis0) * Speed;
                dir.y = Input.GetJoyAxis(0,(int)JoystickList.Axis1) * Speed;
            }

            if(move_component != null) {
                move_component.Direction += dir;
            }
        }
    }
}