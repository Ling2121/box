using Godot;
using System;
using Box;

public class ExportTest : Node {

    string Status = "default";

    public override void _Ready()
    {
        /**[ProcessTreeBuild].Start**/
        var Tree = new ProcessTree();


        var __ptnode_0 = new Box.ProcessTreeNodes.Root();
        var __ptnode_1 = new Box.ProcessTreeNodes.SingleListener();
        var __ptnode_2 = new Box.ProcessTreeNodes.SingleListener();
        Tree.Root = __ptnode_0;

        __ptnode_0.Tree = Tree;
        __ptnode_0._Ready();
        __ptnode_1.Tree = Tree;
        __ptnode_1._Setting("SignalObject",this);
        __ptnode_1._Setting("SignalName","death");
        __ptnode_1._Ready();
        __ptnode_2.Tree = Tree;
        __ptnode_2._Setting("SignalObject",this);
        __ptnode_2._Setting("SignalName","move");
        __ptnode_2._Ready();
        __ptnode_0.Connect("Root",__ptnode_1,"I0");
        __ptnode_0.Connect("Root",__ptnode_2,"I0");
        /**[ProcessTreeBuild].End**/

        Tree.Start();
    }

}