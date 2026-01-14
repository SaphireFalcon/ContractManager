using System.Xml.Serialization;
using System.Collections.Generic;

namespace ContractManager.ContractBlueprint
{

    public class Prerequisite
    {
        // The unique identifier for the prerequisite.
        [XmlElement("uid", DataType = "string")]
        public string uid { get; set; }

        // Type of the prerequisite.
        [XmlElement("type")]
        public PrerequisiteType type { get; set; }

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
            if (type == PrerequisiteType.MaxNumOfferedContracts)
            {
                Console.WriteLine($"  - Require less than {maxNumOfferedContracts} offered contracts");
            }
            if (type == PrerequisiteType.MaxNumAcceptedContracts)
            {
                Console.WriteLine($"  - Require less than {maxNumAcceptedContracts} accepted contracts");
            }
        }

        internal bool Validate()
        {
            // The uid can't be empty
            if (String.IsNullOrEmpty(this.uid))
            {
                Console.WriteLine("[CM] [WARNING] prerequisite uid has be to be defined.");
                return false;
            }
            if (this.type == PrerequisiteType.HasCompletedContract && String.IsNullOrEmpty(this.hasCompletedContract))
            {
                Console.WriteLine("[CM] [WARNING] prerequisite with type 'hasCompletedContract' requires a contract blueprint uid to be defined in hasCompletedContract.");
                return false;
            }
            if (this.type == PrerequisiteType.HasFailedContract && String.IsNullOrEmpty(this.hasFailedContract))
            {
                Console.WriteLine("[CM] [WARNING] prerequisite with type 'hasFailedContract' requires a contract blueprint uid to be defined in hasFailedContract.");
                return false;
            }
            if (this.type == PrerequisiteType.HasAcceptedContract && String.IsNullOrEmpty(this.hasAcceptedContract))
            {
                Console.WriteLine("[CM] [WARNING] prerequisite with type 'hasAcceptedContract' requires a contract blueprint uid to be defined in hasAcceptedContract.");
                return false;
            }
            // PrerequisiteType doesn't need to be validated, loading XML will throw an exception.
            return true;
        }
    }

    public enum PrerequisiteType
    {
        [XmlEnum("maxNumOfferedContracts")]
        MaxNumOfferedContracts,
        [XmlEnum("maxNumAcceptedContracts")]
        MaxNumAcceptedContracts,

        [XmlEnum("maxNumOfferedMissions")]
        MaxNumOfferedMissions,
        [XmlEnum("maxNumAcceptedMissions")]
        MaxNumAcceptedMissions,

        [XmlEnum("maxCompleteCount")]
        MaxCompleteCount,
        [XmlEnum("maxFailedCount")]
        MaxFailedCount,
        [XmlEnum("maxConcurrentCount")]
        MaxConcurrentCount,

        [XmlEnum("hasCompletedContract")]
        HasCompletedContract,
        [XmlEnum("hasFailedContract")]
        HasFailedContract,
        [XmlEnum("hasAcceptedContract")]
        HasAcceptedContract,
        [XmlEnum("hasCompletedMission")]
        HasCompletedMission,
        [XmlEnum("hasFailedMission")]
        HasFailedMission,
        [XmlEnum("hasAcceptedMission")]
        HasAcceptedMission,
        [XmlEnum("minNumberOfVessels")]
        MinNumberOfVessels,
        [XmlEnum("maxNumberOfVessels")]
        MaxNumberOfVessels,
    }
}
