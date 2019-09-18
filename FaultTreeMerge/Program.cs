using System;
using System.IO;
using System.Xml;
using System.Collections.Generic;

namespace FaultTreeMerge
{
    class Program
    {
        static Dictionary<string, Dictionary<string, int>> sBasicEventsLookup;
        static Dictionary<string, BasicEvent> sBasicEvents;
        static XmlDocument outputFile;
        static int sIDCount = 0;
        static void Main(string[] args)
        {
            sBasicEventsLookup = new Dictionary<string, Dictionary<string, int>>();
            sBasicEvents = new Dictionary<string, BasicEvent>();
            outputFile = new XmlDocument();

            LoadFileAsString("C:\\Users\\350809.ADIR\\Downloads\\andre files\\Variants-Automotive-Braking-System-Case-Study\\CutSets(10).xml");

            string mergePathsFile = args[0];
            Console.WriteLine("Loading file of fault tree paths to merge: " + mergePathsFile);
            LoadPaths(mergePathsFile);
            PrintBasicEvents(); 
        }

        static void LoadFileAsString(string pFilePath)
        {
            FileStream fileStream = new FileStream(pFilePath, FileMode.Open);
            using (StreamReader reader = new StreamReader(fileStream))
            {
                string path = reader.ReadToEnd();
                string output = ReplaceEventsInString(path);
            }
        }

        static string ReplaceEventsInString(string pSource)
        {
            Dictionary<int, int> replacements = new Dictionary<int, int>();
            replacements.Add(1, 1);
            replacements.Add(2, 2);
            replacements.Add(21, 27);
            replacements.Add(22, 28);
            replacements.Add(35, 29);
            replacements.Add(36, 30);
            replacements.Add(53, 31);
            replacements.Add(54, 32);
            replacements.Add(77, 33);
            replacements.Add(78, 34);
            replacements.Add(101, 35);
            replacements.Add(102, 36);
            replacements.Add(103, 37);
            replacements.Add(104, 38);
            replacements.Add(172, 39);
            replacements.Add(173, 40);
            replacements.Add(186, 41);
            replacements.Add(187, 42);
            replacements.Add(204, 43);
            replacements.Add(205, 44);
            replacements.Add(228, 45);
            replacements.Add(229, 46);
            replacements.Add(252, 47);
            replacements.Add(253, 48);
            replacements.Add(254, 49);
            replacements.Add(255, 50);
            replacements.Add(611, 51);
            replacements.Add(635, 52);
            replacements.Add(659, 53);
            replacements.Add(660, 54);
            replacements.Add(661, 55);
            replacements.Add(662, 56);
            replacements.Add(683, 57);
            replacements.Add(684, 58);
            replacements.Add(689, 59);
            replacements.Add(690, 60);
            replacements.Add(309, 51);
            replacements.Add(333, 52);
            replacements.Add(357, 53);
            replacements.Add(358, 54);
            replacements.Add(359, 55);
            replacements.Add(360, 56);
            replacements.Add(381, 57);
            replacements.Add(382, 58);
            replacements.Add(387, 59);
            replacements.Add(388, 60);



            foreach (KeyValuePair<int,int> kvp in replacements)
            {
                string replacementLookup = String.Format("<Event ID=\"{0}\" />", kvp.Key);
                string replacement = String.Format("<Event ID=\"{0}\" />", kvp.Value);
                pSource = pSource.Replace(replacementLookup, replacement);
            }

            return pSource;

        }




