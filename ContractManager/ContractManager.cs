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

    // Internal fields
    private double _lastUpdateTime = 0.0d;
    private double _updateInterval = 5.0d;
    // List of all loaded contract blueprints
    private List<ContractBlueprint.ContractBlueprint> _contractBlueprints { get; set; } = new List<ContractBlueprint.ContractBlueprint>();

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
        var blueprintContract1 = ContractBlueprint.ContractBlueprint.LoadFromFile("Content/ContractManager/contracts/example_contract_001.xml");
        blueprintContract1.WriteToConsole();
        this._contractBlueprints.Add(blueprintContract1);

        var dummyContract = new Contract.Contract(in blueprintContract1, 0.0d);
        this.offeredContracts.Add(dummyContract);

        // For testing: create and write an example contract to disk
        CreateExample001Contract();
    }


    [StarMapAfterGui]
    public void AfterGui(double dt)
    {
        // Access the controlled vehicle, needed for periapsis/apoapsis checks etc.
        KSA.Vehicle currentVehicle = Program.ControlledVehicle;
        double playerTime = Program.GetPlayerTime();

        if (playerTime - this._lastUpdateTime > this._updateInterval)
        {
            this._lastUpdateTime = playerTime;
            Console.WriteLine($"[CM] Game time: {playerTime}s offered: {this.offeredContracts.Count} accepted: {this.acceptedContracts.Count} finished: {this.finishedContracts.Count}");
            foreach (Contract.Contract offeredContract in this.offeredContracts)
            {
                // auto-accept for now
                if (offeredContract.status == Contract.ContractStatus.Offered)
                {
                    offeredContract.AcceptOfferedContract(playerTime);
                    acceptedContracts.Add(offeredContract);
                }
            }
            // Cleanup offered contracts
            for (int offeredContractIndex = 0; offeredContractIndex < offeredContracts.Count; offeredContractIndex++)
            {
                if (this.offeredContracts[offeredContractIndex].status != Contract.ContractStatus.Offered)
                {
                    this.offeredContracts.RemoveAt(offeredContractIndex);
                    offeredContractIndex--;
                }
            }
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

        var style = ImGui.GetStyle();

        // Contract Management Window with two panels: left fixed-width, right flexible
        ImGui.SetNextWindowSizeConstraints(
            new Brutal.Numerics.float2 { X = 600.0f, Y = 300.0f },
            new Brutal.Numerics.float2 { X = float.PositiveInfinity, Y = float.PositiveInfinity }  // no max size
        );
        // var imGuiIO = ImGui.GetIO();
        // var currentFont = ImGui.GetFont();
        // float fontSize = ImGui.GetFontSize();
        // var largeFont = imGuiIO.Fonts.AddFont(currentFont, fontSize * 1.5);
        // Console.WriteLine($"[CM] ImGui Font: '{currentFont}' size: {fontSize}");

        if (ImGui.Begin("Contract Management", ImGuiWindowFlags.None))
        {
            // Left panel: fixed width and fill available height so it becomes scrollable when content overflows
            Brutal.Numerics.float2 leftPanelSize = new Brutal.Numerics.float2 { X = 260.0f, Y = 0.0f };
            if (ImGui.BeginChild("LeftPanel", leftPanelSize, ImGuiChildFlags.Borders, ImGuiWindowFlags.None))
            {
                ImGui.SeparatorText("Contracts");

                // Tabs in the left panel
                if (ImGui.BeginTabBar("LeftTabs", ImGuiTabBarFlags.None))
                {
                    if (ImGui.BeginTabItem("Offered"))
                    {
                        var tabContentRegionSize = ImGui.GetContentRegionAvail();
                        Brutal.Numerics.float2 tabSize = new Brutal.Numerics.float2 { X = 0.0f, Y = tabContentRegionSize.Y };
                        // Wrap contens in a child to make it scrollable if needed
                        if (ImGui.BeginChild("OfferedTabChild", tabSize, ImGuiChildFlags.None, ImGuiWindowFlags.NoTitleBar))
                        {
                            // Fill available width for buttons
                            var tabChildContentRegionSize = ImGui.GetContentRegionAvail();
                            Brutal.Numerics.float2 buttonSize = new Brutal.Numerics.float2 { X = tabChildContentRegionSize.X, Y = 0.0f };
                            // Left-align button text
                            ImGui.PushStyleVar(ImGuiStyleVar.ButtonTextAlign, new Brutal.Numerics.float2 { X = 0.0f, Y = 0.5f });
                            if (ImGui.Button("Example Contract 001", buttonSize))
                            {
                                // TODO: handle showing details in the right panel.
                            }
                            if (ImGui.Button("Example Contract 002", buttonSize))
                            {
                                // TODO: handle showing details in the right panel.
                            }
                            if (ImGui.Button("Example Contract 003", buttonSize))
                            {
                                // TODO: handle showing details in the right panel.
                            }
                            if (ImGui.Button("Example Contract 004", buttonSize))
                            {
                                // TODO: handle showing details in the right panel.
                            }
                            ImGui.PopStyleVar();
                            ImGui.EndChild();
                        }
                        ImGui.EndTabItem();
                    }

                    if (ImGui.BeginTabItem("Accepted"))
                    {
                        var tabContentRegionSize = ImGui.GetContentRegionAvail();
                        Brutal.Numerics.float2 tabSize = new Brutal.Numerics.float2 { X = 0.0f, Y = tabContentRegionSize.Y };
                        // Wrap contens in a child to make it scrollable if needed
                        if (ImGui.BeginChild("AcceptedTabChild", tabSize, ImGuiChildFlags.None, ImGuiWindowFlags.NoTitleBar))
                        {
                            // Fill available width for buttons
                            var tabChildContentRegionSize = ImGui.GetContentRegionAvail();
                            Brutal.Numerics.float2 buttonSize = new Brutal.Numerics.float2 { X = tabChildContentRegionSize.X, Y = 0.0f };
                            // Left-align button text
                            ImGui.PushStyleVar(ImGuiStyleVar.ButtonTextAlign, new Brutal.Numerics.float2 { X = 0.0f, Y = 0.5f });
                            if (ImGui.Button("Accepted Contract A", buttonSize)) { }
                            if (ImGui.Button("Accepted Contract B", buttonSize)) { }
                            if (ImGui.Button("Accepted Contract C", buttonSize)) { }
                            if (ImGui.Button("Accepted Contract D", buttonSize)) { }
                            if (ImGui.Button("Accepted Contract E", buttonSize)) { }
                            if (ImGui.Button("Accepted Contract F", buttonSize)) { }
                            if (ImGui.Button("Accepted Contract G", buttonSize)) { }
                            if (ImGui.Button("Accepted Contract H", buttonSize)) { }
                            ImGui.PopStyleVar();
                            ImGui.EndChild();
                        }
                        ImGui.EndTabItem();
                    }

                    if (ImGui.BeginTabItem("Completed"))
                    {
                        var tabContentRegionSize = ImGui.GetContentRegionAvail();
                        Brutal.Numerics.float2 tabSize = new Brutal.Numerics.float2 { X = 0.0f, Y = tabContentRegionSize.Y };
                        // Wrap contens in a child to make it scrollable if needed
                        if (ImGui.BeginChild("CompletedTabChild", tabSize, ImGuiChildFlags.None, ImGuiWindowFlags.NoTitleBar))
                        {
                            // Fill available width for buttons
                            var tabChildContentRegionSize = ImGui.GetContentRegionAvail();
                            Brutal.Numerics.float2 buttonSize = new Brutal.Numerics.float2 { X = tabChildContentRegionSize.X, Y = 0.0f };
                            // Left-align button text
                            ImGui.PushStyleVar(ImGuiStyleVar.ButtonTextAlign, new Brutal.Numerics.float2 { X = 0.0f, Y = 0.5f });
                            if (ImGui.Button("Completed Contract X1", buttonSize)) { }
                            if (ImGui.Button("Completed Contract X2", buttonSize)) { }
                            if (ImGui.Button("Completed Contract X3", buttonSize)) { }
                            if (ImGui.Button("Completed Contract X4", buttonSize)) { }
                            if (ImGui.Button("Completed Contract X5", buttonSize)) { }
                            if (ImGui.Button("Completed Contract X6", buttonSize)) { }
                            if (ImGui.Button("Completed Contract X7", buttonSize)) { }
                            if (ImGui.Button("Completed Contract X8", buttonSize)) { }
                            ImGui.PopStyleVar();
                            ImGui.EndChild();
                        }
                        ImGui.EndTabItem();
                    }
                    ImGui.EndTabBar();
                }
                ImGui.EndChild();  // End of LeftPanel
            }

            ImGui.SameLine();
            // Right panel: fills remaining space on the right
            if (ImGui.BeginChild("RightPanel"))
            {
                var rightPanelRegionSize = ImGui.GetContentRegionAvail();
                Brutal.Numerics.float2 rightPanelSize = new Brutal.Numerics.float2 { X = 0.0f, Y = rightPanelRegionSize.Y - 35.0f};
                // Wrap contens in a child to make it scrollable if needed
                if (ImGui.BeginChild("Contract details: title of contract", rightPanelSize, ImGuiChildFlags.None, ImGuiWindowFlags.None))
                {
                    ImGui.SeparatorText("Contract details: title of contract");
                    // If no contract to be shown
                    ImGui.TextWrapped("Select a contract from the left panel to view details here.");
                    
                    //ImGui.PushFont(FontLarge);
                    ImGui.TextWrapped("Synopsis goes here");
                    //ImGui.PopFont();

                    ImGui.TextWrapped("Full description goes here.\nLorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat. Duis aute irure dolor in reprehenderit in voluptate velit esse cillum dolore eu fugiat nulla pariatur. Excepteur sint occaecat cupidatat non proident, sunt in culpa qui officia deserunt mollit anim id est laborum.");
                        
                    ImGui.SeparatorText("Rewards");
                    ImGui.Text("Funds: K 1000");

                    ImGui.SeparatorText("Requirements");
                    ImGuiTreeNodeFlags requirementTreeNodeFlags = ImGuiTreeNodeFlags.DefaultOpen | ImGuiTreeNodeFlags.DrawLinesToNodes;
                    if (ImGui.TreeNodeEx("Contract requirements:", requirementTreeNodeFlags))
                    {
                        ImGui.Text("Complete all/any of these requirements");
                        if (ImGui.TreeNodeEx("Requirement title 1", requirementTreeNodeFlags))
                        {
                            ImGui.TextWrapped("Requirement synopsis");
                            ImGui.TextWrapped("Requirement details");
                            // Note: should `Orbit` be mentioned here?
                            ImGui.Text("Min Apoapsis 150.000m altitude");
                            ImGui.Text("Max Apoapsis 200.000m altitude");
                            ImGui.Text("Completed on achievement / maintain until completion");
                            ImGui.Text("Complete in order");
                            ImGui.Text("Child requirements:");
                            if (ImGui.TreeNodeEx("Requirement title child 1", requirementTreeNodeFlags))
                            {
                                ImGui.TextWrapped("Requirement synopsis");
                                ImGui.TextWrapped("Requirement details");
                                ImGui.Text("Min Apoapsis 150.000m altitude");
                                ImGui.Text("Max Apoapsis 200.000m altitude");
                                ImGui.TreePop();
                            }
                            if (ImGui.TreeNodeEx("requirement title child 2", requirementTreeNodeFlags))
                            {
                                ImGui.TextWrapped("Requirement synopsis");
                                ImGui.TextWrapped("Requirement details");
                                ImGui.Text("Min Perisapsis 150.000m altitude");
                                ImGui.Text("Max Perisapsis 200.000m altitude");
                                ImGui.TreePop();
                            }
                            ImGui.TreePop();
                        }
                        if (ImGui.TreeNodeEx("Requirement title 2", requirementTreeNodeFlags))
                        {
                            ImGui.TextWrapped("Requirement synopsis");
                            ImGui.TextWrapped("Requirement details");
                            ImGui.Text("Min Perisapsis 150.000m altitude");
                            ImGui.Text("Max Perisapsis 200.000m altitude");
                            ImGui.Text("Child requirements:");
                            if (ImGui.TreeNodeEx("Requirement title child 1", requirementTreeNodeFlags))
                            {
                                ImGui.TextWrapped("Requirement synopsis");
                                ImGui.TextWrapped("Requirement details");
                                ImGui.Text("Min Apoapsis 150.000m altitude");
                                ImGui.Text("Max Apoapsis 200.000m altitude");
                                ImGui.TreePop();
                            }
                            if (ImGui.TreeNodeEx("requirement title child 2", requirementTreeNodeFlags))
                            {
                                ImGui.TextWrapped("Requirement synopsis");
                                ImGui.TextWrapped("Requirement details");
                                ImGui.Text("Min Perisapsis 150.000m altitude");
                                ImGui.Text("Max Perisapsis 200.000m altitude");
                                ImGui.TreePop();
                            }
                            ImGui.TreePop();
                        }
                        ImGui.TreePop();
                    }
                    ImGui.EndChild();  // End of Contract details child
                }
                ImGui.Separator();
                ImGui.PushStyleColor(ImGuiCol.Button, new Brutal.Numerics.float4 { X = 0.5f, Y = 0.1f, Z = 0.1f, W = 1.0f });
                ImGui.PushStyleColor(ImGuiCol.ButtonHovered, new Brutal.Numerics.float4 { X = 0.75f, Y = 0.25f, Z = 0.25f, W = 1.0f });
                ImGui.PushStyleColor(ImGuiCol.ButtonActive, new Brutal.Numerics.float4 { X = 0.75f, Y = 0.1f, Z = 0.1f, W = 1.0f });
                if (ImGui.Button("Reject Contract"))
                {
                }
                ImGui.SameLine();
                ImGui.PopStyleColor(3);
                //var style = ImGui.GetStyle();
                float buttonWidthReject = ImGui.CalcTextSize("Reject Contract").X + style.FramePadding.X * 2.0f;
                float buttonWidthAccept = ImGui.CalcTextSize("Accept Contract").X + style.FramePadding.X * 2.0f;
                float resizeTriangleWidth = 25.0f;
                ImGui.SetCursorPosX(ImGui.GetCursorPosX() + rightPanelRegionSize.X - buttonWidthReject - buttonWidthAccept - resizeTriangleWidth);
                ImGui.PushStyleColor(ImGuiCol.Button, new Brutal.Numerics.float4 { X = 0.2f, Y = 0.5f, Z = 0.2f, W = 1.0f });
                ImGui.PushStyleColor(ImGuiCol.ButtonHovered, new Brutal.Numerics.float4 { X = 0.35f, Y = 0.75f, Z = 0.35f, W = 1.0f });
                ImGui.PushStyleColor(ImGuiCol.ButtonActive, new Brutal.Numerics.float4 { X = 0.2f, Y = 0.75f, Z = 0.2f, W = 1.0f });
                if (ImGui.Button("Accept Contract"))
                {
                }
                ImGui.PopStyleColor(3);
                ImGui.EndChild();  // End of RightPanel
            }
        }
        ImGui.End();  // End of Contract Management Window

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
}

}  // End of ContractManager namespace
