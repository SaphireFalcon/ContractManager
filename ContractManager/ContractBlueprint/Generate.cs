using ContractManager.Mission;
using System;
using System.Collections.Generic;
using System.Text;

namespace ContractManager.ContractBlueprint
{
    internal static class Generate
    {
        // unit-test method to create an example contract and write it to disk
        internal static void ExampleContract001()
        {
            var contractToWrite = new ContractBlueprint
            {
                uid = "example_contract_001",
                title = "Example Contract 001",
                synopsis = "This is an example contract, changing the orbit in 2 steps.",
                description = "Complete the objectives to fulfill this contract. \nFirst, change the Periapsis to within 150 and 200 km altitude. \nNext, change the Apoapsis to within 150 and 200 km altitude."
            };
            
            contractToWrite.prerequisites.Add(new Prerequisite
            {
                type = PrerequisiteType.MaxNumOfferedContracts,
                maxNumOfferedContracts = 1
            });
            contractToWrite.prerequisites.Add(new Prerequisite
            {
                type = PrerequisiteType.MaxNumAcceptedContracts,
                maxNumAcceptedContracts = 1
            });

            contractToWrite.completionCondition = CompletionCondition.All;
            contractToWrite.requirements.Add(new Requirement
            {
                uid = "change_periapsis_150_200km",
                type = RequirementType.Orbit,
                title = "Change Periapsis",
                synopsis = "Change Periapsis to 150~200km.",
                description = "Change the orbit to a low orbit with a Periapsis between 150.000 and 200.000 meter.",
                isCompletedOnAchievement = false,
                orbit = new RequiredOrbit
                {
                    targetBody = "Earth",
                    minPeriapsis = 150000,
                    maxPeriapsis = 200000
                }
            });
            contractToWrite.requirements.Add(new Requirement
            {
                uid = "change_apoapsis_150_200km",
                type = RequirementType.Orbit,
                title = "Change Apoapsis",
                synopsis = "Change Apoapsis to 150~200km.",
                description = "Change the orbit to a low orbit with a Apoapsis between 150.000 and 200.000 meter.",
                orbit = new RequiredOrbit
                {
                    targetBody = "Earth",
                    minApoapsis = 150000,
                    maxApoapsis = 200000
                }
            });
            
            contractToWrite.actions.Add(new Action
            {
                trigger = TriggerType.OnContractComplete,
                type = ActionType.ShowMessage,
                showMessage = "Congratulations! You pounced the example contract."
            });
            contractToWrite.actions.Add(new Action
            {
                trigger = TriggerType.OnContractFail,
                type = ActionType.ShowMessage,
                showMessage = "Keep persevering; The road to success is pawed with failure."
            });

            contractToWrite.WriteToConsole();

            string myDocumentsPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            Console.WriteLine($"[CM] 'My Documents' path: {myDocumentsPath}");
            string savePath = Path.Combine(
                myDocumentsPath,
                @"My Games\Kitten Space Agency\contracts\",
                $"{contractToWrite.uid}.xml"
            );
            Console.WriteLine($"[CM] save path: {savePath}");
            if (!string.IsNullOrEmpty(myDocumentsPath))
            {
                contractToWrite.WriteToFile(savePath);
            }
        }

