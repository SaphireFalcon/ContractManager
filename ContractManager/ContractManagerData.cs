using KSA;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml.Serialization;

namespace ContractManager
{
    public class ContractManagerData
    {
        // XML serializable fields
        // The version for which the data was saved.
        [XmlElement("version")]
        public string version { get; set; } = ContractManager.version;

        // List of offered contracts, loaded from save game / file.
        [XmlElement("offeredContracts")]
        public List<Contract.Contract> offeredContracts { get; set; } = new List<Contract.Contract>();

        // List of accepted contracts, loaded from save game / file.
        [XmlElement("acceptedContracts")]
        public List<Contract.Contract> acceptedContracts { get; set; } = new List<Contract.Contract>();
        
        // List of finished contracts, loaded from save game / file.
        [XmlElement("finishedContracts")]
        public List<Contract.Contract> finishedContracts { get; set; } = new List<Contract.Contract>();

        // Global ContractManager config of max number of contracts that can be offered simultaneously. Should be determined by the launch site management building.
        [XmlElement("maxNumberOfOfferedContracts")]
        public int maxNumberOfOfferedContracts { get; set; } = 4;
    
        // Global ContractManager config of max number of contracts that can be accepted simultaneously. Should be determined by the launch site management building.
        [XmlElement("maxNumberOfAcceptedContracts")]
        public int maxNumberOfAcceptedContracts { get; set; } = 2;
        
        // internal variables
        // List of all loaded contract blueprints
        internal List<ContractBlueprint.ContractBlueprint> contractBlueprints { get; set; } = new List<ContractBlueprint.ContractBlueprint>();

        // List of popup(s) to show
        internal List<GUI.PopupWindow> popupWindows { get; set; } = new List<GUI.PopupWindow>();

        private static XmlSerializer _contractManagerDataXmlSerializer = new XmlSerializer(typeof(ContractManagerData));

        // Load data from save path.
        public void LoadFrom(string savePath)
        {
            Console.WriteLine("[CM] ContractManager.LoadFrom(uncompressedSave)");
            string filePathContractManagerXmlFile = Path.Combine(savePath, "contractmanager.xml");
            if (!File.Exists(filePathContractManagerXmlFile))
            {
                throw new NullReferenceException("contractmanager file '" + filePathContractManagerXmlFile + "' does not exist");
            }

            StreamReader streamReader = new StreamReader(filePathContractManagerXmlFile);
            if (!(_contractManagerDataXmlSerializer.Deserialize(streamReader) is ContractManagerData contractManagerData))
            {
                streamReader.Close();
                throw new NullReferenceException("contract manager data is null");
            }
            // Need to deep copy because the data loaded by the stream reader will be destroyed.
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
            this.maxNumberOfOfferedContracts = contractManagerData.maxNumberOfOfferedContracts;
            this.maxNumberOfAcceptedContracts = contractManagerData.maxNumberOfAcceptedContracts;

            streamReader.Close();
        }

        // Write data to save file
        public void WriteTo(DirectoryInfo directory)
        {
            Console.WriteLine("[CM] ContractManager.WriteTo(directory)");
            XmlHelper.SerializeWithoutNaN(_contractManagerDataXmlSerializer, this, Path.Combine(directory.FullName, "contractmanager.xml"));
        }
    }
}
