using System;
using System.Collections.Generic;
using System.Text;

namespace FaultTreeMerge
{
    class OutputDeviation
    {
        public List<Node> Children = new List<Node>();
        public string Name { get; set; }

        public OutputDeviation(string pName)
        {
            Name = pName;
        }

        public string TotalChildrenChecksum()
        {
            return TotalChildrenChecksum(Children);
        }

        public static string TotalChildrenChecksum(List<Node> children)
        {
            //This string will contain a character for each node present in the output deviation
            // 'E' represents basic events. 'O' represents and Or gate. 'A' represents an And gate. The number after an O or A represents the number of characters in the gate's name.
            string totalChildrenCheck = "";

            foreach (Node child in children)
            {
                if (child is Gate)
                {
                    Gate gate = (Gate)child;

                    if (child is Or)
                    {
                        totalChildrenCheck += "O";
                    }
                    else if (child is And)
                    {
                        totalChildrenCheck += "A";
                    }

                    if (!string.IsNullOrEmpty(gate.Name))
                    {
                        totalChildrenCheck += gate.Name.Length.ToString();
                    }

                    totalChildrenCheck += TotalChildrenChecksum(gate.Children);
                }
                else
                {
                    totalChildrenCheck += "E";
                }
            }

            return totalChildrenCheck;
        }
    }
}
