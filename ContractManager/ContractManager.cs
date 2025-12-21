using Brutal.ImGuiApi;
using KSA;
using StarMap.API;

namespace ContractManager
{
[StarMapMod]
public class ContractManager
{
    [StarMapImmediateLoad]
    public void onImmediateLoad(Mod definingMod)
    {
        Console.WriteLine("[CM] 'onImmediateLoad'");
    }

    [StarMapAllModsLoaded]
    public void OnAllModsLoaded()
    {
        Console.WriteLine("[CM] 'OnAllModsLoaded'");
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
