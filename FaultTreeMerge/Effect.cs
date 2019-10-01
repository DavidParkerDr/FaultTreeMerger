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
        public string SinglePointFailure;

        public int PreviousId;

        public int HiPHOPSResultsIndex;

        public Effect()
        {

        }

        public Effect(string pName, string pSinglePointFailure)
        {
            Name = pName;
            SinglePointFailure = pSinglePointFailure;
        }
    }
}
