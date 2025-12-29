using ContractManager;
using System;
using System.Collections.Generic;
using System.Text;

namespace ContractManager.Contract
{
    internal static class ContractUtils
    {
        // Update tracked requirements based on the tracked state (updated through UpdateStateX functions)
        internal static void UpdateTrackedRequirements(List<TrackedRequirement> trackedRequirements)
        {
            // Call update of the requirement
            for (int trackedRequirementIndex = 0; trackedRequirementIndex < trackedRequirements.Count; trackedRequirementIndex++)
            {
                // Update the tracked requirements
                if (trackedRequirements[trackedRequirementIndex].status
                    is TrackedRequirementStatus.TRACKED
                    or TrackedRequirementStatus.MAINTAINED)
                {
                    trackedRequirements[trackedRequirementIndex].Update();
                }
                // Start first requirement (at all times).
                if (trackedRequirementIndex == 0 && trackedRequirements[trackedRequirementIndex].status == TrackedRequirementStatus.NOT_STARTED)
                {
                    trackedRequirements[trackedRequirementIndex].status = TrackedRequirementStatus.TRACKED;
                }
                // Start next requirement when previous maintained/achieved.
                if (
                    trackedRequirementIndex > 0 &&
                    trackedRequirements[trackedRequirementIndex].status == TrackedRequirementStatus.NOT_STARTED &&
                    trackedRequirements[trackedRequirementIndex - 1].status is TrackedRequirementStatus.MAINTAINED or TrackedRequirementStatus.ACHIEVED
                )
                {
                    trackedRequirements[trackedRequirementIndex].status = TrackedRequirementStatus.TRACKED;
                }
                if (
                    trackedRequirements[trackedRequirementIndex]._blueprintRequirement != null &&
                    trackedRequirements[trackedRequirementIndex]._blueprintRequirement.type == ContractBlueprint.RequirementType.Group)
                {
                    ContractUtils.UpdateTrackedRequirements(((TrackedGroup)trackedRequirements[trackedRequirementIndex]).trackedRequirements);
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
                    // Found matching blueprint requirement.
                    return contractBlueprint;
                }
            }
            return null;
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

        // Trigger action of the given type.
        internal static void TriggerAction(in List<ContractBlueprint.Action> actions, ContractBlueprint.Action.TriggerType triggerType) {
            foreach (ContractBlueprint.Action action in actions) {
                if (action.trigger == triggerType) {
                    // Do action
                }
            }
        }
    }
}
