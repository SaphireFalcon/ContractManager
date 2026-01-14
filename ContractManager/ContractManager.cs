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
    public static readonly Version version = new Version("0.2.2");

    // Internal fields
    private double _lastUpdateTime = 0.0d;
    private double _updateInterval = 1.0d; // once per second.
    public static ContractManagerData data = new ContractManagerData();
        
    internal static ActiveContractsWindow activeContractsWindow = new ActiveContractsWindow();
    internal static ContractManagementWindow contractManagementWindow = new ContractManagementWindow();

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

        this.LoadMissionBlueprints();

        // For testing: create and write an example contract to disk
        Generate.ExampleMission001();
    }

    [StarMapAfterGui]
    public void AfterGui(double dt)
    {
        // game loop
        
        KSA.SimTime simTime = Universe.GetElapsedSimTime();
        double playerTime = Program.GetPlayerTime();
        // Only update on the given interval.
        if (playerTime - this._lastUpdateTime > this._updateInterval)
        {
            this._lastUpdateTime = playerTime;
            this.UpdateMissions(simTime);
            this.UpdateContracts(simTime);
        }

        // Draw GUI
        Contract.Contract? contractToShowDetails = null;
        if (ContractManager.activeContractsWindow != null)
        {
            contractToShowDetails = ContractManager.activeContractsWindow.DrawActiveContractsWindow();
        }
        if (ContractManager.contractManagementWindow != null)
        {
            ContractManager.contractManagementWindow.DrawContractManagementWindow(contractToShowDetails);
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
    private void UpdateContracts(in KSA.SimTime simTime)
    {
        // offer contracts
        this.OfferContracts(simTime);
        // Update offered contracts (to be expired)
        foreach (Contract.Contract offeredContract in ContractManager.data.offeredContracts)
        {
            offeredContract.Update(simTime);
            // Rejected / Expired contracts don't need to be added to finished contracts.
        }
        // Cleanup offered contracts
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
                List<Contract.Contract> alreadyExistingAcceptedContracts = Contract.ContractUtils.FindContractFromBlueprintUID(
                    ContractManager.data.acceptedContracts,
                    contractBlueprintsToOffer[contractBlueprintIndex].uid);
                if (alreadyExistingAcceptedContracts.Count > 0)
                {
                    // already accepted, don't offer and auto-accept again
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
        string blueprintUID = contractBlueprint.uid;
        // Check if the contract is already being offered.
        {
            List<Contract.Contract> offeredContracts = data.offeredContracts.Where(c => c._contractBlueprint.uid == blueprintUID && c.status == Contract.ContractStatus.Offered).ToList();
            if (offeredContracts.Count > 0) { return false; }  // already offered!
        }

        // Check if the contract is part of a mission, only offer if the mission has been accepted
        if (!String.IsNullOrEmpty(contractBlueprint.missionBlueprintUID))
        {
            List<Mission.Mission> missions = Mission.MissionUtils.FindMissionsFromMissionBlueprintUID(data.acceptedMissions, contractBlueprint.missionBlueprintUID);
            if (missions.Count == 0) { return false; }
        }

        // TODO: Add a check if the contract was recently offered and rejected.
        ContractBlueprint.Prerequisite prerequisite = contractBlueprint.prerequisite;
        // Contract specific
        if (ContractManager.data.offeredContracts.Count >= contractBlueprint.prerequisite.maxNumOfferedContracts) { return false; }
        if (ContractManager.data.acceptedContracts.Count >= contractBlueprint.prerequisite.maxNumAcceptedContracts) { return false; }

        List<Contract.Contract> completedContracts = data.finishedContracts.Where(c => c._contractBlueprint.uid == blueprintUID && c.status == Contract.ContractStatus.Completed).ToList();
        if (completedContracts.Count > prerequisite.maxCompleteCount) { return false; }
        List<Contract.Contract> failedContracts = data.finishedContracts.Where(c => c._contractBlueprint.uid == blueprintUID && c.status == Contract.ContractStatus.Failed).ToList();
        if (failedContracts.Count > prerequisite.maxFailedCount) { return false; }
        List<Contract.Contract> acceptedContracts = data.acceptedContracts.Where(c => c._contractBlueprint.uid == blueprintUID && c.status == Contract.ContractStatus.Accepted).ToList();
        if (acceptedContracts.Count > prerequisite.maxConcurrentCount) { return false; }

        // Generic
        return CheckGenericPrerequisite(prerequisite);
    }

    private bool CheckGenericPrerequisite(in ContractBlueprint.Prerequisite prerequisite)
    {
        if (Universe.CurrentSystem != null)
        {
            if (Universe.CurrentSystem.VehicleCount < prerequisite.minNumberOfVessels) { return false; };
            if (Universe.CurrentSystem.VehicleCount > prerequisite.maxNumberOfVessels) { return false; };
        }
        if (!String.IsNullOrEmpty(prerequisite.hasCompletedContract))
        {
            string blueprintUID = prerequisite.hasCompletedContract;
            List<Contract.Contract> completedContracts = data.finishedContracts.Where(c => c._contractBlueprint.uid == blueprintUID && c.status == Contract.ContractStatus.Completed).ToList();
            if (completedContracts.Count == 0) { return false; }
        }
        if (!String.IsNullOrEmpty(prerequisite.hasFailedContract))
        {
            string blueprintUID = prerequisite.hasFailedContract;
            List<Contract.Contract> failedContracts = data.finishedContracts.Where(c => c._contractBlueprint.uid == blueprintUID && c.status == Contract.ContractStatus.Failed).ToList();
            if (failedContracts.Count == 0) { return false; }
        }
        if (!String.IsNullOrEmpty(prerequisite.hasAcceptedContract))
        {
            string blueprintUID = prerequisite.hasAcceptedContract;
            List<Contract.Contract> acceptedContracts = data.acceptedContracts.Where(c => c._contractBlueprint.uid == blueprintUID && c.status == Contract.ContractStatus.Accepted).ToList();
            if (acceptedContracts.Count == 0) { return false; }
        }
        if (!String.IsNullOrEmpty(prerequisite.hasCompletedMission))
        {
            string blueprintUID = prerequisite.hasCompletedMission;
            List<Mission.Mission> completedMissions = data.finishedMissions.Where(c => c._missionBlueprint.uid == blueprintUID && c.status == Mission.MissionStatus.Completed).ToList();
            if (completedMissions.Count == 0) { return false; }
        }
        if (!String.IsNullOrEmpty(prerequisite.hasFailedMission))
        {
            string blueprintUID = prerequisite.hasFailedMission;
            List<Mission.Mission> failedMissions = data.finishedMissions.Where(c => c._missionBlueprint.uid == blueprintUID && c.status == Mission.MissionStatus.Failed).ToList();
            if (failedMissions.Count == 0) { return false; }
        }
        if (!String.IsNullOrEmpty(prerequisite.hasAcceptedMission))
        {
            string blueprintUID = prerequisite.hasAcceptedMission;
            List<Mission.Mission> acceptedMissions = data.acceptedMissions.Where(c => c._missionBlueprint.uid == blueprintUID && c.status == Mission.MissionStatus.Accepted).ToList();
            if (acceptedMissions.Count == 0) { return false; }
        }
        return true;
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
                        ContractBlueprint.ContractBlueprint? blueprintContract = ContractBlueprint.ContractBlueprint.LoadFromFile(file);
                        if (blueprintContract == null)
                        {
                            Console.WriteLine($"[CM] [WARNING] blueprint '{file}' couldn't be loaded.");
                            continue;
                        }
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

    private void UpdateMissions(in KSA.SimTime simTime)
    {
        // offer missions
        this.OfferMissions(simTime);
        // Update offered missions (to be expired)
        foreach (Mission.Mission offeredMission in ContractManager.data.offeredMissions)
        {
            offeredMission.Update(simTime);
            // Rejected / Expired contracts don't need to be added to finished contracts.
        }
        // Cleanup offered missions
        for (int offeredMissionIndex = 0; offeredMissionIndex < ContractManager.data.offeredMissions.Count; offeredMissionIndex++)
        {
            if (ContractManager.data.offeredMissions[offeredMissionIndex].status != Mission.MissionStatus.Offered)
            {
                ContractManager.data.offeredMissions.RemoveAt(offeredMissionIndex);
                offeredMissionIndex--;
            }
        }
        // Update accepted missions
        foreach (Mission.Mission acceptedMission in ContractManager.data.acceptedMissions)
        {
            bool statusUpdated = acceptedMission.Update(simTime);
            if (statusUpdated)
            {
                // Check status and do something with it.
                if (acceptedMission.status is Mission.MissionStatus.Completed or Mission.MissionStatus.Failed)
                {
                    ContractManager.data.finishedMissions.Add(acceptedMission);
                }
            }
        } 
        // Cleanup accepted missions
        for (int acceptedMissionIndex = 0; acceptedMissionIndex < ContractManager.data.acceptedMissions.Count; acceptedMissionIndex++)
        {
            if (ContractManager.data.acceptedMissions[acceptedMissionIndex].status != Mission.MissionStatus.Accepted)
            {
                ContractManager.data.acceptedMissions.RemoveAt(acceptedMissionIndex);
                acceptedMissionIndex--;
            }
        }
    }

    private void OfferMissions(KSA.SimTime simTime)
    {
        if (ContractManager.data.offeredMissions.Count >= ContractManager.data.maxNumberOfOfferedMissions) { return; }

        List<Mission.MissionBlueprint> missionBlueprintsToOffer = this.GetMissionBlueprintsToOffer();
        
        // Auto-accept missions when told to do so.
        for ( int missionBlueprintIndex = 0; missionBlueprintIndex < missionBlueprintsToOffer.Count; missionBlueprintIndex++ ) {
            if (missionBlueprintsToOffer[missionBlueprintIndex].isAutoAccepted)
            {
                // TODO: make sure that this mission is not offered multiple times after completing.
                List<Mission.Mission> alreadyExistingAcceptedMissions = Mission.MissionUtils.FindMissionsFromMissionBlueprintUID(
                    ContractManager.data.acceptedMissions,
                    missionBlueprintsToOffer[missionBlueprintIndex].uid);
                if (alreadyExistingAcceptedMissions.Count > 0)
                {
                    // already accepted, don't offer and auto-accept again
                    missionBlueprintsToOffer.RemoveAt(missionBlueprintIndex);
                    missionBlueprintIndex--;
                }
                else
                {
                    // auto-accept
                    Mission.Mission mission = new Mission.Mission(missionBlueprintsToOffer[missionBlueprintIndex], simTime);
                    mission.AcceptOfferedMission(simTime);
                    ContractManager.data.acceptedMissions.Add(mission);
                    missionBlueprintsToOffer.RemoveAt(missionBlueprintIndex);
                    missionBlueprintIndex--;
                }
            }
        }

        // Randomly select a subset of missions to be offered
        Random randomGenerator = new Random();
        while (missionBlueprintsToOffer.Count + ContractManager.data.offeredMissions.Count > ContractManager.data.maxNumberOfOfferedMissions)
        {
            missionBlueprintsToOffer.RemoveAt(randomGenerator.Next(0, missionBlueprintsToOffer.Count));
        }
        foreach (Mission.MissionBlueprint missionBlueprint in missionBlueprintsToOffer)
        {
            ContractManager.data.offeredMissions.Add(new Mission.Mission(missionBlueprint, simTime));
        }
    }

    private List<Mission.MissionBlueprint> GetMissionBlueprintsToOffer()
    {
        List<Mission.MissionBlueprint> missionBlueprintsToOffer = new List<Mission.MissionBlueprint>();
        foreach (Mission.MissionBlueprint missionBlueprint in ContractManager.data.missionBlueprints)
        {
            if (this.CanOfferMissionFromBlueprint(in missionBlueprint))
            {
                missionBlueprintsToOffer.Add(missionBlueprint);
            }
        }
        return missionBlueprintsToOffer;
    }

    private bool CanOfferMissionFromBlueprint(in Mission.MissionBlueprint missionBlueprint)
    {
        // Check if the mission is already being offered.
        {
            string blueprintUID = missionBlueprint.uid;
            List<Mission.Mission> offeredMissions = data.offeredMissions.Where(c => c._missionBlueprint.uid == blueprintUID && c.status == Mission.MissionStatus.Offered).ToList();
            if (offeredMissions.Count > 0) { return false; }  // already offered!
        }
        bool canOfferContract = true;
        foreach (ContractBlueprint.Prerequisite prerequisite in missionBlueprint.prerequisites)
        {
            // Mision specific
            if (prerequisite.type == PrerequisiteType.MaxNumOfferedMissions && ContractManager.data.offeredMissions.Count >= prerequisite.maxNumOfferedMissions)
            {
                canOfferContract = false;
                break;
            }
            if (prerequisite.type == PrerequisiteType.MaxNumAcceptedMissions && ContractManager.data.acceptedMissions.Count >= prerequisite.maxNumAcceptedMissions)
            {
                canOfferContract = false;
                break;
            }
            if (prerequisite.type == PrerequisiteType.MaxCompleteCount)
            {
                string blueprintUID = missionBlueprint.uid;
                List<Mission.Mission> completedContracts = data.finishedMissions.Where(c => c._missionBlueprint.uid == blueprintUID && c.status == Mission.MissionStatus.Completed).ToList();
                if (completedContracts.Count > prerequisite.maxCompleteCount)
                {
                    canOfferContract = false;
                    break;
                }
            }
            if (prerequisite.type == PrerequisiteType.MaxFailedCount)
            {
                string blueprintUID = missionBlueprint.uid;
                List<Mission.Mission> failedContracts = data.finishedMissions.Where(c => c._missionBlueprint.uid == blueprintUID && c.status == Mission.MissionStatus.Failed).ToList();
                if (failedContracts.Count > prerequisite.maxFailedCount)
                {
                    canOfferContract = false;
                    break;
                }
            }
            if (prerequisite.type == PrerequisiteType.MaxConcurrentCount)
            {
                string blueprintUID = missionBlueprint.uid;
                List<Mission.Mission> acceptedContracts = data.acceptedMissions.Where(c => c._missionBlueprint.uid == blueprintUID && c.status == Mission.MissionStatus.Accepted).ToList();
                if (acceptedContracts.Count > prerequisite.maxConcurrentCount)
                {
                    canOfferContract = false;
                    break;
                }
            }

            // Generic
            canOfferContract = CheckGenericPrerequisite(prerequisite);
            if (!canOfferContract) break;
        }
        return canOfferContract;
    }

    private void LoadMissionBlueprints()
    {
        // Load missions from disk here
        const string contentDirectoryPath = @"Content";
        string[] contentDirectoryDirectories = Directory.GetDirectories(contentDirectoryPath);
        foreach (var contentSubDirectoryPath in contentDirectoryDirectories)
        {
            string missionsDirectoryPath = Path.Combine(contentSubDirectoryPath, @"missions");
            if (Directory.Exists(missionsDirectoryPath))
            {
                string[] files = Directory.GetFiles(missionsDirectoryPath, "*.xml", SearchOption.AllDirectories);
                foreach (string file in files)
                {
                    try
                    {
                        var blueprintMission = Mission.MissionBlueprint.LoadFromFile(file);
                        if (blueprintMission.Validate(ContractManager.data.contractBlueprints))
                        {
                            ContractManager.data.missionBlueprints.Add(blueprintMission);
                        }
                        else
                        {
                            Console.WriteLine($"[CM] [WARNING] mission blueprint '{blueprintMission.title}' won't be loaded do to validation error(s).");
                        }
                    }
                    catch (InvalidOperationException exception)
                    {
                        Console.WriteLine($"[CM] [WARNING] mission blueprint '{file}' won't be loaded do to loading error:\n{exception.Message}");
                    }
                }
            }
        }
        Console.WriteLine($"[CM] loaded {ContractManager.data.missionBlueprints.Count} mission blueprints.");
    }
}

}  // End of ContractManager namespace
