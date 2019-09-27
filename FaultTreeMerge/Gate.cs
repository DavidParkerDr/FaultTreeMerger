using System;
using System.Collections.Generic;
using System.Text;

namespace FaultTreeMerge
{
    class Gate : Node
    {
        public static int GateIdCount = 0;
        public int Id { get; set; }
        public int PreviousId { get; set; }
        public string Name { get; set; }

        public List<Node> Children = new List<Node>();


        static int TotalChildrenCount(List<Node> children)
        {
            int totalChildrenCount = children.Count;




            return totalChildrenCount;
        }
    }
}
