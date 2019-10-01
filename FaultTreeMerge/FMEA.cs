using System;
using System.Collections.Generic;
using System.Text;

namespace FaultTreeMerge
{
    class FMEA
    {
        public List<Component> Components = new List<Component>();

        public FMEA(List<Component> pComponents)
        {
            Components = pComponents;
        }

        public FMEA()
        {
        }

    }
}
