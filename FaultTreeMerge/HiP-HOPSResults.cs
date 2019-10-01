using System;
using System.Collections.Generic;
using System.Text;

namespace FaultTreeMerge
{
    class HiP_HOPSResults
    {
        public string Model { get; set; }
        public string Build { get; set; }
        public string MajorVersion { get; set; }
        public string MinorVersion { get; set; }
        public string Version { get; set; }
        public string VersionDate { get; set; }

        public FMEA FMEA { get; set; }
        public List<FaultTree> FaultTrees = new List<FaultTree>();

        public Dictionary<int, BasicEvent> BasicEventsDictionary = new Dictionary<int, BasicEvent>();

        public HiP_HOPSResults()
        {
        }
    }
}
