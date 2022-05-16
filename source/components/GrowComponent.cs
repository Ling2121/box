using Godot;
using System;

namespace Box.Components {
    [ClassName(nameof(GrowComponent))]
    public class GrowComponent : Node,IComponent {
        public class GrowComponentError : Exception {
            public GrowComponentError():base($"{nameof(GrowComponent)}组件的父节点需要为IGrow类型") {

            }
        }
        
        public override void _Ready()
        {
            Node parent = GetParent();
            if(!(parent is IGrow)) {
                throw new GrowComponentError();
            }
            var grow_system = Game.Instance.GrowSystem;
            grow_system.AddGrow(parent as IGrow);
        }
    }
}