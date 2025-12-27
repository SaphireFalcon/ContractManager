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
            foreach (TrackedRequirement trackedRequirement in trackedRequirements)
            {
                if (!( trackedRequirement.status is TrackedRequirementStatus.NOT_STARTED or TrackedRequirementStatus.ACHIEVED or TrackedRequirementStatus.FAILED ))
                {
                    trackedRequirement.Update();
                }
            }
        }
        
        // Update the tracked state of a tracked requirement using the vehicle data.
        public static void UpdateStateWithVehicle(in KSA.Vehicle vehicle, List<TrackedRequirement> trackedRequirements) {
            foreach (TrackedRequirement trackedRequirement in trackedRequirements)
            {
                if (!( trackedRequirement.status is TrackedRequirementStatus.NOT_STARTED or TrackedRequirementStatus.ACHIEVED or TrackedRequirementStatus.FAILED ))
                {
                    Utils.UpdateStateWithVehicle(in vehicle, trackedRequirement.trackedRequirements);
                    trackedRequirement.UpdateStateWithVehicle(in vehicle);
                }
            }
            return;
        }

        public static TrackedRequirementStatus CheckTrackedRequirementsStatus(List<TrackedRequirement> trackedRequirements)
        {
            TrackedRequirementStatus worstRequirementStatus = TrackedRequirementStatus.ACHIEVED;
            foreach (TrackedRequirement trackedRequirement in trackedRequirements)
            {
                // Check childs
                // TODO: if (trackedRequirement.type == RequirementType.Group)
                TrackedRequirementStatus worstChildStatus = Utils.CheckTrackedRequirementsStatus(trackedRequirement.trackedRequirements);
                if (worstChildStatus < worstRequirementStatus)
                {
                    worstRequirementStatus = worstChildStatus;
                }
                // Check tracked requirement
                if (trackedRequirement.status < worstRequirementStatus)
                {
                    worstRequirementStatus = trackedRequirement.status;
                }
            }
            return worstRequirementStatus;
        }

        // Trigger action of the given typ.
        public static void TriggerAction(in List<ContractBlueprint.Action> actions, ContractBlueprint.Action.TriggerType triggerType) {
            foreach (ContractBlueprint.Action action in actions) {
                if (action.trigger == triggerType) {
                    // Do action
                }
            }
        }
    }
}
