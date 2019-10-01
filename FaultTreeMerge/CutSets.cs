using System;
using System.Collections.Generic;
using System.Text;

namespace FaultTreeMerge
{
    class CutSets
    {
        public int Order { get; set; }
        public bool Pruned { get; set; }
        public string Content { get; set; }

        public CutSets()
        {
        }

      public  CutSets(int pOrder, bool pPruned, string pContent)
        {
            Order = pOrder;
            Pruned = pPruned;
            Content = pContent;
        }
    }
}
