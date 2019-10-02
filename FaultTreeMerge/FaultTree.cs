using System;
using System.Collections.Generic;
using System.Text;

namespace FaultTreeMerge
{
    class FaultTree
    {
        public static int FaultTreeCount = 0;
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string SIL { get; set; }
        public string Unavailability { get; set; }
        public string UnavailabilitySort { get; set; }
        public string Severity { get; set; }
        public OutputDeviation OutputDeviation;
        public int PreviousId { get; set; }

        public int HiPHOPSResultsIndex { get; set; }

        public List<CutSetsSummary> CutSetsSummary = new List<CutSetsSummary>();

        public List<CutSets> AllCutSets = new List<CutSets>();

        public FaultTree()
        {
        }

        public FaultTree(string pName)
        {
            Name = pName;

            Id = ++FaultTreeCount;
        }
    }
}
