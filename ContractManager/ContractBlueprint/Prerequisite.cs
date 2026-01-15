using System.Collections.Generic;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace ContractManager.ContractBlueprint
{

    public class Prerequisite
    {
        // [DEPRECIATED v0.2.1] Type of the prerequisite.
        //[XmlElement("type")]
        //public PrerequisiteType type { get; set; }

        // Fields for specific prerequisite types.

        // Contract specific
        // type: maxNumOfferedContracts - offer contract if number of offered contracts is less than this number.
        [XmlElement("maxNumOfferedContracts", DataType = "unsignedInt")]
        public uint maxNumOfferedContracts { get; set; } = uint.MaxValue;

        // type: maxNumAcceptedContracts  - offer contract if number of accepted contracts is less than this number.
        [XmlElement("maxNumAcceptedContracts", DataType = "unsignedInt")]
        public uint maxNumAcceptedContracts { get; set; } = uint.MaxValue;

        // Mission specific
        // type: maxNumOfferedMissions - offer mission if number of offered missions is less than this number.
        [XmlElement("maxNumOfferedMissions", DataType = "unsignedInt")]
        public uint maxNumOfferedMissions { get; set; } = uint.MaxValue;

        // type: maxNumAcceptedMissions  - offer mission if number of accepted missions is less than this number.
        [XmlElement("maxNumAcceptedMissions", DataType = "unsignedInt")]
        public uint maxNumAcceptedMissions { get; set; } = uint.MaxValue;
        
        // Specific to either contract or mission
        // type: maxCompleteCount - offer if number of completed instances of this contract/mission blueprint is less than this number.
        [XmlElement("maxCompleteCount", DataType = "unsignedInt")]
        public uint maxCompleteCount { get; set; } = 0;

        // type: maxFailedCount - offer if number of failed instances of this contract/mission blueprint is less than this number.
        [XmlElement("maxFailedCount", DataType = "unsignedInt")]
        public uint maxFailedCount { get; set; } = uint.MaxValue;

        // type: maxConcurrentCount - offer if number of accepted instances of this contract/mission blueprint is less than this number.
        [XmlElement("maxConcurrentCount", DataType = "unsignedInt")]
        public uint maxConcurrentCount { get; set; } = 0;

        // Generic
        // type: hasCompletedContract - offer contract if the specified contract blueprint uid has been completed.
        [XmlElement("hasCompletedContract", DataType = "string")]
        public string hasCompletedContract { get; set; }

        // type: hasFailedContract - offer contract if the specified contract blueprint uid has been failed.
        [XmlElement("hasFailedContract", DataType = "string")]
        public string hasFailedContract { get; set; }

        // type: hasAcceptedContract - offer contract if the specified contract blueprint uid has been accepted (and not yet completed/failed).
        [XmlElement("hasAcceptedContract", DataType = "string")]
        public string hasAcceptedContract { get; set; }

        // type: hasCompletedMission - offer contract/mission if the specified mission blueprint uid has been completed.
        [XmlElement("hasCompletedMission", DataType = "string")]
        public string hasCompletedMission { get; set; }

        // type: hasFailedMission - offer contract/mission if the specified mission blueprint uid has been failed.
        [XmlElement("hasFailedMission", DataType = "string")]
        public string hasFailedMission { get; set; }

        // type: hasAcceptedMission - offer contract/mission if the specified mission blueprint uid has been accepted (and not yet completed/failed).
        [XmlElement("hasAcceptedMission", DataType = "string")]
        public string hasAcceptedMission { get; set; }

        // type: minNumberOfVessels - offer contract/mission if number of vessels in current celestial system is more than this number.
        [XmlElement("minNumberOfVessels", DataType = "unsignedInt")]
        public uint minNumberOfVessels { get; set; } = 0;

        // type: maxNumberOfVessels - offer contract/mission if number of vessels in current celestial system is less than this number.
        [XmlElement("maxNumberOfVessels", DataType = "unsignedInt")]
        public uint maxNumberOfVessels { get; set; } = uint.MaxValue;

        public Prerequisite() { }
        
        public void WriteToConsole()
        {
            Console.WriteLine($"  - Require less than {maxNumOfferedContracts} offered contracts");
            Console.WriteLine($"  - Require less than {maxNumAcceptedContracts} accepted contracts");
            Console.WriteLine($"  - Require less than {maxNumOfferedMissions} offered missions");
            Console.WriteLine($"  - Require less than {maxNumAcceptedMissions} accepted missions");
            Console.WriteLine($"  - Require less than {maxCompleteCount} ");
            Console.WriteLine($"  - Require less than {maxFailedCount} ");
            Console.WriteLine($"  - Require less than {maxConcurrentCount} ");
            Console.WriteLine($"  - Require to have compeleted '{hasCompletedContract}' contract");
            Console.WriteLine($"  - Require to have failed '{hasFailedContract}' contract");
            Console.WriteLine($"  - Require to have accepted '{hasAcceptedContract}' contract");
            Console.WriteLine($"  - Require to have compeleted '{hasCompletedMission}' mission");
            Console.WriteLine($"  - Require to have failed '{hasFailedMission}' mission");
            Console.WriteLine($"  - Require to have accepted '{hasAcceptedMission}' mission");
            Console.WriteLine($"  - Require more than {minNumberOfVessels} vessels");
            Console.WriteLine($"  - Require less than {maxNumberOfVessels} vessels");
        }

        internal bool Validate()
        {
            // Validate if the has*Contract/Mission also exists?
            return true;
        }

        // Migrate function for v0.2.1 when PrerequisiteType was depreciated and Prerequisite was flattened to a single entry in Contract and Mission blueprints.
        internal static XElement? MigratePrerequisteWithTypeFlatten(XElement? prerequisitesElement)
        {
            if (prerequisitesElement == null ) { return null; }

            int numPrerequisteElements = prerequisitesElement.Elements("Prerequisite").Count();
            Console.WriteLine($"[CM] [INFO] prerequisite elements {numPrerequisteElements}.");
            if (numPrerequisteElements == 0) { return null; }
            if (numPrerequisteElements == 1)
            {
                // flatten to this element only
                XElement? prerequisiteElement = prerequisitesElement.Element("Prerequisite");
                if (prerequisiteElement == null) { return null; } // wtf, this shouldn't happen...
                // remove the type element.
                XElement? prerequisiteTypeElement = prerequisiteElement.Element("type");
                if (prerequisiteTypeElement != null )
                {
                    prerequisiteTypeElement.Remove();
                }
                return prerequisiteElement;
            }
            else
            if (numPrerequisteElements > 1)
            {
                // need to combine multiple Prerequisite into one.
                XElement migratedPrerequisiteElement = new XElement("prerequisite", null);
                foreach (XElement prerequisiteElement in prerequisitesElement.Elements("Prerequisite"))
                {
                    XElement? prerequisiteTypeElement = prerequisiteElement.Element("type");
                    if (prerequisiteTypeElement == null ) { continue; }
                    string prerequisiteTypeString = (string)prerequisiteTypeElement.Value;
                                
                    if (prerequisiteTypeString == "maxNumOfferedContracts")
                    {
                        XElement? prerequisiteValue = prerequisiteElement.Element("maxNumOfferedContracts");
                        if (prerequisiteValue != null)
                        {
                            migratedPrerequisiteElement.Add("maxNumOfferedContracts", prerequisiteValue.Value);
                        }
                    }
                    else
                    if (prerequisiteTypeString == "maxNumAcceptedContracts")
                    {
                        XElement? prerequisiteValue = prerequisiteElement.Element("maxNumAcceptedContracts");
                        if (prerequisiteValue != null)
                        {
                            migratedPrerequisiteElement.Add("maxNumAcceptedContracts", prerequisiteValue.Value);
                        }
                    }
                    else
                    if (prerequisiteTypeString == "maxNumOfferedMissions")
                    {
                        XElement? prerequisiteValue = prerequisiteElement.Element("maxNumOfferedMissions");
                        if (prerequisiteValue != null)
                        {
                            migratedPrerequisiteElement.Add("maxNumOfferedMissions", prerequisiteValue.Value);
                        }
                    }
                    else
                    if (prerequisiteTypeString == "maxNumAcceptedMissions")
                    {
                        XElement? prerequisiteValue = prerequisiteElement.Element("maxNumAcceptedMissions");
                        if (prerequisiteValue != null)
                        {
                            migratedPrerequisiteElement.Add("maxNumAcceptedMissions", prerequisiteValue.Value);
                        }
                    }
                    else
                    if (prerequisiteTypeString == "maxCompleteCount")
                    {
                        XElement? prerequisiteValue = prerequisiteElement.Element("maxCompleteCount");
                        if (prerequisiteValue != null)
                        {
                            migratedPrerequisiteElement.Add("maxCompleteCount", prerequisiteValue.Value);
                        }
                    }
                    else
                    if (prerequisiteTypeString == "maxFailedCount")
                    {
                        XElement? prerequisiteValue = prerequisiteElement.Element("maxFailedCount");
                        if (prerequisiteValue != null)
                        {
                            migratedPrerequisiteElement.Add("maxFailedCount", prerequisiteValue.Value);
                        }
                    }
                    else
                    if (prerequisiteTypeString == "maxConcurrentCount")
                    {
                        XElement? prerequisiteValue = prerequisiteElement.Element("maxConcurrentCount");
                        if (prerequisiteValue != null)
                        {
                            migratedPrerequisiteElement.Add("maxConcurrentCount", prerequisiteValue.Value);
                        }
                    }
                    else
                    if (prerequisiteTypeString == "hasCompletedContract")
                    {
                        XElement? prerequisiteValue = prerequisiteElement.Element("hasCompletedContract");
                        if (prerequisiteValue != null)
                        {
                            migratedPrerequisiteElement.Add("hasCompletedContract", prerequisiteValue.Value);
                        }
                    }
                    else
                    if (prerequisiteTypeString == "hasFailedContract")
                    {
                        XElement? prerequisiteValue = prerequisiteElement.Element("hasFailedContract");
                        if (prerequisiteValue != null)
                        {
                            migratedPrerequisiteElement.Add("hasFailedContract", prerequisiteValue.Value);
                        }
                    }
                    else
                    if (prerequisiteTypeString == "hasAcceptedContract")
                    {
                        XElement? prerequisiteValue = prerequisiteElement.Element("hasAcceptedContract");
                        if (prerequisiteValue != null)
                        {
                            migratedPrerequisiteElement.Add("hasAcceptedContract", prerequisiteValue.Value);
                        }
                    }
                    else
                    if (prerequisiteTypeString == "hasCompletedMission")
                    {
                        XElement? prerequisiteValue = prerequisiteElement.Element("hasCompletedMission");
                        if (prerequisiteValue != null)
                        {
                            migratedPrerequisiteElement.Add("hasCompletedMission", prerequisiteValue.Value);
                        }
                    }
                    else
                    if (prerequisiteTypeString == "hasFailedMission")
                    {
                        XElement? prerequisiteValue = prerequisiteElement.Element("hasFailedMission");
                        if (prerequisiteValue != null)
                        {
                            migratedPrerequisiteElement.Add("hasFailedMission", prerequisiteValue.Value);
                        }
                    }
                    else
                    if (prerequisiteTypeString == "hasAcceptedMission")
                    {
                        XElement? prerequisiteValue = prerequisiteElement.Element("hasAcceptedMission");
                        if (prerequisiteValue != null)
                        {
                            migratedPrerequisiteElement.Add("hasAcceptedMission", prerequisiteValue.Value);
                        }
                    }
                    else
                    if (prerequisiteTypeString == "minNumberOfVessels")
                    {
                        XElement? prerequisiteValue = prerequisiteElement.Element("minNumberOfVessels");
                        if (prerequisiteValue != null)
                        {
                            migratedPrerequisiteElement.Add("minNumberOfVessels", prerequisiteValue.Value);
                        }
                    }
                    else
                    if (prerequisiteTypeString == "maxNumberOfVessels")
                    { 
                        XElement? prerequisiteValue = prerequisiteElement.Element("maxNumberOfVessels");
                        if (prerequisiteValue != null)
                        {
                            migratedPrerequisiteElement.Add("maxNumberOfVessels", prerequisiteValue.Value);
                        }
                    }
                }
                return migratedPrerequisiteElement;
            }
            return null;
        }
    }

    // [DEPRECIATED v0.2.1]
    //public enum PrerequisiteType
    //{
    //    [XmlEnum("maxNumOfferedContracts")]
    //    MaxNumOfferedContracts,
    //    [XmlEnum("maxNumAcceptedContracts")]
    //    MaxNumAcceptedContracts,

    //    [XmlEnum("maxNumOfferedMissions")]
    //    MaxNumOfferedMissions,
    //    [XmlEnum("maxNumAcceptedMissions")]
    //    MaxNumAcceptedMissions,

    //    [XmlEnum("maxCompleteCount")]
    //    MaxCompleteCount,
    //    [XmlEnum("maxFailedCount")]
    //    MaxFailedCount,
    //    [XmlEnum("maxConcurrentCount")]
    //    MaxConcurrentCount,

    //    [XmlEnum("hasCompletedContract")]
    //    HasCompletedContract,
    //    [XmlEnum("hasFailedContract")]
    //    HasFailedContract,
    //    [XmlEnum("hasAcceptedContract")]
    //    HasAcceptedContract,
    //    [XmlEnum("hasCompletedMission")]
    //    HasCompletedMission,
    //    [XmlEnum("hasFailedMission")]
    //    HasFailedMission,
    //    [XmlEnum("hasAcceptedMission")]
    //    HasAcceptedMission,
    //    [XmlEnum("minNumberOfVessels")]
    //    MinNumberOfVessels,
    //    [XmlEnum("maxNumberOfVessels")]
    //    MaxNumberOfVessels,
    //}
}
