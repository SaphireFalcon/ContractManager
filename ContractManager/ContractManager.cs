using Brutal.ImGuiApi;
using ContractManager.ContractBlueprint;
using KSA;
using StarMap.API;
using System;
using System.IO;

namespace ContractManager
{
[StarMapMod]
public class ContractManager
{
    [StarMapImmediateLoad]
    public void onImmediateLoad(Mod definingMod)
    {
        Console.WriteLine("[CM] 'onImmediateLoad'");
        //KSA.XmlLoader.Load();
    }

    [StarMapAllModsLoaded]
    public void OnAllModsLoaded()
    {
        Console.WriteLine("[CM] 'OnAllModsLoaded'");

        // Load contracts from disk here
        var contract1 = ContractBlueprint.ContractBlueprint.LoadFromFile("Content/ContractManager/contracts/example_contract_001.xml");
        contract1.WriteToConsole();

        // For testing: create and write an example contract to disk
        CreateExample001Contract();
    }


    [StarMapAfterGui]
    public void AfterGui(double dt)
    {
        // Create the UI here

        // Contract Manager Window
        if (ImGui.Begin("Contract Manager", ImGuiWindowFlags.None))
        {
            ImGui.Text("This is the Contract Manager window.");
        }
        ImGui.End();
    }
        
    // unit-test method to create an example contract and write it to disk
    private void CreateExample001Contract()
    {
        var contractToWrite = new ContractBlueprint.ContractBlueprint
        {
            uid = "example_contract_001",
            title = "Example Contract 001",
            synopsis = "This is an example contract, changing the orbit in 2 steps.",
            description = "Complete the objectives to fulfill this contract. \nFirst, change the Periapsis to within 150 and 200 km altitude. \nNext, change the Apoapsis to within 150 and 200 km altitude."
        };
            
        contractToWrite.prerequisites.Add(new ContractBlueprint.Prerequisite
        {
            type = ContractBlueprint.PrerequisiteType.MaxNumOfferedContracts,
            maxNumOfferedContracts = 1
        });
        contractToWrite.prerequisites.Add(new ContractBlueprint.Prerequisite
        {
            type = ContractBlueprint.PrerequisiteType.MaxNumAcceptedContracts,
            maxNumAcceptedContracts = 1
        });

        contractToWrite.completionCondition = CompletionCondition.All;
        contractToWrite.requirements.Add(new ContractBlueprint.Requirement
        {
            uid = "change_periapsis_150_200km",
            type = RequirementType.Orbit,
            title = "Change Periapsis",
            synopsis = "Change Periapsis to 150~200km.",
            description = "Change the orbit to a low orbit with a Periapsis between 150.000 and 200.000 meter.",
            isCompletedOnAchievement = false,
            orbit = new ContractBlueprint.RequiredOrbit
            {
                targetBody = "Earth",
                minPeriapsis = 150000,
                maxPeriapsis = 200000
            }
        });
        contractToWrite.requirements.Add(new ContractBlueprint.Requirement
        {
            uid = "change_apoapsis_150_200km",
            type = RequirementType.Orbit,
            title = "Change Apoapsis",
            synopsis = "Change Apoapsis to 150~200km.",
            description = "Change the orbit to a low orbit with a Apoapsis between 150.000 and 200.000 meter.",
            orbit = new ContractBlueprint.RequiredOrbit
            {
                targetBody = "Earth",
                minApoapsis = 150000,
                maxApoapsis = 200000
            }
        });
            
        contractToWrite.actions.Add(new ContractBlueprint.Action
        {
            trigger = TriggerType.OnContractComplete,
            type = ActionType.ShowMessage,
            showMessage = "Congratulations! You pounced the example contract."
        });
        contractToWrite.actions.Add(new ContractBlueprint.Action
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
}

}  // End of ContractManager namespace
