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
        public string SIL { get; set; }
        public string Unavailability { get; set; }
        public string UnavilabilitySort { get; set; }
        public string Severity { get; set; }
        public OutputDeviation OutputDeviation;

        public List<CutSets> CutSetsSummary = new List<CutSets>();
                
        public FaultTree(string pName, string pSIL, string pUnavailability, string pUnavailabilitySort, string pSeverity) //TODO: does this need this many parameters? 
        {
            Name = pName;
            SIL = pSIL;
            Unavailability = pUnavailability;
            UnavilabilitySort = pUnavailabilitySort;
            Severity = pSeverity;

            Id = FaultTreeCount++;  //TODO: Should the ++ be before FaultTreeCount? Should the Id start at 0 or 1? Example tree starts at 1
        }

        public FaultTree(string pName)  //TODO: Should this be the only constructor?
        {
            Name = pName;

            Id = FaultTreeCount++;
        }
    }
}
