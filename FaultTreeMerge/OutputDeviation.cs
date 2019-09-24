using System;
using System.Collections.Generic;
using System.Text;

namespace FaultTreeMerge
{
    class OutputDeviation
    {
        public List<Node> Children = new List<Node>();
        public string Name { get; set; }

        public OutputDeviation(string pName)
        {
            Name = pName;
        }
    }
}
