using ContractManager.ContractBlueprint;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace ContractManager.Mission
{
    [XmlRoot("Mission")]
    public class MissionBlueprint
    {
        // The version for which the contract was created.
        [XmlElement("version", DataType = "string")]
        public string version { get; set; } = ContractManager.version;
        
        // The unique identifier for the mission
        [XmlElement("uid", DataType = "string")]
        public string uid { get; set; }

        // The title of the mission
        [XmlElement("title", DataType = "string")]
        public string title { get; set; }

        // A brief synopsis of the mission
        [XmlElement("synopsis", DataType = "string")]
        public string synopsis { get; set; } = string.Empty;

        // Detailed description of the mission
        [XmlElement("description", DataType = "string")]
        public string description { get; set; } = string.Empty;

        // When an offered mission will expired, in seconds
        [XmlElement("expiration", DataType = "double")]
        public double expiration { get; set; } = double.PositiveInfinity;  // Never expires

        // Flag if an offered mission from this blueprint can be rejected.
        [XmlElement("isRejectable", DataType = "boolean")]
        public bool isRejectable { get; set; } = true;

        // When an accepted mission will fail, in seconds
        [XmlElement("deadline", DataType = "double")]
        public double deadline { get; set; } = double.PositiveInfinity;  // No deadline

        // Flag if an offered mission is automatically accepted.
        [XmlElement("isAutoAccepted", DataType = "boolean")]
        public bool isAutoAccepted { get; set; } = false;

        // List of prerequisites for the misison to be offered automatically
        [XmlArray("prerequisites")]
        public List<Prerequisite> prerequisites { get; set; } = new List<Prerequisite>();

        // List of actions for the misison
        [XmlArray("actions")]
        public List<ContractBlueprint.Action> actions { get; set; } = new List<ContractBlueprint.Action>();

        // List of contract blueprints UIDs for the mission
        [XmlArray("contractBlueprintUIDs")]
        public List<string> contractBlueprintUIDs { get; set; } = new List<string>();

        // internal flag to indicate if the blueprint can be edited or not
        internal bool isEditable { get; set; } = false;

        public MissionBlueprint() { }
        
        // Write the mission blueprint to an XML file.
        internal void WriteToFile(string filePath)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(MissionBlueprint));
            using (var writer = new System.IO.StreamWriter(filePath))
            {
                serializer.Serialize(writer, this);
            }
        }

        // Load a mission blueprint from an XML file.
        internal static MissionBlueprint LoadFromFile(string filePath)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(MissionBlueprint));
            using (var reader = new System.IO.StreamReader(filePath))
            {
                return (MissionBlueprint)serializer.Deserialize(reader);
            }
            // 
        }

        internal bool Validate(List<ContractBlueprint.ContractBlueprint> contractBlueprints)
        {
            // Validate the contract blueprint.
            // The title can't be empty
            if (String.IsNullOrEmpty(this.title))
            {
                Console.WriteLine("[CM] [WARNING] mission blueprint title has be to be defined.");
                return false;
            }
            // The uid can't be empty
            if (String.IsNullOrEmpty(this.uid))
            {
                Console.WriteLine("[CM] [WARNING] mission blueprint uid has be to be defined.");
                return false;
            }
            // It should have at least one contract linked to the mission.
            if (this.contractBlueprintUIDs.Count == 0)
            {
                Console.WriteLine($"[CM] [WARNING] mission blueprint '{this.title}' has no contracts.");
                return false;
            }
            foreach (var prerequisite in prerequisites)
            {
                if (!prerequisite.Validate()) { return false; }
            }
            foreach (var action in actions)
            {
                if (!action.Validate()) { return false; }
            }

            return true;
        }
    }
}