        static void LoadPaths(string pMergePathsFileName)
        {
            FileStream fileStream = new FileStream(pMergePathsFileName, FileMode.Open);
            using (StreamReader reader = new StreamReader(fileStream))
            {
                while (!reader.EndOfStream)
                {
                    string path = reader.ReadLine();
                    LoadPath(path);
                    
                }
            }
        }
        static void LoadPath(string pPath)
        {
            Console.WriteLine("Loading path: " + pPath);
            Dictionary<int, int> idSwap = new Dictionary<int, int>();
            XmlDocument doc = new XmlDocument();
            doc.Load(pPath + "/faulttrees.xml");
            XmlNodeList basicEvents = doc.SelectNodes("HiP-HOPS_Results/FaultTrees/FMEA/Component/Events/BasicEvent");
            foreach (XmlNode basicEvent in basicEvents)
            {
                XmlAttribute id = basicEvent.Attributes[0];
                int idValue = int.Parse(id.FirstChild.Value);
                XmlNode name = basicEvent.SelectSingleNode("Name");
                string nameValue = name.FirstChild.Value;

                BasicEvent existingEvent;
                if (!sBasicEvents.TryGetValue(nameValue, out existingEvent))
                {
                    BasicEvent newBasicEvent = new BasicEvent(nameValue);
                    sBasicEvents.Add(nameValue, newBasicEvent);
                    idSwap.Add(idValue, newBasicEvent.Id);
                }
                else
                {
                    idSwap.Add(idValue, existingEvent.Id);
                }                
            }
            PrintIdSwap(idSwap);
            TraverseFile(pPath, idSwap);
        }
        static void TraverseFile(string pPath, Dictionary<int, int> pIdSwap)
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(pPath + "/faulttrees.xml");
            XmlNode node = doc.FirstChild.NextSibling;            
            TraverseHiPHOPS_Results(node, pIdSwap);
            outputFile.Save("testOutput.xml");
        }
        static void TraverseHiPHOPS_Results(XmlNode pHHRNode, Dictionary<int, int> pIdSwap)
        {
            XmlNode faultTreesNode = pHHRNode.FirstChild;
            XmlNode HHRNode = outputFile.ImportNode(pHHRNode, false);
            TraverseFaultTreesNode(faultTreesNode, pIdSwap);
        }
        static void TraverseFaultTreesNode(XmlNode pFaultTreesNode, Dictionary<int, int> pIdSwap)
        {
            XmlNode fmeaNode = pFaultTreesNode.FirstChild;
            TraverseFMEANode(fmeaNode, pIdSwap);
        }
        static void TraverseFMEANode(XmlNode pFMEANode, Dictionary<int, int> pIdSwap)
        {
            XmlNodeList components = pFMEANode.ChildNodes;
            foreach (XmlNode componentNode in components)
            {
                TraverseComponentNode(componentNode, pIdSwap);
            }
        }
        static void TraverseComponentNode(XmlNode pComponentNode, Dictionary<int, int> pIdSwap)
        {
            XmlNode nameNode = pComponentNode.FirstChild;
            XmlNode eventsNode = nameNode.NextSibling;
            TraverseEventsNode(eventsNode, pIdSwap);
        }
        static void TraverseEventsNode(XmlNode pEventsNode, Dictionary<int, int> pIdSwap)
        {
            XmlNodeList events = pEventsNode.ChildNodes;
            foreach (XmlNode basicEventNode in events)
            {
                TraverseBasicEventNode(basicEventNode, pIdSwap);
            }
        }
        static void TraverseBasicEventNode(XmlNode pBasicEventNode, Dictionary<int, int> pIdSwap)
        {
            XmlAttribute id = pBasicEventNode.Attributes["ID"];
            int idValue = int.Parse(id.FirstChild.Value);
            XmlElement basicEvent = outputFile.CreateElement("BasicEvent");
            XmlAttribute basicEventId = outputFile.CreateAttribute("ID");
            XmlText basicEventIdValue = outputFile.CreateTextNode(pIdSwap[idValue].ToString());
            basicEventId.AppendChild(basicEventIdValue);
            basicEvent.Attributes.Append(basicEventId);
            XmlNode nameNode = outputFile.ImportNode(pBasicEventNode.FirstChild, true);
            basicEvent.AppendChild(nameNode);
        }
        static void PrintIdSwap(Dictionary<int, int> pIdSwap)
        {
            foreach (KeyValuePair<int, int> kvp in pIdSwap)
            {
                Console.WriteLine(kvp.Key + " " + kvp.Value);
            }
        }
        static void PrintBasicEvents()
        {
            foreach(KeyValuePair <string, BasicEvent> kvp in sBasicEvents)
            {
                Console.WriteLine(kvp.Value.Id + " " + kvp.Value.Name);
            }
        }
    }
}
