using ContractManager.ContractBlueprint;
using System.Collections.Generic;
using System.Text;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace ContractManager.Mission
{
    [XmlRoot("Mission")]
    public class MissionBlueprint
    {
        // The version for which the contract was created.
        [XmlElement("version", DataType = "string")]
        public string version { get; set; } = ContractManager.version.ToString();
        
        // The unique identifier for the mission
        [XmlElement("uid", DataType = "string")]
        public string uid { get; set; }
        internal static int uidMaxLength = 128;

        // The title of the mission
        [XmlElement("title", DataType = "string")]
        public string title { get; set; }
        internal static int titleMaxLength = 128;

        // A brief synopsis of the mission
        [XmlElement("synopsis", DataType = "string")]
        public string synopsis { get; set; } = string.Empty;
        internal static int synopsisMaxLength = 1024;

        // Detailed description of the mission
        [XmlElement("description", DataType = "string")]
        public string description { get; set; } = string.Empty;
        internal static int descriptionMaxLength = 4096;

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

        // [DEPRECIATED v0.2.1] List of prerequisites for the misison to be offered automatically
        //[XmlArray("prerequisites")]
        //public List<Prerequisite> prerequisites { get; set; } = new List<Prerequisite>();

        // [v0.2.1] Prerequisite to offer contract
        [XmlElement("Prerequisite")]
        public Prerequisite prerequisite { get; set; } = new Prerequisite();

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
        internal static MissionBlueprint? LoadFromFile(string filePath)
        {
            XDocument? xmlDocument = null;
            using (var reader = new System.IO.StreamReader(filePath))
            {
                xmlDocument = XDocument.Load(reader);
            }
            if (xmlDocument == null) { return null; }
            if (xmlDocument.Root == null) { return null; }
            XElement? versionElement = xmlDocument.Root.Element("version");
            if (versionElement == null) { return null; }
            Version loadedXMLVersion = new Version(versionElement.Value);
            if (!loadedXMLVersion.valid)
            {
                Console.WriteLine($"[CM] [WARNING] Failed to read version from '{versionElement.Value}'");
                return null;
            }
            
            //  Check for migration
            if (ContractManager.version == loadedXMLVersion)
            {
                Console.WriteLine($"[CM] [INFO] Mod version {ContractManager.version.ToString()} matches loaded Version {loadedXMLVersion.ToString()}'.");
            }
            else
            if (ContractManager.version > loadedXMLVersion)
            {
                Console.WriteLine($"[CM] [INFO] Mod version {ContractManager.version.ToString()} is newer than loaded Version {loadedXMLVersion.ToString()}' for '{filePath}'.");
                // Do migration
                bool migratedSuccessfully = MissionBlueprint.Migrate(ref xmlDocument, ref loadedXMLVersion);
                if (!migratedSuccessfully) { return null;}
                // Export the migrated file to export folder
            }
            else
            if (ContractManager.version < loadedXMLVersion)
            {
                Console.WriteLine($"[CM] [INFO] Mod version {ContractManager.version.ToString()} is older than loaded Version {loadedXMLVersion.ToString()}' for '{filePath}'.");
                return null;
            }

            XmlSerializer serializer = new XmlSerializer(typeof(MissionBlueprint));
            return (MissionBlueprint)serializer.Deserialize(new StringReader(xmlDocument.ToString()));
        }

        private static bool Migrate(ref XDocument xmlDocument, ref Version xmlVersion)
        {
            Console.WriteLine($"[CM] [INFO] Running Migration.");
            if (xmlVersion < "0.2.1")
            {
                // version 0.2.1 flattens prerequisiteElement
                XElement? prerequisitesElement = xmlDocument.Root.Element("prerequisites");
                if (prerequisitesElement != null )
                {
                    XElement? migratedPrerequisiteElement = ContractBlueprint.Prerequisite.MigratePrerequisteWithTypeFlatten(prerequisitesElement);
                    if(migratedPrerequisiteElement != null)
                    {
                        xmlDocument.Root.Add(migratedPrerequisiteElement);
                    }
                    prerequisitesElement.Remove();
                }
                xmlVersion.FromString("0.2.1");
                Console.WriteLine($"[CM] [INFO] migrated to {xmlVersion.ToString()}.");
            }
            
            if (xmlVersion < ContractManager.version)
            {
                xmlVersion.UpdateTo(ContractManager.version);
                Console.WriteLine($"[CM] [INFO] migrated to latest version: {xmlVersion.ToString()} == {ContractManager.version.ToString()}.");
            }
            return true;
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
            if (this.uid.Length >= MissionBlueprint.uidMaxLength)
            {
                Console.WriteLine($"[CM] [WARNING] mission blueprint uid length should be less than {MissionBlueprint.uidMaxLength}.");
                return false;
            }
            if (this.title.Length >= MissionBlueprint.titleMaxLength)
            {
                Console.WriteLine($"[CM] [WARNING] mission blueprint title length should be less than {MissionBlueprint.titleMaxLength}.");
                return false;
            }
            if (this.synopsis.Length >= MissionBlueprint.synopsisMaxLength)
            {
                Console.WriteLine($"[CM] [WARNING] mission blueprint synopsis length should be less than {MissionBlueprint.synopsisMaxLength}.");
                return false;
            }
            if (this.description.Length >= MissionBlueprint.descriptionMaxLength)
            {
                Console.WriteLine($"[CM] [WARNING] mission blueprint description length should be less than {MissionBlueprint.descriptionMaxLength}.");
                return false;
            }

            if (!this.prerequisite.Validate()) { return false; }
            foreach (var action in actions)
            {
                if (!action.Validate()) { return false; }
            }

            return true;
        }
    }
}
