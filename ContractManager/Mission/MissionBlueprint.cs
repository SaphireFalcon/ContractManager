using ContractManager.ContractBlueprint;
using KSA;
using System.Collections.Generic;
using System.Text;
using System.Xml;
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
        
        // [DEPRECIATED v0.4.0] List of contract blueprints for the mission are only linked from contractblueprint to the mission blueprint.
        // List of contract blueprints UIDs for the mission
        //[XmlArray("contractBlueprintUIDs")]
        //public List<string> contractBlueprintUIDs { get; set; } = new List<string>();

        // internal flag to indicate if the blueprint can be edited or not
        internal bool isEditable { get; set; } = false;

        // internal field to store the file path from which the blueprint is loaded, used for editing and saving the blueprint back to the same file.
        internal string loadedFromFilePath { get; set; } = string.Empty;

        public MissionBlueprint() { }
        
        // Write the mission blueprint to an XML file.
        internal void WriteToFile(string filePath)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(MissionBlueprint));
            XmlHelper.SerializeWithoutNaN(serializer, this, filePath);
        }

        // Load a mission blueprint from an XML file.
        internal static MissionBlueprint? LoadFromFile(string filePath)
        {
            XDocument? xmlDocument = null;
            using (var reader = new System.IO.StreamReader(filePath))
            {
                xmlDocument = XDocument.Load(reader);
            }
            if (MissionBlueprint.Migrate(ref xmlDocument, filePath))
            {
                XmlSerializer serializer = new XmlSerializer(typeof(MissionBlueprint));
                MissionBlueprint? missionBlueprint = (MissionBlueprint)serializer.Deserialize(new StringReader(xmlDocument.ToString()));
                if (missionBlueprint != null)
                {
                    missionBlueprint.loadedFromFilePath = filePath;
                }
                return missionBlueprint;
            }
            return null;
        }

        internal MissionBlueprint Clone(List<ContractBlueprint.ContractBlueprint> contractBlueprints)
        {
            MissionBlueprint clonedMissionBlueprint = (MissionBlueprint)this.MemberwiseClone();
            clonedMissionBlueprint.isEditable = true;  // blueprint created from savegame should be editable by default.
            clonedMissionBlueprint.prerequisite = this.prerequisite.Clone();
            foreach (ContractBlueprint.Action action in this.actions)
            {
                clonedMissionBlueprint.actions.Add(action.Clone());
            }
            return clonedMissionBlueprint;
        }

        internal bool Validate(List<ContractBlueprint.ContractBlueprint> contractBlueprints, bool logWarnings = true)
        {
            // Validate the contract blueprint.
            // The title can't be empty
            if (String.IsNullOrEmpty(this.title))
            {
                if (logWarnings)
                {
                    Console.WriteLine("[CM] [WARNING] mission blueprint title has be to be defined.");
                }
                return false;
            }
            // The uid can't be empty
            if (String.IsNullOrEmpty(this.uid))
            {
                if (logWarnings)
                {
                    Console.WriteLine("[CM] [WARNING] mission blueprint uid has be to be defined.");
                }
                return false;
            }
            // It should have at least one contract linked to the mission.
            int countLinkedContractBlueprints = 0;
            foreach (ContractBlueprint.ContractBlueprint contractBlueprint in contractBlueprints)
            {
                if (contractBlueprint.missionBlueprintUID == this.uid)
                {
                    countLinkedContractBlueprints++;
                }
            }
            if (countLinkedContractBlueprints == 0)
            {
                
                if (logWarnings)
                {
                    Console.WriteLine($"[CM] [WARNING] mission blueprint '{this.title}' has no contracts.");
                }
                return false;
            }
            if (this.uid.Length >= MissionBlueprint.uidMaxLength)
            {
                
                if (logWarnings)
                {
                    Console.WriteLine($"[CM] [WARNING] mission blueprint uid length should be less than {MissionBlueprint.uidMaxLength}.");
                }
                return false;
            }
            if (this.title.Length >= MissionBlueprint.titleMaxLength)
            {
                
                if (logWarnings)
                {
                    Console.WriteLine($"[CM] [WARNING] mission blueprint title length should be less than {MissionBlueprint.titleMaxLength}.");
                }
                return false;
            }
            if (this.synopsis.Length >= MissionBlueprint.synopsisMaxLength)
            {
                
                if (logWarnings)
                {
                    Console.WriteLine($"[CM] [WARNING] mission blueprint synopsis length should be less than {MissionBlueprint.synopsisMaxLength}.");
                }
                return false;
            }
            if (this.description.Length >= MissionBlueprint.descriptionMaxLength)
            {
                
                if (logWarnings)
                {
                    Console.WriteLine($"[CM] [WARNING] mission blueprint description length should be less than {MissionBlueprint.descriptionMaxLength}.");
                }
                return false;
            }

            if (!this.prerequisite.Validate(logWarnings)) { return false; }
            foreach (var action in actions)
            {
                if (!action.Validate(logWarnings)) { return false; }
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

            if (xmlDocument.Root == null) { return false; }
            Version xmlVersion = new Version(loadedXMLVersion);
            
            if (!MissionBlueprint.MigratePrerequisteWithTypeFlatten(ref xmlDocument, ref xmlVersion, ref migratedFile)) { return false; }
            if (!MissionBlueprint.MigrateAddActionUID(ref xmlDocument, ref xmlVersion, ref migratedFile)) { return false; }
            if (!MissionBlueprint.MigrateRemoveContractBlueprintUIDList(ref xmlDocument, ref xmlVersion, ref migratedFile)) { return false; }
                        
            if (xmlVersion < ContractManager.version)
            {
                xmlVersion.UpdateTo(ContractManager.version);
            }
            xmlDocument.Root.SetElementValue("version", xmlVersion.ToString());
            if (migratedFile)
            {
                Console.WriteLine($"[CM] [INFO] migrated '{filePath}' to latest version: {xmlVersion.ToString()}.");
                // Write backup to disk before overwriting the original file.
                string modFolderContractPath = Path.GetDirectoryName(filePath);
                string missionsOriginalBackupFilePath = Path.Combine(
                    Path.GetDirectoryName(modFolderContractPath), // mod folder
                    "migration",
                    String.Format("version_{0}_{1}_{2}", loadedXMLVersion.major, loadedXMLVersion.minor, loadedXMLVersion.patch),
                    "missions",
                    Path.GetFileName(filePath)
                );
                try
                {
                    Directory.CreateDirectory(Path.GetDirectoryName(missionsOriginalBackupFilePath));
                }
                catch { }  // silently catch any error.
                if (Directory.Exists(Path.GetDirectoryName(missionsOriginalBackupFilePath)))
                {
                    using (var reader = new System.IO.StreamReader(filePath))
                    {
                        XDocument? originalXmlDocument = XDocument.Load(reader);
                        if (originalXmlDocument != null) {
                            Console.WriteLine($"[CM] [INFO] backup mission to '{missionsOriginalBackupFilePath}'.");
                            originalXmlDocument.Save(missionsOriginalBackupFilePath);
                        }
                    }
                }

                // Write to disk
                Console.WriteLine($"[CM] [INFO] migrated mission '{filePath}'.");
                xmlDocument.Save(filePath);
                // Create/add to popup
                GUI.PopupWindow popupWindow = ContractManager.data.FindPopupWindowFromUID("migration");
                if (popupWindow == null)
                {
                    ContractManager.data.popupWindows.Add(new GUI.PopupWindow
                    {
                        uid = "migration",
                        title = "Migrated files exported to disk.",
                        popupType = GUI.PopupType.Popup,
                        messageToShow = $"Contract Manager found old version of files and has migrated them. Original files can be found in the 'migration' folder.\nMigrated file(s):\n'{filePath}'",
                    }
                    );
                }
                else
                {
                    popupWindow.messageToShow += $"\n'{filePath}'";
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
                    XElement? migratedPrerequisiteElement = ContractBlueprint.Prerequisite.MigratePrerequisteWithTypeFlatten(prerequisitesElement);
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
                    ContractBlueprint.Action.MigrateAddUID(ref actionsElement, uidElement.Value);
                }
                xmlVersion.FromString("0.2.2");
                migratedFile = true;
                Console.WriteLine($"[CM] [INFO] migrated to {xmlVersion.ToString()}.");
            }
            return true;
        }

        private static bool MigrateRemoveContractBlueprintUIDList(ref XDocument xmlDocument, ref Version xmlVersion, ref bool migratedFile)
        {
            if (xmlVersion < "0.4.0")
            {
                // version 0.4.0 removes contractBlueprintUIDs
                XElement? contractBlueprintUIDsElement = xmlDocument.Root.Element("contractBlueprintUIDs");
                if (contractBlueprintUIDsElement != null)
                {
                    contractBlueprintUIDsElement.Remove();
                }
                xmlVersion.FromString("0.4.0");
                migratedFile = true;
                Console.WriteLine($"[CM] [INFO] migrated to {xmlVersion.ToString()}.");
            }
            return true;
        }
    }
}
