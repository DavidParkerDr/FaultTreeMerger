using System;
using System.Collections.Generic;
using System.Text;

namespace FaultTreeMerge
{
    class BasicEvent : Node
    {
        static int sIdCount = 0;
        public string Name { get; set; }
        public string ShortName { get; set; }
        public string Description { get; set; }
        public int Id { get; set; }
        public string Unavailability { get; set; }
        public List<Effect> Effects { get; set; }

        public int PreviousId { get; set; }   //TODO: Is this necessary? Should it also have a link to the fault tree it was linked to?

        public BasicEvent()
        {
            //TODO: This should probably not increment the sIdCount and this method should only be used for duplicate events?
            // Or should this increment sIdCount, and check if the event exists outside of the constructor?
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

