using System;
using System.IO;
using System.Xml;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace FaultTreeMerge
{
    class Program
    {
        static Dictionary<string, Dictionary<string, int>> sBasicEventsLookup;
        static Dictionary<string, BasicEvent> sBasicEvents;
        static XmlDocument outputFile;
        static int sIDCount = 0;

        static List<Dictionary<int, BasicEvent>> BasicEventsDictionaryList = new List<Dictionary<int, BasicEvent>>();
        static List<HiP_HOPSResults> HiP_HOPSResultsList = new List<HiP_HOPSResults>();
        static HiP_HOPSResults CombinedHiP_HOPSResults = new HiP_HOPSResults();

        //TODO:  These are probably unecessary now
        static List<Effect> EffectsList = new List<Effect>();
        static List<BasicEvent> BasicEventsList = new List<BasicEvent>();
        static Dictionary<int, int> effectIdReplacements; //First int is new id, second is old id
        static List<BasicEvent> BasicEventsList2 = new List<BasicEvent>();  //TODO: This list is used in LoadPaths3. Remove this if no code from that method is used, rename otherwise.



        static void Main(string[] args)
        {
            sBasicEventsLookup = new Dictionary<string, Dictionary<string, int>>();
            sBasicEvents = new Dictionary<string, BasicEvent>();
            outputFile = new XmlDocument();

            // LoadFileAsString("C:\\Users\\350809.ADIR\\Downloads\\andre files\\Variants-Automotive-Braking-System-Case-Study\\CutSets(10).xml");

            string mergePathsFile = args[0];
            Console.WriteLine("Loading file of fault tree paths to merge: " + mergePathsFile);
            LoadPaths(mergePathsFile);
            BasicEvent.IdCount = 0;   //TODO: Make sure this should reset the basic event count
            Effect.EffectCount = 0;   //TODO: Make sure this should reset the effect count
            CombineHiP_HOPSResults();

            //  PrintBasicEvents();  //TODO: This is probably unnecessary
        }

        static void CombineHiP_HOPSResults()
        {
            //TODO: Find out what attributes the combined HiP-HOPS Results should have and set them here


            CombineFMEA();

        }

        static void CombineFMEA()
        {
            FMEA combinedFMEA = new FMEA();
            List<Component> components = new List<Component>();

            foreach (HiP_HOPSResults HiPHOPSResults in HiP_HOPSResultsList)
            {
                foreach (Component component in HiPHOPSResults.FMEA.Components)
                {
                    components.Add(component);

                    //TODO: The if part seems redunant, as the components will never match, currently
                    if (!combinedFMEA.Components.Contains(component))
                    {
                        //TODO: For debug purposes, remove when finished
                        foreach (Component storedComponent in combinedFMEA.Components)
                        {
                            if (component.Name == storedComponent.Name)
                            {
                                Console.WriteLine("Duplicate Component Name Found: " + component.Name);
                            }
                        }

                        // combinedFMEA.Components.Add(component);
                    }
                }
            }
            combinedFMEA.Components = CombineComponents(components);

            CombinedHiP_HOPSResults.FMEA = combinedFMEA;
        }

        static List<Component> CombineComponents(List<Component> components)
        {
            List<Component> combinedComponents = new List<Component>();

            //This will store each different version of a component from each XML file
            Dictionary<string, List<Component>> componentVersionsDictionary = new Dictionary<string, List<Component>>();

            //TODO: Probably should change this to a for loop and stop removing from components
            while (components.Count > 0)
            {
                //TODO: probably move tis outside of the while loop, like in the CombineEvents method
                if (componentVersionsDictionary.Count < 1)
                {
                    List<Component> newComponentList = new List<Component>();
                    newComponentList.Add(components[0]);
                    componentVersionsDictionary.Add(components[0].Name, newComponentList);

                    components.RemoveAt(0);
                }
                else
                {
                    bool componentAdded = false;

                    for (int i = 0; i < componentVersionsDictionary.Count; i++)
                    {

                        if (componentVersionsDictionary.ContainsKey(components[0].Name))
                        {
                            componentVersionsDictionary[components[0].Name].Add(components[0]);

                            components.RemoveAt(0);
                            componentAdded = true;

                            //TODO: Are there alternative ways of implementing this?
                            i += componentVersionsDictionary.Count;
                        }
                    }

                    if (!componentAdded)
                    {
                        List<Component> newComponentList = new List<Component>();
                        newComponentList.Add(components[0]);
                        componentVersionsDictionary.Add(components[0].Name, newComponentList);

                        components.RemoveAt(0);
                    }
                }
            }

            foreach(KeyValuePair<string, List<Component>> componentsKVP in componentVersionsDictionary)
            {
                Component newComponent = new Component(componentsKVP.Key);
                List<BasicEvent> events = new List<BasicEvent>();

                foreach(Component component in componentsKVP.Value)
                {
                    foreach(BasicEvent basicEvent in component.Events)
                    {
                        events.Add(basicEvent);
                    }                    
                }

                newComponent.Events = CombineEvents(events);
            }


            return combinedComponents;

        }

        static List<BasicEvent> CombineEvents(List<BasicEvent> events)
        {
            List<BasicEvent> combinedEvents = new List<BasicEvent>();

            /*
            combinedEvents.Add(events[0]);

            //Starts at i = 1 as the first event is added above
            for(int i = 1; i < events.Count; i++)
            {
                for(int j = 0; j < combinedEvents.Count; j++)
                {
                    if(events[i].Name == combinedEvents[j].Name)
                    {


                        j += combinedEvents.Count;
                    }

                }
            }
            */

            //This will create a new event, with IDs starting at 0 as it was reset after the xml files were loaded
            combinedEvents.Add(new BasicEvent(events[0].Name));
            combinedEvents[0].ShortName = events[0].ShortName;
            combinedEvents[0].Description = events[0].Description;
            combinedEvents[0].Unavailability = events[0].Unavailability;


            events.RemoveAt(0);

            while (events.Count > 0)
            {
                for(int i = 0; i < combinedEvents.Count; i++)
                {
                    bool eventAdded = false;

                    if(events[0].Name == combinedEvents[i].Name)
                    {
                        foreach(Effect newEffect in events[0].Effects)
                        {
                            //TODO: Check the Name and the SinglePointValue of each effect in the effects list of the current event (events[0]). Then create a new effect (if not already in), so the id starts at 1.
                        }

                        eventAdded = true;
                    }

                    if(!eventAdded)
                    {
                        combinedEvents.Add(new BasicEvent(events[0].Name));
                        events.RemoveAt(0);
                    }
                }
            }


            return combinedEvents;
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

            XmlReaderSettings settings = new XmlReaderSettings();
            settings.Async = false;
            settings.IgnoreWhitespace = true;

            BasicEventsDictionaryList.Add(new Dictionary<int, BasicEvent>());

            using (XmlReader reader = XmlReader.Create(new StreamReader(pPath + "/faulttrees.xml"), settings))
            {
                reader.ReadToFollowing("HiP-HOPS_Results");
                HiP_HOPSResults newHiP_HOPSResults = ReadHiPHOPSResults(reader);
                HiP_HOPSResultsList.Add(newHiP_HOPSResults);
            }
        }

        static HiP_HOPSResults ReadHiPHOPSResults(XmlReader reader)
        {
            HiP_HOPSResults HiP_HOPSResults = new HiP_HOPSResults();

            HiP_HOPSResults.Model = reader.GetAttribute("model");
            HiP_HOPSResults.Build = reader.GetAttribute("build");
            HiP_HOPSResults.MajorVersion = reader.GetAttribute("majorVersion");
            HiP_HOPSResults.MinorVersion = reader.GetAttribute("minorVersion");
            HiP_HOPSResults.Version = reader.GetAttribute("version");
            HiP_HOPSResults.VersionDate = reader.GetAttribute("versionDate");

            reader.ReadStartElement();

            if (reader.Name == "FaultTrees")
            {
                reader.ReadStartElement();

                if (reader.Name == "FMEA")
                {
                    HiP_HOPSResults.FMEA = ReadFMEA(reader);
                }

                while (reader.Name == "FaultTree")
                {
                    HiP_HOPSResults.FaultTrees.Add(ReadFaultTree(reader));
                }

                reader.Read();
            }

            //TODO: Make sure the WarningList and SafetyAllocations can be ignored

            return HiP_HOPSResults;
        }

        static FaultTree ReadFaultTree(XmlReader reader)
        {
            int previousId = int.Parse(reader.GetAttribute("ID"));
            string name = "";

            reader.ReadStartElement();

            if (reader.Name == "Name")
            {
                name = reader.ReadElementContentAsString();
            }
            FaultTree faultTree = new FaultTree(name);
            faultTree.PreviousId = previousId;

            if (reader.Name == "Description")
            {
                faultTree.Description = reader.ReadElementContentAsString();
            }
            if (reader.Name == "SIL")
            {
                faultTree.SIL = reader.ReadElementContentAsString();
            }
            if (reader.Name == "Unavailability")
            {
                faultTree.Unavailability = reader.ReadElementContentAsString();
            }
            if (reader.Name == "UnavailabilitySort")
            {
                faultTree.UnavailabilitySort = reader.ReadElementContentAsString();
            }
            if (reader.Name == "Severity")
            {
                faultTree.Severity = reader.ReadElementContentAsString();
            }

            if (reader.Name == "OutputDeviation")
            {
                faultTree.OutputDeviation = ReadOutputDeviation(reader);
            }

            if (reader.Name == "CutSetsSummary")
            {
                reader.ReadStartElement();

                while (reader.Name == "CutSets")
                {
                    faultTree.CutSetsSummary.Add(ReadCutSets(reader));
                }
                reader.Read();
            }

            reader.Read();
            return faultTree;
        }

        static CutSets ReadCutSets(XmlReader reader)
        {
            CutSets cutSets = new CutSets();

            cutSets.Order = int.Parse(reader.GetAttribute("order"));
            cutSets.Pruned = bool.Parse(reader.GetAttribute("pruned"));

            cutSets.Content = reader.ReadElementContentAsString();   //TODO: Make sure this is the correct method

            //    reader.ReadStartElement();


            //   reader.Read();
            return cutSets;
        }

        static OutputDeviation ReadOutputDeviation(XmlReader reader)
        {
            reader.ReadStartElement();

            string name = "";
            if (reader.Name == "Name")
            {
                name = reader.ReadElementContentAsString();
            }

            OutputDeviation outputDeviation = new OutputDeviation(name);


            if (reader.Name == "Children")
            {
                outputDeviation.Children = ReadChildren(reader);
            }


            reader.Read();
            return outputDeviation;
        }

        static List<Node> ReadChildren(XmlReader reader)
        {
            reader.ReadStartElement();

            List<Node> nodes = new List<Node>();

            while (reader.Name == "Or" || reader.Name == "And" || reader.Name == "Event")
            {

                if (reader.Name == "Or")
                {
                    nodes.Add(ReadOr(reader));
                }
                else if (reader.Name == "And")
                {
                    nodes.Add(ReadAnd(reader));
                }
                else if (reader.Name == "Event")
                {
                    //TODO: find out what needs to go here
                    // The name for this part is 'Event', not 'BasicEvent', so ReadEvent will probably not work as it is.
                    // Change ReadEvent to read events titles 'Event'?   maybe reader.Name.Countains("Event")?
                    // Maybe none if this is relevent and maybe it will just work

                    nodes.Add(ReadEvent(reader));
                }

            }
            reader.Read();
            return nodes;
        }

        static Or ReadOr(XmlReader reader)
        {
            int previousId = int.Parse(reader.GetAttribute("ID"));

            reader.ReadStartElement();

            Or or = new Or();
            or.PreviousId = previousId;

            if (reader.Name == "Name")
            {
                or.Name = reader.ReadElementContentAsString();
            }
            if (reader.Name == "Children")
            {
                or.Children = ReadChildren(reader);
            }


            reader.Read();
            return or;
        }

        static And ReadAnd(XmlReader reader)
        {
            int previousId = int.Parse(reader.GetAttribute("ID"));

            reader.ReadStartElement();

            And and = new And();
            and.PreviousId = previousId;

            if (reader.Name == "Name")
            {
                and.Name = reader.ReadElementContentAsString();
            }
            if (reader.Name == "Children")
            {
                and.Children = ReadChildren(reader);
            }

            reader.Read();
            return and;
        }

        static FMEA ReadFMEA(XmlReader reader)
        {
            //   reader.ReadStartElement();

            List<Component> components = new List<Component>();

            reader.ReadStartElement();

            while (reader.Name == "Component")
            {
                components.Add(ReadComponent(reader));
            }

            FMEA newFMEA = new FMEA(components);

            reader.Read();
            return newFMEA;
        }

        static Component ReadComponent(XmlReader reader)
        {
            string name = "";
            List<BasicEvent> basicEvents = new List<BasicEvent>();

            reader.ReadStartElement();

            if (reader.Name == "Name")
            {
                name = reader.ReadElementContentAsString();
            }
            Component component = new Component(name);

            //  reader.Read();

            if (reader.Name == "Events")
            {
                reader.ReadStartElement();

                while (reader.Name == "BasicEvent")
                {
                    basicEvents.Add(ReadEvent(reader));
                }
                reader.Read();
                component.Events = basicEvents;
            }

            reader.Read();
            return component;
        }

        static BasicEvent ReadEvent(XmlReader reader)
        {
            string name = "";
            string shortName = "";
            string description = "";
            string unavailability = "";
            List<Effect> effects = new List<Effect>();

            int previousId = int.Parse(reader.GetAttribute("ID"));
            BasicEvent basicEvent = new BasicEvent();
            //    basicEvent.PreviousId = previousId;

            if (!BasicEventsDictionaryList[HiP_HOPSResultsList.Count].ContainsKey(previousId))
            {

                reader.ReadStartElement();

                if (reader.Name == "Name")
                {
                    name = reader.ReadElementContentAsString();
                }

                // This constructor increments the ID count for BasicEvents by one
                //TODO: This is probably not the best way of doing this. It may just be better to increment the ID count here instead.
                basicEvent = new BasicEvent(name);
                //  basicEvent.PreviousId = previousId;

                if (reader.Name == "ShortName")
                {
                    shortName = reader.ReadElementContentAsString();   //TODO: This can probably be simplified
                    basicEvent.ShortName = shortName;
                }
                if (reader.Name == "Description")
                {
                    description = reader.ReadElementContentAsString();
                    basicEvent.Description = description;
                }
                if (reader.Name == "Unavailability")
                {
                    unavailability = reader.ReadElementContentAsString();
                    basicEvent.Unavailability = unavailability;
                }

                if (reader.Name == "Effects")
                {
                    reader.ReadStartElement();

                    while (reader.Name == "Effect")
                    {
                        effects.Add(ReadEffect(reader));
                    }

                    basicEvent.Effects = effects;

                    reader.Read();
                }


                BasicEventsDictionaryList[HiP_HOPSResultsList.Count].Add(previousId, basicEvent);
            }
            else
            {
                //TODO: Should this return the full event, or an event containing only PreviousId, to indicate it already exists? Probably the former, as the event should probably be looked-up anyway.
                reader.Read();
                return BasicEventsDictionaryList[HiP_HOPSResultsList.Count][previousId];
            }

            reader.Read();
            return basicEvent;
        }

        static Effect ReadEffect(XmlReader reader)
        {

            int previousId = int.Parse(reader.GetAttribute("ID"));
            string name = "";
            string singlePointFailure = "";

            reader.ReadStartElement();

            if (reader.Name == "Name")
            {
                name = reader.ReadElementContentAsString();
            }
            if (reader.Name == "SinglePointFailure")
            {
                singlePointFailure = reader.ReadElementContentAsString();
            }

            Effect effect = new Effect(name, singlePointFailure);
            //  effect.PreviousId = previousId;

            reader.Read();

            return effect;
        }


        static void LoadPath3(string pPath)
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

                    BasicEventsList2.Add(newBasicEvent);
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
