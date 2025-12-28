using Brutal.ImGuiApi;
using ContractManager.ContractBlueprint;
using KSA;
using StarMap.API;
using System;
using System.IO;
using System.Numerics;
using System.Xml.Serialization;
using System.Collections.Generic;

namespace ContractManager
{
[StarMapMod]
public class ContractManager
{
    // XML serializable fields
    // List of offered contracts, loaded from save game / file.
    [XmlElement("offeredContracts")]
    public List<Contract.Contract> offeredContracts {  get; set; } = new List<Contract.Contract>();

    // List of accepted contracts, loaded from save game / file.
    [XmlElement("acceptedContracts")]
    public List<Contract.Contract> acceptedContracts {  get; set; } = new List<Contract.Contract>();
        
    // List of finished contracts, loaded from save game / file.
    [XmlElement("finishedContracts")]
    public List<Contract.Contract> finishedContracts {  get; set; } = new List<Contract.Contract>();

    // Global ContractManager config of max number of contracts that can be offered simultaneously. Should be determined by the launch site management building.
    [XmlElement("maxNumberOfOfferedContracts")]
    public int maxNumberOfOfferedContracts { get; set; } = 2;
    
    // Global ContractManager config of max number of contracts that can be accepted simultaneously. Should be determined by the launch site management building.
    [XmlElement("maxNumberOfAcceptedContracts")]
    public int maxNumberOfAcceptedContracts { get; set; } = 1;

    // Internal fields
    private double _lastUpdateTime = 0.0d;
    private double _updateInterval = 5.0d;
    // List of all loaded contract blueprints
    private List<ContractBlueprint.ContractBlueprint> _contractBlueprints { get; set; } = new List<ContractBlueprint.ContractBlueprint>();
        
    private ActiveContractsWindow? _activeContractsWindow = null;
    private ContractManagementWindow? _contractManagementWindow = null;

    [StarMapImmediateLoad]
    public void onImmediateLoad(Mod definingMod)
    {
        Console.WriteLine("[CM] 'onImmediateLoad'");

        this._contractManagementWindow = new ContractManagementWindow(this.offeredContracts, this.acceptedContracts, this.finishedContracts);
        this._activeContractsWindow = new ActiveContractsWindow(this.acceptedContracts);
    }

    [StarMapAllModsLoaded]
    public void OnAllModsLoaded()
    {
        Console.WriteLine("[CM] 'OnAllModsLoaded'");

        // Load contracts from disk here
        var blueprintContract1 = ContractBlueprint.ContractBlueprint.LoadFromFile("Content/ContractManager/contracts/example_contract_002.xml");
        blueprintContract1.WriteToConsole();
        this._contractBlueprints.Add(blueprintContract1);

        // For testing: create and write an example contract to disk
        //Generate.Example002Contract();
    }

    [StarMapAfterGui]
    public void AfterGui(double dt)
    {
        // Access the controlled vehicle, needed for periapsis/apoapsis checks etc.
        KSA.Vehicle? currentVehicle = Program.ControlledVehicle;
        double playerTime = Program.GetPlayerTime();

        // Update contracts
        this.UpdateContracts();

        // Draw GUI
        Contract.Contract? contractToShowDetails = null;
        if (this._activeContractsWindow != null)
        {
            contractToShowDetails = this._activeContractsWindow.DrawActiveContractsWindow();
        }
        if (this._contractManagementWindow != null)
        {
            this._contractManagementWindow.DrawContractManagementWindow(contractToShowDetails);
        }
    }

    // Contract Management back-end functions
    private void UpdateContracts()
    {
        double playerTime = Program.GetPlayerTime();
        if (playerTime - this._lastUpdateTime < this._updateInterval) { return; }

        this._lastUpdateTime = playerTime;
        Console.WriteLine($"[CM] Game time: {playerTime}s blueprints {this._contractBlueprints.Count} offered: {this.offeredContracts.Count} accepted: {this.acceptedContracts.Count} finished: {this.finishedContracts.Count}");

        // offer contracts
        this.OfferContracts(playerTime);

        // Update accepted contracts
        KSA.Vehicle currentVehicle = Program.ControlledVehicle;
        foreach (Contract.Contract acceptedContract in this.acceptedContracts)
        {
            if (currentVehicle != null)
            {
                acceptedContract.UpdateStateWithVehicle(currentVehicle);
            }
            bool statusUpdated = acceptedContract.Update(playerTime);
            if (statusUpdated)
            {
                // Check status and do something with it.
                if (acceptedContract.status == Contract.ContractStatus.Completed)
                {
                    finishedContracts.Add(acceptedContract);
                }
            }
        } 
        // Cleanup accepted contracts
        for (int acceptedContractIndex = 0; acceptedContractIndex < acceptedContracts.Count; acceptedContractIndex++)
        {
            if (this.acceptedContracts[acceptedContractIndex].status != Contract.ContractStatus.Accepted)
            {
                this.acceptedContracts.RemoveAt(acceptedContractIndex);
                acceptedContractIndex--;
            }
        }
    }

    private void OfferContracts(double playerTime)
    {
        if (this.offeredContracts.Count >= this.maxNumberOfOfferedContracts) { return; }

        List<ContractBlueprint.ContractBlueprint> contractBlueprintsToOffer = this.GetContractBlueprintsToOffer();
        Random randomGenerator = new Random();
        while (contractBlueprintsToOffer.Count + this.offeredContracts.Count > this.maxNumberOfOfferedContracts)
        {
            // Randomly select a subset of contracts to be offered
            contractBlueprintsToOffer.RemoveAt(randomGenerator.Next(0, contractBlueprintsToOffer.Count));
        }
        foreach (ContractBlueprint.ContractBlueprint contractBlueprint in contractBlueprintsToOffer)
        {
            this.offeredContracts.Add(new Contract.Contract(contractBlueprint, playerTime));
        }
    }
        
    private List<ContractBlueprint.ContractBlueprint> GetContractBlueprintsToOffer()
    {
        List<ContractBlueprint.ContractBlueprint> contractBlueprintsToOffer = new List<ContractBlueprint.ContractBlueprint>();
        foreach (ContractBlueprint.ContractBlueprint contractBlueprint in this._contractBlueprints)
        {
            if (this.CanOfferContractFromBlueprint(in contractBlueprint))
            {
                contractBlueprintsToOffer.Add(contractBlueprint);
            }
        }
        return contractBlueprintsToOffer;
    }

    private bool CanOfferContractFromBlueprint(in ContractBlueprint.ContractBlueprint contractBlueprint)
    {
        bool canOfferContract = true;
        foreach (ContractBlueprint.Prerequisite prerequisite in contractBlueprint.prerequisites)
        {
            if (prerequisite.type == PrerequisiteType.MaxNumOfferedContracts && this.offeredContracts.Count >= prerequisite.maxNumOfferedContracts)
            {
                canOfferContract = false;
                break;
            }
            if (prerequisite.type == PrerequisiteType.MaxNumAcceptedContracts && this.acceptedContracts.Count >= prerequisite.maxNumAcceptedContracts)
            {
                canOfferContract = false;
                break;
            }
            // TODO: Add a check if the contract was recently offered and rejected.
        }
        return canOfferContract;
    }
}

}  // End of ContractManager namespace
