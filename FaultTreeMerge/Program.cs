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

        static List<Effect> EffectsList = new List<Effect>();
        static List<BasicEvent> BasicEventsList = new List<BasicEvent>();

        //       static Dictionary<int, string> Effect

        static Dictionary<int, int> effectIdReplacements; //First int is new id, second is old id

        static void Main(string[] args)
        {
            sBasicEventsLookup = new Dictionary<string, Dictionary<string, int>>();
            sBasicEvents = new Dictionary<string, BasicEvent>();
            outputFile = new XmlDocument();

            // LoadFileAsString("C:\\Users\\350809.ADIR\\Downloads\\andre files\\Variants-Automotive-Braking-System-Case-Study\\CutSets(10).xml");

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



            foreach (KeyValuePair<int, int> kvp in replacements)
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

            //TODO: Should this be a static dictionary outside of the method?
        //    Dictionary<int, int> effectIdSwap = new Dictionary<int, int>();  //First int old id, second int new id
            Dictionary<int, int> basicEventIdSwap = new Dictionary<int, int>();  //First int old id, second int new id

            XmlDocument doc = new XmlDocument();
            doc.Load(pPath + "/faulttrees.xml");

            XmlNodeList effects = doc.SelectNodes("HiP-HOPS_Results/FaultTrees/FMEA/Component/Events/BasicEvent/Effects/Effect");
            foreach (XmlNode effect in effects)
            {
                XmlAttribute id = effect.Attributes[0];
                int idValue = int.Parse(id.FirstChild.Value);
                XmlNode name = effect.SelectSingleNode("Name");
                string nameValue = name.FirstChild.Value;
                XmlNode singlePointFailure = effect.SelectSingleNode("SinglePointFailure");
                string singlePointFailureValue = singlePointFailure.FirstChild.Value;

                //TODO: This needs to do something with the old id (idValue) so it can compare it with the new value generated in the constructor
                //This takes every unqiue effect from the xml files and adds it to the EffectsList list of effects
                Effect newEffect = new Effect(nameValue, singlePointFailureValue);
                EffectsList.Add(newEffect);

                //This keeps the old and new IDs together in a list so it can be looked up
              //  effectIdSwap.Add(newEffect.Id, idValue);



                /*
                if (EffectsList.Count < 1)
                {
                    EffectsList.Add(new Effect(nameValue, singlePointFailureValue));
                }
                else
                {
                    for (int i = 0; i < EffectsList.Count; i++)
                    {

                    if (EffectsList[i].Name != nameValue && EffectsList[i].SinglePointFailure != singlePointFailureValue)
                    {
                        Effect newEffect = new Effect(nameValue, singlePointFailureValue);
                        EffectsList.Add(newEffect);
                        effectIdSwap.Add(idValue, newEffect.Id);

                        i = EffectsList.Count;
                    }
                    else
                    {
                        //TODO: Remove. For Debugging purposes.
                        Console.WriteLine("Duplicate Effect: " + idValue.ToString());
                    }
                  }
                }
                */
            }

            XmlNodeList basicEvents = doc.SelectNodes("HiP-HOPS_Results/FaultTrees/FMEA/Component/Events/BasicEvent");
            foreach (XmlNode basicEvent in basicEvents)
            {
                XmlAttribute id = basicEvent.Attributes[0];
                int idValue = int.Parse(id.FirstChild.Value);

                if (!basicEventIdSwap.ContainsValue(idValue))
                {


                    XmlNode name = basicEvent.SelectSingleNode("Name");
                    string nameValue = name.FirstChild.Value;

                    XmlNode shortName = basicEvent.SelectSingleNode("ShortName");
                    string shortNameValue = shortName.FirstChild.Value;
                    XmlNode description = basicEvent.SelectSingleNode("Description");
                    string descriptionValue = "";
                    if (description.HasChildNodes)   //TODO: I should probably do this for each property
                    {
                        descriptionValue = description.FirstChild.Value;
                    }
                    XmlNode unavailability = basicEvent.SelectSingleNode("Unavailability");
                    string unavailabilityValue = unavailability.FirstChild.Value;

                    //TODO: get all effects from the file and add them to a dictionary(?) like with basic events

                    XmlNode eventEffects = basicEvent.SelectSingleNode("Effects");

                    List<Effect> effectsValues = new List<Effect>(); //TODO: Should this be a list of effects?  Or Should this look of the list of effects and add that event when using the basic event constructor?

                    for (int i = 0; i < eventEffects.ChildNodes.Count; i++)
                    {
                        XmlAttribute effectId = eventEffects.ChildNodes[i].Attributes[0];
                        int effectIdValue = int.Parse(effectId.FirstChild.Value);

                        XmlNode effectName = eventEffects.ChildNodes[i].SelectSingleNode("Name");
                        string effectNameValue = effectName.FirstChild.Value;
                        XmlNode effectSinglePointFailure = eventEffects.ChildNodes[i].SelectSingleNode("SinglePointFailure");
                        string effectSinglePointFailureeValue = effectSinglePointFailure.FirstChild.Value;
                        

                        for (int j = 0; j < EffectsList.Count; j++)
                        {
                            if (EffectsList[j].Name == effectNameValue && EffectsList[j].SinglePointFailure == effectSinglePointFailureeValue)
                            {
                                effectsValues.Add(EffectsList[j]);

                                //This should endure that no duplicate effect IDs are written
                                //EffectsList should be empty when all BasicEvents have been read in
                                EffectsList.RemoveAt(j);

                                j = EffectsList.Count;
                            }
                        }
                    }

                    BasicEvent newBasicEvent = new BasicEvent(nameValue, shortNameValue, descriptionValue, unavailabilityValue, effectsValues);

                    BasicEventsList.Add(newBasicEvent);
                    basicEventIdSwap.Add(newBasicEvent.Id, idValue);

                }
            }

            //TODO: The program should next read in either every And gate, Or gate 
            //TODO: Find out if the gates can share IDs


            //TODO: Check what should go here.
            PrintIdSwap(idSwap);
            TraverseFile(pPath, idSwap);
        }

        static void LoadPath2(string pPath)
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
            //    outputFile.Save("testOutput.xml");   //TODO: outputFile is not a valid xml document here
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
            foreach (KeyValuePair<string, BasicEvent> kvp in sBasicEvents)
            {
                Console.WriteLine(kvp.Value.Id + " " + kvp.Value.Name);
            }
        }
    }
}
