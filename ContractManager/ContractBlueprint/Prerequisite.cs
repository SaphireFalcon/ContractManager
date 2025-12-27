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
        [XmlElement("maxNumOfferedContracts")]
        public uint maxNumOfferedContracts { get; set; } = uint.MaxValue;

        // type: maxNumAcceptedContracts  - offer contract if number of accept contracts is less than this number.
        [XmlElement("maxNumAcceptedContracts")]
        public uint maxNumAcceptedContracts { get; set; } = uint.MaxValue;

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
    }

    public enum PrerequisiteType
    {
        [XmlEnum("maxNumOfferedContracts")]
        MaxNumOfferedContracts,
        [XmlEnum("maxNumAcceptedContracts")]
        MaxNumAcceptedContracts
    }
}
