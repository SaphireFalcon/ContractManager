using ContractManager;
using System;
using System.Collections.Generic;
using System.Text;

namespace ContractManager.Contract
{
    internal static class ContractUtils
    {
        // Update tracked requirements based on the tracked state (updated through UpdateStateX functions)
        internal static void UpdateTrackedRequirements(List<TrackedRequirement> trackedRequirements, in Contract contract)
        {
            // Call update of the requirement
            for (int trackedRequirementIndex = 0; trackedRequirementIndex < trackedRequirements.Count; trackedRequirementIndex++)
            {
                // Update the tracked requirements
                TrackedRequirement trackedRequirement = trackedRequirements[trackedRequirementIndex];
                if (trackedRequirement.status is TrackedRequirementStatus.TRACKED or TrackedRequirementStatus.MAINTAINED)
                {
                    trackedRequirement.Update();
                }
                // Start first requirement (at all times).
                if (trackedRequirementIndex == 0 && trackedRequirement.status == TrackedRequirementStatus.NOT_STARTED)
                {
                    trackedRequirement.status = TrackedRequirementStatus.TRACKED;
                    ContractUtils.TriggerAction(contract, ContractBlueprint.TriggerType.OnRequirementTracked, trackedRequirement);
                }
                // Start next requirement when previous maintained/achieved.
                if (
                    trackedRequirementIndex > 0 &&
                    trackedRequirement.status == TrackedRequirementStatus.NOT_STARTED &&
                    trackedRequirements[trackedRequirementIndex - 1].status is TrackedRequirementStatus.MAINTAINED or TrackedRequirementStatus.ACHIEVED
                )
                {
                    trackedRequirement.status = TrackedRequirementStatus.TRACKED;
                    ContractUtils.TriggerAction(contract, ContractBlueprint.TriggerType.OnRequirementTracked, trackedRequirement);
                }
                if (
                    trackedRequirement._blueprintRequirement != null &&
                    trackedRequirement._blueprintRequirement.type == ContractBlueprint.RequirementType.Group)
                {
                    ContractUtils.UpdateTrackedRequirements(((TrackedGroup)trackedRequirement).trackedRequirements, contract);
                }
            }
        }
        
        // Update the tracked state of a tracked requirement using the vehicle data.
        internal static void UpdateStateWithVehicle(in KSA.Vehicle vehicle, List<TrackedRequirement> trackedRequirements) {
            foreach (TrackedRequirement trackedRequirement in trackedRequirements)
            {
                if (!( trackedRequirement.status is TrackedRequirementStatus.NOT_STARTED or TrackedRequirementStatus.ACHIEVED or TrackedRequirementStatus.FAILED ))
                {
                    //Utils.UpdateStateWithVehicle(in vehicle, trackedRequirement.trackedRequirements); -> call for group type
                    trackedRequirement.UpdateStateWithVehicle(in vehicle);
                }
            }
        }

        // Check the status of all tracked requirements and get worst status.
        internal static TrackedRequirementStatus GetWorstTrackedRequirementStatus(List<TrackedRequirement> trackedRequirements)
        {
            TrackedRequirementStatus worstRequirementStatus = TrackedRequirementStatus.ACHIEVED;
            foreach (TrackedRequirement trackedRequirement in trackedRequirements)
            {
                // Check group childs
                if (
                    trackedRequirement._blueprintRequirement != null &&
                    trackedRequirement._blueprintRequirement.type == ContractBlueprint.RequirementType.Group &&
                    ((TrackedGroup)trackedRequirement).trackedRequirements.Count > 0)
                {
                    TrackedRequirementStatus worstChildStatus = ContractUtils.GetWorstTrackedRequirementStatus(((TrackedGroup)trackedRequirement).trackedRequirements);
                    if (worstChildStatus is TrackedRequirementStatus.MAINTAINED)
                    {
                        // set all the childs to achieved
                        foreach (TrackedRequirement trackedChildRequirement in ((TrackedGroup)trackedRequirement).trackedRequirements)
                        {
                            if (trackedChildRequirement.status == TrackedRequirementStatus.MAINTAINED)
                            {
                                trackedChildRequirement.status = TrackedRequirementStatus.ACHIEVED;
                                ContractUtils.TriggerAction(trackedRequirement._contract, ContractBlueprint.TriggerType.OnRequirementAchieved, trackedRequirement);
                            }
                        }
                    }
                    if (worstChildStatus < worstRequirementStatus)
                    {
                        worstRequirementStatus = worstChildStatus;
                    }
                }
                // Check tracked requirement
                if (trackedRequirement.status < worstRequirementStatus)
                {
                    worstRequirementStatus = trackedRequirement.status;
                }
            }
            return worstRequirementStatus;
        }

