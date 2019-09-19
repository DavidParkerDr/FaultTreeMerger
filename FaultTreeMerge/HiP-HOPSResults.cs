﻿using System;
using System.Collections.Generic;
using System.Text;

namespace FaultTreeMerge
{
    class HiP_HOPSResults
    {
        public string Model { get; set; }
        public string Build { get; set; }
        public string MajorVersion { get; set; }
        public string MinorVersion { get; set; }
        public string Version { get; set; }
        public string VersionDate { get; set; }

        public List<Component> FMEA = new List<Component>();
        public List<FaultTree> FaultTrees = new List<FaultTree>();

        HiP_HOPSResults()  //TODO: Find out which of the properties above will need to be added to the constructor
        {
            
        }
    }
}