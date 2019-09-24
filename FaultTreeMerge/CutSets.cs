using System;
using System.Collections.Generic;
using System.Text;

namespace FaultTreeMerge
{
    class CutSets
    {
        public int Order { get; set; }  //TODO: Should this be a string, as the program is writing it to a file anyway?
        public bool Pruned { get; set; } //TODO: Should this be a string, as the program is writing it to a file anyway?
        public string Content { get; set; } //TODO: Is this an appropriate name? It appears to hold a number e.g. 36 or 72

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
