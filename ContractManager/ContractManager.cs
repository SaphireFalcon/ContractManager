using Brutal.ImGuiApi;
using ContractManager.ContractBlueprint;
using ContractManager.Patches;
using KSA;
using StarMap.API;
using System;
using System.IO;
using System.Numerics;
using System.Xml.Serialization;
using System.Collections.Generic;
using ContractManager.GUI;

namespace ContractManager
{
[StarMapMod]
public class ContractManager
{
    // Version to be used across the code.
    public static readonly string version = "0.1.0";

    // Internal fields
    private double _lastUpdateTime = 0.0d;
    private double _updateInterval = 1.0d; // once per second.
    public static ContractManagerData data = new ContractManagerData();
        
    private ActiveContractsWindow _activeContractsWindow = new ActiveContractsWindow();
    private ContractManagementWindow _contractManagementWindow = new ContractManagementWindow();

    [StarMapImmediateLoad]
    public void onImmediateLoad(Mod definingMod)
    {
        Console.WriteLine("[CM] 'onImmediateLoad'");
    }

    [StarMapAllModsLoaded]
    public void OnFullyLoaded()
    {
        Console.WriteLine("[CM] OnFullyLoaded");
        Patches.UniverseDataPatchWriteTo.Patch();
        Patches.UncompressedSavePatchLoad.Patch();
    }

    [StarMapUnload]
    public void Unload()
    {
        Patches.UniverseDataPatchWriteTo.Unload();
        Patches.UncompressedSavePatchLoad.Unload();
    }

    [StarMapAllModsLoaded]
    public void OnAllModsLoaded()
    {
        Console.WriteLine("[CM] 'OnAllModsLoaded'");

        this.LoadContractBlueprints();

        // For testing: create and write an example contract to disk
        //Generate.Example002Contract();
    }

    [StarMapAfterGui]
    public void AfterGui(double dt)
    {
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
        for (int popupIndex = 0; popupIndex < ContractManager.data.popupWindows.Count; popupIndex++)
        {
            if (ContractManager.data.popupWindows[popupIndex].drawPopup)
            {
                ContractManager.data.popupWindows[popupIndex].DrawPopup();
            }
            else
            {
                ContractManager.data.popupWindows.RemoveAt(popupIndex);
                popupIndex--;
            }
        }
    }

    // Contract Management back-end functions
    private void UpdateContracts()
    {
        KSA.SimTime simTime = Universe.GetElapsedSimTime();
        double playerTime = Program.GetPlayerTime();
        // Only update on the given interval.
        if (playerTime - this._lastUpdateTime < this._updateInterval) { return; }

        this._lastUpdateTime = playerTime;

        // offer contracts
        this.OfferContracts(simTime);
        // Update offered contracts (to be expired)
        foreach (Contract.Contract offeredContract in ContractManager.data.offeredContracts)
        {
            offeredContract.Update(simTime);
            // Rejected / Expired contracts don't need to be added to finished contracts.
        }
        // Cleanup accepted contracts
        for (int offeredContractIndex = 0; offeredContractIndex < ContractManager.data.offeredContracts.Count; offeredContractIndex++)
        {
            if (ContractManager.data.offeredContracts[offeredContractIndex].status != Contract.ContractStatus.Offered)
            {
                ContractManager.data.offeredContracts.RemoveAt(offeredContractIndex);
                offeredContractIndex--;
            }
        }

        // Update accepted contracts
        // Access the controlled vehicle, needed for periapsis/apoapsis checks etc.
        KSA.Vehicle? currentVehicle = Program.ControlledVehicle;
        foreach (Contract.Contract acceptedContract in ContractManager.data.acceptedContracts)
        {
            if (currentVehicle != null)
            {
                acceptedContract.UpdateStateWithVehicle(currentVehicle);
            }
            bool statusUpdated = acceptedContract.Update(simTime);
            if (statusUpdated)
            {
                // Check status and do something with it.
                if (acceptedContract.status is Contract.ContractStatus.Completed or Contract.ContractStatus.Failed)
                {
                    ContractManager.data.finishedContracts.Add(acceptedContract);
                }
            }
        } 
        // Cleanup accepted contracts
        for (int acceptedContractIndex = 0; acceptedContractIndex < ContractManager.data.acceptedContracts.Count; acceptedContractIndex++)
        {
            if (ContractManager.data.acceptedContracts[acceptedContractIndex].status != Contract.ContractStatus.Accepted)
            {
                ContractManager.data.acceptedContracts.RemoveAt(acceptedContractIndex);
                acceptedContractIndex--;
            }
        }
    }