        internal static void ExampleContract002()
        {
            var contractToWrite = new ContractBlueprint
            {
                uid = "example_contract_002",
                title = "Example Contract 002",
                synopsis = "This is an example contract, changing the orbit 2 times.",
                description = "Complete the objectives to fulfill this contract. \nFirst, change the Periapsis to within 150 and 200 km altitude. \nNext, change the Apoapsis to within 150 and 200 km altitude.\nThen, change both Apoapsis and Periapsis to with 200 and 220km altitude."
            };
            
            contractToWrite.prerequisites.Add(new Prerequisite
            {
                type = PrerequisiteType.MaxNumOfferedContracts,
                maxNumOfferedContracts = 1
            });
            contractToWrite.prerequisites.Add(new Prerequisite
            {
                type = PrerequisiteType.MaxNumAcceptedContracts,
                maxNumAcceptedContracts = 1
            });

            contractToWrite.completionCondition = CompletionCondition.All;
            List<Requirement> groupRequirements = new List<Requirement>();
            groupRequirements.Add(new Requirement
            {
                uid = "change_periapsis_150_200km",
                type = RequirementType.Orbit,
                title = "Change Periapsis",
                synopsis = "Change Periapsis to 150~200km.",
                description = "Change the orbit to a low orbit with a Periapsis between 150.000 and 200.000 meter.",
                isCompletedOnAchievement = false,
                orbit = new RequiredOrbit
                {
                    targetBody = "Earth",
                    minPeriapsis = 150000,
                    maxPeriapsis = 200000
                }
            });
            groupRequirements.Add(new Requirement
            {
                uid = "change_apoapsis_150_200km",
                type = RequirementType.Orbit,
                title = "Change Apoapsis",
                synopsis = "Change Apoapsis to 150~200km.",
                description = "Change the orbit to a low orbit with a Apoapsis between 150.000 and 200.000 meter.",
                orbit = new RequiredOrbit
                {
                    targetBody = "Earth",
                    minApoapsis = 150000,
                    maxApoapsis = 200000
                }
            });
            contractToWrite.requirements.Add(new Requirement
            {
                uid = "change_periapsis_150_200km",
                type = RequirementType.Group,
                title = "Change Orbit",
                group = new RequiredGroup
                {
                    completionCondition = CompletionCondition.All,
                    requirements = groupRequirements
                }
            });
            contractToWrite.requirements.Add(new Requirement
            {
                uid = "change_apoapsis_150_200km",
                type = RequirementType.Orbit,
                title = "Change Apoapsis",
                synopsis = "Change orbit to 200~220km.",
                description = "Change the orbit to a higher orbit with an Apoapsis and Periapsis between 200.000 and 220.000 meter.",
                isHidden = true,
                orbit = new RequiredOrbit
                {
                    targetBody = "Earth",
                    minApoapsis = 200000,
                    maxApoapsis = 220000,
                    minPeriapsis = 200000,
                    maxPeriapsis = 220000
                }
            });
            
            contractToWrite.actions.Add(new Action
            {
                trigger = TriggerType.OnContractComplete,
                type = ActionType.ShowMessage,
                showMessage = "Congratulations! You pounced the example contract."
            });
            contractToWrite.actions.Add(new Action
            {
                trigger = TriggerType.OnContractFail,
                type = ActionType.ShowMessage,
                showMessage = "Keep persevering; The road to success is pawed with failure."
            });

            contractToWrite.WriteToConsole();

            string myDocumentsPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            Console.WriteLine($"[CM] 'My Documents' path: {myDocumentsPath}");
            string savePath = Path.Combine(
                myDocumentsPath,
                @"My Games\Kitten Space Agency\contracts\",
                $"{contractToWrite.uid}.xml"
            );
            Console.WriteLine($"[CM] save path: {savePath}");
            if (!string.IsNullOrEmpty(myDocumentsPath))
            {
                contractToWrite.WriteToFile(savePath);
            }
        }

