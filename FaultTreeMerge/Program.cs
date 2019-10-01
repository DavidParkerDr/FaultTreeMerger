using System;
using System.IO;
using System.Xml;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace FaultTreeMerge
{
    class Program
    {
        static XmlDocument outputFile;

        static List<Dictionary<int, BasicEvent>> BasicEventsDictionaryList = new List<Dictionary<int, BasicEvent>>();
        static List<HiP_HOPSResults> HiP_HOPSResultsList = new List<HiP_HOPSResults>();
        static HiP_HOPSResults CombinedHiP_HOPSResults = new HiP_HOPSResults();

        static List<CutSets> CutSetsList = new List<CutSets>();

        //  static Dictionary<BasicEvent, bool> EventUsedDictionary = new 
        static Dictionary<string, BasicEvent> BasicEventsDictionary = new Dictionary<string, BasicEvent>();

        static List<Dictionary<int, Effect>> FaultTreesDictionaryLookup = new List<Dictionary<int, Effect>>();

        static void Main(string[] args)
        {
            outputFile = new XmlDocument();

            string mergePathsFile = args[0];
            Console.WriteLine("Loading file of fault tree paths to merge: " + mergePathsFile);
            LoadPaths(mergePathsFile);
            BasicEvent.IdCount = 0;   //TODO: Make sure this should reset the basic event count
            Effect.EffectCount = 0;   //TODO: Make sure this should reset the effect count
            CombineHiP_HOPSResults();
            WriteHiP_HOPSResults();
        }

        static void WriteHiP_HOPSResults()
        {

            // Write FaultTrees.xml file here

            //   using (XmlWriter writer)



            XmlWriterSettings settings = new XmlWriterSettings()
            {
                Async = false,
                Indent = true,
                IndentChars = "    "
            };

            using (XmlWriter writer = XmlWriter.Create("FaultTrees.xml", settings))
            {
                Console.WriteLine("Writing File: FaultTrees.xml");  //TODO: Should this have the file path?

                writer.WriteStartDocument();
                WriteHiPHOPSResults(writer, CombinedHiP_HOPSResults);
            }

            foreach (FaultTree cutSets in CombinedHiP_HOPSResults.FaultTrees)
            {

                // Write CutSets(x).xml files here
                // x represents the ID of the fault tree

            }

        }

        static void WriteHiPHOPSResults(XmlWriter writer, HiP_HOPSResults hiP_HOPSResults)
        {
            writer.WriteStartElement("HiP-HOPS_Results");
            writer.WriteAttributeString("model", CombinedHiP_HOPSResults.Model);
            writer.WriteAttributeString("build", CombinedHiP_HOPSResults.Build);
            writer.WriteAttributeString("maorVersion", CombinedHiP_HOPSResults.MajorVersion);
            writer.WriteAttributeString("minorVersion", CombinedHiP_HOPSResults.MinorVersion);
            writer.WriteAttributeString("version", CombinedHiP_HOPSResults.Version);
            writer.WriteAttributeString("versionDate", CombinedHiP_HOPSResults.VersionDate);

            writer.WriteStartElement("FaultTrees");

            WriteFMEA(writer, hiP_HOPSResults.FMEA);
            WriteFaultTrees(writer, hiP_HOPSResults.FaultTrees);
        }

        static void WriteFaultTrees(XmlWriter writer, List<FaultTree> faultTrees)
        {
            //TODO: Can the cut sets be written here?

            foreach (FaultTree faultTree in faultTrees)
            {
                writer.WriteStartElement("FaultTree");
                writer.WriteAttributeString("ID", faultTree.Id.ToString());

                writer.WriteStartElement("Name");
                writer.WriteString(faultTree.Name);
                writer.WriteEndElement();

                writer.WriteStartElement("Description");
                if (!string.IsNullOrEmpty(faultTree.Description))
                {
                    writer.WriteString(faultTree.Description);
                }
                writer.WriteEndElement();

                writer.WriteStartElement("Name");
                writer.WriteString(faultTree.Name);
                writer.WriteEndElement();

                writer.WriteStartElement("Unavailability");
                writer.WriteString(faultTree.Unavailability);
                writer.WriteEndElement();

                writer.WriteStartElement("UnavailabilitySort");
                writer.WriteString(faultTree.UnavailabilitySort);
                writer.WriteEndElement();

                writer.WriteStartElement("Severity");
                writer.WriteString(faultTree.Severity);
                writer.WriteEndElement();

                writer.WriteStartElement("OutputDeviation");
                writer.WriteStartElement("Name");
                writer.WriteString(faultTree.OutputDeviation.Name);
                writer.WriteEndElement();

                writer.WriteStartElement("Children");
                WriteChildren(writer, faultTree.OutputDeviation.Children);
                writer.WriteEndElement();


                writer.WriteEndElement();
            }
        }

        static void WriteChildren(XmlWriter writer, List<Node> nodes)
        {
            foreach (Node node in nodes)
            {

            }

            writer.WriteEndElement();

        }

        static void WriteFMEA(XmlWriter writer, FMEA fmea)
        {
            writer.WriteStartElement("FMEA");

            WriteComponents(writer, fmea.Components);

            writer.WriteEndElement();
        }

        static void WriteComponents(XmlWriter writer, List<Component> components)
        {
            foreach (Component component in components)
            {
                //   writer.WriteStartAttribute("Component");

                writer.WriteStartElement("Component");

                writer.WriteStartElement("Name");
                writer.WriteString(component.Name);
                writer.WriteEndElement();

                writer.WriteStartElement("Events");
                WriteEvents(writer, component.Events);
                writer.WriteEndElement();

                writer.WriteEndElement();
            }
        }

        static void WriteEvents(XmlWriter writer, List<BasicEvent> events)
        {
            foreach (BasicEvent basicEvent in events)
            {

                writer.WriteStartElement("BasicEvent");
                writer.WriteAttributeString("ID", basicEvent.Id.ToString());

                writer.WriteStartElement("Name");
                writer.WriteString(basicEvent.Name);
                writer.WriteEndElement();

                writer.WriteStartElement("ShortName");
                writer.WriteString(basicEvent.ShortName);
                writer.WriteEndElement();

                writer.WriteStartElement("Description");
                if (!string.IsNullOrEmpty(basicEvent.Description))
                {
                    writer.WriteString(basicEvent.Description);
                }
                writer.WriteEndElement();

                writer.WriteStartElement("Unavailability");
                writer.WriteString(basicEvent.Unavailability);
                writer.WriteEndElement();

                writer.WriteStartElement("Effects");
                WriteEffects(writer, basicEvent.Effects);
                writer.WriteEndElement();
            }
        }

        static void WriteEffects(XmlWriter writer, List<Effect> effects)
        {
            foreach (Effect effect in effects)
            {
                writer.WriteStartElement("Effect");
                writer.WriteAttributeString("ID", effect.Id.ToString());

                writer.WriteStartElement("Name");
                writer.WriteString(effect.Name);
                writer.WriteEndElement();

                writer.WriteStartElement("SinglePointFailure");
                writer.WriteString(effect.SinglePointFailure);
                writer.WriteEndElement();

                writer.WriteEndElement();
            }
            writer.WriteEndElement();
        }

        static void CombineHiP_HOPSResults()
        {
            //This takes each of the attributes from the first HiP-HOPS Results read in
            CombinedHiP_HOPSResults.Model = HiP_HOPSResultsList[0].Model + "_merged";
            CombinedHiP_HOPSResults.Build = HiP_HOPSResultsList[0].Build;
            CombinedHiP_HOPSResults.MajorVersion = HiP_HOPSResultsList[0].MajorVersion;
            CombinedHiP_HOPSResults.MinorVersion = HiP_HOPSResultsList[0].MinorVersion;
            CombinedHiP_HOPSResults.Version = HiP_HOPSResultsList[0].Version;
            CombinedHiP_HOPSResults.VersionDate = HiP_HOPSResultsList[0].VersionDate;

            CombineFaultTrees();
            CombineFMEA();
        }

        static void CombineFaultTrees()
        {
            List<FaultTree> UncombinedFaultTrees = new List<FaultTree>();

            for (int i = 0; i < HiP_HOPSResultsList.Count; i++)
            {
                FaultTreesDictionaryLookup.Add(new Dictionary<int, Effect>());
                foreach (FaultTree faultTree in HiP_HOPSResultsList[i].FaultTrees)
                {
                    faultTree.HiPHOPSResultsIndex = i;
                    UncombinedFaultTrees.Add(faultTree);
                }
            }

            CombinedHiP_HOPSResults.FaultTrees = CombineFaultTrees(UncombinedFaultTrees);

        }

        static List<FaultTree> CombineFaultTrees(List<FaultTree> uncombinedFaultTrees)
        {
            List<FaultTree> combinedFaultTrees = new List<FaultTree>();

            Dictionary<string, FaultTree> faultTreeListDictionary = new Dictionary<string, FaultTree>();

            Dictionary<string, int> duplicateNameCount = new Dictionary<string, int>();

            //TODO: Remove this after debugging
            List<FaultTree> duplicateFaultTrees = new List<FaultTree>();

            foreach (FaultTree faultTree in uncombinedFaultTrees)
            {
                //  int newId = EffectsDictionaryList[faultTree.HiPHOPSResultsIndex][faultTree.PreviousId].Id;

                string outputDeviationChecksum = faultTree.OutputDeviation.TotalChildrenChecksum();

                if (!faultTreeListDictionary.ContainsKey(outputDeviationChecksum))
                {
                    FaultTree newFaultTree = new FaultTree(faultTree.Name);


                    if (duplicateNameCount.ContainsKey(faultTree.Name))
                    {
                        newFaultTree.Name += (++duplicateNameCount[faultTree.Name]).ToString();
                    }
                    else
                    {
                        duplicateNameCount.Add(faultTree.Name, 1);
                    }

                    newFaultTree.Description = faultTree.Description;
                    newFaultTree.SIL = faultTree.SIL;
                    newFaultTree.Unavailability = faultTree.Unavailability;
                    newFaultTree.UnavailabilitySort = faultTree.UnavailabilitySort;
                    newFaultTree.Severity = faultTree.Severity;
                    newFaultTree.OutputDeviation = faultTree.OutputDeviation;

                    faultTreeListDictionary.Add(newFaultTree.OutputDeviation.TotalChildrenChecksum(), newFaultTree);
                    combinedFaultTrees.Add(newFaultTree);

                    Effect newEffect = new Effect();
                    newEffect.Id = newFaultTree.Id;
                    newEffect.Name = newFaultTree.Name;

                    FaultTreesDictionaryLookup[faultTree.HiPHOPSResultsIndex].Add(faultTree.PreviousId, newEffect);


                }
                else
                {
                    Effect newEffect = new Effect();
                    newEffect.Id = faultTreeListDictionary[outputDeviationChecksum].Id;
                    newEffect.Name = faultTreeListDictionary[outputDeviationChecksum].Name;

                    FaultTreesDictionaryLookup[faultTree.HiPHOPSResultsIndex].Add(faultTree.PreviousId, newEffect);
                }
            }

            return combinedFaultTrees;
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

            foreach (KeyValuePair<string, List<Component>> componentsKVP in componentVersionsDictionary)
            {
                Component newComponent = new Component(componentsKVP.Key);
                List<BasicEvent> uncombinedEvents = new List<BasicEvent>();

                foreach (Component component in componentsKVP.Value)
                {
                    foreach (BasicEvent basicEvent in component.Events)
                    {
                        uncombinedEvents.Add(basicEvent);
                    }
                }

                newComponent.Events = CombineEvents(uncombinedEvents);
                combinedComponents.Add(newComponent);
            }


            return combinedComponents;

        }

        static List<BasicEvent> CombineEvents(List<BasicEvent> events)
        {
            List<BasicEvent> combinedEvents = new List<BasicEvent>();

            Dictionary<string, List<BasicEvent>> eventListDictionary = new Dictionary<string, List<BasicEvent>>();

            while (events.Count > 0)
            {
                if (!eventListDictionary.ContainsKey(events[0].Name))
                {
                    List<BasicEvent> newEvents = new List<BasicEvent>();
                    newEvents.Add(events[0]);

                    eventListDictionary.Add(events[0].Name, newEvents);
                }
                else
                {
                    eventListDictionary[events[0].Name].Add(events[0]);

                }

                events.RemoveAt(0);
            }

            foreach (KeyValuePair<string, List<BasicEvent>> basicEventKVP in eventListDictionary)
            {
                BasicEvent newEvent = new BasicEvent(basicEventKVP.Key);   //This constructor will increment the basic event count
                newEvent.ShortName = basicEventKVP.Value[0].ShortName;
                newEvent.Description = basicEventKVP.Value[0].Description;
                newEvent.Unavailability = basicEventKVP.Value[0].Unavailability;

                List<Effect> uncombinedEffects = new List<Effect>();
                foreach (BasicEvent basicEvent in basicEventKVP.Value)
                {
                    foreach (Effect effect in basicEvent.Effects)
                    {
                        uncombinedEffects.Add(effect);
                    }

                }
                newEvent.Effects = CombineEffects(uncombinedEffects);

                //TODO: Is this part necessary, or should this be in a dictionary so it can be looked up?
                //Should the dictionary be specific to the HiP-HOPS results it came from?
                newEvent.PreviousId = basicEventKVP.Value[0].PreviousId;


                combinedEvents.Add(newEvent);

                //TODO: Is this part necessary
                BasicEventsDictionary.Add(newEvent.Name, newEvent);

            }

            return combinedEvents;
        }

        static List<Effect> CombineEffects(List<Effect> effects)
        {
            List<Effect> combinedEffects = new List<Effect>();

            Dictionary<int, Effect> effectDictionary = new Dictionary<int, Effect>();


            foreach (Effect effect in effects)
            {
                int newID = FaultTreesDictionaryLookup[effect.HiPHOPSResultsIndex][effect.PreviousId].Id;
                Effect newEffect = effect;

                if (!effectDictionary.ContainsKey(newID))
                {
                    newEffect.Name = FaultTreesDictionaryLookup[effect.HiPHOPSResultsIndex][effect.PreviousId].Name;
                    newEffect.Id = newID;

                    effectDictionary.Add(newID, newEffect);
                    combinedEffects.Add(newEffect);
                }
            }
            return combinedEffects;
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
            FaultTree faultTree = new FaultTree();
            faultTree.Name = name;
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

            // faultTree.HiPHOPSResultsIndex = HiP_HOPSResultsList.Count;

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

            or.Id = previousId;

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

            and.Id = previousId;

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
            basicEvent.PreviousId = previousId;

            if (!BasicEventsDictionaryList[HiP_HOPSResultsList.Count].ContainsKey(previousId))
            {

                reader.ReadStartElement();

                if (reader.Name == "Name")
                {
                    name = reader.ReadElementContentAsString();
                    basicEvent.Name = name;
                }

                basicEvent.PreviousId = previousId;

                if (reader.Name == "ShortName")
                {
                    shortName = reader.ReadElementContentAsString();
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

            Effect effect = new Effect();
            effect.Name = name;
            effect.SinglePointFailure = singlePointFailure;

            effect.PreviousId = previousId;

            effect.HiPHOPSResultsIndex = HiP_HOPSResultsList.Count;

            reader.Read();

            return effect;
        }
    }
}
