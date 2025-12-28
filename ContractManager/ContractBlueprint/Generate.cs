using System;
using System.Collections.Generic;
using System.Text;

namespace ContractManager.ContractBlueprint
{
    internal static class Generate
    {
        // unit-test method to create an example contract and write it to disk
        internal static void Example001Contract()
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
                trigger = Action.TriggerType.OnContractComplete,
                type = Action.ActionType.ShowMessage,
                showMessage = "Congratulations! You pounced the example contract."
            });
            contractToWrite.actions.Add(new Action
            {
                trigger = Action.TriggerType.OnContractFail,
                type = Action.ActionType.ShowMessage,
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

        internal static void Example002Contract()
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
                trigger = Action.TriggerType.OnContractComplete,
                type = Action.ActionType.ShowMessage,
                showMessage = "Congratulations! You pounced the example contract."
            });
            contractToWrite.actions.Add(new Action
            {
                trigger = Action.TriggerType.OnContractFail,
                type = Action.ActionType.ShowMessage,
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
    }
}
