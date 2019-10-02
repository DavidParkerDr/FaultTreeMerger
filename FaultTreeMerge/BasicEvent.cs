using System;
using System.Collections.Generic;
using System.Text;

namespace FaultTreeMerge
{
    class BasicEvent : Node
    {
        static int sIdCount = 0;
        public static int IdCount { get { return sIdCount; } set { sIdCount = value; } }
        public string Name { get; set; }
        public string ShortName { get; set; }
        public string Description { get; set; }
        public int Id { get; set; }
        public string Unavailability { get; set; }
        public List<Effect> Effects = new List<Effect>();

        public int PreviousId { get; set; }
        public int HiPHOPSResultsIndex;


        public BasicEvent()
        {
        }

        public BasicEvent(string pName)
        {
            Name = pName;
            Id = ++sIdCount;
        }

        public BasicEvent(string pName, string pShortName, string pDescription, string pUnavailability, List<Effect> pEffects)
        {
            Name = pName;
            ShortName = pShortName;
            Description = pDescription;
            Unavailability = pUnavailability;
            Effects = pEffects;

            Id = ++sIdCount;
        }
    }
}

