using Brutal.ImGuiApi;
using Brutal.ImGuiApi.Extensions;
using Brutal.Numerics;
using ContractManager.Contract;
using ContractManager.ContractBlueprint;
using ContractManager.Mission;
using KSA;
using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Text;

namespace ContractManager.GUI
{
    // The panel that goes inside the "Mission Planner" tab of the `ContractManagementWindow`
    internal class PlannerPanel
    {
        private ContractBlueprintEditingPanel? _contractBlueprintEditingPanel = null;
        private MissionBlueprintEditingPanel? _missionBlueprintEditingPanel = null;

        internal PlannerPanel() { }

        internal void DrawPlannerLeftPanel()
        {
            // Left panel: fixed width and fill available height so it becomes scrollable when content overflows
            var style = ImGui.GetStyle();
            float leftPanelWidth = 260.0f;
            Brutal.Numerics.float2 leftPanelSize = new Brutal.Numerics.float2 { X = leftPanelWidth, Y = 0.0f };
            ImGui.PushStyleVar(ImGuiStyleVar.WindowPadding, new Brutal.Numerics.float2 { X = 0.0f, Y = 0.0f });
            if (ImGui.BeginChild("PlannerLeftPanel", leftPanelSize, ImGuiChildFlags.None, ImGuiWindowFlags.None))
            {
                if (
                    ContractManager.contractManagementWindow.rightPanelDetailSubType == RightPanelDetailType.NONE &&
                    ContractManager.contractManagementWindow.rightPanelDetailType is RightPanelDetailType.NONE or RightPanelDetailType.CONTRACTBLUEPRINT or RightPanelDetailType.MISSIONBLUEPRINT)
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
                        List<LeftPanelListItem> listItems = LeftPanelListItem.GetLeftPanelListItems(ContractManager.contractManagementWindow, ContractManager.data.missionBlueprints);
                        ImGui.Text(String.Format("list: {0}", listItems.Count));
                        ContractManager.contractManagementWindow.DrawItemList(listItems, "planner_missionBlueprints");
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
                        List<LeftPanelListItem> listItems = LeftPanelListItem.GetLeftPanelListItems(ContractManager.contractManagementWindow, ContractManager.data.contractBlueprints);
                        ImGui.Text(String.Format("list: {0}", listItems.Count));
                        ContractManager.contractManagementWindow.DrawItemList(listItems, "planner_contractBlueprints");
                        ImGui.EndChild();  // End of LeftContractPanel
                    }
                }
                else
                if (ContractManager.contractManagementWindow.rightPanelDetailSubType == RightPanelDetailType.PREREQUISITE)
                {
                    ImGui.SeparatorText("Prerequisites");
                    if (ImGui.ArrowButton("##planner_leftPrerequisites_return", ImGuiDir.Left))
                    {
                        ContractManager.contractManagementWindow.rightPanelDetailSubType = RightPanelDetailType.NONE;
                        ContractManager.contractManagementWindow.rightPanelDetailSubUID = string.Empty;
                    }
                }
                else
                if (ContractManager.contractManagementWindow.rightPanelDetailSubType == RightPanelDetailType.REQUIREMENT)
                {
                    ImGui.SeparatorText("Requirements");
                    if (ImGui.ArrowButton("##planner_leftRequirements_return", ImGuiDir.Left))
                    {
                        ContractManager.contractManagementWindow.rightPanelDetailSubType = RightPanelDetailType.NONE;
                        ContractManager.contractManagementWindow.rightPanelDetailSubUID = string.Empty;
                    }
                }
                else
                if (ContractManager.contractManagementWindow.rightPanelDetailSubType == RightPanelDetailType.ACTION)
                {
                    ImGui.SeparatorText("Actions");
                    if (ImGui.ArrowButton("##planner_leftActions_return", ImGuiDir.Left))
                    {
                        ContractManager.contractManagementWindow.rightPanelDetailSubType = RightPanelDetailType.NONE;
                        ContractManager.contractManagementWindow.rightPanelDetailSubUID = string.Empty;
                    }
                }
                else
                {
                    // TODO: switching from manager to planner type while contract/mission selected will trigger this.
                    ImGui.Text(String.Format("Unhandled RightPanelDetailType: {0}", ContractManager.contractManagementWindow.rightPanelDetailType));
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
                    if (ContractManager.contractManagementWindow.rightPanelDetailType == RightPanelDetailType.NONE || ContractManager.contractManagementWindow.rightPanelDetailUID == string.Empty)
                    {
                        ImGui.TextWrapped("Select an item from the left panel to view details here.");
                    }
                    else
                    if (ContractManager.contractManagementWindow.rightPanelDetailType == RightPanelDetailType.CONTRACTBLUEPRINT)
                    {
                        ContractBlueprint.ContractBlueprint? contractBlueprintToShow = ContractUtils.FindContractBlueprintFromUID(
                            ContractManager.data.contractBlueprints,
                            ContractManager.contractManagementWindow.rightPanelDetailUID
                        );
                        if (contractBlueprintToShow != null) {
                            this.DrawContractBlueprintDetails(contractBlueprintToShow);
                        }
                        else
                        {
                            // Something bad happened to the detail UID, it became corrupted.
                            ContractManager.contractManagementWindow.rightPanelDetailType = RightPanelDetailType.NONE;
                            ContractManager.contractManagementWindow.rightPanelDetailUID = string.Empty;
                        }
                    }
                    else
                    if (ContractManager.contractManagementWindow.rightPanelDetailType == RightPanelDetailType.MISSIONBLUEPRINT)
                    {
                        Mission.MissionBlueprint? missionBlueprintToShow = MissionUtils.FindMissionBlueprintFromUID(
                            ContractManager.data.missionBlueprints,
                            ContractManager.contractManagementWindow.rightPanelDetailUID
                        );
                        if (missionBlueprintToShow != null) {
                            this.DrawMissionBlueprintDetails(missionBlueprintToShow);
                        }
                        else
                        {
                            // Something bad happened to the detail UID, it became corrupted.
                            ContractManager.contractManagementWindow.rightPanelDetailType = RightPanelDetailType.NONE;
                            ContractManager.contractManagementWindow.rightPanelDetailUID = string.Empty;
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

        internal void DrawMissionBlueprintDetails(Mission.MissionBlueprint missionBlueprintToShow)
        {
            if (this._missionBlueprintEditingPanel == null)
            {
                this._missionBlueprintEditingPanel = new MissionBlueprintEditingPanel(ref missionBlueprintToShow);
            }
            else
            if (this._missionBlueprintEditingPanel.blueprintUID != missionBlueprintToShow.uid)
            {
                this._missionBlueprintEditingPanel = new MissionBlueprintEditingPanel(ref missionBlueprintToShow);
            }
            this._missionBlueprintEditingPanel.Draw();
        }
    }

    internal class ContractBlueprintEditingPanel
    {
        private ContractBlueprint.ContractBlueprint _contractBlueprint;
        internal string blueprintUID = string.Empty;

        // variables for inputs
        private Brutal.ImGuiApi.ImInputString _blueprintUID;
        private Brutal.ImGuiApi.ImInputString _title;
        private Brutal.ImGuiApi.ImInputString _synopsis;
        private Brutal.ImGuiApi.ImInputString _description;
        private double _expiration;
        private bool _isRejectable;
        private double _deadline;
        private bool _isAutoAccepted;


        internal ContractBlueprintEditingPanel(ref ContractBlueprint.ContractBlueprint contractBlueprintToEdit)
        {
            this._contractBlueprint = contractBlueprintToEdit;
            this.blueprintUID = contractBlueprintToEdit.uid;
            this._blueprintUID = new ImInputString(ContractBlueprint.ContractBlueprint.uidMaxLength, contractBlueprintToEdit.uid);
            this._title = new ImInputString(ContractBlueprint.ContractBlueprint.titleMaxLength, contractBlueprintToEdit.title);
            this._synopsis = new ImInputString(ContractBlueprint.ContractBlueprint.synopsisMaxLength, contractBlueprintToEdit.synopsis);
            this._description = new ImInputString(ContractBlueprint.ContractBlueprint.descriptionMaxLength, contractBlueprintToEdit.description);
            this._expiration = contractBlueprintToEdit.expiration;
            this._isRejectable = contractBlueprintToEdit.isRejectable;
            this._deadline = contractBlueprintToEdit.deadline;
            this._isAutoAccepted = contractBlueprintToEdit.isAutoAccepted;
        }

        internal void Draw()
        {
            // Draw contract blueprint details
            ImGui.SeparatorText("Contract blueprint : " + this._contractBlueprint.title);

            if (ImGui.BeginTable("PlannerRightPanel_EditContractBlueprint", 2))
            {
                ImGui.TableSetupColumn("Field", ImGuiTableColumnFlags.WidthFixed);
                ImGui.TableSetupColumn("Input", ImGuiTableColumnFlags.WidthStretch);

                ImGui.TableNextRow();
                ImGui.TableNextColumn();
                ImGui.Text("blueprintUID:");
                ImGui.SameLine();
                ContractManagementWindow.DrawHelpTooltip("The Unique Identifier of this contract blueprint. Needs to be unique across all contract blueprints. (max 64)");
                ImGui.TableNextColumn();
                ImGui.SetNextItemWidth(-1.0f); // make the input use the full width.
                if (ImGui.InputText("##ContractBlueprint_input_blueprintUID", this._blueprintUID))
                {
                    this._contractBlueprint.uid = this._blueprintUID.ToString();
                    // Update to prevent editing this will not showing this blueprint.
                    ContractManager.contractManagementWindow.rightPanelDetailUID = this._contractBlueprint.uid;
                }
                
                ImGui.TableNextRow();
                ImGui.TableNextColumn();
                ImGui.Text("missionBlueprintUID:");
                ImGui.SameLine();
                ContractManagementWindow.DrawHelpTooltip("The optional mission this contract blueprint is part of.");
                ImGui.TableNextColumn();
                ImGui.SetNextItemWidth(-1.0f); // make the input use the full width.
                string selectedMissionBlueprint = "None";
                if (!string.IsNullOrEmpty(this._contractBlueprint.missionBlueprintUID))
                {
                    Mission.MissionBlueprint? mission = MissionUtils.FindMissionBlueprintFromUID(ContractManager.data.missionBlueprints, this._contractBlueprint.missionBlueprintUID);
                    if (mission != null)
                    {
                        selectedMissionBlueprint = mission.title;
                    }
                }
                if (ImGui.BeginCombo("##ContractBlueprint_combo_missionBlueprintUID", selectedMissionBlueprint))
                {
                    foreach  (Mission.MissionBlueprint missionBlueprint in ContractManager.data.missionBlueprints)
                    {
                        if (ImGui.Selectable(missionBlueprint.title))
                        {
                            this._contractBlueprint.missionBlueprintUID = missionBlueprint.uid;
                        }
                        if (selectedMissionBlueprint == missionBlueprint.title)
                        {
                            ImGui.SetItemDefaultFocus();
                        }
                    }
                    if (ImGui.Selectable("None"))
                    {
                        this._contractBlueprint.missionBlueprintUID = string.Empty;
                    }
                    if (selectedMissionBlueprint == "None")
                    {
                        ImGui.SetItemDefaultFocus();
                    }
                    ImGui.EndCombo();
                }
                
                ImGui.TableNextRow();
                ImGui.TableNextColumn();
                ImGui.Text("title:");
                ImGui.SameLine();
                ContractManagementWindow.DrawHelpTooltip("The title of the contract (blueprint). Should be something short and comprehensible. (max 64)");
                ImGui.TableNextColumn();
                ImGui.SetNextItemWidth(-1.0f); // make the input use the full width.
                if (ImGui.InputText("##ContractBlueprint_input_title", this._title))
                {
                    this._contractBlueprint.title = this._title.ToString();
                }
                
                ImGui.TableNextRow();
                ImGui.TableNextColumn();
                ImGui.Text("Synopsis:");
                ImGui.SameLine();
                ContractManagementWindow.DrawHelpTooltip("The synopsis; A short description or summary of the contract. (max 1024)");
                ImGui.TableNextColumn();
                Brutal.Numerics.float2 synopsisSize = new Brutal.Numerics.float2 { X = 0.0f, Y = 4 * ImGui.GetTextLineHeight() };
                ImGui.SetNextItemWidth(-1.0f); // make the input use the full width.
                if (ImGui.InputTextMultiline("##ContractBlueprint_input_synopsis", this._synopsis, synopsisSize, ImGuiInputTextFlags.None))
                {
                    this._contractBlueprint.synopsis = this._synopsis.ToString();
                }
                
                ImGui.TableNextRow();
                ImGui.TableNextColumn();
                ImGui.Text("Description:");
                ImGui.SameLine();
                ContractManagementWindow.DrawHelpTooltip("The full description of the contract. Can be fairly long and have new lines (ctrl+enter). Could be used to tell a story like in campaign mode. (max 4096)");
                ImGui.TableNextColumn();
                Brutal.Numerics.float2 descriptionSize = new Brutal.Numerics.float2 { X = 0.0f, Y = 16 * ImGui.GetTextLineHeight() };
                ImGui.SetNextItemWidth(-1.0f); // make the input use the full width.
                if (ImGui.InputTextMultiline("##ContractBlueprint_input_description", this._description, descriptionSize, ImGuiInputTextFlags.CtrlEnterForNewLine))
                {
                    this._contractBlueprint.description = this._description.ToString();
                }
                
                ImGui.TableNextRow();
                ImGui.TableNextColumn();
                ImGui.Text("Expiration:");
                ImGui.SameLine();
                ContractManagementWindow.DrawHelpTooltip("The time in seconds after the contract is offered for it to expire. Use 'inf' to disable.");
                ImGui.TableNextColumn();
                ImGui.SetNextItemWidth(-1.0f); // make the input use the full width.
                if (ImGui.InputDouble("##ContractBlueprint_input_expiration", ref this._expiration, -1.0, Double.PositiveInfinity, "%.0f"))
                {
                    this._contractBlueprint.expiration = this._expiration;
                }
                
                ImGui.TableNextRow();
                ImGui.TableNextColumn();
                ImGui.Text("Rejectable:");
                ImGui.SameLine();
                ContractManagementWindow.DrawHelpTooltip("Flag whether the contract can be rejected. Use this to make a contract that the player won't ignore so easily.");
                ImGui.TableNextColumn();
                ImGui.SetNextItemWidth(-1.0f); // make the input use the full width.
                if (ImGui.Checkbox("##ContractBlueprint_input_isRejectable", ref this._isRejectable))
                {
                    this._contractBlueprint.isRejectable = this._isRejectable;
                }
                
                ImGui.TableNextRow();
                ImGui.TableNextColumn();
                ImGui.Text("Deadline:");
                ImGui.SameLine();
                ContractManagementWindow.DrawHelpTooltip("The time in seconds after the contract is accepted for it to expire, and it will fail. Use 'inf' to disable.");
                ImGui.TableNextColumn();
                ImGui.SetNextItemWidth(-1.0f); // make the input use the full width.
                if (ImGui.InputDouble("##ContractBlueprint_input_deadline", ref this._deadline, -1.0, Double.PositiveInfinity, "%.0f"))
                {
                    this._contractBlueprint.deadline = this._deadline;
                }
                
                ImGui.TableNextRow();
                ImGui.TableNextColumn();
                ImGui.Text("Auto accepted:");
                ImGui.SameLine();
                ContractManagementWindow.DrawHelpTooltip("Flag whether the contract is automatically accepted. Use this to make a contract that the player has to do, for example to make a campaign-like experience..");
                ImGui.TableNextColumn();
                ImGui.SetNextItemWidth(-1.0f); // make the input use the full width.
                if (ImGui.Checkbox("##ContractBlueprint_input_isAutoAccepted", ref this._isAutoAccepted))
                {
                    this._contractBlueprint.isAutoAccepted = this._isAutoAccepted;
                }
                ImGui.EndTable();
            }
            
            ImGui.SeparatorText("Prerequisites");
            ImGuiTreeNodeFlags prerequisiteTreeNodeFlags = ImGuiTreeNodeFlags.DefaultOpen | ImGuiTreeNodeFlags.DrawLinesToNodes;
            if (ImGui.TreeNodeEx("Contract prerequisites:", prerequisiteTreeNodeFlags))
            {
                foreach (ContractBlueprint.Prerequisite prerequisite in this._contractBlueprint.prerequisites)
                {
                    this.DrawPrerequisiteTreeNode(prerequisite);
                }
                ImGui.TreePop();
            }

            ImGui.SeparatorText("Requirements");
            ImGui.Text("Contract completion condition:");
            ImGui.SameLine();
            ContractManagementWindow.DrawHelpTooltip("Which part of the requirements need to be achieved for the contract to be completed.");
            ImGui.SetNextItemWidth(-1.0f); // make the input use the full width.
            string selectedCompletionCondition = this._contractBlueprint.completionCondition.ToString();
            if (ImGui.BeginCombo("##ContractBlueprint_combo_missionBlueprintUID", selectedCompletionCondition))
            {
                if (ImGui.Selectable(CompletionCondition.All.ToString()))
                {
                    this._contractBlueprint.completionCondition = CompletionCondition.All;
                }
                if (selectedCompletionCondition == CompletionCondition.All.ToString())
                {
                    ImGui.SetItemDefaultFocus();
                }
                if (ImGui.Selectable(CompletionCondition.Any.ToString()))
                {
                    this._contractBlueprint.completionCondition = CompletionCondition.Any;
                }
                if (selectedCompletionCondition == CompletionCondition.Any.ToString())
                {
                    ImGui.SetItemDefaultFocus();
                }
                ImGui.EndCombo();
            }
            ImGuiTreeNodeFlags requirementTreeNodeFlags = ImGuiTreeNodeFlags.DefaultOpen | ImGuiTreeNodeFlags.DrawLinesToNodes;
            if (ImGui.TreeNodeEx("Contract requirements:", requirementTreeNodeFlags))
            {
                foreach (ContractBlueprint.Requirement requirement in this._contractBlueprint.requirements)
                {
                    this.DrawRequirementTreeNode(requirement);
                }
                ImGui.TreePop();
            }
            
            
            ImGui.SeparatorText("Actions");
            ImGuiTreeNodeFlags actionTreeNodeFlags = ImGuiTreeNodeFlags.DefaultOpen | ImGuiTreeNodeFlags.DrawLinesToNodes;
            if (ImGui.TreeNodeEx("Contract actions:", actionTreeNodeFlags))
            {
                foreach (ContractBlueprint.Action action in this._contractBlueprint.actions)
                {
                    this.DrawActionTreeNode(action);
                }
                ImGui.TreePop();
            }
             
            ImGui.SeparatorText("Debug");           
            ImGui.Text(String.Format("uid: {0}", this._contractBlueprint.uid));
            ImGui.Text(String.Format("missionBlueprintUID: {0}", this._contractBlueprint.missionBlueprintUID));
            ImGui.Text(String.Format("title: {0}", this._contractBlueprint.title));
            ImGui.Text("synopsis:");
            ImGui.TextWrapped(this._contractBlueprint.synopsis);
            ImGui.Text("description:");
            ImGui.TextWrapped(this._contractBlueprint.description);
            ImGui.Text(String.Format("expiration: {0}", this._contractBlueprint.expiration));
            ImGui.Text(String.Format("isRejectable: {0}", this._contractBlueprint.isRejectable));
            ImGui.Text(String.Format("deadline: {0}", this._contractBlueprint.deadline));
            ImGui.Text(String.Format("isAutoAccepted: {0}", this._contractBlueprint.isAutoAccepted));
            ImGui.Text(String.Format("completionCondition: {0}", this._contractBlueprint.completionCondition.ToString()));
        }
        

        internal void DrawPrerequisiteTreeNode(ContractBlueprint.Prerequisite prerequisite)
        {
            var style = ImGui.GetStyle();
            float buttonEditWidth = ImGui.CalcTextSize("Edit").X + style.FramePadding.X * 2.0f;
            float buttonDeleteWidth = ImGui.CalcTextSize("Delete").X + style.FramePadding.X * 2.0f;
            float ContentRegionAvailWidth = ImGui.GetContentRegionAvail().X;
            float triangleWidth = 20.0f * 2.0f;  // Width of the tree node triangle to fold the node. (twice because of the parent?)
            float maxTitleWidth = ContentRegionAvailWidth - buttonEditWidth - buttonDeleteWidth - triangleWidth;
            // Ensure the title can fit in a way to leave space for the buttons
            string titleForNode = prerequisite.type.ToString(); // TODO: convert to easier to read strings
            float textSize = ImGui.CalcTextSize(titleForNode).X + style.FramePadding.X * 2.0f;
            while (textSize > maxTitleWidth)
            {
                titleForNode = titleForNode[0..^3] + "..";
                textSize = ImGui.CalcTextSize(titleForNode).X + style.FramePadding.X * 2.0f;
            }
            if (ImGui.TreeNodeEx(String.Format("{0}##{1}", titleForNode, prerequisite.uid), ImGuiTreeNodeFlags.DefaultOpen | ImGuiTreeNodeFlags.DrawLinesToNodes))
            {
                ImGui.SameLine();
                ImGui.SetCursorPosX(ImGui.GetCursorPosX() + ContentRegionAvailWidth - textSize - buttonEditWidth - buttonDeleteWidth - triangleWidth);
                if (ImGui.SmallButton("Edit"))
                {
                    ContractManager.contractManagementWindow.rightPanelDetailSubType = RightPanelDetailType.PREREQUISITE;
                    ContractManager.contractManagementWindow.rightPanelDetailSubUID = prerequisite.uid;
                }
                ImGui.SameLine();
                if (ImGui.SmallButton("Delete"))
                {
                    // TODO: need to set some flag to edit the prerequisites outside of the foreach loop
                }
                ImGui.TreePop();
            }
        }


        internal void DrawRequirementTreeNode(ContractBlueprint.Requirement requirement)
        {
            var style = ImGui.GetStyle();
            float buttonEditWidth = ImGui.CalcTextSize("Edit").X + style.FramePadding.X * 2.0f;
            float buttonDeleteWidth = ImGui.CalcTextSize("Delete").X + style.FramePadding.X * 2.0f;
            float ContentRegionAvaiablelWidth = ImGui.GetContentRegionAvail().X;
            float triangleWidth = 20.0f * 2;  // Width of the tree node triangle to fold the node. (twice because of the parent?)
            float maxTitleWidth = ContentRegionAvaiablelWidth - buttonEditWidth - buttonDeleteWidth - triangleWidth;
            // Ensure the title can fit in a way to leave space for the buttons
            string titleForNode = requirement.title;
            float textSize = ImGui.CalcTextSize(titleForNode).X + style.FramePadding.X * 2.0f;
            while (textSize > maxTitleWidth)
            {
                titleForNode = titleForNode[0..^3] + "..";
                textSize = ImGui.CalcTextSize(titleForNode).X + style.FramePadding.X * 2.0f;
            }
            if (ImGui.TreeNodeEx(String.Format("{0}##{1}", titleForNode, requirement.uid), ImGuiTreeNodeFlags.DefaultOpen | ImGuiTreeNodeFlags.DrawLinesToNodes))
            {
                ImGui.SameLine();
                ImGui.SetCursorPosX(ImGui.GetCursorPosX() + ContentRegionAvaiablelWidth - textSize - buttonEditWidth - buttonDeleteWidth - triangleWidth);
                if (ImGui.SmallButton("Edit"))
                {
                    ContractManager.contractManagementWindow.rightPanelDetailSubType = RightPanelDetailType.REQUIREMENT;
                    ContractManager.contractManagementWindow.rightPanelDetailSubUID = requirement.uid;
                }
                ImGui.SameLine();
                if (ImGui.SmallButton("Delete"))
                {
                    // TODO: need to set some flag to edit the requirements outside of the foreach loop
                }
                if (requirement.type == RequirementType.Group && requirement.group != null)
                {
                    foreach (ContractBlueprint.Requirement childRequirement in requirement.group.requirements)
                    {
                        this.DrawRequirementTreeNode(childRequirement);
                    }
                }
                ImGui.TreePop();
            }
        }

        internal void DrawActionTreeNode(ContractBlueprint.Action action)
        {
            var style = ImGui.GetStyle();
            float buttonEditWidth = ImGui.CalcTextSize("Edit").X + style.FramePadding.X * 2.0f;
            float buttonDeleteWidth = ImGui.CalcTextSize("Delete").X + style.FramePadding.X * 2.0f;
            float ContentRegionAvailWidth = ImGui.GetContentRegionAvail().X;
            float triangleWidth = 20.0f * 2.0f;  // Width of the tree node triangle to fold the node. (twice because of the parent?)
            float maxTitleWidth = ContentRegionAvailWidth - buttonEditWidth - buttonDeleteWidth - triangleWidth;
            // Ensure the title can fit in a way to leave space for the buttons
            string titleForNode = action.type.ToString(); // TODO: convert to easier to read strings
            float textSize = ImGui.CalcTextSize(titleForNode).X + style.FramePadding.X * 2.0f;
            while (textSize > maxTitleWidth)
            {
                titleForNode = titleForNode[0..^3] + "..";
                textSize = ImGui.CalcTextSize(titleForNode).X + style.FramePadding.X * 2.0f;
            }
            if (ImGui.TreeNodeEx(String.Format("{0}##{1}", titleForNode, action.uid), ImGuiTreeNodeFlags.DefaultOpen | ImGuiTreeNodeFlags.DrawLinesToNodes))
            {
                ImGui.SameLine();
                ImGui.SetCursorPosX(ImGui.GetCursorPosX() + ContentRegionAvailWidth - textSize - buttonEditWidth - buttonDeleteWidth - triangleWidth);
                if (ImGui.SmallButton("Edit"))
                {
                    ContractManager.contractManagementWindow.rightPanelDetailSubType = RightPanelDetailType.ACTION;
                    ContractManager.contractManagementWindow.rightPanelDetailSubUID = action.uid;
                }
                ImGui.SameLine();
                if (ImGui.SmallButton("Delete"))
                {
                    // TODO: need to set some flag to edit the actions outside of the foreach loop
                }
                ImGui.TreePop();
            }
        }
    }

    internal class MissionBlueprintEditingPanel
    {
        private Mission.MissionBlueprint _missionBlueprint;
        internal string blueprintUID = string.Empty;

        // variables for inputs
        private Brutal.ImGuiApi.ImInputString _blueprintUID;
        private Brutal.ImGuiApi.ImInputString _title;
        private Brutal.ImGuiApi.ImInputString _synopsis;
        private Brutal.ImGuiApi.ImInputString _description;
        private double _expiration;
        private bool _isRejectable;
        private double _deadline;
        private bool _isAutoAccepted;

        internal MissionBlueprintEditingPanel(ref Mission.MissionBlueprint missionBlueprintToEdit)
        {
            this._missionBlueprint = missionBlueprintToEdit;
            this.blueprintUID = missionBlueprintToEdit.uid;
            this._blueprintUID = new ImInputString(64, missionBlueprintToEdit.uid);
            this._title = new ImInputString(64, missionBlueprintToEdit.title);
            this._synopsis = new ImInputString(1024, missionBlueprintToEdit.synopsis);
            this._description = new ImInputString(4096, missionBlueprintToEdit.description);
            this._expiration = missionBlueprintToEdit.expiration;
            this._isRejectable = missionBlueprintToEdit.isRejectable;
            this._deadline = missionBlueprintToEdit.deadline;
            this._isAutoAccepted = missionBlueprintToEdit.isAutoAccepted;
        }

        internal void Draw()
        {
            ImGui.SeparatorText("Mission blueprint : " + this._missionBlueprint.title);

            if (ImGui.BeginTable("PlannerRightPanel_EditMissionBlueprint", 2))
            {
                ImGui.TableSetupColumn("Field", ImGuiTableColumnFlags.WidthFixed);
                ImGui.TableSetupColumn("Input", ImGuiTableColumnFlags.WidthStretch);

                ImGui.TableNextRow();
                ImGui.TableNextColumn();
                ImGui.Text("blueprintUID:");
                ImGui.SameLine();
                ContractManagementWindow.DrawHelpTooltip("The Unique Identifier of this mission blueprint. Needs to be unique across all mission blueprints. (max 64)");
                ImGui.TableNextColumn();
                ImGui.SetNextItemWidth(-1.0f);
                if (ImGui.InputText("##MissionBlueprint_input_blueprintUID", this._blueprintUID))
                {
                    this._missionBlueprint.uid = this._blueprintUID.ToString();
                }

                ImGui.TableNextRow();
                ImGui.TableNextColumn();
                ImGui.Text("title:");
                ImGui.SameLine();
                ContractManagementWindow.DrawHelpTooltip("The title of the mission blueprint. Should be something short and comprehensible. (max 64)");
                ImGui.TableNextColumn();
                ImGui.SetNextItemWidth(-1.0f);
                if (ImGui.InputText("##MissionBlueprint_input_title", this._title))
                {
                    this._missionBlueprint.title = this._title.ToString();
                }

                ImGui.TableNextRow();
                ImGui.TableNextColumn();
                ImGui.Text("Synopsis:");
                ImGui.SameLine();
                ContractManagementWindow.DrawHelpTooltip("A short description or summary of the mission. (max 1024)");
                ImGui.TableNextColumn();
                Brutal.Numerics.float2 synopsisSize = new Brutal.Numerics.float2 { X = 0.0f, Y = 4 * ImGui.GetTextLineHeight() };
                ImGui.SetNextItemWidth(-1.0f);
                if (ImGui.InputTextMultiline("##MissionBlueprint_input_synopsis", this._synopsis, synopsisSize, ImGuiInputTextFlags.None))
                {
                    this._missionBlueprint.synopsis = this._synopsis.ToString();
                }

                ImGui.TableNextRow();
                ImGui.TableNextColumn();
                ImGui.Text("Description:");
                ImGui.SameLine();
                ContractManagementWindow.DrawHelpTooltip("The full description of the mission. Can be fairly long and have new lines (ctrl+enter). (max 4096)");
                ImGui.TableNextColumn();
                Brutal.Numerics.float2 descriptionSize = new Brutal.Numerics.float2 { X = 0.0f, Y = 16 * ImGui.GetTextLineHeight() };
                ImGui.SetNextItemWidth(-1.0f);
                if (ImGui.InputTextMultiline("##MissionBlueprint_input_description", this._description, descriptionSize, ImGuiInputTextFlags.CtrlEnterForNewLine))
                {
                    this._missionBlueprint.description = this._description.ToString();
                }

                ImGui.TableNextRow();
                ImGui.TableNextColumn();
                ImGui.Text("Expiration:");
                ImGui.SameLine();
                ContractManagementWindow.DrawHelpTooltip("The time in seconds after the mission is offered for it to expire. Use 'inf' to disable.");
                ImGui.TableNextColumn();
                ImGui.SetNextItemWidth(-1.0f);
                if (ImGui.InputDouble("##MissionBlueprint_input_expiration", ref this._expiration, -1.0, Double.PositiveInfinity, "%.0f"))
                {
                    this._missionBlueprint.expiration = this._expiration;
                }

                ImGui.TableNextRow();
                ImGui.TableNextColumn();
                ImGui.Text("Rejectable:");
                ImGui.SameLine();
                ContractManagementWindow.DrawHelpTooltip("Flag whether the mission can be rejected.");
                ImGui.TableNextColumn();
                ImGui.SetNextItemWidth(-1.0f);
                if (ImGui.Checkbox("##MissionBlueprint_input_isRejectable", ref this._isRejectable))
                {
                    this._missionBlueprint.isRejectable = this._isRejectable;
                }

                ImGui.TableNextRow();
                ImGui.TableNextColumn();
                ImGui.Text("Deadline:");
                ImGui.SameLine();
                ContractManagementWindow.DrawHelpTooltip("The time in seconds after the mission is accepted for it to expire, and it will fail. Use 'inf' to disable.");
                ImGui.TableNextColumn();
                ImGui.SetNextItemWidth(-1.0f);
                if (ImGui.InputDouble("##MissionBlueprint_input_deadline", ref this._deadline, -1.0, Double.PositiveInfinity, "%.0f"))
                {
                    this._missionBlueprint.deadline = this._deadline;
                }

                ImGui.TableNextRow();
                ImGui.TableNextColumn();
                ImGui.Text("Auto accepted:");
                ImGui.SameLine();
                ContractManagementWindow.DrawHelpTooltip("Flag whether the mission is automatically accepted.");
                ImGui.TableNextColumn();
                ImGui.SetNextItemWidth(-1.0f);
                if (ImGui.Checkbox("##MissionBlueprint_input_isAutoAccepted", ref this._isAutoAccepted))
                {
                    this._missionBlueprint.isAutoAccepted = this._isAutoAccepted;
                }

                ImGui.EndTable();
            }

            ImGui.Text(String.Format("uid: {0}", this._missionBlueprint.uid));
            ImGui.Text(String.Format("title: {0}", this._missionBlueprint.title));
            ImGui.Text("synopsis:");
            ImGui.TextWrapped(this._missionBlueprint.synopsis);
            ImGui.Text("description:");
            ImGui.TextWrapped(this._missionBlueprint.description);
            ImGui.Text(String.Format("expiration: {0}", this._missionBlueprint.expiration));
            ImGui.Text(String.Format("isRejectable: {0}", this._missionBlueprint.isRejectable));
            ImGui.Text(String.Format("deadline: {0}", this._missionBlueprint.deadline));
            ImGui.Text(String.Format("isAutoAccepted: {0}", this._missionBlueprint.isAutoAccepted));
        }
    }
}
