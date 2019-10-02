using System;
using System.Collections.Generic;
using System.Text;

namespace FaultTreeMerge
{
    class CutSetsSummary
    {
        public string Order { get; set; }
        public string Pruned { get; set; }
        public string Content { get; set; }

        public CutSetsSummary()
        {
        }

      public  CutSetsSummary(string pOrder, string pPruned, string pContent)
        {
            Order = pOrder;
            Pruned = pPruned;
            Content = pContent;
        }
    }
}
