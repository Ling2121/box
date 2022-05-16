using System.Collections.Generic;
namespace Box {
    public interface IGrow {
        public int Stage {get;set;}
        //每个阶段的时间间隔 分钟为单位 从第一个阶段开始
        public Dictionary<int,long> StageSetting {get;set;} /* 
            {
                0,xxxx//第一阶段到第二阶段的时间
                1,xxxx//第二阶段到第三阶段的时间
                ...
            }
        */
        public void _EnterNextStage(int stage);
    }
}