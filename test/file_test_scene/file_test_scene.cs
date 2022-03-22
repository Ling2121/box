using System.IO;
using System;
using SysFile = System.IO.File;

public class file_test_scene : Godot.Node2D
{
    // Declare member variables here. Examples:
    // private int a = 2;
    // private string b = "text";

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        int v1 = 12;
        int v2 = 2333;
        int v3 = -1;

        FileStream sys_file = SysFile.Open("test/file_test_scene/sys_file.f",FileMode.OpenOrCreate);
        BinaryWriter sys_file_w = new BinaryWriter(sys_file);
        sys_file_w.Write(v1);
        sys_file_w.Write(v2);
        sys_file_w.Write(v3);
        sys_file_w.Close();
        sys_file.Close();

        Godot.File gd_file = new Godot.File();
        gd_file.Open("test/file_test_scene/gd_file.f",Godot.File.ModeFlags.WriteRead);
        gd_file.Store32((uint)v1);
        gd_file.Store32((uint)v2);
        gd_file.Store32((uint)v3);
        gd_file.Close();
    }

//  // Called every frame. 'delta' is the elapsed time since the previous frame.
//  public override void _Process(float delta)
//  {
//      
//  }
}
