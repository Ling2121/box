using System.Threading.Tasks;
using Godot;
using System;

public class ThreadTestScene : Node2D
{
    Timer timer = new Timer();
    Sprite icon;

    public override void _Ready()
    {
        icon = GetNode<Sprite>("Icon");
        Task.Run(()=>{
            while(true) {
                icon.Position = new Vector2((float)GD.RandRange(0,500),(float)GD.RandRange(0,500));
            }
        });

        Task.Run(()=>{
            while(true) {
                icon.Position = new Vector2((float)GD.RandRange(0,500),(float)GD.RandRange(0,500));
            }
        });
        // Task.Run(()=>{
        //     while(true) {
        //         System.Threading.Thread.Sleep(1000);
        //         Sprite sprite = new Sprite();
        //         sprite.Texture = GD.Load<Texture>("res://icon.png");

        //         sprite.Position = new Vector2((float)GD.RandRange(0,500),(float)GD.RandRange(0,500));
        //         //this.CallDeferred("add_child",sprite);
        //         AddChild(sprite);
        //     }
        // });

        // Task.Run(()=>{
        //     while(true) {
        //         System.Threading.Thread.Sleep(1000);
        //         Sprite sprite = new Sprite();
        //         sprite.Texture = GD.Load<Texture>("res://icon.png");
        //         sprite.Position = new Vector2((float)GD.RandRange(0,500),(float)GD.RandRange(0,500));
        //         //this.CallDeferred("add_child",sprite);
        //         AddChild(sprite);
        //     }
        // });

        // timer.WaitTime = 1;
        // timer.Autostart = true;
        // timer.Start();

        // timer.Connect("timeout",this,"Timeout");

        // AddChild(timer);
    }

    public void Timeout()
    {
        // GD.Print("aa");
        // Sprite sprite = new Sprite();
        // sprite.Texture = GD.Load<Texture>("res://icon.png");

        // this.AddChild(sprite);
    }
}
