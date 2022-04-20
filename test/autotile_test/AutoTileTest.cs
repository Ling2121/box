using System.Diagnostics;
using Godot;
using System;

public class AutoTileTest : Node2D
{
    Stopwatch time = new Stopwatch();
    public override void _Ready()
    {
        
        
    }

    public override void _Process(float delta)
    {
        if(Input.IsActionJustPressed("attack")) {
            time.Start();
        }
        if (Input.IsActionJustReleased("attack"))
        {
            time.Stop();
            GD.Print(time.ElapsedMilliseconds);
            time.Reset();
        }
    }

    public override void _Input(InputEvent @event)
    {
        // if(@event is InputEventMouseButton){
        //     if(@event.IsActionPressed("attack")){
        //         time.Start();
        //     }

        //     if(@event.IsActionReleased("attack")) {
        //         time.Stop();
        //         GD.Print(time.ElapsedMilliseconds);
        //         time.Reset();
        //     }
        // }
    }

}
