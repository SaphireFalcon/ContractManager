using System.Xml.Serialization;
using System.Collections.Generic;

namespace ContractManager.ContractBlueprint
{
    [XmlRoot("Contract")]    
    public class ContractBlueprint  // Note needs for serialization
    {
        // Fields as loaded from XML
        [XmlElement("uid")]
        public string uid {  get; set;  }
        [XmlElement("title")]
        public string title { get; set; }
        [XmlElement("synopsis")]
        public string synopsis { get; set; }
        [XmlElement("details")]
        public string details { get; set; }

        public ContractBlueprint() { }

        internal void WriteToConsole()
        {
            Console.WriteLine($"Contract Blueprint:");
            Console.WriteLine($"  UID: {uid}");
            Console.WriteLine($"  Title: {title}");
            Console.WriteLine($"  Synopsis: {synopsis}");
            Console.WriteLine($"  Details: {details}");
        }

        internal void WriteToFile(string filePath)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(ContractBlueprint));
            using (var writer = new System.IO.StreamWriter(filePath))
            {
                serializer.Serialize(writer, this);
            }
        }

        internal static ContractBlueprint LoadFromFile(string filePath)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(ContractBlueprint));
            using (var reader = new System.IO.StreamReader(filePath))
            {
                return (ContractBlueprint)serializer.Deserialize(reader);
            }
        }
    }
}
