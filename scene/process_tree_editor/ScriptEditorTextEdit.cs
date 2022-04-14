using Godot;
using System;

public class ScriptEditorTextEdit : TextEdit
{
    // Declare member variables here. Examples:
    // private int a = 2;
    // private string b = "text";

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        AddKeywordColor("var",Colors.SkyBlue);
        
        AddKeywordColor("if",Colors.SkyBlue);
        AddKeywordColor("else",Colors.SkyBlue);
        AddKeywordColor("while",Colors.SkyBlue);
        AddKeywordColor("switch",Colors.SkyBlue);
        AddKeywordColor("case",Colors.SkyBlue);
        AddKeywordColor("default",Colors.SkyBlue);
        
        AddKeywordColor("void",Colors.Purple);
        AddKeywordColor("Action",Colors.Purple);
        AddKeywordColor("Func",Colors.Purple);
        AddKeywordColor("string",Colors.Purple);
        AddKeywordColor("int",Colors.Purple);
        AddKeywordColor("bool",Colors.Purple);
        AddKeywordColor("float",Colors.Purple);
        AddKeywordColor("double",Colors.Purple);
    }

//  // Called every frame. 'delta' is the elapsed time since the previous frame.
//  public override void _Process(float delta)
//  {
//      
//  }
}
