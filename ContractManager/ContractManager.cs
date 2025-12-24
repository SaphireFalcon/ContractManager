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
        var contract2 = ContractBlueprint.ContractBlueprint.LoadFromFile("Content/ContractManager/contracts/example_contract_002.xml");
        contract2.WriteToConsole();

        // For testing: create and write an example contract to disk
        CreateExample002Contract();
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
    private void CreateExample002Contract()
    {
        var contractToWrite = new ContractBlueprint.ContractBlueprint
        {
            uid = "example_contract_002",
            title = "Example Contract",
            synopsis = "This is an example contract.",
            description = "Complete the objectives to fulfill this contract."
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
        var requiredOrbit = new ContractBlueprint.RequiredOrbit
        {
            targetBody = "Earth",
            minApoapsis = 150000,
            maxApoapsis = 160000,
            minPeriapsis = 150000,
            maxPeriapsis = 160000
        };
        contractToWrite.requirements.Add(new ContractBlueprint.Requirement
        {
            type = RequirementType.Orbit,
            title = "Change orbit",
            synopsis = "Change orbit to be xyz.",
            description = "Change the orbit by increasing the apoapsis, then increasing the periapsis.",
            orbit = requiredOrbit
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
