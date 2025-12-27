using ContractManager;
using System;
using System.Collections.Generic;
using System.Text;

namespace ContractManager.Contract
{
    public static class Utils
    {
        // Update tracked requirements based on the tracked state (updated through UpdateStateX functions)
        public static void UpdateTrackedRequirements(List<TrackedRequirement> trackedRequirements)
        {
            // Call update of the requirement
            // FIXME: make for-loop and start a tracked requirement if it can be started
            //foreach (TrackedRequirement trackedRequirement in trackedRequirements)
            for (int trackedRequirementIndex = 0; trackedRequirementIndex < trackedRequirements.Count; trackedRequirementIndex++)
            {
                // Update the tracked requirements
                if (!(
                    trackedRequirements[trackedRequirementIndex].status
                    is TrackedRequirementStatus.NOT_STARTED
                    or TrackedRequirementStatus.ACHIEVED
                    or TrackedRequirementStatus.FAILED
                ))
                {
                    trackedRequirements[trackedRequirementIndex].Update();
                }
                // Start first requirement (at all times).
                if (trackedRequirementIndex == 0 && trackedRequirements[trackedRequirementIndex].status == TrackedRequirementStatus.NOT_STARTED)
                {
                    
                    Console.WriteLine($"[CM] Utils.UpdateTrackedRequirements() '{trackedRequirements[trackedRequirementIndex].requirementUID}' -> TRACKED");
                    trackedRequirements[trackedRequirementIndex].status = TrackedRequirementStatus.TRACKED;
                }
                // Start next requirement when previous maintained/achieved.
                if (
                    trackedRequirementIndex > 0 &&
                    trackedRequirements[trackedRequirementIndex].status == TrackedRequirementStatus.NOT_STARTED &&
                    trackedRequirements[trackedRequirementIndex - 1].status is TrackedRequirementStatus.MAINTAINED or TrackedRequirementStatus.ACHIEVED
                )
                {
                    Console.WriteLine($"[CM] Utils.UpdateTrackedRequirements() '{trackedRequirements[trackedRequirementIndex].requirementUID}' -> TRACKED");
                    trackedRequirements[trackedRequirementIndex].status = TrackedRequirementStatus.TRACKED;
                }
            }
        }
        
        // Update the tracked state of a tracked requirement using the vehicle data.
        public static void UpdateStateWithVehicle(in KSA.Vehicle vehicle, List<TrackedRequirement> trackedRequirements) {
            foreach (TrackedRequirement trackedRequirement in trackedRequirements)
            {
                // Console.WriteLine($"[CM] Utils.UpdateStateWithVehicle() trackedRequirement: {trackedRequirement.requirementUID} {trackedRequirement.status}");
                if (!( trackedRequirement.status is TrackedRequirementStatus.NOT_STARTED or TrackedRequirementStatus.ACHIEVED or TrackedRequirementStatus.FAILED ))
                {
                    //Utils.UpdateStateWithVehicle(in vehicle, trackedRequirement.trackedRequirements); -> call for group type
                    trackedRequirement.UpdateStateWithVehicle(in vehicle);
                }
            }
        }

        // Check the status of all tracked requirements and get worse status.
        public static TrackedRequirementStatus GetWorstTrackedRequirementStatus(List<TrackedRequirement> trackedRequirements)
        {
            TrackedRequirementStatus worstRequirementStatus = TrackedRequirementStatus.ACHIEVED;
            foreach (TrackedRequirement trackedRequirement in trackedRequirements)
            {
                // Check childs
                // TODO: if (trackedRequirement.type == RequirementType.Group)
                if (trackedRequirement.trackedRequirements.Count > 0) {
                    TrackedRequirementStatus worstChildStatus = Utils.GetWorstTrackedRequirementStatus(trackedRequirement.trackedRequirements);
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
            Console.WriteLine($"[CM] Utils.GetWorstTrackedRequirementStatus() worstRequirementStatus: {worstRequirementStatus}");
            return worstRequirementStatus;
        }

        // Trigger action of the given type.
        public static void TriggerAction(in List<ContractBlueprint.Action> actions, ContractBlueprint.Action.TriggerType triggerType) {
            foreach (ContractBlueprint.Action action in actions) {
                if (action.trigger == triggerType) {
                    // Do action
                }
            }
        }
    }
}
