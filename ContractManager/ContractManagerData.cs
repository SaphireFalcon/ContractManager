using KSA;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace ContractManager
{
    public class ContractManagerData
    {
        // XML serializable fields
        // The version for which the data was saved.
        [XmlElement("version")]
        public string version { get; set; } = ContractManager.version.ToString();

        // List of offered contracts, loaded from save game / file.
        [XmlArray("offeredContracts")]
        public List<Contract.Contract> offeredContracts { get; set; } = new List<Contract.Contract>();

        // List of accepted contracts, loaded from save game / file.
        [XmlArray("acceptedContracts")]
        public List<Contract.Contract> acceptedContracts { get; set; } = new List<Contract.Contract>();
        
        // List of finished contracts, loaded from save game / file.
        [XmlArray("finishedContracts")]
        public List<Contract.Contract> finishedContracts { get; set; } = new List<Contract.Contract>();

        // List of offered missions, loaded from save game / file.
        [XmlArray("offeredMissions")]
        public List<Mission.Mission> offeredMissions { get; set; } = new List<Mission.Mission>();

        // List of accepted missions, loaded from save game / file.
        [XmlArray("acceptedMissions")]
        public List<Mission.Mission> acceptedMissions { get; set; } = new List<Mission.Mission>();
        
        // List of finished missions, loaded from save game / file.
        [XmlArray("finishedMissions")]
        public List<Mission.Mission> finishedMissions { get; set; } = new List<Mission.Mission>();
        
        // Global ContractManager config of max number of contracts that can be offered simultaneously. Should be determined by the management building at the launch site.
        [XmlElement("maxNumberOfOfferedContracts")]
        public int maxNumberOfOfferedContracts { get; set; } = 4;
    
        // Global ContractManager config of max number of contracts that can be accepted simultaneously. Should be determined by the management building at the launch site.
        [XmlElement("maxNumberOfAcceptedContracts")]
        public int maxNumberOfAcceptedContracts { get; set; } = 2;

        // Global ContractManager config of max number of mission that can be offered simultaneously. Should be determined by the management building at the launch site.
        [XmlElement("maxNumberOfOfferedMissions")]
        public int maxNumberOfOfferedMissions { get; set; } = 4;
    
        // Global ContractManager config of max number of mission that can be accepted simultaneously. Should be determined by the management building at the launch site.
        [XmlElement("maxNumberOfAcceptedMissions")]
        public int maxNumberOfAcceptedMissions { get; set; } = 2;

        // internal variables
        // List of all loaded contract blueprints
        internal List<ContractBlueprint.ContractBlueprint> contractBlueprints { get; set; } = new List<ContractBlueprint.ContractBlueprint>();

        // List of all loaded mission blueprints
        internal List<Mission.MissionBlueprint> missionBlueprints { get; set; } = new List<Mission.MissionBlueprint>();

        // List of popup(s) to show
        internal List<GUI.PopupWindow> popupWindows { get; set; } = new List<GUI.PopupWindow>();

        internal GUI.PopupWindow? FindPopupWindowFromUID(string uid)
        {
            foreach (GUI.PopupWindow popupWindow in this.popupWindows)
            {
                if (popupWindow.uid == uid) { return popupWindow; }
            }
            return null;
        }

        // Load data from save path.
        public bool LoadFrom(string savePath)
        {
            Console.WriteLine("[CM] ContractManager.LoadFrom(uncompressedSave)");
            string filePathContractManagerXmlFile = Path.Combine(savePath, "contractmanager.xml");
            if (!File.Exists(filePathContractManagerXmlFile)) { return false; }

            XDocument? xmlDocument = null;
            using (var reader = new System.IO.StreamReader(filePathContractManagerXmlFile))
            {
                xmlDocument = XDocument.Load(reader);
            }

            if (!this.Migrate(ref xmlDocument, filePathContractManagerXmlFile))
            {
                Console.WriteLine("[CM] ContractManager.LoadFrom migrating failed.");
                return false;
            }
            // Overwrite the save file?

            XmlSerializer serializer = new XmlSerializer(typeof(ContractManagerData));
            ContractManagerData? contractManagerData = (ContractManagerData)serializer.Deserialize(new StringReader(xmlDocument.ToString()));
            if (contractManagerData == null)
            {
                Console.WriteLine("[CM] ContractManager.LoadFrom can't deserialize from migrated data.");
                return false;
            }
            // Need to deep copy because the data loaded by the stream reader will be destroyed.
            // TODO: Is this still true when using string reader?
            this.offeredContracts.Clear();
            foreach (Contract.Contract contract in contractManagerData.offeredContracts)
            {
                Contract.Contract? clonedContract = contract.Clone(this.contractBlueprints);
                if (clonedContract != null)
                {
                    this.offeredContracts.Add(clonedContract);
                }
            }
            this.acceptedContracts.Clear();
            foreach (Contract.Contract contract in contractManagerData.acceptedContracts)
            {
                Contract.Contract? clonedContract = contract.Clone(this.contractBlueprints);
                if (clonedContract != null)
                {
                    this.acceptedContracts.Add(clonedContract);
                }
            }
            this.finishedContracts.Clear();
            foreach (Contract.Contract contract in contractManagerData.finishedContracts)
            {
                Contract.Contract? clonedContract = contract.Clone(this.contractBlueprints);
                if (clonedContract != null)
                {
                    this.finishedContracts.Add(clonedContract);
                }
            }
            
            this.offeredMissions.Clear();
            foreach (Mission.Mission mission in contractManagerData.offeredMissions)
            {
                Mission.Mission? clonedMission = mission.Clone(this.missionBlueprints);
                if (clonedMission != null)
                {
                    this.offeredMissions.Add(clonedMission);
                }
            }
            this.acceptedMissions.Clear();
            foreach (Mission.Mission mission in contractManagerData.acceptedMissions)
            {
                Mission.Mission? clonedMission = mission.Clone(this.missionBlueprints);
                if (clonedMission != null)
                {
                    this.acceptedMissions.Add(clonedMission);
                }
            }
            this.finishedMissions.Clear();
            foreach (Mission.Mission mission in contractManagerData.finishedMissions)
            {
                Mission.Mission? clonedMission = mission.Clone(this.missionBlueprints);
                if (clonedMission != null)
                {
                    this.finishedMissions.Add(clonedMission);
                }
            }

            this.maxNumberOfOfferedContracts = contractManagerData.maxNumberOfOfferedContracts;
            this.maxNumberOfAcceptedContracts = contractManagerData.maxNumberOfAcceptedContracts;
            this.maxNumberOfOfferedMissions = contractManagerData.maxNumberOfOfferedMissions;
            this.maxNumberOfAcceptedMissions = contractManagerData.maxNumberOfAcceptedMissions;
            return true;
        }

        // Write data to save file
        public void WriteTo(DirectoryInfo directory)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(ContractManagerData));
            XmlHelper.SerializeWithoutNaN(serializer, this, Path.Combine(directory.FullName, "contractmanager.xml"));
        }

        private bool Migrate(ref XDocument xmlDocument, string filePathContractManagerXmlFile)
        {
            bool migratedFile = false;
            Version loadedXMLVersion = new Version(xmlDocument);
            if (!loadedXMLVersion.valid) { return false; }
            if (ContractManager.version < loadedXMLVersion)
            {
                Console.WriteLine($"[CM] [INFO] Mod version {ContractManager.version.ToString()} is older than loaded Version {loadedXMLVersion.ToString()}'.");
                return false;
            }
            Console.WriteLine($"[CM] [INFO] Running Migration.");
            if (xmlDocument.Root == null) { return false; }
            Version xmlVersion = new Version(loadedXMLVersion);
            if (!this.MigrateContractMissionAsList(ref xmlDocument, ref xmlVersion, ref migratedFile)) { return false; }
            if (!this.MigrateContractMissionUID(ref xmlDocument, ref xmlVersion, ref migratedFile)) { return false; }
            
            if (xmlVersion < ContractManager.version)
            {
                xmlVersion.UpdateTo(ContractManager.version);
                Console.WriteLine($"[CM] [INFO] migrated to latest version: {xmlVersion.ToString()}.");
            }
            xmlDocument.Root.SetElementValue("version", xmlVersion.ToString());
            if (migratedFile)
            {
                string backupPath = filePathContractManagerXmlFile.Substring(0, filePathContractManagerXmlFile.Length - 3);
                backupPath += String.Format("v{0}_{1}_{2}", loadedXMLVersion.major, loadedXMLVersion.minor, loadedXMLVersion.patch);
                try
                {
                    File.Move(filePathContractManagerXmlFile, backupPath);
                    Console.WriteLine($"[CM] [INFO] backup to '{backupPath}'");
                }
                catch { }

                xmlDocument.Save(filePathContractManagerXmlFile);
            }
            return true;
        }

        private bool MigrateContractMissionAsList(ref XDocument xmlDocument, ref Version xmlVersion, ref bool migratedFile)
        {
            if (xmlVersion < "0.2.3")
            {
                XElement newOfferedContracts = new XElement("offeredContracts", null);
                foreach (XElement offeredContractsElement in xmlDocument.Root.Elements("offeredContracts"))
                {
                    XElement migratedOfferedContract = new XElement(offeredContractsElement);
                    migratedOfferedContract.Name = "Contract";
                    newOfferedContracts.Add(migratedOfferedContract);
                }
                xmlDocument.Root.Elements("offeredContracts").Remove();
                xmlDocument.Root.Add(newOfferedContracts);

                XElement newAcceptedContracts = new XElement("acceptedContracts", null);
                foreach (XElement acceptedContractsElement in xmlDocument.Root.Elements("acceptedContracts"))
                {
                    XElement migratedAcceptedContract = new XElement(acceptedContractsElement);
                    migratedAcceptedContract.Name = "Contract";
                    newAcceptedContracts.Add(migratedAcceptedContract);
                }
                xmlDocument.Root.Elements("acceptedContracts").Remove();
                xmlDocument.Root.Add(newAcceptedContracts);

                XElement newFinishedContracts = new XElement("finishedContracts", null);
                foreach (XElement finishedContractsElement in xmlDocument.Root.Elements("finishedContracts"))
                {
                    XElement migratedFinishedContract = new XElement(finishedContractsElement);
                    migratedFinishedContract.Name = "Contract";
                    newFinishedContracts.Add(migratedFinishedContract);
                }
                xmlDocument.Root.Elements("finishedContracts").Remove();
                xmlDocument.Root.Add(newFinishedContracts);

                XElement newOfferedMissions = new XElement("offeredMissions", null);
                foreach (XElement offeredMissionsElement in xmlDocument.Root.Elements("offeredMissions"))
                {
                    XElement migratedOfferedContract = new XElement(offeredMissionsElement);
                    migratedOfferedContract.Name = "Mission";
                    newOfferedMissions.Add(migratedOfferedContract);
                }
                xmlDocument.Root.Elements("offeredMissions").Remove();
                xmlDocument.Root.Add(newOfferedMissions);

                XElement newAcceptedMissions = new XElement("acceptedMissions", null);
                foreach (XElement acceptedMissionsElement in xmlDocument.Root.Elements("acceptedMissions"))
                {
                    XElement migratedAcceptedContract = new XElement(acceptedMissionsElement);
                    migratedAcceptedContract.Name = "Mission";
                    newAcceptedMissions.Add(migratedAcceptedContract);
                }
                xmlDocument.Root.Elements("acceptedMissions").Remove();
                xmlDocument.Root.Add(newAcceptedMissions);

                XElement newFinishedMissions = new XElement("finishedMissions", null);
                foreach (XElement finishedMissionsElement in xmlDocument.Root.Elements("finishedMissions"))
                {
                    XElement migratedFinishedContract = new XElement(finishedMissionsElement);
                    migratedFinishedContract.Name = "Mission";
                    newFinishedMissions.Add(migratedFinishedContract);
                }
                xmlDocument.Root.Elements("finishedMissions").Remove();
                xmlDocument.Root.Add(newFinishedMissions);

                xmlVersion.FromString("0.2.3");
                migratedFile = true;
                Console.WriteLine($"[CM] [INFO] migrated to {xmlVersion.ToString()}.");
            }
            return true;
        }

        private bool MigrateContractMissionUID(ref XDocument xmlDocument, ref Version xmlVersion, ref bool migratedFile)
        {
            if (xmlVersion < "0.2.4")
            {
                XElement? offeredContractsElement = xmlDocument.Root.Element("offeredContracts");
                if (offeredContractsElement != null)
                {
                    foreach (XElement contractElement in offeredContractsElement.Elements("Contract"))
                    {
                        XElement? uidElement = contractElement.Element("contractUID");
                        if (uidElement != null)
                        {
                            uidElement.Name = "uid";
                        }
                    }
                }

                XElement? acceptedContractsElement = xmlDocument.Root.Element("acceptedContracts");
                if (acceptedContractsElement != null)
                {
                    foreach (XElement contractElement in acceptedContractsElement.Elements("Contract"))
                    {
                        XElement? uidElement = contractElement.Element("contractUID");
                        if (uidElement != null)
                        {
                            uidElement.Name = "uid";
                        }
                    }
                }

                XElement? finishedContractsElement = xmlDocument.Root.Element("finishedContracts");
                if (finishedContractsElement != null)
                {
                    foreach (XElement contractElement in finishedContractsElement.Elements("Contract"))
                    {
                        XElement? uidElement = contractElement.Element("contractUID");
                        if (uidElement != null)
                        {
                            uidElement.Name = "uid";
                        }
                    }
                }
                
                XElement? offeredMissionsElement = xmlDocument.Root.Element("offeredMissions");
                if (offeredMissionsElement != null)
                {
                    foreach (XElement missionElement in offeredMissionsElement.Elements("Mission"))
                    {
                        XElement? uidElement = missionElement.Element("missionUID");
                        if (uidElement != null)
                        {
                            uidElement.Name = "uid";
                        }
                    }
                }

                XElement? acceptedMissionsElement = xmlDocument.Root.Element("acceptedMissions");
                if (acceptedMissionsElement != null)
                {
                    foreach (XElement missionElement in acceptedMissionsElement.Elements("Mission"))
                    {
                        XElement? uidElement = missionElement.Element("missionUID");
                        if (uidElement != null)
                        {
                            uidElement.Name = "uid";
                        }
                    }
                }

                XElement? finishedMissionsElement = xmlDocument.Root.Element("finishedMissions");
                if (finishedMissionsElement != null)
                {
                    foreach (XElement missionElement in finishedMissionsElement.Elements("Mission"))
                    {
                        XElement? uidElement = missionElement.Element("missionUID");
                        if (uidElement != null)
                        {
                            uidElement.Name = "uid";
                        }
                    }
                }

                xmlVersion.FromString("0.2.4");
                migratedFile = true;
                Console.WriteLine($"[CM] [INFO] migrated to {xmlVersion.ToString()}.");
            }
            return true;
        }
    }
}
