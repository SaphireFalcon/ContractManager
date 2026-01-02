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
    private double _updateInterval = 5.0d;
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
        double playerTime = Program.GetPlayerTime();
        // Only update on the given interval.
        if (playerTime - this._lastUpdateTime < this._updateInterval) { return; }

        this._lastUpdateTime = playerTime;
        Console.WriteLine($"[CM] Game time: {playerTime}s blueprints {ContractManager.data.contractBlueprints.Count} offered: {ContractManager.data.offeredContracts.Count} accepted: {ContractManager.data.acceptedContracts.Count} finished: {ContractManager.data.finishedContracts.Count}");

        // offer contracts
        this.OfferContracts(playerTime);

        // Update accepted contracts
        // Access the controlled vehicle, needed for periapsis/apoapsis checks etc.
        KSA.Vehicle? currentVehicle = Program.ControlledVehicle;
        foreach (Contract.Contract acceptedContract in ContractManager.data.acceptedContracts)
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

    private void OfferContracts(double playerTime)
    {
        if (ContractManager.data.offeredContracts.Count >= ContractManager.data.maxNumberOfOfferedContracts) { return; }

        List<ContractBlueprint.ContractBlueprint> contractBlueprintsToOffer = this.GetContractBlueprintsToOffer();
        Random randomGenerator = new Random();
        while (contractBlueprintsToOffer.Count + ContractManager.data.offeredContracts.Count > ContractManager.data.maxNumberOfOfferedContracts)
        {
            // Randomly select a subset of contracts to be offered
            contractBlueprintsToOffer.RemoveAt(randomGenerator.Next(0, contractBlueprintsToOffer.Count));
        }
        foreach (ContractBlueprint.ContractBlueprint contractBlueprint in contractBlueprintsToOffer)
        {
            ContractManager.data.offeredContracts.Add(new Contract.Contract(contractBlueprint, playerTime));
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
