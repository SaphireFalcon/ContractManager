using Brutal.ImGuiApi;
using ContractManager.ContractBlueprint;
using KSA;
using StarMap.API;
using System;
using System.IO;
using System.Numerics;
using System.Xml.Serialization;
using System.Collections.Generic;

namespace ContractManager
{
[StarMapMod]
public class ContractManager
{
    // XML serializable fields
    // List of offered contracts, loaded from save game / file.
    [XmlElement("offeredContracts")]
    public List<Contract.Contract> offeredContracts {  get; set; } = new List<Contract.Contract>();

    // List of accepted contracts, loaded from save game / file.
    [XmlElement("acceptedContracts")]
    public List<Contract.Contract> acceptedContracts {  get; set; } = new List<Contract.Contract>();
        
    // List of finished contracts, loaded from save game / file.
    [XmlElement("finishedContracts")]
    public List<Contract.Contract> finishedContracts {  get; set; } = new List<Contract.Contract>();

    // Global ContractManager config of max number of contracts that can be offered simultaneously. Should be determined by the launch site management building.
    [XmlElement("maxNumberOfOfferedContracts")]
    public int maxNumberOfOfferedContracts { get; set; } = 2;
    
    // Global ContractManager config of max number of contracts that can be accepted simultaneously. Should be determined by the launch site management building.
    [XmlElement("maxNumberOfAcceptedContracts")]
    public int maxNumberOfAcceptedContracts { get; set; } = 1;

    // Internal fields
    private double _lastUpdateTime = 0.0d;
    private double _updateInterval = 5.0d;
    // List of all loaded contract blueprints
    private List<ContractBlueprint.ContractBlueprint> _contractBlueprints { get; set; } = new List<ContractBlueprint.ContractBlueprint>();

    private ContractManagementWindow? _contractManagementWindow = null;

    [StarMapImmediateLoad]
    public void onImmediateLoad(Mod definingMod)
    {
        Console.WriteLine("[CM] 'onImmediateLoad'");

        this._contractManagementWindow = new ContractManagementWindow(this.offeredContracts, this.acceptedContracts, this.finishedContracts);
    }

    [StarMapAllModsLoaded]
    public void OnAllModsLoaded()
    {
        Console.WriteLine("[CM] 'OnAllModsLoaded'");

        // Load contracts from disk here
        var blueprintContract1 = ContractBlueprint.ContractBlueprint.LoadFromFile("Content/ContractManager/contracts/example_contract_001.xml");
        blueprintContract1.WriteToConsole();
        this._contractBlueprints.Add(blueprintContract1);

        // For testing: create and write an example contract to disk
        //CreateExample001Contract();
    }

