using System;
using System.Collections.Generic;
using System.Text;

namespace FaultTreeMerge
{
    class Effect
    {
        public static int EffectCount = 0;
        public int Id;
        public string Name;
        public string SinglePointFailure; //TODO: Should this be a string, as it does going to be written to a file?

        public int PreviousId;

        public Effect(string pName, string pSinglePointFailure)
        {
            Id = ++EffectCount;

            Name = pName;
            SinglePointFailure = pSinglePointFailure;
        }
    }
}