    private void OfferContracts(KSA.SimTime simTime)
    {
        if (ContractManager.data.offeredContracts.Count >= ContractManager.data.maxNumberOfOfferedContracts) { return; }

        List<ContractBlueprint.ContractBlueprint> contractBlueprintsToOffer = this.GetContractBlueprintsToOffer();

        // Auto-accept contracts when told to do so.
        for ( int contractBlueprintIndex = 0; contractBlueprintIndex < contractBlueprintsToOffer.Count; contractBlueprintIndex++ ) {
            if (contractBlueprintsToOffer[contractBlueprintIndex].isAutoAccepted)
            {
                // TODO: make sure that this contract is not offered multiple times after completing.
                Contract.Contract? alreadyExistingAcceptedContract = Contract.ContractUtils.FindContractFromUID(
                    ContractManager.data.acceptedContracts,
                    contractBlueprintsToOffer[contractBlueprintIndex].uid);
                if (alreadyExistingAcceptedContract != null)
                {
                    // already accepted, don't offer and autoaccept again
                    contractBlueprintsToOffer.RemoveAt(contractBlueprintIndex);
                    contractBlueprintIndex--;
                }
                else
                {
                    // auto-accept
                    Contract.Contract contract = new Contract.Contract(contractBlueprintsToOffer[contractBlueprintIndex], simTime);
                    contract.AcceptOfferedContract(simTime);
                    ContractManager.data.acceptedContracts.Add(contract);
                    contractBlueprintsToOffer.RemoveAt(contractBlueprintIndex);
                    contractBlueprintIndex--;
                }
            }
        }

        // Randomly select a subset of contracts to be offered
        Random randomGenerator = new Random();
        while (contractBlueprintsToOffer.Count + ContractManager.data.offeredContracts.Count > ContractManager.data.maxNumberOfOfferedContracts)
        {
            contractBlueprintsToOffer.RemoveAt(randomGenerator.Next(0, contractBlueprintsToOffer.Count));
        }
        foreach (ContractBlueprint.ContractBlueprint contractBlueprint in contractBlueprintsToOffer)
        {
            ContractManager.data.offeredContracts.Add(new Contract.Contract(contractBlueprint, simTime));
        }
    }
        
