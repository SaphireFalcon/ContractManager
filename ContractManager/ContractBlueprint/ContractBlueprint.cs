using ContractManager.Mission;
using KSA;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace ContractManager.ContractBlueprint
{
    public enum CompletionCondition
    {
        [XmlEnum("all")]
        All,
        [XmlEnum("any")]
        Any
    }

    [XmlRoot("Contract")]    
    public class ContractBlueprint
    {
        // Details of the contract
        // The version for which the contract was created.
        [XmlElement("version", DataType = "string")]
        public string version { get; set; } = ContractManager.version.ToString();

        // The unique identifier for the contract
        [XmlElement("uid", DataType = "string")]
        public string uid { get; set; }
        internal static int uidMaxLength = 128;

        // Mission unique identifier if the blueprint is linked to a mission.
        [XmlElement("missionBlueprintUID", DataType = "string")]
        public string missionBlueprintUID { get; set; } = string.Empty;
        internal static int missionBlueprintUIDMaxLength = 128;

        // The title of the contract
        [XmlElement("title", DataType = "string")]
        public string title { get; set; }
        internal static int titleMaxLength = 128;

        // A brief synopsis of the contract
        [XmlElement("synopsis", DataType = "string")]
        public string synopsis { get; set; } = string.Empty;
        internal static int synopsisMaxLength = 1024;

        // Detailed description of the contract
        [XmlElement("description", DataType = "string")]
        public string description { get; set; } = string.Empty;
        internal static int descriptionMaxLength = 4096;

        // When an offered contract will expired, in seconds
        [XmlElement("expiration", DataType = "double")]
        public double expiration { get; set; } = double.PositiveInfinity;  // Never expires

        // Flag if an offered contract from this blueprint can be rejected.
        [XmlElement("isRejectable", DataType = "boolean")]
        public bool isRejectable { get; set; } = true;

        // When an accepted contract will fail, in seconds
        [XmlElement("deadline", DataType = "double")]
        public double deadline { get; set; } = double.PositiveInfinity;  // No deadline

        // Flag if an offered contract is automatically accepted.
        [XmlElement("isAutoAccepted", DataType = "boolean")]
        public bool isAutoAccepted { get; set; } = false;

        // [DEPRECIATED v0.2.1] List of prerequisites for the contract
        //[XmlArray("prerequisites")]
        //public List<Prerequisite> prerequisites { get; set; } = new List<Prerequisite>();
        
        // [v0.2.1] Prerequisite to offer contract
        [XmlElement("Prerequisite")]
        public Prerequisite prerequisite { get; set; } = new Prerequisite();

        // Completion condition of the contract based on the requirements.
        [XmlElement("completionCondition")]
        public CompletionCondition completionCondition { get; set; } = CompletionCondition.All;

        // List of requirements for the contract
        [XmlArray("requirements")]
        public List<Requirement> requirements { get; set; } = new List<Requirement>();

        // List of actions for the contract
        [XmlArray("actions")]
        public List<Action> actions { get; set; } = new List<Action>();

        // internal flag to indicate if the blueprint can be edited or not
        internal bool isEditable { get; set; } = false;

        public ContractBlueprint() { }

        //  Doesn't write anything to console in-game, only on StarMap launcher console.
        internal void WriteToConsole()
        {
            Console.WriteLine($"Contract Blueprint:");
            Console.WriteLine($"  UID: {uid}");
            Console.WriteLine($"  Title: {title}");
            Console.WriteLine($"  Synopsis: {synopsis}");
            Console.WriteLine($"  Description: {description}");
            Console.WriteLine($"  Prerequisiste:");
            prerequisite.WriteToConsole();
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
            XmlHelper.SerializeWithoutNaN(serializer, this, filePath);
        }

        // Load a contract blueprint from an XML file.
        internal static ContractBlueprint? LoadFromFile(string filePath)
        {
            XDocument? xmlDocument = null;
            using (var reader = new System.IO.StreamReader(filePath))
            {
                xmlDocument = XDocument.Load(reader);
            }
            if (ContractBlueprint.Migrate(ref xmlDocument, filePath))
            {
                XmlSerializer serializer = new XmlSerializer(typeof(ContractBlueprint));
                return (ContractBlueprint)serializer.Deserialize(new StringReader(xmlDocument.ToString()));
            }
            return null;
        }

        internal bool Validate()
        {
            // Validate the contract blueprint.
            // The title can't be empty
            if (String.IsNullOrEmpty(this.title))
            {
                Console.WriteLine("[CM] [WARNING] contract blueprint title has be to be defined.");
                return false;
            }
            // The uid can't be empty
            if (String.IsNullOrEmpty(this.uid))
            {
                Console.WriteLine("[CM] [WARNING] contract blueprint uid has be to be defined.");
                return false;
            }
            // FIXME: It should have at least one prerequisiteElement to know when to offer a contract from the contract 
            // It should have at least one requirement to know when to the contract should be completed.
            if (this.requirements.Count == 0)
            {
                Console.WriteLine($"[CM] [WARNING] contract blueprint '{this.title}' has no prerequisites.");
                return false;
            }
            if (this.uid.Length >= ContractBlueprint.uidMaxLength)
            {
                Console.WriteLine($"[CM] [WARNING] contract blueprint uid should be less than {ContractBlueprint.uidMaxLength} in length");
                return false;
            }
            if (this.title.Length >= ContractBlueprint.titleMaxLength)
            {
                Console.WriteLine($"[CM] [WARNING] contract blueprint title should be less than {ContractBlueprint.titleMaxLength} in length");
                return false;
            }
            if (this.synopsis.Length >= ContractBlueprint.synopsisMaxLength)
            {
                Console.WriteLine($"[CM] [WARNING] contract blueprint synopsis should be less than {ContractBlueprint.synopsisMaxLength} in length");
                return false;
            }
            if (this.description.Length >= ContractBlueprint.descriptionMaxLength)
            {
                Console.WriteLine($"[CM] [WARNING] contract blueprint description should be less than {ContractBlueprint.descriptionMaxLength} in length");
                return false;
            }

            prerequisite.Validate();

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

        private static bool Migrate(ref XDocument xmlDocument, string filePath)
        {
            bool migratedFile = false;
            Version loadedXMLVersion = new Version(xmlDocument);
            if (!loadedXMLVersion.valid) { return false; }
            if (ContractManager.version < loadedXMLVersion)
            {
                Console.WriteLine($"[CM] [INFO] Mod version {ContractManager.version.ToString()} is older than loaded Version {loadedXMLVersion.ToString()}' for '{filePath}'.");
                return false;
            }

            Console.WriteLine($"[CM] [INFO] Running Migration.");
            if (xmlDocument.Root == null) { return false; }
            Version xmlVersion = new Version(loadedXMLVersion);
            
            if (!ContractBlueprint.MigratePrerequisteWithTypeFlatten(ref xmlDocument, ref xmlVersion, ref migratedFile)) { return false; }
            if (!ContractBlueprint.MigrateAddActionUID(ref xmlDocument, ref xmlVersion, ref migratedFile)) { return false; }

            if (xmlVersion < ContractManager.version)
            {
                xmlVersion.UpdateTo(ContractManager.version);
                Console.WriteLine($"[CM] [INFO] migrated to latest version: {xmlVersion.ToString()}.");
            }
            xmlDocument.Root.SetElementValue("version", xmlVersion.ToString());
            if (migratedFile)
            {
                // Write to disk
                string modFolderContractPath = Path.GetDirectoryName(filePath);
                string contentDirectoryPath = Path.GetFullPath(@"Content");
                if (modFolderContractPath.StartsWith(contentDirectoryPath))
                {
                    // This has to be true, because contracts are loaded from Content/[mod]/contracts
                    modFolderContractPath = modFolderContractPath.Substring(contentDirectoryPath.Length);
                }
                string contractsVersionExportFolderPath = Path.Combine(
                    KSA.Constants.DocumentsFolderPath,
                    "migration",
                    String.Format("version_{0}_{1}_{2}", xmlVersion.major, xmlVersion.minor, xmlVersion.patch),
                    modFolderContractPath
                );
                try
                {
                    Directory.CreateDirectory(contractsVersionExportFolderPath);
                }
                catch { }  // silently catch any error.
                if (Directory.Exists(contractsVersionExportFolderPath))
                {
                    string migratedContractExportPath = Path.Combine(contractsVersionExportFolderPath, Path.GetFileName(filePath));
                    Console.WriteLine($"[CM] [INFO] export migrated contract to: {migratedContractExportPath}.");
                    xmlDocument.Save(migratedContractExportPath);
                    // Create/add to popup
                    GUI.PopupWindow popupWindow = ContractManager.data.FindPopupWindowFromUID("migration");
                    if (popupWindow == null)
                    {
                        ContractManager.data.popupWindows.Add(new GUI.PopupWindow
                        {
                            uid = "migration",
                            title = "Migrated files exported to disk.",
                            popupType = GUI.PopupType.Popup,
                            messageToShow = $"Contract Manager found old files and has migrated these, please move them into the respective mod/game folders:\n'{migratedContractExportPath}'",
                        }
                        );
                    }
                    else
                    {
                        popupWindow.messageToShow += $"\n'{migratedContractExportPath}'";
                    }
                }
            }
            return true;
        }

        private static bool MigratePrerequisteWithTypeFlatten(ref XDocument xmlDocument, ref Version xmlVersion, ref bool migratedFile)
        {
            if (xmlVersion < "0.2.1")
            {
                // version 0.2.1 flattens prerequisiteElement
                XElement? prerequisitesElement = xmlDocument.Root.Element("prerequisites");
                if (prerequisitesElement != null )
                {
                    XElement? migratedPrerequisiteElement = Prerequisite.MigratePrerequisteWithTypeFlatten(prerequisitesElement);
                    if(migratedPrerequisiteElement != null)
                    {
                        xmlDocument.Root.Add(migratedPrerequisiteElement);
                    }
                    prerequisitesElement.Remove();
                }
                xmlVersion.FromString("0.2.1");
                migratedFile = true;
                Console.WriteLine($"[CM] [INFO] migrated to {xmlVersion.ToString()}.");
            }
            return true;
        }

        private static bool MigrateAddActionUID(ref XDocument xmlDocument, ref Version xmlVersion, ref bool migratedFile)
        {
            if (xmlVersion < "0.2.2")
            {
                // version 0.2.2 adds uid to Action
                XElement? actionsElement = xmlDocument.Root.Element("actions");
                XElement? uidElement = xmlDocument.Root.Element("uid");
                if (actionsElement != null && uidElement != null )
                {
                    Action.MigrateAddUID(ref actionsElement, uidElement.Value);
                }
                xmlVersion.FromString("0.2.2");
                migratedFile = true;
                Console.WriteLine($"[CM] [INFO] migrated to {xmlVersion.ToString()}.");
            }
            return true;
        }
    }
}
