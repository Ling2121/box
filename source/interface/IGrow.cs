using System.Collections.Generic;

namespace Box {
    public interface IGrow {
        int Stage {get;set;}
        //每个阶段的时间间隔 分钟为单位 从第一个阶段开始
        List<long> StageSetting {get;set;} /* 
            {
                xxxx//第一阶段到第二阶段的时间(分钟)
                xxxx//第二阶段到第三阶段的时间(分钟)
                ...
            }
        */
        void _EnterNextStage(int stage);
    }
}