using Godot;
using System.Collections.Generic;

namespace Box.Components {
    [ClassName(nameof(EntityRecycleComponent))]
    public class EntityRecycleComponent : Node {
        protected Node2D parent;
        protected Game game;

        protected int old_region_x = int.MaxValue;
        protected int old_region_y = int.MaxValue;

        protected SandboxRegion region;

        public override void _Ready()
        {
            game = Game.Instance;
            parent = GetParent<Node2D>();
        }

        public override void _Process(float delta)
        {
            (int rx,int ry) = Sandbox.WorldToRegion(parent.Position);
            if(rx != old_region_x || ry != old_region_y) {
                SandboxRegion new_region = game.Sandbox.GetRegion<SandboxRegion>(rx,ry);
                if(region != null) {
                    region.Objects.Remove(parent);
                }
                if(new_region != null) {
                    new_region.Objects[parent] = parent;
                    region = new_region;
                }
                
                old_region_x = rx;
                old_region_y = ry;
            }
            if(region != null && region.Status != SandboxRegionStatus.Loading) {
                if(IsInsideTree()) {
                    parent.GetParent().RemoveChild(parent);
                }
            }
        }
    }
}