    [StarMapAfterGui]
    public void AfterGui(double dt)
    {
        // Access the controlled vehicle, needed for periapsis/apoapsis checks etc.
        KSA.Vehicle currentVehicle = Program.ControlledVehicle;
        double playerTime = Program.GetPlayerTime();

        // Update contracts
        this.UpdateContracts();

        // Draw GUI
        this._contractManagementWindow.DrawContractManagementWindow();

        var style = ImGui.GetStyle();

        // Active Contracts Window
        // Compute the size based on title and the Details button
        //var style = ImGui.GetStyle();
        float textWidthContractTitle = ImGui.CalcTextSize("Contract title 1:").X + style.FramePadding.X * 2.0f;
        float buttonWidthDetails = ImGui.CalcTextSize("Details").X + style.FramePadding.X * 2.0f;
        ImGui.SetNextWindowSizeConstraints(
            new Brutal.Numerics.float2 { X = textWidthContractTitle + buttonWidthDetails + 50, Y = 100.0f },
            new Brutal.Numerics.float2 { X = 500.0f, Y = float.PositiveInfinity }  // no max size
        );
        if (ImGui.Begin("Active Contracts", ImGuiWindowFlags.None))
        {
            //ImGui.SeparatorText("Active Contracts");
            ImGui.Text("No active contracts");

            ImGuiTreeNodeFlags requirementTreeNodeFlags = ImGuiTreeNodeFlags.DrawLinesToNodes;
            var activeContractsWindowSize = ImGui.GetContentRegionAvail();
            // Option 1: shorter lines
            if (ImGui.TreeNodeEx("Contract title 1:", requirementTreeNodeFlags))
            {
                ImGui.SameLine();
                ImGui.SetCursorPosX(ImGui.GetCursorPosX() + activeContractsWindowSize.X - textWidthContractTitle - buttonWidthDetails - 25);
                ImGui.SmallButton("Details");

                // Completed requirement -> should be hidden?
                ImGui.PushStyleColor(ImGuiCol.Header, new Brutal.Numerics.float4 { X = 0.2f, Y = 0.75f, Z = 0.2f, W = 1.0f });  // Doesn't seem to work inside TreeNodeEx?
                ImGui.PushStyleColor(ImGuiCol.HeaderHovered, new Brutal.Numerics.float4 { X = 0.35f, Y = 0.75f, Z = 0.35f, W = 1.0f });
                ImGui.PushStyleColor(ImGuiCol.HeaderActive, new Brutal.Numerics.float4 { X = 0.2f, Y = 0.75f, Z = 0.2f, W = 1.0f });
                if (ImGui.TreeNodeEx("Requirement title 1.1", requirementTreeNodeFlags))
                {
                    ImGui.TextWrapped("Requirement synopsis");
                    ImGui.TextColored(new Brutal.Numerics.float4 { X = 0.2f, Y = 0.75f, Z = 0.2f, W = 1.0f }, "Apoapsis: 175.000 m");
                    ImGui.TreePop();
                }
                ImGui.PopStyleColor(3);
                    
                // ongoing requirement
                ImGui.PushStyleColor(ImGuiCol.Header, new Brutal.Numerics.float4 { X = 0.75f, Y = 0.4f, Z = 0.1f, W = 1.0f });
                ImGui.PushStyleColor(ImGuiCol.HeaderHovered, new Brutal.Numerics.float4 { X = 0.75f, Y = 0.5f, Z = 0.2f, W = 1.0f });
                ImGui.PushStyleColor(ImGuiCol.HeaderActive, new Brutal.Numerics.float4 { X = 0.75f, Y = 0.4f, Z = 0.1f, W = 1.0f });
                if (ImGui.TreeNodeEx("Requirement title 1.2", requirementTreeNodeFlags))
                {
                    ImGui.TextWrapped("Requirement synopsis");
                    ImGui.TextColored(new Brutal.Numerics.float4 {X = 0.2f, Y = 0.75f, Z = 0.2f, W = 1.0f }, "min Apoapsis: 150.000 m");
                    ImGui.TextColored(new Brutal.Numerics.float4 { X = 0.75f, Y = 0.2f, Z = 0.2f, W = 1.0f }, "Apoapsis: 207.000 m");
                    ImGui.TextColored(new Brutal.Numerics.float4 { X = 0.75f, Y = 0.2f, Z = 0.2f, W = 1.0f }, "max Apoapsis: 200.000 m");
                    
                    // holding requirement
                    ImGui.PushStyleColor(ImGuiCol.Header, new Brutal.Numerics.float4 { X = 0.2f, Y = 0.75f, Z = 0.2f, W = 1.0f });
                    ImGui.PushStyleColor(ImGuiCol.HeaderHovered, new Brutal.Numerics.float4 { X = 0.35f, Y = 0.75f, Z = 0.35f, W = 1.0f });
                    ImGui.PushStyleColor(ImGuiCol.HeaderActive, new Brutal.Numerics.float4 { X = 0.2f, Y = 0.75f, Z = 0.2f, W = 1.0f });
                    if (ImGui.TreeNodeEx("Requirement title child 1", requirementTreeNodeFlags))
                    {
                        ImGui.TextWrapped("Requirement synopsis");
                        ImGui.TextColored(new Brutal.Numerics.float4 { X = 0.75f, Y = 0.4f, Z = 0.1f, W = 1.0f }, "Min Apoapsis 150.000 m");
                        ImGui.TextColored(new Brutal.Numerics.float4 {X = 0.2f, Y = 0.75f, Z = 0.2f, W = 1.0f }, "Apoapsis 175.000 m");
                        ImGui.TextColored(new Brutal.Numerics.float4 { X = 0.75f, Y = 0.4f, Z = 0.1f, W = 1.0f }, "Min Apoapsis 200.000 m");
                        ImGui.TreePop();
                    }
                    ImGui.PopStyleColor(3);
                        
                    // ongoing requirement
                    ImGui.PushStyleColor(ImGuiCol.Header, new Brutal.Numerics.float4 { X = 0.75f, Y = 0.4f, Z = 0.1f, W = 1.0f });
                    ImGui.PushStyleColor(ImGuiCol.HeaderHovered, new Brutal.Numerics.float4 { X = 0.75f, Y = 0.5f, Z = 0.2f, W = 1.0f });
                    ImGui.PushStyleColor(ImGuiCol.HeaderActive, new Brutal.Numerics.float4 { X = 0.75f, Y = 0.4f, Z = 0.1f, W = 1.0f });
                    if (ImGui.TreeNodeEx("requirement title child 2", requirementTreeNodeFlags))
                    {
                        ImGui.TextWrapped("Requirement synopsis, do in order");
                        ImGui.TextColored(new Brutal.Numerics.float4 {X = 0.2f, Y = 0.75f, Z = 0.2f, W = 1.0f }, "min Perisapsis: 150.000 m");
                        ImGui.TextColored(new Brutal.Numerics.float4 { X = 0.75f, Y = 0.2f, Z = 0.2f, W = 1.0f }, "Perisapsis: 207.000 m");
                        ImGui.TextColored(new Brutal.Numerics.float4 { X = 0.75f, Y = 0.2f, Z = 0.2f, W = 1.0f }, "max Perisapsis: 200.000 m");
                        ImGui.TreePop();
                    }
                    ImGui.PopStyleColor(3);
                    ImGui.TreePop();
                }
                ImGui.PopStyleColor(3);

                // locked requirement
                ImGui.PushStyleColor(ImGuiCol.Header, new Brutal.Numerics.float4 { X = 0.5f, Y = 0.5f, Z = 0.5f, W = 1.0f });
                ImGui.PushStyleColor(ImGuiCol.HeaderHovered, new Brutal.Numerics.float4 { X = 0.5f, Y = 0.5f, Z = 0.5f, W = 1.0f });
                ImGui.PushStyleColor(ImGuiCol.HeaderActive, new Brutal.Numerics.float4 { X = 0.5f, Y = 0.5f, Z = 0.5f, W = 1.0f });
                if (ImGui.TreeNodeEx("Requirement title 1.3", requirementTreeNodeFlags))
                {
                    // TODO: make synopsis grayed out
                    ImGui.TextWrapped ("Requirement synopsis, do in order");
                    ImGui.TextDisabled("Min Periapsis 150.0000 m");
                    ImGui.TextDisabled("Periapsis 207.000 m");
                    ImGui.TextDisabled("Max Periapsis 200.000 m");
                    ImGui.TreePop();
                }
                ImGui.PopStyleColor(3);
                ImGui.TreePop();
            }
            
            // Option 2: longer lines with min/max
            if (ImGui.TreeNodeEx("Contract title 2:", requirementTreeNodeFlags))
            {
                if (ImGui.TreeNodeEx("Requirement title 2.1", requirementTreeNodeFlags))
                {
                    ImGui.TextWrapped("Requirement synopsis");
                    ImGui.TextColored(new Brutal.Numerics.float4 { X = 0.2f, Y = 0.75f, Z = 0.2f, W = 1.0f }, "min Apoapsis 150.000 < 207.000 < 200.000 m");
                    ImGui.TextColored(new Brutal.Numerics.float4 { X = 0.75f, Y = 0.2f, Z = 0.2f, W = 1.0f }, "min Apoapsis 150.000 < 207.000 < 200.000 m");
                    ImGui.TextDisabled("Min Periapsis 150.000 < 207.000 < 200.000 m");
                    if (ImGui.TreeNodeEx("Requirement title child 2.1", requirementTreeNodeFlags))
                    {
                        ImGui.TextWrapped("Requirement synopsis");
                        ImGui.TextColored(new Brutal.Numerics.float4 { X = 0.75f, Y = 0.4f, Z = 0.1f, W = 1.0f }, "Min Apoapsis 150.000 < 207.000 < 200.000 m");
                        ImGui.TreePop();
                    }
                    if (ImGui.TreeNodeEx("requirement title child 2.2", requirementTreeNodeFlags))
                    {
                        ImGui.TextWrapped("Requirement synopsis, do in order");
                        ImGui.TextColored(new Brutal.Numerics.float4 { X = 0.75f, Y = 0.2f, Z = 0.2f, W = 1.0f }, "Min Periapsis 150.000 < 207.000 < 200.000 m");
                        ImGui.TreePop();
                    }
                    ImGui.TreePop();
                }
                ImGui.TreePop();
            }
        }
        ImGui.End();  // End of Active Contracts Window
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
            trigger = ContractBlueprint.Action.TriggerType.OnContractComplete,
            type = ContractBlueprint.Action.ActionType.ShowMessage,
            showMessage = "Congratulations! You pounced the example contract."
        });
        contractToWrite.actions.Add(new ContractBlueprint.Action
        {
            trigger = ContractBlueprint.Action.TriggerType.OnContractFail,
            type = ContractBlueprint.Action.ActionType.ShowMessage,
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

    // Contract Management back-end functions
    
    private void UpdateContracts()
    {
        double playerTime = Program.GetPlayerTime();
        if (playerTime - this._lastUpdateTime < this._updateInterval) { return; }

        this._lastUpdateTime = playerTime;
        Console.WriteLine($"[CM] Game time: {playerTime}s blueprints {this._contractBlueprints.Count} offered: {this.offeredContracts.Count} accepted: {this.acceptedContracts.Count} finished: {this.finishedContracts.Count}");

        // offer contracts
        this.OfferContracts(playerTime);

        // Update accepted contracts
        KSA.Vehicle currentVehicle = Program.ControlledVehicle;
        foreach (Contract.Contract acceptedContract in this.acceptedContracts)
        {
            if (currentVehicle != null)
            {
                acceptedContract.UpdateStateWithVehicle(currentVehicle);
            }
            bool statusUpdated = acceptedContract.Update(playerTime);
            if (statusUpdated)
            {
                // Check status and do something with it.
                if (acceptedContract.status == Contract.ContractStatus.Completed)
                {
                    finishedContracts.Add(acceptedContract);
                }
            }
        } 
        // Cleanup accepted contracts
        for (int acceptedContractIndex = 0; acceptedContractIndex < acceptedContracts.Count; acceptedContractIndex++)
        {
            if (this.acceptedContracts[acceptedContractIndex].status != Contract.ContractStatus.Accepted)
            {
                this.acceptedContracts.RemoveAt(acceptedContractIndex);
                acceptedContractIndex--;
            }
        }
    }

    private void OfferContracts(double playerTime)
    {
        if (this.offeredContracts.Count >= this.maxNumberOfOfferedContracts) { return; }

        List<ContractBlueprint.ContractBlueprint> contractBlueprintsToOffer = this.GetContractBlueprintsToOffer();
        Random randomGenerator = new Random();
        while (contractBlueprintsToOffer.Count + this.offeredContracts.Count > this.maxNumberOfOfferedContracts)
        {
            // Randomly select a subset of contracts to be offered
            contractBlueprintsToOffer.RemoveAt(randomGenerator.Next(0, contractBlueprintsToOffer.Count));
        }
        foreach (ContractBlueprint.ContractBlueprint contractBlueprint in contractBlueprintsToOffer)
        {
            this.offeredContracts.Add(new Contract.Contract(contractBlueprint, playerTime));
        }
    }
        
    private List<ContractBlueprint.ContractBlueprint> GetContractBlueprintsToOffer()
    {
        List<ContractBlueprint.ContractBlueprint> contractBlueprintsToOffer = new List<ContractBlueprint.ContractBlueprint>();
        foreach (ContractBlueprint.ContractBlueprint contractBlueprint in this._contractBlueprints)
        {
            if (this.CanOfferContractFromBlueprint(in contractBlueprint))
            {
                contractBlueprintsToOffer.Add(contractBlueprint);
            }
        }
        return contractBlueprintsToOffer;
    }

    private bool CanOfferContractFromBlueprint(in ContractBlueprint.ContractBlueprint contractBlueprint)
    {
        bool canOfferContract = true;
        foreach (ContractBlueprint.Prerequisite prerequisite in contractBlueprint.prerequisites)
        {
            if (prerequisite.type == PrerequisiteType.MaxNumOfferedContracts && this.offeredContracts.Count >= prerequisite.maxNumOfferedContracts)
            {
                canOfferContract = false;
                break;
            }
            if (prerequisite.type == PrerequisiteType.MaxNumAcceptedContracts && this.acceptedContracts.Count >= prerequisite.maxNumAcceptedContracts)
            {
                canOfferContract = false;
                break;
            }
            // TODO: Add a check if the contract was recently offered and rejected.
        }
        return canOfferContract;
    }
}

}  // End of ContractManager namespace
