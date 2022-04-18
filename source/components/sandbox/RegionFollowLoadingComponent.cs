using Godot;

namespace Box.Components {
    [ClassName(nameof(RegionFollowLoadingComponent))]
    public class RegionFollowLoadingComponent : Node,IComponent {
        [Export]
        public int LoadRadius = 4;
        protected int old_cx = 0;
        protected int old_cy = 0;

        protected int old_min_x = 0;
        protected int old_min_y = 0;
        protected int old_max_x = 0;
        protected int old_max_y = 0;

        protected void UpdateLoad(bool is_unload) {
            Sandbox sandbox = Game.Instance.Sandbox;
            (int cx,int cy) = Sandbox.WorldToRegion(GetParent<Node2D>().Position);
            int min_x = cx - LoadRadius,min_y = cy - LoadRadius;
            int max_x = cx + LoadRadius,max_y = cy + LoadRadius;

            if(is_unload) {
                int rx2 = old_min_x,ry2 = old_min_y;
                for(int ry = min_y;ry <= max_y;ry++) {
                    for(int rx = min_x;rx <= max_x;rx++) {
                        /* 当区块的IndexCount为0时会进行卸载 */
                        if(Util.PointIsNotIntersectBox(old_min_x,old_min_y,old_max_x,old_max_y,rx,ry)) {
                            /* 加到队列中会给区块的IndexCount加1 */
                            sandbox.RegionLoadInstructQueue.Enqueue((rx,ry));
                        }
                        if(Util.PointIsNotIntersectBox(min_x,min_y,max_x,max_y,rx2,ry2)) {
                            /* 加到队列中会给区块的IndexCount减1 */
                            sandbox.RegionUnloadInstructQueue.Enqueue((rx2,ry2));
                        }
                        rx2++;
                    }
                    ry2 ++; 
                    rx2 = old_min_x;
                }
            } else {
                for(int ry = min_y;ry <= max_y;ry++) {
                    for(int rx = min_x;rx <= max_x;rx++) {
                        sandbox.RegionLoadInstructQueue.Enqueue((rx,ry));
                    }
                }
            }
            old_cx = cx;
            old_cy = cy;
            old_min_x = min_x;
            old_min_y = min_y;
            old_max_x = max_x;
            old_max_y = max_y;
        }

        public override void _Ready()
        {
            UpdateLoad(false);
        }

        public override void _Process(float delta)
        {
            (int cx,int cy) = Sandbox.WorldToRegion(GetParent<Node2D>().Position);
            if(cx != old_cx || cy != old_cy) {
                UpdateLoad(true);
            }
        }
    }
}