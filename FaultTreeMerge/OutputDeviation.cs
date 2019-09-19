using System;
using System.Collections.Generic;
using System.Text;

namespace FaultTreeMerge
{
    class OutputDeviation
    {
        public List<Gate> Children = new List<Gate>();
        public string Name { get; set; }

        public OutputDeviation(string pName)
        {
            Name = pName;
        }
    }
}
