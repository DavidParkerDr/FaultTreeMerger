using System;
using System.Collections.Generic;
using System.Text;

namespace FaultTreeMerge
{
    class BasicEvent
    {
        static int sIdCount = 0;
        public string Name { get; set; }
        public string ShortName { get; set; }
        public string Description { get; set; }
        public int Id { get; set; }
        public string Unavailability { get; set; }

        public BasicEvent(string pName)
        {
            Name = pName;
            Id = sIdCount++;
        }

    }
}

