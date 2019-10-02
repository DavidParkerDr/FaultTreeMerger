using System;
using System.Collections.Generic;
using System.Text;

namespace FaultTreeMerge
{
    class CutSet
    {
        public string Unavailability { get; set; }
        public string UnavailabilitySort { get; set; }
        public List<BasicEvent> Events = new List<BasicEvent>();

        public CutSet()
        {
        }
    }
}
