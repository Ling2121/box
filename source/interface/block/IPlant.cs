using System.Collections.Generic;

namespace Box {
    public interface IPlant : IBlock {
        // 生长阶段定义表
        // {
        //     1000,//第一到第二阶段的时间
        //     2000,//第二到第三阶段的时间
        //     ...
        // }
        List<int> GrowthProcess {get;}
        //每进入一个新的生长阶段时都会执行
        void _Growth(int phase,SandboxLayer layer,int x,int y);
    }
}