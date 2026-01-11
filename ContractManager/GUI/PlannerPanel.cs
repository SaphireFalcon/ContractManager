using Brutal.ImGuiApi;
using Brutal.ImGuiApi.Extensions;
using Brutal.Numerics;
using ContractManager.Contract;
using ContractManager.Mission;
using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Text;

namespace ContractManager.GUI
{
    // The panel that goes inside the "Mission Planner" tab of the `ContractManagementWindow`
    internal class PlannerPanel
    {
        private readonly ContractManagementWindow _managementWindow;

        private ContractBlueprintEditingPanel? _contractBlueprintEditingPanel = null;

        internal PlannerPanel(in ContractManagementWindow managementWindow)
        {
            this._managementWindow = managementWindow;
        }

        internal void DrawPlannerLeftPanel()
        {
            // Left panel: fixed width and fill available height so it becomes scrollable when content overflows
            var style = ImGui.GetStyle();
            float leftPanelWidth = 260.0f;
            Brutal.Numerics.float2 leftPanelSize = new Brutal.Numerics.float2 { X = leftPanelWidth, Y = 0.0f };
            ImGui.PushStyleVar(ImGuiStyleVar.WindowPadding, new Brutal.Numerics.float2 { X = 0.0f, Y = 0.0f });
            if (ImGui.BeginChild("PlannerLeftPanel", leftPanelSize, ImGuiChildFlags.None, ImGuiWindowFlags.None))
            {
                
                // Draw Mission section
                var tabLeftPanelRegionSize = ImGui.GetContentRegionAvail();
                float leftMissionPanelHeightRatio = 0.3f;
                
                Brutal.Numerics.float2 leftMissionPanelSize = new Brutal.Numerics.float2 {
                    X = leftPanelWidth,
                    Y = (tabLeftPanelRegionSize.Y * leftMissionPanelHeightRatio) - style.FramePadding.Y,
                };
                if (ImGui.BeginChild("PlannerLeftMissionPanel", leftMissionPanelSize, ImGuiChildFlags.Borders, ImGuiWindowFlags.None))
                {
                    ImGui.SeparatorText("Missions");
                    List<LeftPanelListItem> listItems = LeftPanelListItem.GetLeftPanelListItems(this._managementWindow, ContractManager.data.missionBlueprints);
                    ImGui.Text(String.Format("list: {0}", listItems.Count));
                    this._managementWindow.DrawItemList(listItems, "planner_missionBlueprints");
                    ImGui.EndChild();  // End of LeftContractPanel
                }

                // Draw Contract section
                Brutal.Numerics.float2 leftContractPanelSize = new Brutal.Numerics.float2 {
                    X = leftPanelWidth,
                    Y = (tabLeftPanelRegionSize.Y * (1.0f - leftMissionPanelHeightRatio)) - style.FramePadding.Y
                };
                if (ImGui.BeginChild("PlannerLeftContractPanel", leftContractPanelSize, ImGuiChildFlags.Borders, ImGuiWindowFlags.None))
                {
                    ImGui.SeparatorText("Contracts");
                    List<LeftPanelListItem> listItems = LeftPanelListItem.GetLeftPanelListItems(this._managementWindow, ContractManager.data.contractBlueprints);
                    ImGui.Text(String.Format("list: {0}", listItems.Count));
                    this._managementWindow.DrawItemList(listItems, "planner_contractBlueprints");
                    ImGui.EndChild();  // End of LeftContractPanel
                }

                ImGui.EndChild();  // End of LeftPanel
            }
            ImGui.PopStyleVar();
        }

