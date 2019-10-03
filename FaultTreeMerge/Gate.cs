using System;
using System.Collections.Generic;
using System.Text;

namespace FaultTreeMerge
{
    class Gate : Node
    {
        public static int GateIdCount = 0;
        public int Id { get; set; }
        public string Name { get; set; }
        public List<Node> Children = new List<Node>();
    }
}
