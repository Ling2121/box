using Godot;
using System;
using Box.Components;

public class Entity2 : StaticBody2D
{
    CollisionEventComponent collision_event;

    public override void _Ready()
    {
        collision_event = GetNodeOrNull<CollisionEventComponent>(nameof(CollisionEventComponent));
        collision_event?.Connect(nameof(CollisionEventComponent.CollisionEntered),this,nameof(_Collision));
    }

    public void _Collision(CollisionObject2D a,CollisionObject2D b) {
        GD.Print(Name,":你碰到我了 ",b.Name);
    }
}
