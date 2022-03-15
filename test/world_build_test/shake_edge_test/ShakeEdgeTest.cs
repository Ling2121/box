using System.Collections.Generic;
using Godot;
using System;
using Box.VoronoiMap;

public class ShakeEdgeTest : Node2D
{
    // Declare member variables here. Examples:
    // private int a = 2;
    // private string b = "text";

    // Called when the node enters the scene tree for the first time.
    Vertex root;

    public override void _Ready()
    {
        Position2D p1_node = GetNode<Position2D>("P1");
        Position2D p2_node = GetNode<Position2D>("P2");
        Position2D p3_node = GetNode<Position2D>("P3");
        Position2D p4_node = GetNode<Position2D>("P4");

        RandomNumberGenerator random = new RandomNumberGenerator();
        random.Seed = 23;



        root = new Vertex(p1_node.Position);
        Vertex p2 = new Vertex(p2_node.Position);
        Vector2 p3 = p3_node.Position;
        Vector2 p4 = p4_node.Position;
        //Voronoi.ShakeEdge(root,p2,p3,p4,4,0.6f,0.85f,random);

        
    }

    public override void _Draw()
    {
        List<Vector2> points = new List<Vector2>();
        Vertex node = root;
        while(node != null) {
            points.Add(node.Vector);
            node = node.Next;
        }

        var arr = points.ToArray();
        DrawColoredPolygon(arr,Colors.Blue);
        DrawPolyline(arr,Colors.Red);

        node = root;
        while(node != null) {
            DrawCircle(node.Vector,1,Colors.Green);
            node = node.Next;
        }
    }

}
