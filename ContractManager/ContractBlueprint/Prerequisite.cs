using System.Xml.Serialization;
using System.Collections.Generic;

namespace ContractManager.ContractBlueprint
{
    public class Prerequisite
    {
        // Type of the prerequisite.
        [XmlElement("type")]
        public string type { get; set; }

        // Fields for specific prerequisite types.
        // type: maxNumOfferedContracts - offer contract if number of offered contracts is less than this number.
        [XmlElement("maxNumOfferedContracts")]
        public uint maxNumOfferedContracts { get; set; }

        // type: maxNumAcceptedContracts  - offer contract if number of accept contracts is less than this number.
        [XmlElement("maxNumAcceptedContracts")]
        public uint maxNumAcceptedContracts { get; set; }

        public Prerequisite() { }
        
        public void WriteToConsole()
        {
            if (type == "maxNumOfferedContracts")
            {
                Console.WriteLine($"  - Require less than {maxNumOfferedContracts} offered contracts");
            }
            if (type == "maxNumAcceptedContracts")
            {
                Console.WriteLine($"  - Require less than {maxNumAcceptedContracts} accepted contracts");
            }
        }
    }
}
