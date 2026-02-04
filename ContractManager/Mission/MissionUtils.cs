using ContractManager.Contract;
using System;
using System.Collections.Generic;
using System.Text;

namespace ContractManager.Mission
{
    internal class MissionUtils
    {
        internal static Mission? FindMissionFromMissionUID(List<Mission> missions, string missionUID)
        {
            bool foundMission = false;
            foreach (Mission mission in missions)
            {
                foundMission = mission.uid == missionUID;
                if (foundMission)
                {
                    // Found matching mission.
                    return mission;
                }
            }
            return null;
        }

        internal static Mission? FindMissionFromMissionUID(string missionUID)
        {
            Mission? mission = null;
            mission = MissionUtils.FindMissionFromMissionUID(ContractManager.data.offeredMissions, missionUID);
            if (mission != null) { return mission; }
            mission = MissionUtils.FindMissionFromMissionUID(ContractManager.data.acceptedMissions, missionUID);
            if (mission != null) { return mission; }
            mission = MissionUtils.FindMissionFromMissionUID(ContractManager.data.finishedMissions, missionUID);
            return mission;
        }

        internal static List<Mission> FindMissionsFromMissionBlueprintUID(List<Mission> missions, string missionBlueprintUID)
        {
            List<Mission> foundMissions = new List<Mission>();
            foreach (Mission mission in missions)
            {
                if (mission._missionBlueprint.uid == missionBlueprintUID)
                {
                    // Found matching mission.
                    foundMissions.Add(mission);
                }
            }
            return foundMissions;
        }

        internal static MissionBlueprint? FindMissionBlueprintFromUID(List<MissionBlueprint> blueprints, string missionBlueprintUID)
        {
            bool foundBlueprint = false;
            foreach (MissionBlueprint blueprint in blueprints)
            {
                foundBlueprint = blueprint.uid == missionBlueprintUID;
                if (foundBlueprint)
                {
                    // Found matching blueprint.
                    return blueprint;
                }
            }
            return null;
        }

        // Trigger action of the given type.
        internal static void TriggerAction(
            Mission mission,
            ContractBlueprint.TriggerType triggerType,
            TrackedRequirement? trackedRequirement = null
        )
        {
            if (mission._missionBlueprint == null) {  return; }
            foreach (ContractBlueprint.Action action in mission._missionBlueprint.actions) {
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
                    action.DoAction(mission);
                }
            }
        }
    }
}
