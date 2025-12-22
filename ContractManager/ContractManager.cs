using Brutal.ImGuiApi;
using KSA;
using StarMap.API;
using System.Diagnostics.Contracts;

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
        var contract = ContractBlueprint.ContractBlueprint.LoadFromFile("Content/ContractManager/contracts/example_contract_001.xml");
        contract.WriteToConsole();

        //var contractToWrite = new ContractBlueprint.ContractBlueprint
        //{
        //    uid = "example_contract_001",
        //    title = "Example Contract",
        //    synopsis = "This is an example contract.",
        //    details = "Complete the objectives to fulfill this contract."
        //};
        //contractToWrite.WriteToConsole();
        //contractToWrite.WriteToFile(@"${HOME}\My Games\Kitten Space Agency\contracts\example_contract_001.xml");
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
}

}  // End of ContractManager namespace
