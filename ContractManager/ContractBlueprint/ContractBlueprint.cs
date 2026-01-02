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
        public string synopsis { get; set; } = string.Empty;

        // Detailed description of the contract
        [XmlElement("description", DataType = "string")]
        public string description { get; set; } = string.Empty;

        // When an offered contract will expired, in seconds
        [XmlElement("expiration", DataType = "double")]
        public double expiration { get; set; } = double.PositiveInfinity;  // Never expires

        // Flag if an offered contract from this blueprint can be rejected.
        [XmlElement("isRejectable", DataType = "boolean")]
        public bool isRejectable { get; set; } = true;

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

        internal bool Validate()
        {
            // Validate the contract blueprint.
            // The title can't be empty
            if (String.IsNullOrEmpty(this.title))
            {
                Console.WriteLine("[CM] [WARNING] blueprint title has be to be defined.");
                return false;
            }
            // The uid can't be empty
            if (String.IsNullOrEmpty(this.uid))
            {
                Console.WriteLine("[CM] [WARNING] blueprint uid has be to be defined.");
                return false;
            }
            // It should have at least one prerequisite to know when to offer a contract from the blueprint
            if (this.prerequisites.Count == 0)
            {
                Console.WriteLine($"[CM] [WARNING] blueprint '{this.title}' has no prerequisites.");
                return false;
            }
            // It should have at least one requirement to know when to the contract should be completed.
            if (this.requirements.Count == 0)
            {
                Console.WriteLine($"[CM] [WARNING] blueprint '{this.title}' has no prerequisites.");
                return false;
            }
            foreach (var prerequisite in prerequisites)
            {
                if (!prerequisite.Validate()) { return false; }
            }
            foreach (var requirement in requirements)
            {
                if (!requirement.Validate()) { return false; }
            }
            foreach (var action in actions)
            {
                if (!action.Validate()) { return false; }
            }

            return true;
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