        internal static ContractBlueprint.ContractBlueprint? FindContractBlueprintFromUID(List<ContractBlueprint.ContractBlueprint> contractBlueprints, string blueprintUID)
        {
            bool foundBlueprint = false;
            foreach (ContractBlueprint.ContractBlueprint contractBlueprint in contractBlueprints)
            {
                foundBlueprint = contractBlueprint.uid == blueprintUID;
                if (foundBlueprint)
                {
                    // Found matching contract blueprint.
                    return contractBlueprint;
                }
            }
            return null;
        }

        internal static List<Contract> FindContractFromBlueprintUID(List<Contract> contracts, string blueprintUID)
        {
            List<Contract> foundContracts = new List<Contract>();
            foreach (Contract contract in contracts)
            {
                if (contract._contractBlueprint.uid == blueprintUID)
                {
                    // Found matching contract.
                    foundContracts.Add(contract);
                }
            }
            return foundContracts;
        }

        internal static Contract? FindContractFromContractUID(List<Contract> contracts, string contractUID)
        {
            bool foundContract = false;
            foreach (Contract contract in contracts)
            {
                foundContract = contract.uid == contractUID;
                if (foundContract)
                {
                    // Found matching contract.
                    return contract;
                }
            }
            return null;
        }

        internal static Contract? FindContractFromContractUID(string contractUID)
        {
            Contract? contract = null;
            contract = ContractUtils.FindContractFromContractUID(ContractManager.data.offeredContracts, contractUID);
            if (contract != null) { return contract; }
            contract = ContractUtils.FindContractFromContractUID(ContractManager.data.acceptedContracts, contractUID);
            if (contract != null) { return contract; }
            contract = ContractUtils.FindContractFromContractUID(ContractManager.data.finishedContracts, contractUID);
            return contract;
        }

        internal static ContractBlueprint.Requirement? FindRequirementFromUID(List<ContractBlueprint.Requirement> blueprintRequirements, string requirementUID)
        {
            bool foundBlueprintRequirement = false;
            foreach (ContractBlueprint.Requirement blueprintRequirement in blueprintRequirements)
            {
                foundBlueprintRequirement = blueprintRequirement.uid == requirementUID;
                if (foundBlueprintRequirement)
                {
                    // Found matching blueprint requirement.
                    return blueprintRequirement;
                }
            }
            return null;
        }

        internal static ContractBlueprint.Action? FindActionFromUID(List<ContractBlueprint.Action> blueprintActions, string prerequisiteUID)
        {
            bool foundBlueprintAction = false;
            foreach (ContractBlueprint.Action blueprintAction in blueprintActions)
            {
                foundBlueprintAction = blueprintAction.uid == prerequisiteUID;
                if (foundBlueprintAction)
                {
                    // Found matching blueprint requirement.
                    return blueprintAction;
                }
            }
            return null;
        }

        // Trigger action of the given type.
        internal static void TriggerAction(
            Contract contract,
            ContractBlueprint.TriggerType triggerType,
            TrackedRequirement? trackedRequirement = null
        )
        {
            if (contract._contractBlueprint == null) {  return; }
            foreach (ContractBlueprint.Action action in contract._contractBlueprint.actions) {
                if (action.trigger == triggerType) {
                    if (trackedRequirement != null &&
                        (
                            triggerType is
                            ContractBlueprint.TriggerType.OnRequirementTracked or
                            ContractBlueprint.TriggerType.OnRequirementMaintained or
                            ContractBlueprint.TriggerType.OnRequirementReverted or
                            ContractBlueprint.TriggerType.OnRequirementAchieved or
                            ContractBlueprint.TriggerType.OnRequirementFailed
                        ) &&
                        trackedRequirement.requirementUID != action.onRequirement
                    )
                    {
                        continue;
                    }
                    action.DoAction(contract);
                }
            }
        }
    }
}
