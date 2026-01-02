using System.Xml.Serialization;
using System.Collections.Generic;

namespace ContractManager.ContractBlueprint
{

    public class Prerequisite
    {
        // Type of the prerequisite.
        [XmlElement("type")]
        public PrerequisiteType type { get; set; }

        // Fields for specific prerequisite types.
        // type: maxNumOfferedContracts - offer contract if number of offered contracts is less than this number.
        [XmlElement("maxNumOfferedContracts", DataType = "unsignedInt")]
        public uint maxNumOfferedContracts { get; set; } = uint.MaxValue;

        // type: maxNumAcceptedContracts  - offer contract if number of accept contracts is less than this number.
        [XmlElement("maxNumAcceptedContracts", DataType = "unsignedInt")]
        public uint maxNumAcceptedContracts { get; set; } = uint.MaxValue;

        // type: maxCompleteCount - offer contract if number of completed instances of this contract blueprint is less than this number.
        [XmlElement("maxCompleteCount", DataType = "unsignedInt")]
        public uint maxCompleteCount { get; set; } = 0;

        // type: maxFailedCount - offer contract if number of completed instances of this contract blueprint is less than this number.
        [XmlElement("maxFailedCount", DataType = "unsignedInt")]
        public uint maxFailedCount { get; set; } = uint.MaxValue;

        // type: maxConcurrentCount - offer contract if number of completed instances of this contract blueprint is less than this number.
        [XmlElement("maxConcurrentCount", DataType = "unsignedInt")]
        public uint maxConcurrentCount { get; set; } = 0;

        // type: hasCompletedContract - offer contract if ...
        [XmlElement("hasCompletedContract", DataType = "string")]
        public string hasCompletedContract { get; set; }

        // type: hasFailedContract - offer contract if ...
        [XmlElement("hasFailedContract", DataType = "string")]
        public string hasFailedContract { get; set; }

        // type: hasAcceptedContract - offer contract if ...
        [XmlElement("hasAcceptedContract", DataType = "string")]
        public string hasAcceptedContract { get; set; }


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
            // nothing to validate.
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
        //[XmlEnum("minNumberOfVessels")]
        //MinNumberOfVessels,
        //[XmlEnum("maxNumberOfVessels")]
        //MaxNumberOfVessels
    }
}
