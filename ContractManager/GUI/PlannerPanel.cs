using Brutal.ImGuiApi;
using Brutal.Numerics;
using ContractManager;
using ContractManager.ContractBlueprint;
using KSA;
using System.Numerics;

namespace ContractManager.GUI
{
    // The panel that goes inside the "Mission Planner" tab of the `ContractManagementWindow`
    internal class PlannerPanel
    {
        private ContractBlueprintEditingPanel? _contractBlueprintEditingPanel = null;
        private MissionBlueprintEditingPanel? _missionBlueprintEditingPanel = null;
        private RequirementEditingPanel? _requirementEditingPanel = null;
        private ActionEditingPanel? _actionEditingPanel = null;

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
                var tabLeftPanelRegionSize = ImGui.GetContentRegionAvail();
                float leftMissionPanelHeightRatio = 0.3f;
                
                Brutal.Numerics.float2 leftMissionPanelSize = new Brutal.Numerics.float2 {
                    X = leftPanelWidth,
                    Y = (tabLeftPanelRegionSize.Y * leftMissionPanelHeightRatio) - style.FramePadding.Y,
                };

                // If no sub-type is selected show mission and contracts on the left
                if (ContractManager.contractManagementWindow.rightPanelDetailSubType == RightPanelDetailType.NONE &&
                    ContractManager.contractManagementWindow.rightPanelDetailType is RightPanelDetailType.NONE or RightPanelDetailType.CONTRACTBLUEPRINT or RightPanelDetailType.MISSIONBLUEPRINT)
                {
                    // Draw Mission section
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
                // Contractblueprint is selected with a sub-type.
                if (ContractManager.contractManagementWindow.rightPanelDetailType == RightPanelDetailType.CONTRACTBLUEPRINT &&
                    ContractManager.contractManagementWindow.rightPanelDetailUID != string.Empty)
                { 
                    ContractBlueprint.ContractBlueprint? contractBlueprint = Contract.ContractUtils.FindContractBlueprintFromUID(
                        ContractManager.data.contractBlueprints,
                        ContractManager.contractManagementWindow.rightPanelDetailUID
                    );
                    if ( contractBlueprint == null ) {
                        // reset
                        ContractManager.contractManagementWindow.rightPanelDetailType = RightPanelDetailType.NONE;
                        ContractManager.contractManagementWindow.rightPanelDetailUID = string.Empty;
                    }
                    else
                    if (ContractManager.contractManagementWindow.rightPanelDetailSubType == RightPanelDetailType.REQUIREMENT)
                    {
                        // TODO: Figure out a way how to show the requirements in a group, use the left column?
                        //  separate by `,`?
                        if (ImGui.BeginChild("PlannerLeftRequirementsPanel", leftMissionPanelSize, ImGuiChildFlags.Borders, ImGuiWindowFlags.None))
                        {
                            Brutal.Numerics.float2 leftRequirementsPanel = ImGui.GetContentRegionAvail();
                            ImGui.SeparatorText("Requirements");

                            // return button
                            if (ImGui.ArrowButton("##planner_leftRequirements_return", ImGuiDir.Left))
                            {
                                ContractManager.contractManagementWindow.rightPanelDetailSubType = RightPanelDetailType.NONE;
                                ContractManager.contractManagementWindow.rightPanelDetailSubUID = string.Empty;

                            }
                            ImGui.SameLine();
                            float leftArrowButtonWidth = 35.0f + style.FramePadding.X * 2.0f;  // guestimate
                            float removeButtonWidth = ImGui.CalcTextSize("[-]").X + style.FramePadding.X * 2.0f;
                            float addButtonWidth = ImGui.CalcTextSize("[+]").X + style.FramePadding.X * 2.0f;
                            ImGui.SetCursorPosX(ImGui.GetCursorPosX() + leftRequirementsPanel.X - removeButtonWidth - addButtonWidth - leftArrowButtonWidth - (style.FramePadding.X * 2.0f));
                            // delete button (-)
                            if (ContractManager.contractManagementWindow.rightPanelDetailSubUID == string.Empty) { ImGui.BeginDisabled(); }  // TODO: check editable
                            if (ImGui.Button("[-]##planner_leftRequirements_delete"))
                            {
                                for (int requirementIndex = 0; requirementIndex < contractBlueprint.requirements.Count; requirementIndex++)
                                {
                                    if (contractBlueprint.requirements[requirementIndex].uid == ContractManager.contractManagementWindow.rightPanelDetailSubUID)
                                    {
                                        contractBlueprint.requirements.RemoveAt(requirementIndex);
                                        break;
                                    }
                                }
                            }
                            if (ContractManager.contractManagementWindow.rightPanelDetailSubUID == string.Empty) { ImGui.EndDisabled(); }
                            ImGui.SameLine();
                            // add button (+)
                            if (ImGui.Button("[+]##planner_leftRequirements_add"))
                            {
                                contractBlueprint.requirements.Add(
                                    new ContractBlueprint.Requirement
                                    {
                                        uid = String.Format("{0}_new_requirement_{1}", contractBlueprint.uid, KSA.Universe.GetElapsedSimTime().Seconds()),
                                        title = "!update me!"
                                    }
                                );
                            }

                            List<LeftPanelListItem> listItems = LeftPanelListItem.GetLeftPanelListItems(ContractManager.contractManagementWindow, contractBlueprint.requirements);
                            ContractManager.contractManagementWindow.DrawItemList(listItems, "planner_contractBlueprints_requirements");

                            ImGui.EndChild();  // End of LeftContractPanel
                        }
                    }
                    else
                    if (ContractManager.contractManagementWindow.rightPanelDetailSubType == RightPanelDetailType.ACTION)
                    {
                        if (ImGui.BeginChild("PlannerLeftActionsPanel", leftMissionPanelSize, ImGuiChildFlags.Borders, ImGuiWindowFlags.None))
                        {
                            if (ImGui.ArrowButton("##planner_leftActions_return", ImGuiDir.Left))
                            {
                                ContractManager.contractManagementWindow.rightPanelDetailSubType = RightPanelDetailType.NONE;
                                ContractManager.contractManagementWindow.rightPanelDetailSubUID = string.Empty;
                            }
                            ImGui.SameLine();
                            ImGui.SeparatorText("Actions");
                            ImGui.SameLine();
                            if (ImGui.Button("+##planner_leftActions_add"))
                            {

                            }
                            
                            List<LeftPanelListItem> listItems = LeftPanelListItem.GetLeftPanelListItems(ContractManager.contractManagementWindow, contractBlueprint.actions);
                            ImGui.Text(String.Format("list: {0}", listItems.Count));
                            ContractManager.contractManagementWindow.DrawItemList(listItems, "planner_contractBlueprints_actions");

                            ImGui.EndChild();  // End of LeftContractPanel
                        }
                    }
                    else
                    {
                        ImGui.Text(String.Format("Unhandled RightPanelDetailType: {0}", ContractManager.contractManagementWindow.rightPanelDetailType));
                    }
                }
                else
                {
                    ContractManager.contractManagementWindow.rightPanelDetailType = RightPanelDetailType.NONE;
                    ContractManager.contractManagementWindow.rightPanelDetailUID = string.Empty;
                    ContractManager.contractManagementWindow.rightPanelDetailSubType = RightPanelDetailType.NONE;
                    ContractManager.contractManagementWindow.rightPanelDetailSubUID = string.Empty;
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
                    // TODO: Remove these debug text
                    ImGui.Text(String.Format("ContractManager.contractManagementWindow.rightPanelDetailType: {0}", ContractManager.contractManagementWindow.rightPanelDetailType));
                    ImGui.Text(String.Format("ContractManager.contractManagementWindow.rightPanelDetailUID: {0}", ContractManager.contractManagementWindow.rightPanelDetailUID));
                    ImGui.Text(String.Format("ContractManager.contractManagementWindow.rightPanelDetailSubType: {0}", ContractManager.contractManagementWindow.rightPanelDetailSubType));
                    ImGui.Text(String.Format("ContractManager.contractManagementWindow.rightPanelDetailSubUID: {0}", ContractManager.contractManagementWindow.rightPanelDetailSubUID));
                    //------

                    if (ContractManager.contractManagementWindow.rightPanelDetailType == RightPanelDetailType.NONE || ContractManager.contractManagementWindow.rightPanelDetailUID == string.Empty)
                    {
                        ImGui.TextWrapped("Select an item from the left panel to view details here.");
                    }
                    else
                    if (ContractManager.contractManagementWindow.rightPanelDetailType == RightPanelDetailType.CONTRACTBLUEPRINT)
                    {
                        ContractBlueprint.ContractBlueprint? contractBlueprintToShow = Contract.ContractUtils.FindContractBlueprintFromUID(
                            ContractManager.data.contractBlueprints,
                            ContractManager.contractManagementWindow.rightPanelDetailUID
                        );
                        if (contractBlueprintToShow == null)
                        {
                            // Something bad happened to the detail UID, it became corrupted.
                            ContractManager.contractManagementWindow.rightPanelDetailType = RightPanelDetailType.NONE;
                            ContractManager.contractManagementWindow.rightPanelDetailUID = string.Empty;
                        }
                        else
                        if (ContractManager.contractManagementWindow.rightPanelDetailSubType == RightPanelDetailType.REQUIREMENT &&
                            ContractManager.contractManagementWindow.rightPanelDetailSubUID != string.Empty)
                        {
                            ContractBlueprint.Requirement? requirementToShow = Contract.ContractUtils.FindRequirementFromUID(
                                contractBlueprintToShow.requirements,
                                ContractManager.contractManagementWindow.rightPanelDetailSubUID
                            );
                            if (requirementToShow != null )
                            {
                                this.DrawRequirementDetails(requirementToShow);
                            }
                            else
                            {
                                ContractManager.contractManagementWindow.rightPanelDetailSubUID = string.Empty;
                            }
                        }
                        else
                        if (ContractManager.contractManagementWindow.rightPanelDetailSubType == RightPanelDetailType.ACTION &&
                            ContractManager.contractManagementWindow.rightPanelDetailSubUID != string.Empty)
                        {
                            ContractBlueprint.Action? actionToShow = Contract.ContractUtils.FindActionFromUID(
                                contractBlueprintToShow.actions,
                                ContractManager.contractManagementWindow.rightPanelDetailSubUID
                            );
                            if (actionToShow != null )
                            {
                                this.DrawActionDetails(actionToShow);
                            }
                            else
                            {
                                ContractManager.contractManagementWindow.rightPanelDetailSubUID = string.Empty;
                            }

                        }
                        else
                        {
                            this.DrawContractBlueprintDetails(contractBlueprintToShow);
                        }
                    }
                    else
                    if (ContractManager.contractManagementWindow.rightPanelDetailType == RightPanelDetailType.MISSIONBLUEPRINT)
                    {
                        Mission.MissionBlueprint? missionBlueprintToShow = Mission.MissionUtils.FindMissionBlueprintFromUID(
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
        
        internal void DrawRequirementDetails(ContractBlueprint.Requirement requirementToShow)
        {
            if (this._requirementEditingPanel == null)
            {
                this._requirementEditingPanel = new RequirementEditingPanel(ref requirementToShow);
            }
            else
            if (this._requirementEditingPanel.requirementUID != requirementToShow.uid)
            {
                this._requirementEditingPanel = new RequirementEditingPanel(ref requirementToShow);
            }
            this._requirementEditingPanel.Draw();
        }
        
        internal void DrawActionDetails(ContractBlueprint.Action actionToShow)
        {
            if (this._actionEditingPanel == null)
            {
                this._actionEditingPanel = new ActionEditingPanel(ref actionToShow);
            }
            else
            if (this._actionEditingPanel.actionUID != actionToShow.uid)
            {
                this._actionEditingPanel = new ActionEditingPanel(ref actionToShow);
            }
            this._actionEditingPanel.Draw();
        }
    }

    internal class ContractBlueprintEditingPanel
    {
        private ContractBlueprint.ContractBlueprint _contractBlueprint;
        internal string blueprintUID = string.Empty;
        private Prerequisite? _prerequisite = null;
        private PrerequisiteEditingPanel? _prerequisiteEditingPanel = null;

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
            this._prerequisite = contractBlueprintToEdit.prerequisite;
            this._prerequisiteEditingPanel = new PrerequisiteEditingPanel(ref this._prerequisite);
        }

        internal void Draw()
        {
            // Draw contract blueprint details
            ImGui.SeparatorText("Contract blueprint: " + this._contractBlueprint.title);

            if (ImGui.BeginTable("PlannerRightPanel_EditContractBlueprint", 3))
            {
                ImGui.TableSetupColumn("Label", ImGuiTableColumnFlags.WidthFixed);
                ImGui.TableSetupColumn("Input", ImGuiTableColumnFlags.WidthStretch);
                ImGui.TableSetupColumn("Required", ImGuiTableColumnFlags.WidthFixed);

                ImGui.TableNextRow();
                ImGui.TableNextColumn();
                ImGui.Text("uid:");
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
                ImGui.TableNextColumn();
                ImGui.Text("(*)");
                
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
                    Mission.MissionBlueprint? mission = Mission.MissionUtils.FindMissionBlueprintFromUID(ContractManager.data.missionBlueprints, this._contractBlueprint.missionBlueprintUID);
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
                ImGui.TableNextColumn();
                ImGui.Text("(*)");
                
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
                ContractManagementWindow.DrawHelpTooltip("The time in seconds after the contract is offered for it to expire. Use 'inf' to disable. Use ctrl to in/decrease by 1 day.");
                ImGui.TableNextColumn();
                ImGui.Text(Utils.FormatSimTimeAsRelative(new KSA.SimTime(this._contractBlueprint.expiration)));
                ImGui.SameLine();
                ImGui.SetNextItemWidth(-1.0f); // make the input use the full width.
                if (ImGui.InputDouble("##ContractBlueprint_input_expiration", ref this._expiration, 3600, 24*3600, "%.0f"))
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
                ContractManagementWindow.DrawHelpTooltip("The time in seconds after the contract is accepted for it to expire, and it will fail. Use 'inf' to disable.Use ctrl to in/decrease by 1 day.");
                ImGui.TableNextColumn();
                ImGui.Text(Utils.FormatSimTimeAsRelative(new KSA.SimTime(this._contractBlueprint.deadline)));
                ImGui.SameLine();
                ImGui.SetNextItemWidth(-1.0f); // make the input use the full width.
                if (ImGui.InputDouble("##ContractBlueprint_input_deadline", ref this._deadline, 3600, 24*3600, "%.0f"))
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
            
            ImGui.Text("(*): required.");
            
            ImGui.SeparatorText("Prerequisites");
            if (this._prerequisiteEditingPanel != null)
            {
                this._prerequisiteEditingPanel.Draw();
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

            // TODO: remove debug
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
        private Prerequisite? _prerequisite = null;
        private PrerequisiteEditingPanel? _prerequisiteEditingPanel = null;

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
            this._blueprintUID = new ImInputString(Mission.MissionBlueprint.uidMaxLength, missionBlueprintToEdit.uid);
            this._title = new ImInputString(Mission.MissionBlueprint.titleMaxLength, missionBlueprintToEdit.title);
            this._synopsis = new ImInputString(Mission.MissionBlueprint.synopsisMaxLength, missionBlueprintToEdit.synopsis);
            this._description = new ImInputString(Mission.MissionBlueprint.descriptionMaxLength, missionBlueprintToEdit.description);
            this._expiration = missionBlueprintToEdit.expiration;
            this._isRejectable = missionBlueprintToEdit.isRejectable;
            this._deadline = missionBlueprintToEdit.deadline;
            this._isAutoAccepted = missionBlueprintToEdit.isAutoAccepted;
            this._prerequisite = missionBlueprintToEdit.prerequisite;
            this._prerequisiteEditingPanel = new PrerequisiteEditingPanel(ref this._prerequisite);
        }

        internal void Draw()
        {
            ImGui.SeparatorText("Mission blueprint: " + this._missionBlueprint.title);

            if (ImGui.BeginTable("PlannerRightPanel_EditMissionBlueprint", 3))
            {
                ImGui.TableSetupColumn("Label", ImGuiTableColumnFlags.WidthFixed);
                ImGui.TableSetupColumn("Input", ImGuiTableColumnFlags.WidthStretch);
                ImGui.TableSetupColumn("Required", ImGuiTableColumnFlags.WidthFixed);

                ImGui.TableNextRow();
                ImGui.TableNextColumn();
                ImGui.Text("uid:");
                ImGui.SameLine();
                ContractManagementWindow.DrawHelpTooltip("The Unique Identifier of this mission blueprint. Needs to be unique across all mission blueprints. (max 64)");
                ImGui.TableNextColumn();
                ImGui.SetNextItemWidth(-1.0f);
                if (ImGui.InputText("##MissionBlueprint_input_blueprintUID", this._blueprintUID))
                {
                    this._missionBlueprint.uid = this._blueprintUID.ToString();
                }
                ImGui.TableNextColumn();
                ImGui.Text("(*)");

                ImGui.TableNextRow();
                ImGui.TableNextColumn();
                ImGui.Text("title:");
                ImGui.SameLine();
                ContractManagementWindow.DrawHelpTooltip("The title of the mission blueprint. Should be something short and comprehensible. (max 128)");
                ImGui.TableNextColumn();
                ImGui.SetNextItemWidth(-1.0f);
                if (ImGui.InputText("##MissionBlueprint_input_title", this._title))
                {
                    this._missionBlueprint.title = this._title.ToString();
                }
                ImGui.TableNextColumn();
                ImGui.Text("(*)");

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
                ContractManagementWindow.DrawHelpTooltip("The description of the mission. Can be fairly long and have new lines (ctrl+enter). Could be used as part of the story-line. (max 4096)");
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
                ContractManagementWindow.DrawHelpTooltip("The time in seconds after the mission is offered for it to expire. Use 'inf' to disable. Use ctrl to in/decrease by 1 day.");
                ImGui.TableNextColumn();
                ImGui.Text(Utils.FormatSimTimeAsRelative(new KSA.SimTime(this._missionBlueprint.expiration)));
                ImGui.SameLine();
                ImGui.SetNextItemWidth(-1.0f);
                if (ImGui.InputDouble("##MissionBlueprint_input_expiration", ref this._expiration, 3600, 24 * 3600, "%.0f"))
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
                ContractManagementWindow.DrawHelpTooltip("The time in seconds after the mission is accepted for it to expire, and it will fail. Use 'inf' to disable. Use ctrl to in/decrease by 1 day.");
                ImGui.TableNextColumn();
                ImGui.Text(Utils.FormatSimTimeAsRelative(new KSA.SimTime(this._missionBlueprint.deadline)));
                ImGui.SameLine();
                ImGui.SetNextItemWidth(-1.0f);
                if (ImGui.InputDouble("##MissionBlueprint_input_deadline", ref this._deadline, 3600, 24 * 3600, "%.0f"))
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
            
            ImGui.Text("(*): required.");
            
            ImGui.SeparatorText("Prerequisites");
            if (this._prerequisiteEditingPanel != null)
            {
                this._prerequisiteEditingPanel.Draw();
            }

            // TODO: remove
            ImGui.SeparatorText("Debug");
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

    internal class PrerequisiteEditingPanel
    {
        private ContractBlueprint.Prerequisite _prerequisite;
        internal string prerequisiteUID = string.Empty;

        // Input fields
        private int _selectedTypeIndex;
        private int _maxNumOfferedContracts;
        private int _maxNumAcceptedContracts;
        private int _maxNumOfferedMissions;
        private int _maxNumAcceptedMissions;
        private int _maxCompleteCount;
        private int _maxFailedCount;
        private int _maxConcurrentCount;
        private Brutal.ImGuiApi.ImInputString _hasCompletedContract;
        private Brutal.ImGuiApi.ImInputString _hasFailedContract;
        private Brutal.ImGuiApi.ImInputString _hasAcceptedContract;
        private Brutal.ImGuiApi.ImInputString _hasCompletedMission;
        private Brutal.ImGuiApi.ImInputString _hasFailedMission;
        private Brutal.ImGuiApi.ImInputString _hasAcceptedMission;
        private int _minNumberOfVessels;
        private int _maxNumberOfVessels;

        internal PrerequisiteEditingPanel(ref ContractBlueprint.Prerequisite prerequisiteToEdit)
        {
            this._prerequisite = prerequisiteToEdit;
            this._maxNumOfferedContracts = (int)prerequisiteToEdit.maxNumOfferedContracts;
            this._maxNumAcceptedContracts = (int)prerequisiteToEdit.maxNumAcceptedContracts;
            this._maxNumOfferedMissions = (int)prerequisiteToEdit.maxNumOfferedMissions;
            this._maxNumAcceptedMissions = (int)prerequisiteToEdit.maxNumAcceptedMissions;
            this._maxCompleteCount = (int)prerequisiteToEdit.maxCompleteCount;
            this._maxFailedCount = (int)prerequisiteToEdit.maxFailedCount;
            this._maxConcurrentCount = (int)prerequisiteToEdit.maxConcurrentCount;
            this._hasCompletedContract = new Brutal.ImGuiApi.ImInputString(ContractBlueprint.ContractBlueprint.uidMaxLength, prerequisiteToEdit.hasCompletedContract ?? string.Empty);
            this._hasFailedContract = new Brutal.ImGuiApi.ImInputString(ContractBlueprint.ContractBlueprint.uidMaxLength, prerequisiteToEdit.hasFailedContract ?? string.Empty);
            this._hasAcceptedContract = new Brutal.ImGuiApi.ImInputString(ContractBlueprint.ContractBlueprint.uidMaxLength, prerequisiteToEdit.hasAcceptedContract ?? string.Empty);
            this._hasCompletedMission = new Brutal.ImGuiApi.ImInputString(Mission.MissionBlueprint.uidMaxLength, prerequisiteToEdit.hasCompletedMission ?? string.Empty);
            this._hasFailedMission = new Brutal.ImGuiApi.ImInputString(Mission.MissionBlueprint.uidMaxLength, prerequisiteToEdit.hasFailedMission ?? string.Empty);
            this._hasAcceptedMission = new Brutal.ImGuiApi.ImInputString(Mission.MissionBlueprint.uidMaxLength, prerequisiteToEdit.hasAcceptedMission ?? string.Empty);
            this._minNumberOfVessels = (int)prerequisiteToEdit.minNumberOfVessels;
            this._maxNumberOfVessels = (int)prerequisiteToEdit.maxNumberOfVessels;
        }

        internal void Draw()
        {
            if (ImGui.BeginTable("PrerequisitePanelTable", 2))
            {
                ImGui.TableSetupColumn("Label", ImGuiTableColumnFlags.WidthFixed);
                ImGui.TableSetupColumn("Input", ImGuiTableColumnFlags.WidthStretch);

                // Show fields
                if (ContractManager.contractManagementWindow.rightPanelDetailType == RightPanelDetailType.CONTRACTBLUEPRINT)
                {
                    // MaxNumOfferedContracts:
                    ImGui.TableNextRow();
                    ImGui.TableNextColumn();
                    ImGui.Text("Max Offered Contracts:");
                    ImGui.SameLine();
                    ContractManagementWindow.DrawHelpTooltip("Offer contract if number of offered contracts is less than this number. Use -1 for unlimited.");
                    ImGui.TableNextColumn();
                    ImGui.SetNextItemWidth(-1.0f);
                    if (ImGui.InputInt("##Prerequisite_input_maxNumOfferedContracts", ref this._maxNumOfferedContracts))
                    {
                        if (this._maxNumOfferedContracts < -1)
                        {
                            this._maxNumOfferedContracts = -1;
                        }
                        else
                        {
                            this._prerequisite.maxNumOfferedContracts = (uint)this._maxNumOfferedContracts;
                        }
                    }
                    // MaxNumAcceptedContracts:
                    ImGui.TableNextRow();
                    ImGui.TableNextColumn();
                    ImGui.Text("Max Accepted Contracts:");
                    ImGui.SameLine();
                    ContractManagementWindow.DrawHelpTooltip("Offer contract if number of accepted contracts is less than this number. Use -1 for unlimited.");
                    ImGui.TableNextColumn();
                    ImGui.SetNextItemWidth(-1.0f);
                    if (ImGui.InputInt("##Prerequisite_input_maxNumAcceptedContracts", ref this._maxNumAcceptedContracts))
                    {
                        if (this._maxNumAcceptedContracts < -1)
                        {
                            this._maxNumAcceptedContracts = -1;
                        }
                        else
                        {
                            this._prerequisite.maxNumAcceptedContracts = (uint)this._maxNumAcceptedContracts;
                        }
                    }
                }
                if (ContractManager.contractManagementWindow.rightPanelDetailType == RightPanelDetailType.MISSIONBLUEPRINT)
                {
                    // MaxNumOfferedMissions:
                    ImGui.TableNextRow();
                    ImGui.TableNextColumn();
                    ImGui.Text("Max Offered Missions:");
                    ImGui.SameLine();
                    ContractManagementWindow.DrawHelpTooltip("Offer mission if number of offered missions is less than this number. Use -1 for unlimited.");
                    ImGui.TableNextColumn();
                    ImGui.SetNextItemWidth(-1.0f);
                    if (ImGui.InputInt("##Prerequisite_input_maxNumOfferedMissions", ref this._maxNumOfferedMissions))
                    {
                        if (this._maxNumOfferedMissions < -1)
                        {
                            this._maxNumOfferedMissions = -1;
                        }
                        else
                        {
                            this._prerequisite.maxNumOfferedMissions = (uint)this._maxNumOfferedMissions;
                        }
                    }
                    // MaxNumAcceptedMissions:
                    ImGui.TableNextRow();
                    ImGui.TableNextColumn();
                    ImGui.Text("Max Accepted Missions:");
                    ImGui.SameLine();
                    ContractManagementWindow.DrawHelpTooltip("Offer mission if number of accepted missions is less than this number. Use -1 for unlimited.");
                    ImGui.TableNextColumn();
                    ImGui.SetNextItemWidth(-1.0f);
                    if (ImGui.InputInt("##Prerequisite_input_maxNumAcceptedMissions", ref this._maxNumAcceptedMissions))
                    {
                        if (this._maxNumAcceptedMissions < -1)
                        {
                            this._maxNumAcceptedMissions = -1;
                        }
                        else
                        {
                            this._prerequisite.maxNumAcceptedMissions = (uint)this._maxNumAcceptedMissions;
                        }
                    }
                }

                string parentType = ContractManager.contractManagementWindow.rightPanelDetailType == RightPanelDetailType.CONTRACTBLUEPRINT ? "contract" : "mission";
                // MaxCompleteCount:
                ImGui.TableNextRow();
                ImGui.TableNextColumn();
                ImGui.Text("Max Complete Count:");
                ImGui.SameLine();
                ContractManagementWindow.DrawHelpTooltip($"Offer {parentType} if number of completed instances of this {parentType} is less than this number. Use -1 for unlimited.");
                ImGui.TableNextColumn();
                ImGui.SetNextItemWidth(-1.0f);
                if (ImGui.InputInt("##Prerequisite_input_maxCompleteCount", ref this._maxCompleteCount))
                {
                    if (this._maxCompleteCount < -1)
                    {
                        this._maxCompleteCount = -1;
                    }
                    else
                    {
                        this._prerequisite.maxCompleteCount = (uint)this._maxCompleteCount;
                    }
                }
                // MaxFailedCount:
                ImGui.TableNextRow();
                ImGui.TableNextColumn();
                ImGui.Text("Max Failed Count:");
                ImGui.SameLine();
                ContractManagementWindow.DrawHelpTooltip($"Offer {parentType} if number of failed instances of this {parentType} is less than this number. Use -1 for unlimited.");
                ImGui.TableNextColumn();
                ImGui.SetNextItemWidth(-1.0f);
                if (ImGui.InputInt("##Prerequisite_input_maxFailedCount", ref this._maxFailedCount))
                {
                    if (this._maxFailedCount < -1)
                    {
                        this._maxFailedCount = -1;
                    }
                    else
                    {
                        this._prerequisite.maxFailedCount = (uint)this._maxFailedCount;
                    }
                }
                // MaxConcurrentCount:
                ImGui.TableNextRow();
                ImGui.TableNextColumn();
                ImGui.Text("Max Concurrent Count:");
                ImGui.SameLine();
                ContractManagementWindow.DrawHelpTooltip($"Offer {parentType} if number of accepted instances of this {parentType} is less than this number.");
                ImGui.TableNextColumn();
                ImGui.SetNextItemWidth(-1.0f);
                if (ImGui.InputInt("##Prerequisite_input_maxConcurrentCount", ref this._maxConcurrentCount))
                {
                    if (this._maxConcurrentCount < 0)
                    {
                        this._maxConcurrentCount = 0;
                    }
                    else
                    {
                        this._prerequisite.maxConcurrentCount = (uint)this._maxConcurrentCount;
                    }
                }
                // HasCompletedContract:
                // TODO: convert to combo
                ImGui.TableNextRow();
                ImGui.TableNextColumn();
                ImGui.Text("Has Completed Contract UID:");
                ImGui.SameLine();
                ContractManagementWindow.DrawHelpTooltip($"Offer {parentType} if specified contract has been completed.");
                ImGui.TableNextColumn();
                ImGui.SetNextItemWidth(-1.0f);
                if (ImGui.InputText("##Prerequisite_input_hasCompletedContract", this._hasCompletedContract))
                {
                    this._prerequisite.hasCompletedContract = this._hasCompletedContract.ToString();
                }
                // HasFailedContract:
                // TODO: convert to combo
                ImGui.TableNextRow();
                ImGui.TableNextColumn();
                ImGui.Text("Has Failed Contract UID:");
                ImGui.SameLine();
                ContractManagementWindow.DrawHelpTooltip($"Offer {parentType} if specified contract has been failed.");
                ImGui.TableNextColumn();
                ImGui.SetNextItemWidth(-1.0f);
                if (ImGui.InputText("##Prerequisite_input_hasFailedContract", this._hasFailedContract))
                {
                    this._prerequisite.hasFailedContract = this._hasFailedContract.ToString();
                }
                // HasAcceptedContract:
                // TODO: convert to combo
                ImGui.TableNextRow();
                ImGui.TableNextColumn();
                ImGui.Text("Has Accepted Contract UID:");
                ImGui.SameLine();
                ContractManagementWindow.DrawHelpTooltip($"Offer {parentType} if specified contract has been accepted.");
                ImGui.TableNextColumn();
                ImGui.SetNextItemWidth(-1.0f);
                if (ImGui.InputText("##Prerequisite_input_hasAcceptedContract", this._hasAcceptedContract))
                {
                    this._prerequisite.hasAcceptedContract = this._hasAcceptedContract.ToString();
                }
                // HasCompletedMission:
                // TODO: convert to combo
                ImGui.TableNextRow();
                ImGui.TableNextColumn();
                ImGui.Text("Has Completed Mission UID:");
                ImGui.SameLine();
                ContractManagementWindow.DrawHelpTooltip($"Offer {parentType} if specified mission has been completed.");
                ImGui.TableNextColumn();
                ImGui.SetNextItemWidth(-1.0f);
                if (ImGui.InputText("##Prerequisite_input_hasCompletedMission", this._hasCompletedMission))
                {
                    this._prerequisite.hasCompletedMission = this._hasCompletedMission.ToString();
                }
                // HasFailedMission:
                // TODO: convert to combo
                ImGui.TableNextRow();
                ImGui.TableNextColumn();
                ImGui.Text("Has Failed Mission UID:");
                ImGui.SameLine();
                ContractManagementWindow.DrawHelpTooltip($"Offer {parentType} if specified mission has been failed.");
                ImGui.TableNextColumn();
                ImGui.SetNextItemWidth(-1.0f);
                if (ImGui.InputText("##Prerequisite_input_hasFailedMission", this._hasFailedMission))
                {
                    this._prerequisite.hasFailedMission = this._hasFailedMission.ToString();
                }
                // HasAcceptedMission:
                // TODO: convert to combo
                ImGui.TableNextRow();
                ImGui.TableNextColumn();
                ImGui.Text("Has Accepted Mission UID:");
                ImGui.SameLine();
                ContractManagementWindow.DrawHelpTooltip($"Offer {parentType} if specified mission has been accepted.");
                ImGui.TableNextColumn();
                ImGui.SetNextItemWidth(-1.0f);
                if (ImGui.InputText("##Prerequisite_input_hasAcceptedMission", this._hasAcceptedMission))
                {
                    this._prerequisite.hasAcceptedMission = this._hasAcceptedMission.ToString();
                }
                // MinNumberOfVessels:
                ImGui.TableNextRow();
                ImGui.TableNextColumn();
                ImGui.Text("Min Number Of Vessels:");
                ImGui.SameLine();
                ContractManagementWindow.DrawHelpTooltip($"Offer {parentType} if there are more than this number of vessels in the current celestial system.");
                ImGui.TableNextColumn();
                ImGui.SetNextItemWidth(-1.0f);
                if (ImGui.InputInt("##Prerequisite_input_minNumberOfVessels", ref this._minNumberOfVessels))
                {
                    if (this._minNumberOfVessels < 0)
                    {
                        this._minNumberOfVessels = 0;
                    }
                    else
                    {
                        this._prerequisite.minNumberOfVessels = (uint)this._minNumberOfVessels;
                    }
                }
                // MaxNumberOfVessels:
                ImGui.TableNextRow();
                ImGui.TableNextColumn();
                ImGui.Text("Max Number Of Vessels:");
                ImGui.SameLine();
                ContractManagementWindow.DrawHelpTooltip($"Offer {parentType} if there are less than this number of vessels in the current celestial system. Use -1 for unlimited.");
                ImGui.TableNextColumn();
                ImGui.SetNextItemWidth(-1.0f);
                if (ImGui.InputInt("##Prerequisite_input_maxNumberOfVessels", ref this._maxNumberOfVessels))
                {
                    if (this._maxNumberOfVessels < -1)
                    {
                        this._maxNumberOfVessels = -1;
                    }
                    else
                    {
                        this._prerequisite.maxNumberOfVessels = (uint)this._maxNumberOfVessels;
                    }
                }

                ImGui.EndTable();
            }

            // TODO: Remove
            ImGui.SeparatorText("Debug");
            ImGui.Text(String.Format("maxNumOfferedContracts: {0}", this._prerequisite.maxNumOfferedContracts));
            ImGui.Text(String.Format("maxNumAcceptedContracts: {0}", this._prerequisite.maxNumAcceptedContracts));
            ImGui.Text(String.Format("maxNumOfferedMissions: {0}", this._prerequisite.maxNumOfferedMissions));
            ImGui.Text(String.Format("maxNumAcceptedMissions: {0}", this._prerequisite.maxNumAcceptedMissions));
            ImGui.Text(String.Format("maxCompleteCount: {0}", this._prerequisite.maxCompleteCount));
            ImGui.Text(String.Format("maxFailedCount: {0}", this._prerequisite.maxFailedCount));
            ImGui.Text(String.Format("maxConcurrentCount: {0}", this._prerequisite.maxConcurrentCount));
            ImGui.Text(String.Format("hasCompletedContract: {0}", this._prerequisite.hasCompletedContract));
            ImGui.Text(String.Format("hasFailedContract: {0}", this._prerequisite.hasFailedContract));
            ImGui.Text(String.Format("hasAcceptedContract: {0}", this._prerequisite.hasAcceptedContract));
            ImGui.Text(String.Format("hasCompletedMission: {0}", this._prerequisite.hasCompletedMission));
            ImGui.Text(String.Format("hasFailedMission: {0}", this._prerequisite.hasFailedMission));
            ImGui.Text(String.Format("hasAcceptedMission: {0}", this._prerequisite.hasAcceptedMission));
            ImGui.Text(String.Format("minNumberOfVessels: {0}", this._prerequisite.minNumberOfVessels));
            ImGui.Text(String.Format("maxNumberOfVessels: {0}", this._prerequisite.maxNumberOfVessels));
        }
    }

    internal class RequirementEditingPanel
    {
        private ContractBlueprint.Requirement _requirement;
        internal string requirementUID = string.Empty;

        // Input fields
        private Brutal.ImGuiApi.ImInputString _uid;
        private Brutal.ImGuiApi.ImInputString _title;
        private Brutal.ImGuiApi.ImInputString _synopsis;
        private Brutal.ImGuiApi.ImInputString _description;
        private int _selectedTypeIndex;
        private bool _isCompletedOnAchievement;
        private bool _isHidden;
        private bool _completeInOrder;

        // RequiredOrbit fields
        private Brutal.ImGuiApi.ImInputString _orbitTargetBody;
        private int _orbitTypeIndex;
        private double _minApoapsis;
        private double _maxApoapsis;
        private double _minPeriapsis;
        private double _maxPeriapsis;
        private double _minEccentricity;
        private double _maxEccentricity;
        private double _minPeriod;
        private double _maxPeriod;
        private double _minLongitudeOfAscendingNode;
        private double _maxLongitudeOfAscendingNode;
        private double _minInclination;
        private double _maxInclination;
        private double _minArgumentOfPeriapsis;
        private double _maxArgumentOfPeriapsis;

        internal RequirementEditingPanel(ref ContractBlueprint.Requirement requirementToEdit)
        {
            this._requirement = requirementToEdit;
            this.requirementUID = requirementToEdit.uid;
            this._uid = new Brutal.ImGuiApi.ImInputString(ContractBlueprint.Requirement.uidMaxLength, requirementToEdit.uid);
            this._title = new Brutal.ImGuiApi.ImInputString(ContractBlueprint.Requirement.titleMaxLength, requirementToEdit.title ?? string.Empty);
            this._synopsis = new Brutal.ImGuiApi.ImInputString(ContractBlueprint.Requirement.synopsisMaxLength, requirementToEdit.synopsis ?? string.Empty);
            this._description = new Brutal.ImGuiApi.ImInputString(ContractBlueprint.Requirement.descriptionMaxLength, requirementToEdit.description ?? string.Empty);
            this._selectedTypeIndex = (int)requirementToEdit.type;
            this._isCompletedOnAchievement = requirementToEdit.isCompletedOnAchievement;
            this._isHidden = requirementToEdit.isHidden;
            this._completeInOrder = requirementToEdit.completeInOrder;

            // RequiredOrbit fields
            var orbit = requirementToEdit.orbit;
            this._orbitTargetBody = new Brutal.ImGuiApi.ImInputString(64, orbit?.targetBody ?? string.Empty);
            this._orbitTypeIndex = orbit != null ? (int)orbit.type : 0;
            this._minApoapsis = orbit?.minApoapsis ?? double.NaN;
            this._maxApoapsis = orbit?.maxApoapsis ?? double.NaN;
            this._minPeriapsis = orbit?.minPeriapsis ?? double.NaN;
            this._maxPeriapsis = orbit?.maxPeriapsis ?? double.NaN;
            this._minEccentricity = orbit?.minEccentricity ?? double.NaN;
            this._maxEccentricity = orbit?.maxEccentricity ?? double.NaN;
            this._minPeriod = orbit?.minPeriod ?? double.NaN;
            this._maxPeriod = orbit?.maxPeriod ?? double.NaN;
            this._minLongitudeOfAscendingNode = orbit?.minLongitudeOfAscendingNode ?? double.NaN;
            this._maxLongitudeOfAscendingNode = orbit?.maxLongitudeOfAscendingNode ?? double.NaN;
            this._minInclination = orbit?.minInclination ?? double.NaN;
            this._maxInclination = orbit?.maxInclination ?? double.NaN;
            this._minArgumentOfPeriapsis = orbit?.minArgumentOfPeriapsis ?? double.NaN;
            this._maxArgumentOfPeriapsis = orbit?.maxArgumentOfPeriapsis ?? double.NaN;
        }

        internal void Draw()
        {
            ImGui.SeparatorText("Edit Requirement");

            if (ImGui.BeginTable("RequirementPanelTable", 3))
            {
                ImGui.TableSetupColumn("Label", ImGuiTableColumnFlags.WidthFixed);
                ImGui.TableSetupColumn("Input", ImGuiTableColumnFlags.WidthStretch);
                ImGui.TableSetupColumn("Required", ImGuiTableColumnFlags.WidthFixed);

                // UID
                ImGui.TableNextRow();
                ImGui.TableNextColumn();
                ImGui.Text("UID:");
                ImGui.SameLine();
                ContractManagementWindow.DrawHelpTooltip("The Unique Identifier of this requirement. Needs to be unique across all requirements. (max 128)");
                ImGui.TableNextColumn();
                ImGui.SetNextItemWidth(-1.0f);
                if (ImGui.InputText("##Requirement_input_uid", this._uid))
                {
                    this._requirement.uid = this._uid.ToString();
                    ContractManager.contractManagementWindow.rightPanelDetailSubUID = this._requirement.uid;
                }
                ImGui.TableNextColumn();
                ImGui.Text("(*)");

                // Title
                ImGui.TableNextRow();
                ImGui.TableNextColumn();
                ImGui.Text("Title:");
                ImGui.SameLine();
                ContractManagementWindow.DrawHelpTooltip("The title of the requirement, shown in the Active Contracts window. Should be something short and comprehensible. (max 128)");
                ImGui.TableNextColumn();
                ImGui.SetNextItemWidth(-1.0f);
                if (ImGui.InputText("##Requirement_input_title", this._title))
                {
                    this._requirement.title = this._title.ToString();
                }
                ImGui.TableNextColumn();
                ImGui.Text("(*)");

                // Synopsis
                ImGui.TableNextRow();
                ImGui.TableNextColumn();
                ImGui.Text("Synopsis:");
                ImGui.SameLine();
                ContractManagementWindow.DrawHelpTooltip("A short description or summary of the requirement. (max 1024)");
                ImGui.TableNextColumn();
                ImGui.SetNextItemWidth(-1.0f);
                if (ImGui.InputTextMultiline("##Requirement_input_synopsis", this._synopsis, new Brutal.Numerics.float2 { X = 0.0f, Y = 4 * ImGui.GetTextLineHeight() }, ImGuiInputTextFlags.CtrlEnterForNewLine))
                {
                    this._requirement.synopsis = this._synopsis.ToString();
                }

                // Description
                ImGui.TableNextRow();
                ImGui.TableNextColumn();
                ImGui.Text("Description:");
                ImGui.SameLine();
                ContractManagementWindow.DrawHelpTooltip("The description of the requirement. Can be fairly long and have new lines (ctrl+enter). Could be used to give hints on how to achieve this requirement, or as part of the story-line. (max 4096)");
                ImGui.TableNextColumn();
                ImGui.SetNextItemWidth(-1.0f);
                if (ImGui.InputTextMultiline("##Requirement_input_description", this._description, new Brutal.Numerics.float2 { X = 0.0f, Y = 4 * ImGui.GetTextLineHeight() }, ImGuiInputTextFlags.CtrlEnterForNewLine))
                {
                    this._requirement.description = this._description.ToString();
                }
                
                // isCompletedOnAchievement
                ImGui.TableNextRow();
                ImGui.TableNextColumn();
                ImGui.Text("Completed on Achievement:");
                ImGui.SameLine();
                ContractManagementWindow.DrawHelpTooltip("Flag if the requirement is completed upon achievement. If not, the requirements after this one need to be achieved as well.");
                ImGui.TableNextColumn();
                ImGui.SetNextItemWidth(-1.0f);
                if (ImGui.Checkbox("##MissionBlueprint_input_isCompletedOnAchievement", ref this._isCompletedOnAchievement))
                {
                    this._requirement.isCompletedOnAchievement = this._isCompletedOnAchievement;
                }
                
                // isHidden
                ImGui.TableNextRow();
                ImGui.TableNextColumn();
                ImGui.Text("Hidden:");
                ImGui.SameLine();
                ContractManagementWindow.DrawHelpTooltip("Flag if the requirement is hidden until previous requirement has been achieved. Can be used to hide things for story-telling.");
                ImGui.TableNextColumn();
                ImGui.SetNextItemWidth(-1.0f);
                if (ImGui.Checkbox("##MissionBlueprint_input_isHidden", ref this._isHidden))
                {
                    this._requirement.isHidden = this._isHidden;
                }
                
                // completeInOrder
                ImGui.TableNextRow();
                ImGui.TableNextColumn();
                ImGui.Text("Rejectable:");
                ImGui.SameLine();
                ContractManagementWindow.DrawHelpTooltip("Flag if the requirement needs to be completed in order. If enabled, this requirement can only be achieved (started) if the previous requirement has been achieved.");
                ImGui.TableNextColumn();
                ImGui.SetNextItemWidth(-1.0f);
                if (ImGui.Checkbox("##MissionBlueprint_input_completeInOrder", ref this._completeInOrder))
                {
                    this._requirement.completeInOrder = this._completeInOrder;
                }

                // Type
                ImGui.TableNextRow();
                ImGui.TableNextColumn();
                ImGui.Text("Type:");
                ImGui.SameLine();
                ContractManagementWindow.DrawHelpTooltip("The type of requirement. Please check the documentation for more details.");
                ImGui.TableNextColumn();
                ImGui.SetNextItemWidth(-1.0f);
                string[] typeNames = Enum.GetNames(typeof(ContractBlueprint.RequirementType));
                string selectedTypeName = typeNames[this._selectedTypeIndex];
                if (ImGui.BeginCombo("##Requirement_combo_type", selectedTypeName))
                {
                    for (int typeIndex = 0; typeIndex < typeNames.Length; typeIndex++)
                    {
                        if (ImGui.Selectable(typeNames[typeIndex]))
                        {
                            this._requirement.type = (ContractBlueprint.RequirementType)typeIndex;
                            this._selectedTypeIndex = typeIndex;
                        }
                        if (selectedTypeName == typeNames[typeIndex])
                        {
                            ImGui.SetItemDefaultFocus();
                        }
                        if (this._requirement.type == RequirementType.Orbit && this._requirement.orbit == null)
                        {
                            this._requirement.orbit = new ContractBlueprint.RequiredOrbit();
                        }
                        if (this._requirement.type == RequirementType.Group && this._requirement.group == null)
                        {
                            this._requirement.group = new ContractBlueprint.RequiredGroup();
                        }
                    }
                    ImGui.EndCombo();
                }

                // RequiredOrbit fields (only show if type is Orbit)
                if (this._requirement.type == ContractBlueprint.RequirementType.Orbit && this._requirement.orbit != null)
                {
                    ImGui.SeparatorText("Required Orbit");

                    ImGui.TableNextRow();
                    ImGui.TableNextColumn();
                    ImGui.Text("Target Body:");
                    ImGui.SameLine();
                    ContractManagementWindow.DrawHelpTooltip("The name of the target body to orbit.");
                    ImGui.TableNextColumn();
                    ImGui.SetNextItemWidth(-1.0f);
                    // TODO: Create a dropdown from the in-game available bodies.
                    if (ImGui.InputText("##Requirement_input_orbitTargetBody", this._orbitTargetBody))
                    {
                        this._requirement.orbit.targetBody = this._orbitTargetBody.ToString();
                    }
                    ImGui.TableNextColumn();
                    ImGui.Text("(*)");

                    ImGui.TableNextRow();
                    ImGui.TableNextColumn();
                    ImGui.Text("Orbit Type:");
                    ImGui.SameLine();
                    ContractManagementWindow.DrawHelpTooltip("The type of orbit, elliptical as orbiting a body, suborbit as intersecting with the orbited body, and escape as an orbit that will leave the current orbited body sphere of influence. Use 'Invalid' to disable.");
                    ImGui.TableNextColumn();
                    ImGui.SetNextItemWidth(-1.0f);
                    string[] orbitTypeNames = Enum.GetNames(typeof(ContractBlueprint.OrbitType));
                    string selectedOrbitTypeName = this._requirement.orbit.type.ToString();
                    if (ImGui.BeginCombo("##Requirement_combo_orbitType", selectedOrbitTypeName))
                    {
                        for (int orbitTypeIndex = 0; orbitTypeIndex < orbitTypeNames.Length; orbitTypeIndex++)
                        {
                            if (ImGui.Selectable(orbitTypeNames[orbitTypeIndex]))
                            {
                                this._requirement.orbit.type = (ContractBlueprint.OrbitType)orbitTypeIndex;
                                this._orbitTypeIndex = orbitTypeIndex;
                            }
                            if (selectedOrbitTypeName == orbitTypeNames[orbitTypeIndex])
                            {
                                ImGui.SetItemDefaultFocus();
                            }
                        }
                        ImGui.EndCombo();
                    }

                    ImGui.TableNextRow();
                    ImGui.TableNextColumn();
                    ImGui.Text("Min Apoapsis:");
                    ImGui.SameLine();
                    ContractManagementWindow.DrawHelpTooltip("The minimum height of the Apopasis in meters. Apopasis is defined as the highest point of the orbit. Use 'NaN' to disable.");
                    ImGui.TableNextColumn();
                    ImGui.Text(Utils.FormatDistance(this._requirement.orbit.minApoapsis));
                    ImGui.SameLine();
                    ImGui.SetNextItemWidth(-1.0f);
                    if (ImGui.InputDouble("m##Requirement_input_minApoapsis", ref this._minApoapsis, 1e3, 1e5, "%.0f"))
                    {
                        if (this._minApoapsis < 0) { this._minApoapsis = 0; }
                        if (this._minApoapsis > this._maxApoapsis) { this._minApoapsis = this._maxApoapsis; }
                        this._requirement.orbit.minApoapsis = this._minApoapsis;
                    }

                    ImGui.TableNextRow();
                    ImGui.TableNextColumn();
                    ImGui.Text("Max Apoapsis:");
                    ImGui.SameLine();
                    ContractManagementWindow.DrawHelpTooltip("The maximum height of the Apopasis in meters. Apopasis is defined as the highest point of the orbit. Use 'NaN' to disable.");
                    ImGui.TableNextColumn();
                    ImGui.Text(Utils.FormatDistance(this._requirement.orbit.maxApoapsis));
                    ImGui.SameLine();
                    ImGui.SetNextItemWidth(-1.0f);
                    if (ImGui.InputDouble("m##Requirement_input_maxApoapsis", ref this._maxApoapsis, 1e3, 1e5, "%.0f"))
                    {
                        if (this._maxApoapsis < 0) { this._maxApoapsis = 0; }
                        if (this._maxApoapsis < this._minApoapsis) { this._maxApoapsis = this._minApoapsis; }
                        this._requirement.orbit.maxApoapsis = this._maxApoapsis;
                    }

                    ImGui.TableNextRow();
                    ImGui.TableNextColumn();
                    ImGui.Text("Min Periapsis:");
                    ImGui.SameLine();
                    ContractManagementWindow.DrawHelpTooltip("The minimum height of the Periapsis in meters. Periapsis is defined as the lowest point of the orbit. Use 'NaN' to disable.");
                    ImGui.TableNextColumn();
                    ImGui.Text(Utils.FormatDistance(this._requirement.orbit.minPeriapsis));
                    ImGui.SameLine();
                    ImGui.SetNextItemWidth(-1.0f);
                    if (ImGui.InputDouble("m##Requirement_input_minPeriapsis", ref this._minPeriapsis, 1e3, 1e5, "%.0f"))
                    {
                        if (this._minPeriapsis < 0) { this._minPeriapsis = 0; }
                        if (this._minPeriapsis > this._maxPeriapsis) { this._minPeriapsis = this._maxPeriapsis; }
                        this._requirement.orbit.minPeriapsis = this._minPeriapsis;
                    }

                    ImGui.TableNextRow();
                    ImGui.TableNextColumn();
                    ImGui.Text("Max Periapsis:");
                    ImGui.SameLine();
                    ContractManagementWindow.DrawHelpTooltip("The maximum height of the Periapsis in meters. Periapsis is defined as the lowest point of the orbit. Use 'NaN' to disable.");
                    ImGui.TableNextColumn();
                    ImGui.Text(Utils.FormatDistance(this._requirement.orbit.maxPeriapsis));
                    ImGui.SameLine();
                    ImGui.SetNextItemWidth(-1.0f);
                    if (ImGui.InputDouble("m##Requirement_input_maxPeriapsis", ref this._maxPeriapsis, 1e3, 1e5, "%.0f"))
                    {
                        if (this._maxPeriapsis < 0) { this._maxPeriapsis = 0; }
                        if (this._maxPeriapsis < this._minPeriapsis) { this._maxPeriapsis = this._minPeriapsis; }
                        this._requirement.orbit.maxPeriapsis = this._maxPeriapsis;
                    }

                    ImGui.TableNextRow();
                    ImGui.TableNextColumn();
                    ImGui.Text("Min Eccentricity:");
                    ImGui.SameLine();
                    ContractManagementWindow.DrawHelpTooltip("The minimum eccentricity of the orbit. Zero as perfect circular, <1 as elliptical, >1 hyperbolic, 1 as parabolic. Use 'NaN' to disable.");
                    ImGui.TableNextColumn();
                    ImGui.SetNextItemWidth(-1.0f);
                    if (ImGui.InputDouble("##Requirement_input_minEccentricity", ref this._minEccentricity, 1e-4, 1e-2, "%.6f"))
                    {
                        if (this._minEccentricity < 0) { this._minEccentricity = 0; }
                        if (this._minEccentricity > this._maxPeriapsis) { this._minEccentricity = this._maxPeriapsis; }
                        this._requirement.orbit.minEccentricity = this._minEccentricity;
                    }

                    ImGui.TableNextRow();
                    ImGui.TableNextColumn();
                    ImGui.Text("Max Eccentricity:");
                    ImGui.SameLine();
                    ContractManagementWindow.DrawHelpTooltip("The maximum eccentricity of the orbit. Zero as perfect circular, <1 as elliptical, >1 hyperbolic, 1 as parabolic. Use 'NaN' to disable.");
                    ImGui.TableNextColumn();
                    ImGui.SetNextItemWidth(-1.0f);
                    if (ImGui.InputDouble("##Requirement_input_maxEccentricity", ref this._maxEccentricity, 1e-4, 1e-2, "%.6f"))
                    {
                        if (this._maxEccentricity < 0) { this._maxEccentricity = 0; }
                        if (this._maxEccentricity < this._minEccentricity) { this._maxEccentricity = this._minEccentricity; }
                        this._requirement.orbit.maxEccentricity = this._maxEccentricity;
                    }

                    ImGui.TableNextRow();
                    ImGui.TableNextColumn();
                    ImGui.Text("Min Period:");
                    ImGui.SameLine();
                    ContractManagementWindow.DrawHelpTooltip("The minimum time in seconds for a complete circumnavigation of the orbited body. Use 'NaN' to disable.");
                    ImGui.TableNextColumn();
                    ImGui.Text(Utils.FormatSimTimeAsRelative(new KSA.SimTime(this._requirement.orbit.minPeriod), true));
                    ImGui.SameLine();
                    ImGui.SetNextItemWidth(-1.0f);
                    if (ImGui.InputDouble("s##Requirement_input_minPeriod", ref this._minPeriod, 60, 3600, "%.0f"))
                    {
                        if (this._minPeriod < 0) { this._minPeriod = 0; }
                        if (this._minPeriod > this._maxPeriod) { this._minPeriod = this._maxPeriod; }
                        this._requirement.orbit.minPeriod = this._minPeriod;
                    }

                    ImGui.TableNextRow();
                    ImGui.TableNextColumn();
                    ImGui.Text("Max Period:");
                    ImGui.SameLine();
                    ContractManagementWindow.DrawHelpTooltip("The maximum time in seconds for a complete circumnavigation of the orbited body. Use 'NaN' to disable.");
                    ImGui.TableNextColumn();
                    ImGui.Text(Utils.FormatSimTimeAsRelative(new KSA.SimTime(this._requirement.orbit.maxPeriod), true));
                    ImGui.SameLine();
                    ImGui.SetNextItemWidth(-1.0f);
                    if (ImGui.InputDouble("s##Requirement_input_maxPeriod", ref this._maxPeriod, 60, 3600, "%.0f"))
                    {
                        if (this._maxPeriod < 0) { this._maxPeriod = 0; }
                        if (this._maxPeriod < this._minPeriod) { this._maxPeriod = this._minPeriod; }
                        this._requirement.orbit.maxPeriod = this._maxPeriod;
                    }

                    ImGui.TableNextRow();
                    ImGui.TableNextColumn();
                    ImGui.Text("Min Longitude Of Ascending Node:");
                    ImGui.SameLine();
                    ContractManagementWindow.DrawHelpTooltip("The minimum angle in degrees of the Longitude of the Ascending Node, defined by the angle between the reference direction and Ascending Node. Use 'NaN' to disable.");
                    ImGui.TableNextColumn();
                    ImGui.SetNextItemWidth(-1.0f);
                    if (ImGui.InputDouble("deg##Requirement_input_minLongitudeOfAscendingNode", ref this._minLongitudeOfAscendingNode, 0.1, 5, "%.3f"))
                    {
                        if (this._minLongitudeOfAscendingNode < 0) { this._minLongitudeOfAscendingNode = 360; }
                        if (this._minLongitudeOfAscendingNode > 360) { this._minLongitudeOfAscendingNode = 0; }
                        if (this._minLongitudeOfAscendingNode > this._maxLongitudeOfAscendingNode) { this._minLongitudeOfAscendingNode = this._maxLongitudeOfAscendingNode; }
                        this._requirement.orbit.minLongitudeOfAscendingNode = this._minLongitudeOfAscendingNode;
                    }

                    ImGui.TableNextRow();
                    ImGui.TableNextColumn();
                    ImGui.Text("Max Longitude Of Ascending Node:");
                    ImGui.SameLine();
                    ContractManagementWindow.DrawHelpTooltip("The maximum angle in degrees of the Longitude of the Ascending Node, defined by the angle between the reference direction and Ascending Node. Use 'NaN' to disable.");
                    ImGui.TableNextColumn();
                    ImGui.SetNextItemWidth(-1.0f);
                    if (ImGui.InputDouble("deg##Requirement_input_maxLongitudeOfAscendingNode", ref this._maxLongitudeOfAscendingNode, 0.1, 5, "%.3f"))
                    {
                        if (this._maxLongitudeOfAscendingNode < 0) { this._maxLongitudeOfAscendingNode = 360; }
                        if (this._maxLongitudeOfAscendingNode > 360) { this._maxLongitudeOfAscendingNode = 0; }
                        if (this._maxLongitudeOfAscendingNode < this._minLongitudeOfAscendingNode) { this._maxLongitudeOfAscendingNode = this._minLongitudeOfAscendingNode; }
                        this._requirement.orbit.maxLongitudeOfAscendingNode = this._maxLongitudeOfAscendingNode;
                    }

                    ImGui.TableNextRow();
                    ImGui.TableNextColumn();
                    ImGui.Text("Min Inclination:");
                    ImGui.SameLine();
                    ContractManagementWindow.DrawHelpTooltip("The minimum angle in degrees of the inclination (0-180), defined by the angle between the reference plane and the orbital plane. Negative angle is an orbit in retrograde direction. Use 'NaN' to disable.");
                    ImGui.TableNextColumn();
                    ImGui.SetNextItemWidth(-1.0f);
                    if (ImGui.InputDouble("deg##Requirement_input_minInclination", ref this._minInclination, 0.1, 5, "%.3f"))
                    {
                        if (this._minInclination < -180) { this._minInclination = -180; }
                        if (this._minInclination > 180) { this._minInclination = 180; }
                        if (this._minInclination > this._maxInclination) { this._minInclination = this._maxInclination; }
                        this._requirement.orbit.minInclination = this._minInclination;
                    }

                    ImGui.TableNextRow();
                    ImGui.TableNextColumn();
                    ImGui.Text("Max Inclination:");
                    ImGui.SameLine();
                    ContractManagementWindow.DrawHelpTooltip("The maximum angle in degrees of the inclination (0-180), defined by the angle between the reference plane and the orbital plane. Negative angle is an orbit in retrograde direction. Use 'NaN' to disable.");
                    ImGui.TableNextColumn();
                    ImGui.SetNextItemWidth(-1.0f);
                    if (ImGui.InputDouble("deg##Requirement_input_maxInclination", ref this._maxInclination, 0.1, 5, "%.3f"))
                    {
                        if (this._maxInclination < -180) { this._maxInclination = -180; }
                        if (this._maxInclination > 180) { this._maxInclination = 180; }
                        if (this._maxInclination < this._minInclination) { this._maxInclination = this._minInclination; }
                        this._requirement.orbit.maxInclination = this._maxInclination;
                    }

                    ImGui.TableNextRow();
                    ImGui.TableNextColumn();
                    ImGui.Text("Min Argument Of Periapsis:");
                    ImGui.SameLine();
                    ContractManagementWindow.DrawHelpTooltip("The minimum angle in degrees of the Argument of Periapsis, defined by the angle between the Ascending Node and the Periapsis. Use 'NaN' to disable.");
                    ImGui.TableNextColumn();
                    ImGui.SetNextItemWidth(-1.0f);
                    if (ImGui.InputDouble("deg##Requirement_input_minArgumentOfPeriapsis", ref this._minArgumentOfPeriapsis, 0.1, 5, "%.3f"))
                    {
                        if (this._minArgumentOfPeriapsis < 0) { this._minArgumentOfPeriapsis = 360; }
                        if (this._minArgumentOfPeriapsis > 360) { this._minArgumentOfPeriapsis = 0; }
                        if (this._minArgumentOfPeriapsis > this._maxArgumentOfPeriapsis) { this._minArgumentOfPeriapsis = this._maxArgumentOfPeriapsis; }
                        this._requirement.orbit.minArgumentOfPeriapsis = this._minArgumentOfPeriapsis;
                    }

                    ImGui.TableNextRow();
                    ImGui.TableNextColumn();
                    ImGui.Text("Max Argument Of Periapsis:");
                    ImGui.SameLine();
                    ContractManagementWindow.DrawHelpTooltip("The maximum angle in degrees of the Argument of Periapsis, defined by the angle between the Ascending Node and the Periapsis. Use 'NaN' to disable.");
                    ImGui.TableNextColumn();
                    ImGui.SetNextItemWidth(-1.0f);
                    if (ImGui.InputDouble("deg##Requirement_input_maxArgumentOfPeriapsis", ref this._maxArgumentOfPeriapsis, 0.1, 5, "%.3f"))
                    {
                        if (this._maxArgumentOfPeriapsis < 0) { this._maxArgumentOfPeriapsis = 360; }
                        if (this._maxArgumentOfPeriapsis > 360) { this._maxArgumentOfPeriapsis = 0; }
                        if (this._maxArgumentOfPeriapsis < this._minArgumentOfPeriapsis) { this._maxArgumentOfPeriapsis = this._minArgumentOfPeriapsis; }
                        this._requirement.orbit.maxArgumentOfPeriapsis = this._maxArgumentOfPeriapsis;
                    }

                }
            
                ImGui.EndTable();
            }
                
            ImGui.Text("(*): required.");

            //ImGui.SeparatorText("Debug");
            //ImGui.Text(String.Format("uid: {0}", this._requirement.uid));
            //ImGui.Text(String.Format("title: {0}", this._requirement.title));
            //ImGui.Text(String.Format("type: {0}", this._requirement.type));
            //ImGui.Text(String.Format("description: {0}", this._requirement.description));
            //if (this._requirement.type == ContractBlueprint.RequirementType.Orbit && this._requirement.orbit != null)
            //{
            //    ImGui.Text(String.Format("targetBody: {0}", this._requirement.orbit.targetBody));
            //    ImGui.Text(String.Format("orbitType: {0}", this._requirement.orbit.type));
            //    ImGui.Text(String.Format("minApoapsis: {0}", this._requirement.orbit.minApoapsis));
            //    ImGui.Text(String.Format("maxApoapsis: {0}", this._requirement.orbit.maxApoapsis));
            //    ImGui.Text(String.Format("minPeriapsis: {0}", this._requirement.orbit.minPeriapsis));
            //    ImGui.Text(String.Format("maxPeriapsis: {0}", this._requirement.orbit.maxPeriapsis));
            //    ImGui.Text(String.Format("minEccentricity: {0}", this._requirement.orbit.minEccentricity));
            //    ImGui.Text(String.Format("maxEccentricity: {0}", this._requirement.orbit.maxEccentricity));
            //    ImGui.Text(String.Format("minPeriod: {0}", this._requirement.orbit.minPeriod));
            //    ImGui.Text(String.Format("maxPeriod: {0}", this._requirement.orbit.maxPeriod));
            //    ImGui.Text(String.Format("minLongitudeOfAscendingNode: {0}", this._requirement.orbit.minLongitudeOfAscendingNode));
            //    ImGui.Text(String.Format("maxLongitudeOfAscendingNode: {0}", this._requirement.orbit.maxLongitudeOfAscendingNode));
            //    ImGui.Text(String.Format("minInclination: {0}", this._requirement.orbit.minInclination));
            //    ImGui.Text(String.Format("maxInclination: {0}", this._requirement.orbit.maxInclination));
            //    ImGui.Text(String.Format("minArgumentOfPeriapsis: {0}", this._requirement.orbit.minArgumentOfPeriapsis));
            //    ImGui.Text(String.Format("maxArgumentOfPeriapsis: {0}", this._requirement.orbit.maxArgumentOfPeriapsis));
            //}
        }
    }

    internal class ActionEditingPanel
    {
        private ContractBlueprint.Action _action;
        internal string actionUID = string.Empty;

        // Input fields
        private Brutal.ImGuiApi.ImInputString _uid;
        private int _selectedTriggerIndex;
        private int _selectedTypeIndex;
        private Brutal.ImGuiApi.ImInputString _showMessage;
        private Brutal.ImGuiApi.ImInputString _onRequirement;

        internal ActionEditingPanel(ref ContractBlueprint.Action actionToEdit)
        {
            this._action = actionToEdit;
            this.actionUID = actionToEdit.uid;
            this._uid = new Brutal.ImGuiApi.ImInputString(64, actionToEdit.uid ?? string.Empty);
            this._selectedTriggerIndex = (int)actionToEdit.trigger;
            this._selectedTypeIndex = (int)actionToEdit.type;
            this._showMessage = new Brutal.ImGuiApi.ImInputString(256, actionToEdit.showMessage ?? string.Empty);
            this._onRequirement = new Brutal.ImGuiApi.ImInputString(64, actionToEdit.onRequirement ?? string.Empty);
        }

        internal void Draw()
        {
            ImGui.SeparatorText("Edit Action");

            if (ImGui.BeginTable("ActionPanelTable", 3))
            {
                ImGui.TableSetupColumn("Label", ImGuiTableColumnFlags.WidthFixed);
                ImGui.TableSetupColumn("Input", ImGuiTableColumnFlags.WidthStretch);
                ImGui.TableSetupColumn("Required", ImGuiTableColumnFlags.WidthFixed);

                // UID
                ImGui.TableNextRow();
                ImGui.TableNextColumn();
                ImGui.Text("UID:");
                ImGui.SameLine();
                ContractManagementWindow.DrawHelpTooltip("The Unique Identifier of this action. Needs to be unique across all actions. (max 64)");
                ImGui.TableNextColumn();
                ImGui.SetNextItemWidth(-1.0f);
                if (ImGui.InputText("##Action_input_uid", this._uid))
                {
                    this._action.uid = this._uid.ToString();
                    ContractManager.contractManagementWindow.rightPanelDetailSubUID = this._action.uid;
                }
                ImGui.TableNextColumn();
                ImGui.Text("(*)");

                // Trigger
                ImGui.TableNextRow();
                ImGui.TableNextColumn();
                ImGui.Text("Trigger:");
                ImGui.SameLine();
                ContractManagementWindow.DrawHelpTooltip("The trigger for this action. When a contract, requirement, or mission, is accepted, completed etc. Please check the documentation for more details.");
                ImGui.TableNextColumn();
                ImGui.SetNextItemWidth(-1.0f);
                string[] triggerNames = Enum.GetNames(typeof(ContractBlueprint.TriggerType));
                string selectedTriggerName = triggerNames[this._selectedTriggerIndex];
                if (ImGui.BeginCombo("##Action_combo_trigger", selectedTriggerName))
                {
                    for (int triggerIndex = 0; triggerIndex < triggerNames.Length; triggerIndex++)
                    {
                        // TODO: skip OnMission* if the parent is a contract blueprint, and OnContract* if the parent is a mission blueprint.
                        if (ImGui.Selectable(triggerNames[triggerIndex]))
                        {
                            this._action.trigger = (ContractBlueprint.TriggerType)triggerIndex;
                            this._selectedTriggerIndex = triggerIndex;
                        }
                        if (selectedTriggerName == triggerNames[triggerIndex])
                        {
                            ImGui.SetItemDefaultFocus();
                        }
                    }
                    ImGui.EndCombo();
                }
                ImGui.TableNextColumn();
                ImGui.Text("(*)");

                // Type
                ImGui.TableNextRow();
                ImGui.TableNextColumn();
                ImGui.Text("Type:");
                ImGui.SameLine();
                ContractManagementWindow.DrawHelpTooltip("The type of action to execute. Please check the documentation for more details.");
                ImGui.TableNextColumn();
                ImGui.SetNextItemWidth(-1.0f);
                string[] typeNames = Enum.GetNames(typeof(ContractBlueprint.ActionType));
                string selectedTypeName = typeNames[this._selectedTypeIndex];
                if (ImGui.BeginCombo("##Action_combo_type", selectedTypeName))
                {
                    for (int typeIndex = 0; typeIndex < typeNames.Length; typeIndex++)
                    {
                        // TODO: only show `OnRequirement` for contract blueprints.
                        if (ImGui.Selectable(typeNames[typeIndex]))
                        {
                            this._action.type = (ContractBlueprint.ActionType)typeIndex;
                            this._selectedTypeIndex = typeIndex;
                        }
                        if (selectedTypeName == typeNames[typeIndex])
                        {
                            ImGui.SetItemDefaultFocus();
                        }
                    }
                    ImGui.EndCombo();
                }
                ImGui.TableNextColumn();
                ImGui.Text("(*)");

                // ShowMessage (only for ShowMessage or ShowBlockingPopup)
                if (this._action.type == ContractBlueprint.ActionType.ShowMessage ||
                    this._action.type == ContractBlueprint.ActionType.ShowBlockingPopup)
                {
                    ImGui.TableNextRow();
                    ImGui.TableNextColumn();
                    ImGui.Text("Show Message:");
                    ImGui.SameLine();
                    ContractManagementWindow.DrawHelpTooltip("The message to show in the popup or modal. Use ctrl+enter for a new line.");
                    ImGui.TableNextColumn();
                    ImGui.SetNextItemWidth(-1.0f);
                    if (ImGui.InputTextMultiline(
                        "##Action_input_showMessage",
                        this._showMessage,
                        new Brutal.Numerics.float2 { X = 0.0f, Y = 4 * ImGui.GetTextLineHeight() },
                        ImGuiInputTextFlags.CtrlEnterForNewLine // | ImGuiInputTextFlags.WordWrap
                    ))
                    {
                        this._action.showMessage = this._showMessage.ToString();
                    }
                    ImGui.TableNextColumn();
                    ImGui.Text("(*)");
                }

                // onRequirement (only for triggers that use it)
                // TODO: Make this a combo, where the list is restricted to the requirements of the parent contract.
                if (this._action.trigger is
                    ContractBlueprint.TriggerType.OnRequirementTracked or
                    ContractBlueprint.TriggerType.OnRequirementMaintained or
                    ContractBlueprint.TriggerType.OnRequirementReverted or
                    ContractBlueprint.TriggerType.OnRequirementAchieved or
                    ContractBlueprint.TriggerType.OnRequirementFailed)
                {
                    ImGui.TableNextRow();
                    ImGui.TableNextColumn();
                    ImGui.Text("On Requirement:");
                    ImGui.SameLine();
                    ContractManagementWindow.DrawHelpTooltip("The requirement uid on which the action will be triggered.");
                    ImGui.TableNextColumn();
                    ImGui.SetNextItemWidth(-1.0f);
                    if (ImGui.InputText("##Action_input_onRequirement", this._onRequirement))
                    {
                        this._action.onRequirement = this._onRequirement.ToString();
                    }
                    ImGui.TableNextColumn();
                    ImGui.Text("(*)");
                }

                ImGui.EndTable();
            }
            
            ImGui.Text("(*): required.");

            // TODO: remove
            ImGui.SeparatorText("Debug");
            ImGui.Text(String.Format("uid: {0}", this._action.uid));
            ImGui.Text(String.Format("trigger: {0}", this._action.trigger));
            ImGui.Text(String.Format("type: {0}", this._action.type));
            ImGui.Text(String.Format("showMessage: {0}", this._action.showMessage));
            ImGui.Text(String.Format("onRequirement: {0}", this._action.onRequirement));
        }
    }
}