        internal void DrawPlannerRightPanel()
        {
            ImGui.SameLine();
            // Right panel: fills remaining space on the right
            if (ImGui.BeginChild("PlannerRightPanel"))
            {
                Brutal.Numerics.float2 rightPanelRegionSize = ImGui.GetContentRegionAvail();
                Brutal.Numerics.float2 rightPanelSize = new Brutal.Numerics.float2 { X = 0.0f, Y = rightPanelRegionSize.Y - 35.0f};
                // Wrap contents in a child to make it scrollable if needed
                if (ImGui.BeginChild("PlannerDetails", rightPanelSize, ImGuiChildFlags.None, ImGuiWindowFlags.None))
                {
                    if (this._managementWindow.rightPanelDetailType == RightPanelDetailType.NONE || this._managementWindow.rightPanelDetailUID == string.Empty)
                    {
                        ImGui.TextWrapped("Select an item from the left panel to view details here.");
                    }
                    else
                    if (this._managementWindow.rightPanelDetailType == RightPanelDetailType.CONTRACTBLUEPRINT)
                    {
                        ContractBlueprint.ContractBlueprint? contractBlueprintToShow = ContractUtils.FindContractBlueprintFromUID(
                            ContractManager.data.contractBlueprints,
                            this._managementWindow.rightPanelDetailUID
                        );
                        if (contractBlueprintToShow != null) {
                            this.DrawContractBlueprintDetails(contractBlueprintToShow);
                        }
                        else
                        {
                            // Something bad happened to the detail UID, it became corrupted.
                            this._managementWindow.rightPanelDetailType = RightPanelDetailType.NONE;
                            this._managementWindow.rightPanelDetailUID = string.Empty;
                        }
                    }
                    else
                    if (this._managementWindow.rightPanelDetailType == RightPanelDetailType.MISSIONBLUEPRINT)
                    {
                        Mission.MissionBlueprint? missionBlueprintToShow = MissionUtils.FindMissionBlueprintFromUID(
                            ContractManager.data.missionBlueprints,
                            this._managementWindow.rightPanelDetailUID
                        );
                        if (missionBlueprintToShow != null) {
                            this.DrawMissionlueprintDetails(missionBlueprintToShow);
                        }
                        else
                        {
                            // Something bad happened to the detail UID, it became corrupted.
                            this._managementWindow.rightPanelDetailType = RightPanelDetailType.NONE;
                            this._managementWindow.rightPanelDetailUID = string.Empty;
                        }
                    }
                    ImGui.EndChild();  // End of Planner details child
                }
                ImGui.EndChild();  // End of RightPanel
            }
        }

        internal void DrawContractBlueprintDetails(ContractBlueprint.ContractBlueprint contractBlueprintToShow)
        {
            if (this._contractBlueprintEditingPanel == null)
            {
                this._contractBlueprintEditingPanel = new ContractBlueprintEditingPanel(ref contractBlueprintToShow);
            }
            else
            if (this._contractBlueprintEditingPanel.blueprintUID != contractBlueprintToShow.uid)
            {
                this._contractBlueprintEditingPanel = new ContractBlueprintEditingPanel(ref contractBlueprintToShow);
            }
            this._contractBlueprintEditingPanel.Draw();
        }

        internal void DrawMissionlueprintDetails(Mission.MissionBlueprint missionBlueprintToShow)
        {
            // Draw mission blueprint details
            ImGui.SeparatorText("Mission blueprint : " + missionBlueprintToShow.title);
        }
    }

    internal class ContractBlueprintEditingPanel
    {
        private ContractBlueprint.ContractBlueprint _contractBlueprint;
        internal string blueprintUID = string.Empty;

        // variables for inputs
        private Brutal.ImGuiApi.ImInputString _missionBlueprintUID;

        internal ContractBlueprintEditingPanel(ref ContractBlueprint.ContractBlueprint contractBlueprintToEdit)
        {
            this._contractBlueprint = contractBlueprintToEdit;
            this.blueprintUID = contractBlueprintToEdit.uid;
            _missionBlueprintUID = new ImInputString(256, contractBlueprintToEdit.missionBlueprintUID);
        }

        internal void Draw()
        {
            // Draw contract blueprint details
            ImGui.SeparatorText("Contract blueprint : " + this._contractBlueprint.title);

            ImGui.Indent();
            ImGui.Text("missionBlueprintUID:");
            ImGui.SameLine();
            ContractManagementWindow.DrawHelpTooltip("tooltip text, missionBlueprintUID");
            ImGui.SameLine();
            if (ImGui.InputText("##ContractBlueprint_input_missionBlueprintUID", this._missionBlueprintUID))
            {
                this._contractBlueprint.missionBlueprintUID = this._missionBlueprintUID.ToString();
            }
            ImGui.SameLine();
            ImGui.Text(String.Format("{0}", this._contractBlueprint.missionBlueprintUID));

            ImGui.Unindent(); // at the end of a section to be similar aligned.
        }
    }
}
