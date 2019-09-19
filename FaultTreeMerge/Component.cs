using System;
using System.Collections.Generic;
using System.Text;

namespace FaultTreeMerge
{
    class Component
    {
        public List<BasicEvent> Events = new List<BasicEvent>();
        string Name { get; set; } 

        public Component(string pName)
        {
            Name = pName;
        }
    }
}
