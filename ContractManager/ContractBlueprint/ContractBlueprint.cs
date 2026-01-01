using System.Xml.Serialization;
using System.Collections.Generic;

namespace ContractManager.ContractBlueprint
{
    [XmlRoot("Contract")]    
    public class ContractBlueprint
    {
        // Details of the contract
        // The version for which the contract was created.
        [XmlElement("version", DataType = "string")]
        public string version { get; set; } = ContractManager.version;

        // The unique identifier for the contract
        [XmlElement("uid", DataType = "string")]
        public string uid { get; set; }

        // The title of the contract
        [XmlElement("title", DataType = "string")]
        public string title { get; set; }

        // A brief synopsis of the contract
        [XmlElement("synopsis", DataType = "string")]
        public string synopsis { get; set; }

        // Detailed description of the contract
        [XmlElement("description", DataType = "string")]
        public string description { get; set; }

        // List of prerequisites for the contract
        [XmlArray("prerequisites")]
        public List<Prerequisite> prerequisites { get; set; } = new List<Prerequisite>();

        // Completion condition of the contract based on the requirements.
        [XmlElement("completionCondition")]
        public CompletionCondition completionCondition { get; set; } = CompletionCondition.All;

        // List of requirements for the contract
        [XmlArray("requirements")]
        public List<Requirement> requirements { get; set; } = new List<Requirement>();

        // List of actions for the contract
        [XmlArray("actions")]
        public List<Action> actions { get; set; } = new List<Action>();

        public ContractBlueprint() { }

        //  Doesn't write anything to console in-game, only on StarMap launcher console.
        internal void WriteToConsole()
        {
            Console.WriteLine($"Contract Blueprint:");
            Console.WriteLine($"  UID: {uid}");
            Console.WriteLine($"  Title: {title}");
            Console.WriteLine($"  Synopsis: {synopsis}");
            Console.WriteLine($"  Description: {description}");
            Console.WriteLine($"  Prerequisistes: {prerequisites.Count}");
            foreach (Prerequisite prerequisite in prerequisites)
            {
                prerequisite.WriteToConsole();
            }
            Console.WriteLine($"  Complete {completionCondition} of {requirements.Count} requirements:");
            foreach (Requirement requirement in requirements)
            {
                requirement.WriteToConsole();
            }
            Console.WriteLine($"  Actions: {actions.Count}");
            foreach (Action action in actions)
            {
                action.WriteToConsole();
            }
            Console.WriteLine($"  ");
        }

        // Write the contract blueprint to an XML file.
        internal void WriteToFile(string filePath)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(ContractBlueprint));
            using (var writer = new System.IO.StreamWriter(filePath))
            {
                serializer.Serialize(writer, this);
            }
        }

        // Load a contract blueprint from an XML file.
        internal static ContractBlueprint LoadFromFile(string filePath)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(ContractBlueprint));
            using (var reader = new System.IO.StreamReader(filePath))
            {
                return (ContractBlueprint)serializer.Deserialize(reader);
            }
        }
    }

    public enum CompletionCondition
    {
        [XmlEnum("all")]
        All,
        [XmlEnum("any")]
        Any
    }
}