        internal static void ExampleMission001()
        {
            // Mission
            var missionToWrite = new MissionBlueprint
            {
                uid = "example_mission_001",
                title = "Example Mission 001",
                synopsis = "This is an example mission, with 2 contracts.",
                description = "Orbit around the Moon and fly back to Earth."
            };

            // Contract to the Moon
            var contractToMoon = new ContractBlueprint
            {
                uid = "example_mission_001_contract_001",
                title = "Fly to the Moon",
                synopsis = "Fly to the Moon and achieve a stable orbit",
                description = "Create a randevous trajectory with the Moon. Then achieve an orbit around the Moon below 30km.",
                isAutoAccepted = true,
                isRejectable = false,
            };
            contractToMoon.prerequisites.Add(new Prerequisite
            {
                type = PrerequisiteType.MaxNumOfferedContracts,
                maxNumOfferedContracts = 4,
            });
            contractToMoon.prerequisites.Add(new Prerequisite
            {
                type = PrerequisiteType.HasAcceptedMission,
                hasAcceptedMission = "example_mission_001",
            });
            
            contractToMoon.requirements.Add(new Requirement
            {
                uid = "orbit_earth",
                type = RequirementType.Orbit,
                title = "Orbit Earth",
                synopsis = "Starting condition is to orbit Earth below 210km.",
                orbit = new RequiredOrbit
                {
                    targetBody = "Earth",
                    type = OrbitType.Elliptical,
                    maxApoapsis = 210000,
                }
            });
            contractToMoon.requirements.Add(new Requirement
            {
                uid = "to_the_moon",
                type = RequirementType.Orbit,
                title = "Rendevous with the Moon",
                synopsis = "Rendevous with the Moon with apoapsis below 200km.",
                description = "Change your orbit around Earth such that your trajectory will rendevous with the Moon. TODO: Add hints.",
                orbit = new RequiredOrbit
                {
                    targetBody = "Moon",
                    maxApoapsis = 200000,
                    type = OrbitType.Escape,
                }
            });
            contractToMoon.requirements.Add(new Requirement
            {
                uid = "orbit_the_moon",
                type = RequirementType.Orbit,
                title = "Orbit the Moon",
                synopsis = "Orbit the Moon with apoapsis below 30km.",
                description = "Change your orbit around the Moon to a low orbit of below 30km.",
                orbit = new RequiredOrbit
                {
                    targetBody = "Moon",
                    maxApoapsis = 30000,
                    type = OrbitType.Elliptical,
                }
            });
            missionToWrite.contractBlueprintUIDs.Add(contractToMoon.uid);
            
            // Contract back to Earth
            var contractToEarth = new ContractBlueprint
            {
                uid = "example_mission_001_contract_002",
                title = "Fly to the Moon",
                synopsis = "Fly to the Moon and achieve a stable orbit",
                description = "Create an escape trajectory from the Moon. Then achieve an orbit around Earth between 150~200km.",
                isAutoAccepted = true,
                isRejectable = false,
            };
            contractToEarth.prerequisites.Add(new Prerequisite
            {
                type = PrerequisiteType.HasCompletedContract,
                hasCompletedContract = "example_mission_001_contract_001",
            });
            contractToEarth.requirements.Add(new Requirement
            {
                uid = "escape_the_moon",
                type = RequirementType.Orbit,
                title = "Escape the Moon",
                synopsis = "Escape the Moon back to Earth.",
                description = "Change your orbit around the Moon to a low orbit of below 30km.",
                orbit = new RequiredOrbit
                {
                    targetBody = "Moon",
                    type = OrbitType.Escape,
                }
            });
            contractToEarth.requirements.Add(new Requirement
            {
                uid = "orbit_earth_again",
                type = RequirementType.Orbit,
                title = "Orbit Earth again",
                synopsis = "Orbit Earth again.",
                description = "Change your orbit around Earth to a orbit of between 150~200km.",
                orbit = new RequiredOrbit
                {
                    targetBody = "Earth",
                    type = OrbitType.Elliptical,
                    maxApoapsis = 200000,
                    maxPeriapsis = 200000,
                    minApoapsis = 150000,
                    minPeriapsis = 150000,
                }
            });
            missionToWrite.contractBlueprintUIDs.Add(contractToEarth.uid);
            
            string myDocumentsPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            Console.WriteLine($"[CM] 'My Documents' path: {myDocumentsPath}");
            string savePath = Path.Combine(
                myDocumentsPath,
                @"My Games\Kitten Space Agency\contracts\",
                $"{contractToMoon.uid}.xml"
            );
            Console.WriteLine($"[CM] save path: {savePath}");
            if (!string.IsNullOrEmpty(myDocumentsPath))
            {
                contractToMoon.WriteToFile(savePath);
            }
            
            savePath = Path.Combine(
                myDocumentsPath,
                @"My Games\Kitten Space Agency\contracts\",
                $"{contractToEarth.uid}.xml"
            );
            Console.WriteLine($"[CM] save path: {savePath}");
            if (!string.IsNullOrEmpty(myDocumentsPath))
            {
                contractToEarth.WriteToFile(savePath);
            }
            
            savePath = Path.Combine(
                myDocumentsPath,
                @"My Games\Kitten Space Agency\missions\",
                $"{missionToWrite.uid}.xml"
            );
            Console.WriteLine($"[CM] save path: {savePath}");
            if (!string.IsNullOrEmpty(myDocumentsPath))
            {
                missionToWrite.WriteToFile(savePath);
            }
        }
    }
}
