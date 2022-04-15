using Godot;
using System;


namespace Box.Test.BlockTest {
    public class Start : Node2D
    {
        Sandbox sandbox;
        public override void _Ready()
        {
            sandbox = GetNode<Sandbox>("Sandbox");
            for(int x = 0;x < 16;x++) {
                for(int y = 0;y < 16;y++) {
                    sandbox.SetCell(SandboxLayer.Land,x,y,"sand");
                    sandbox.SetCell(SandboxLayer.Wall,x,y,"thorns");
                }
            }
        }

        public override void _Input(InputEvent @event)
        {
            if(@event is InputEventMouseButton){
                InputEventMouseButton mouse = @event as InputEventMouseButton;
                Vector2 position = (GetLocalMousePosition() / Sandbox.REGION_CELL_PIXEL_SIZE).Floor();        
                if(mouse.IsPressed()){
                    GD.Print(position);
                    if(mouse.ButtonIndex == (int)ButtonList.Left) {
                        sandbox.SetCell(SandboxLayer.Wall,(int)position.x,(int)position.y,"thorns");
                    }
                    if(mouse.ButtonIndex == (int)ButtonList.Right) {
                        sandbox.SetCell(SandboxLayer.Wall,(int)position.x,(int)position.y,"");
                    }
                }
            }
        }
    }
}
