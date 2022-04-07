using System.Collections.Generic;

namespace Box.ProcessTreeNodes {
    public class Root : ProcessTreeNode {
        public override Dictionary<string, ProcessTreeNodePortType> _Ports()
        {
            return new Dictionary<string, ProcessTreeNodePortType>{
                {"Root",ProcessTreeNodePortType.Output}
            };
        }

        public override Dictionary<string, int> _PortsLayer()
        {
            return new Dictionary<string, int> {
                {"Root",0},
            };
        }
    }
}