using Godot;
using System;


namespace Box.Test.BlockTest {
    public class Start : Node2D
    {
        Sandbox sandbox;
        public override void _Ready()
        {
            sandbox = GetNode<Sandbox>("Sandbox");
            for(int x = 0;x < 2;x++) {
                for(int y = 0;y < 2;y++) {
                    sandbox.SetCell(SandboxLayer.Land,x,y,"grass");
                    Node node = sandbox.SetCellBlockInstance(SandboxLayer.Land,x,y,"grass") as Node;
                    //sandbox.SetCell(SandboxLayer.Wall,x,y,"thorns");
                }
            }
        }

        public override void _Input(InputEvent @event)
        {
            if(@event is InputEventMouseButton){
                InputEventMouseButton mouse = @event as InputEventMouseButton;
                Vector2 position = Sandbox.WorldToCell(GetLocalMousePosition());        
                if(mouse.IsPressed()){
                    // if(mouse.ButtonIndex == (int)ButtonList.Left) {
                    //     sandbox.SetCell(SandboxLayer.Wall,(int)position.x,(int)position.y,"thorns");
                    // }
                }
            }
        }
    }
}
