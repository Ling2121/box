using Godot;
using System.Collections.Generic;

namespace Box {
    public class GrowSystem : Node {
        public class GrowItem {
            public IGrow Object;
            public long Timestamp;
            public long NextTimestamp;
        }

        public List<GrowItem> GrowItems = new List<GrowItem>();
        public TimeSystem TimeSystem;

        public override void _EnterTree()
        {
            Game.Instance.GrowSystem = this;
        }

        public override void _Ready()
        {
            TimeSystem = Game.Instance.TimeSystem;
            TimeSystem.Connect(nameof(TimeSystem.minute_step),this,nameof(_MinuteStep));
        }

        public bool UpdateNextTimestamp(GrowItem item) {
            IGrow grow = item.Object;

            int stage = grow.Stage;
            int next_stage = stage + 1;
            if(next_stage >= grow.Stages.Count) return false;
            long next_stage_t = grow.Stages[next_stage];
            item.NextTimestamp = item.Timestamp + next_stage_t;

            return true;
        }

        public void AddGrow(IGrow grow) {
            GrowItem item = new GrowItem();
            item.Object = grow;
            item.Timestamp = TimeSystem.Timestamp;
            
            if(UpdateNextTimestamp(item)) {
                GrowItems.Add(item);
            }
        }

        protected List<GrowItem> remove_items = new List<GrowItem>();
        public void _MinuteStep() {
            long ct = TimeSystem.Timestamp;
            for(int i = 0;i < GrowItems.Count;i++) {
                GrowItem item = GrowItems[i];
                if(ct >= item.NextTimestamp) {
                    item.Object.Stage++;
                    item.Object._EnterNextStage(item.Object.Stage);
                    if(!UpdateNextTimestamp(item)) {
                        remove_items.Add(item);
                    }
                }
            }

            if(remove_items.Count > 0) {
                foreach(var item in remove_items) {
                    GrowItems.Remove(item);
                }
            }
        }
    }
}