    private List<ContractBlueprint.ContractBlueprint> GetContractBlueprintsToOffer()
    {
        List<ContractBlueprint.ContractBlueprint> contractBlueprintsToOffer = new List<ContractBlueprint.ContractBlueprint>();
        foreach (ContractBlueprint.ContractBlueprint contractBlueprint in ContractManager.data.contractBlueprints)
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
        {
            string blueprintUID = contractBlueprint.uid;
            List<Contract.Contract> offeredContracts = data.offeredContracts.Where(c => c._contractBlueprint.uid == blueprintUID && c.status == Contract.ContractStatus.Offered).ToList();
            if (offeredContracts.Count > 0) { return false; }  // already offered!
        }
        bool canOfferContract = true;
        foreach (ContractBlueprint.Prerequisite prerequisite in contractBlueprint.prerequisites)
        {
            if (prerequisite.type == PrerequisiteType.MaxNumOfferedContracts && ContractManager.data.offeredContracts.Count >= prerequisite.maxNumOfferedContracts)
            {
                canOfferContract = false;
                break;
            }
            if (prerequisite.type == PrerequisiteType.MaxNumAcceptedContracts && ContractManager.data.acceptedContracts.Count >= prerequisite.maxNumAcceptedContracts)
            {
                canOfferContract = false;
                break;
            }
            if (prerequisite.type == PrerequisiteType.MaxCompleteCount)
            {
                string blueprintUID = contractBlueprint.uid;
                List<Contract.Contract> completedContracts = data.finishedContracts.Where(c => c._contractBlueprint.uid == blueprintUID && c.status == Contract.ContractStatus.Completed).ToList();
                if (completedContracts.Count > prerequisite.maxCompleteCount)
                {
                    canOfferContract = false;
                    break;
                }
            }
            if (prerequisite.type == PrerequisiteType.MaxFailedCount)
            {
                string blueprintUID = contractBlueprint.uid;
                List<Contract.Contract> failedContracts = data.finishedContracts.Where(c => c._contractBlueprint.uid == blueprintUID && c.status == Contract.ContractStatus.Failed).ToList();
                if (failedContracts.Count > prerequisite.maxFailedCount)
                {
                    canOfferContract = false;
                    break;
                }
            }
            if (prerequisite.type == PrerequisiteType.MaxConcurrentCount)
            {
                string blueprintUID = contractBlueprint.uid;
                List<Contract.Contract> acceptedContracts = data.acceptedContracts.Where(c => c._contractBlueprint.uid == blueprintUID && c.status == Contract.ContractStatus.Accepted).ToList();
                if (acceptedContracts.Count > prerequisite.maxConcurrentCount)
                {
                    canOfferContract = false;
                    break;
                }
            }
            if (prerequisite.type == PrerequisiteType.HasCompletedContract)
            {
                string blueprintUID = prerequisite.hasCompletedContract;
                List<Contract.Contract> completedContracts = data.finishedContracts.Where(c => c._contractBlueprint.uid == blueprintUID && c.status == Contract.ContractStatus.Completed).ToList();
                if (completedContracts.Count == 0)
                {
                    canOfferContract = false;
                    break;
                }
            }
            if (prerequisite.type == PrerequisiteType.HasFailedContract)
            {

                string blueprintUID = prerequisite.hasFailedContract;
                List<Contract.Contract> failedContracts = data.finishedContracts.Where(c => c._contractBlueprint.uid == blueprintUID && c.status == Contract.ContractStatus.Failed).ToList();
                if (failedContracts.Count == 0)
                {
                    canOfferContract = false;
                    break;
                }
            }
            if (prerequisite.type == PrerequisiteType.HasAcceptedContract)
            {
                string blueprintUID = prerequisite.hasAcceptedContract;
                List<Contract.Contract> acceptedContracts = data.acceptedContracts.Where(c => c._contractBlueprint.uid == blueprintUID && c.status == Contract.ContractStatus.Accepted).ToList();
                if (acceptedContracts.Count == 0)
                {
                    canOfferContract = false;
                    break;
                }
            }
            // TODO: Add a check if the contract was recently offered and rejected.
        }
        return canOfferContract;
    }

    private void LoadContractBlueprints()
    {
        // Load contracts from disk here
        const string contentDirectoryPath = @"Content";
        string[] contentDirectoryDirectories = Directory.GetDirectories(contentDirectoryPath);
        foreach (var contentSubDirectoryPath in contentDirectoryDirectories)
        {
            string contractsDirectoryPath = Path.Combine(contentSubDirectoryPath, @"contracts");
            if (Directory.Exists(contractsDirectoryPath))
            {
                string[] files = Directory.GetFiles(contractsDirectoryPath, "*.xml", SearchOption.AllDirectories);
                foreach (string file in files)
                {
                    try
                    {
                        var blueprintContract = ContractBlueprint.ContractBlueprint.LoadFromFile(file);
                        if (blueprintContract.Validate())
                        {
                            ContractManager.data.contractBlueprints.Add(blueprintContract);
                        }
                        else
                        {
                            Console.WriteLine($"[CM] [WARNING] blueprint '{blueprintContract.title}' won't be loaded do to validation error(s).");
                        }
                    }
                    catch (InvalidOperationException exception)
                    {
                        Console.WriteLine($"[CM] [WARNING] blueprint '{file}' won't be loaded do to loading error:\n{exception.Message}");
                    }
                }
            }
        }
        Console.WriteLine($"[CM] loaded {ContractManager.data.contractBlueprints.Count} contract blueprints.");
    }
}

}  // End of ContractManager namespace
