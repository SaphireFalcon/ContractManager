using Brutal.ImGuiApi;
using ContractManager.Contract;
using ContractManager.ContractBlueprint;
using KSA;
using System;
using System.Collections.Generic;
using System.Text;

namespace ContractManager
{
    internal class ContractManagementWindow
    {
        private Contract.Contract? _contractToShowDetails { get; set; } = null;
        
        private List<Contract.Contract> _offeredContracts { get; }
        private List<Contract.Contract> _acceptedContracts { get; }
        private List<Contract.Contract> _finishedContracts { get; }

        private string _lastActiveTab {  get; set; } = string.Empty;
        
        public ContractManagementWindow(List<Contract.Contract> offeredContracts, List<Contract.Contract> acceptedContracts, List<Contract.Contract> finishedContracts)
        {
            this._offeredContracts = offeredContracts;
            this._acceptedContracts = acceptedContracts;
            this._finishedContracts = finishedContracts;
        }

        public void DrawContractManagementWindow(Contract.Contract? contractToShowDetails)
        {
            if (contractToShowDetails != null)
            {
                this._contractToShowDetails = contractToShowDetails;
            }
            // Contract Management Window with two panels: left fixed-width, right flexible
            ImGui.SetNextWindowSizeConstraints(
                new Brutal.Numerics.float2 { X = 600.0f, Y = 300.0f },
                new Brutal.Numerics.float2 { X = float.PositiveInfinity, Y = float.PositiveInfinity }  // no max size
            );

            if (ImGui.Begin("Contract Management", ImGuiWindowFlags.None))
            {
                // Draw left panel
                this.DrawLeftPanel();

                // Draw right panel
                this.DrawRightPanel();
            }
            ImGui.End();  // End of Contract Management Window
        }

        private void DrawLeftPanel()
        {
            // Left panel: fixed width and fill available height so it becomes scrollable when content overflows
            Brutal.Numerics.float2 leftPanelSize = new Brutal.Numerics.float2 { X = 260.0f, Y = 0.0f };
            if (ImGui.BeginChild("LeftPanel", leftPanelSize, ImGuiChildFlags.Borders, ImGuiWindowFlags.None))
            {
                ImGui.SeparatorText("Contracts");

                // Tabs in the left panel
                if (ImGui.BeginTabBar("LeftTabs", ImGuiTabBarFlags.None))
                {
                    this.DrawContractTab(this._offeredContracts, "Offered");
                    this.DrawContractTab(this._acceptedContracts, "Accepted");
                    this.DrawContractTab(this._finishedContracts, "Finished");

                    ImGui.EndTabBar();
                }
                ImGui.EndChild();  // End of LeftPanel
            }
        }

        // Draw tab with contracts, tabTitle has to be unique.
        private void DrawContractTab(List<Contract.Contract> contractsToShowInTab, string tabTitle)
        {
            if (ImGui.BeginTabItem(tabTitle))
            {
                if (this._lastActiveTab != tabTitle)
                {
                    this._contractToShowDetails = null;
                    this._lastActiveTab = tabTitle;
                }
                var style = ImGui.GetStyle();
                var tabContentRegionSize = ImGui.GetContentRegionAvail();
                Brutal.Numerics.float2 tabSize = new Brutal.Numerics.float2 { X = 0.0f, Y = tabContentRegionSize.Y };
                // Wrap contents in a child to make it scrollable if needed
                if (ImGui.BeginChild(tabTitle + "TabChild", tabSize, ImGuiChildFlags.None, ImGuiWindowFlags.NoTitleBar))
                {
                    // Fill available width for buttons
                    var tabChildContentRegionSize = ImGui.GetContentRegionAvail();
                    Brutal.Numerics.float2 buttonSize = new Brutal.Numerics.float2 { X = tabChildContentRegionSize.X, Y = 0.0f };
                    // Left-align button text
                    ImGui.PushStyleVar(ImGuiStyleVar.ButtonTextAlign, new Brutal.Numerics.float2 { X = 0.0f, Y = 0.5f });
                    foreach (Contract.Contract contractForTab in contractsToShowInTab)
                    {
                        // Ensure the title can fit in the fix-width button
                        string titleForButton = contractForTab._contractBlueprint.title;
                        float textSize = ImGui.CalcTextSize(titleForButton).X + style.FramePadding.X * 2.0f;
                        while (textSize > buttonSize.X)
                        {
                            titleForButton = titleForButton[0..^3] + "..";
                            textSize = ImGui.CalcTextSize(titleForButton).X + style.FramePadding.X * 2.0f;
                        }
                        if (ImGui.Button(titleForButton, buttonSize))
                        {
                            // Toggle the contract to show.
                            if (this._contractToShowDetails == contractForTab)
                            {
                                this._contractToShowDetails = null;
                            }
                            else
                            {
                                this._contractToShowDetails = contractForTab;
                            }
                        }
                    }
                    ImGui.PopStyleVar();
                    ImGui.EndChild();
                }
                ImGui.EndTabItem();
            }
        }

        private void DrawRightPanel()
        {
            ImGui.SameLine();
            // Right panel: fills remaining space on the right
            if (ImGui.BeginChild("RightPanel"))
            {
                Brutal.Numerics.float2 rightPanelRegionSize = ImGui.GetContentRegionAvail();
                Brutal.Numerics.float2 rightPanelSize = new Brutal.Numerics.float2 { X = 0.0f, Y = rightPanelRegionSize.Y - 35.0f};
                // Wrap contents in a child to make it scrollable if needed
                if (ImGui.BeginChild("Contract details", rightPanelSize, ImGuiChildFlags.None, ImGuiWindowFlags.None))
                {
                    if (this._contractToShowDetails != null)
                    {
                        // Draw contract details
                        ImGui.SeparatorText("Contract details: " + this._contractToShowDetails._contractBlueprint.title);

                        if (this._contractToShowDetails.status == ContractStatus.Rejected)
                        {
                            ImGui.Text("Status: Rejected.");
                        }
                        if (this._contractToShowDetails.status == ContractStatus.Completed)
                        {
                            ImGui.Text("Status: Completed.");
                        }
                        
                        if (this._contractToShowDetails._contractBlueprint.synopsis != string.Empty)
                        {
                            // TODO: make bold.
                            ImGui.TextWrapped(this._contractToShowDetails._contractBlueprint.synopsis);
                        }
                        
                        if (this._contractToShowDetails._contractBlueprint.description != string.Empty)
                        {
                            ImGui.TextWrapped(this._contractToShowDetails._contractBlueprint.description);
                        }
                        
                        ImGui.SeparatorText("Rewards");
                        ImGui.Text("None implemented yet.");

                        this.DrawRequirements();
                    }
                    else
                    {
                        ImGui.TextWrapped("Select a contract from the left panel to view details here.");
                    }
                    ImGui.EndChild();  // End of Contract details child
                }
                ImGui.Separator();
                if (this._contractToShowDetails != null)
                {
                    this.DrawRejectButton();

                    this.DrawAcceptButton(rightPanelRegionSize);
                }
                ImGui.EndChild();  // End of RightPanel
            }
        }

        // Draw tree of requirements
        private void DrawRequirements()
        {
            if (this._contractToShowDetails == null) { return; }

            ImGui.SeparatorText("Requirements");
            
            ImGuiTreeNodeFlags requirementTreeNodeFlags = ImGuiTreeNodeFlags.DefaultOpen | ImGuiTreeNodeFlags.DrawLinesToNodes;
            if (ImGui.TreeNodeEx("Contract requirements:", requirementTreeNodeFlags))
            {
                ImGui.Text(String.Format("Complete {0} of these requirements", this._contractToShowDetails._contractBlueprint.completionCondition));
                foreach (Contract.TrackedRequirement trackedRequirement in this._contractToShowDetails.trackedRequirements)
                {
                    this.DrawRequirement(trackedRequirement);
                }
                ImGui.TreePop();
            }
        }

        // Draw tree node for requirement
        private void DrawRequirement(Contract.TrackedRequirement trackedRequirement)
        {
            if (trackedRequirement._blueprintRequirement == null) { return; }

            if (ImGui.TreeNodeEx("Requirement" + trackedRequirement._blueprintRequirement.title, ImGuiTreeNodeFlags.DefaultOpen | ImGuiTreeNodeFlags.DrawLinesToNodes))
            {
                if (trackedRequirement._blueprintRequirement.synopsis != string.Empty)
                {
                    ImGui.TextWrapped(trackedRequirement._blueprintRequirement.synopsis);
                }
                if (trackedRequirement._blueprintRequirement.description != string.Empty)
                {
                    ImGui.TextWrapped(trackedRequirement._blueprintRequirement.description);
                }
                if (trackedRequirement._blueprintRequirement.completeInOrder)
                {
                    ImGui.Text("Complete in order");
                }
                if (!trackedRequirement._blueprintRequirement.isCompletedOnAchievement)
                {
                    ImGui.Text("Maintain requirement untill all requirements are achieved.");
                }
                // Draw requirement type specific fields
                if (trackedRequirement._blueprintRequirement.type == RequirementType.Orbit)
                {
                    this.DrawRequiredOrbit(trackedRequirement);
                }
                ImGui.TreePop();
            }
        }

        private void DrawRequiredOrbit(Contract.TrackedRequirement trackedRequirement)
        {
            if (trackedRequirement._blueprintRequirement.orbit == null) { return; }
            if (this._contractToShowDetails == null) { return; }
            RequiredOrbit requiredOrbit = trackedRequirement._blueprintRequirement.orbit;

            if (this._contractToShowDetails.status == ContractStatus.Offered)
            {
                // Show the requirement details
                if (!Double.IsNaN(requiredOrbit.minApoapsis))
                {
                    ImGui.Text(String.Format("Min Apoapsis {0:F0} m altitude", requiredOrbit.minApoapsis));
                }
                if (!Double.IsNaN(requiredOrbit.maxApoapsis))
                {
                    ImGui.Text(String.Format("Max Apoapsis {0:F0} m altitude", requiredOrbit.maxApoapsis));
                }
                if (!Double.IsNaN(requiredOrbit.minPeriapsis))
                {
                    ImGui.Text(String.Format("Min Periapsis {0:F0} m altitude", requiredOrbit.minPeriapsis));
                }
                if (!Double.IsNaN(requiredOrbit.maxPeriapsis))
                {
                    ImGui.Text(String.Format("Max Periapsis {0:F0} m altitude", requiredOrbit.maxPeriapsis));
                }
            }
            else
            if (this._contractToShowDetails.status == ContractStatus.Accepted || this._contractToShowDetails.status == ContractStatus.Completed)
            {
                var color = Colors.green;
                // Show the status of the requirement
                if (trackedRequirement.status == TrackedRequirementStatus.NOT_STARTED)
                {
                    color = Colors.gray;
                    ImGui.TextColored(color, String.Format("Status: Not yet started..."));
                }
                else
                if (trackedRequirement.status == TrackedRequirementStatus.TRACKED)
                {
                    color = Colors.orange;
                    ImGui.TextColored(color, String.Format("Status: Not yet achieved..."));
                }
                else
                if (trackedRequirement.status == TrackedRequirementStatus.MAINTAINED)
                {
                    color = Colors.yellow;
                    ImGui.TextColored(color, String.Format("Status: Maintain until other requirements are achieved."));
                }
                else
                if (trackedRequirement.status == TrackedRequirementStatus.ACHIEVED)
                {
                    color = Colors.green;
                    ImGui.TextColored(color, String.Format("Status: Achieved!"));
                }
                else
                if (trackedRequirement.status == TrackedRequirementStatus.FAILED)
                {
                    color = Colors.red;
                    ImGui.TextColored(color, String.Format("Status: Failed!"));
                }

                // Show requirement(s) and current tracked state
                // Apoapsis
                if (!Double.IsNaN(requiredOrbit.minApoapsis) && !Double.IsNaN(requiredOrbit.maxApoapsis))
                {
                    ImGui.TextColored(
                        color,
                        String.Format(
                            "Apoapsis {0} < {1} < {2} altitude",
                            Utils.FormatDistance(requiredOrbit.minApoapsis),
                            Utils.FormatDistance(((TrackedOrbit)trackedRequirement).apoapsis),
                            Utils.FormatDistance(requiredOrbit.maxApoapsis)));
                }
                else
                if (!Double.IsNaN(requiredOrbit.minApoapsis))
                {
                    ImGui.TextColored(
                        color,
                        String.Format(
                            "Apoapsis {0} < {1} altitude",
                            Utils.FormatDistance(requiredOrbit.minApoapsis),
                            Utils.FormatDistance(((TrackedOrbit)trackedRequirement).apoapsis)));
                }
                else
                if (!Double.IsNaN(requiredOrbit.maxApoapsis))
                {
                    ImGui.TextColored(
                        color,
                        String.Format(
                            "Apoapsis {0} > {1} altitude",
                            Utils.FormatDistance(requiredOrbit.maxApoapsis),
                            Utils.FormatDistance(((TrackedOrbit)trackedRequirement).apoapsis)));
                }
                // Periapsis
                if (!Double.IsNaN(requiredOrbit.minPeriapsis) && !Double.IsNaN(requiredOrbit.maxPeriapsis))
                {
                    ImGui.TextColored(
                        color,
                        String.Format(
                            "Periapsis {0} < {1} < {2} altitude",
                            Utils.FormatDistance(requiredOrbit.minPeriapsis),
                            Utils.FormatDistance(((TrackedOrbit)trackedRequirement).periapsis),
                            Utils.FormatDistance(requiredOrbit.maxPeriapsis)));
                }
                else
                if (!Double.IsNaN(requiredOrbit.minPeriapsis))
                {
                    ImGui.TextColored(
                        color,
                        String.Format(
                            "Periapsis {0} < {1} altitude",
                            Utils.FormatDistance(requiredOrbit.minPeriapsis),
                            Utils.FormatDistance(((TrackedOrbit)trackedRequirement).periapsis)));
                }
                else
                if (!Double.IsNaN(requiredOrbit.maxPeriapsis))
                {
                    ImGui.TextColored(
                        color,
                        String.Format(
                            "Periapsis {0} > {1} altitude",
                            Utils.FormatDistance(requiredOrbit.maxPeriapsis),
                            Utils.FormatDistance(((TrackedOrbit)trackedRequirement).periapsis)));
                }
            }
        }

        private void DrawRejectButton()
        {
            if (this._contractToShowDetails == null) { return; }
            if (!(this._contractToShowDetails.status is ContractStatus.Offered or ContractStatus.Accepted)) { return; }

            // Show reject button
            ImGui.PushStyleColor(ImGuiCol.Button, new Brutal.Numerics.float4 { X = 0.5f, Y = 0.1f, Z = 0.1f, W = 1.0f });
            ImGui.PushStyleColor(ImGuiCol.ButtonHovered, new Brutal.Numerics.float4 { X = 0.75f, Y = 0.25f, Z = 0.25f, W = 1.0f });
            ImGui.PushStyleColor(ImGuiCol.ButtonActive, new Brutal.Numerics.float4 { X = 0.75f, Y = 0.1f, Z = 0.1f, W = 1.0f });
            if (ImGui.Button("Reject Contract"))
            {
                this._contractToShowDetails.RejectContract(Program.GetPlayerTime());
                if (this._offeredContracts.Contains(this._contractToShowDetails))
                {
                    this._offeredContracts.Remove(this._contractToShowDetails);
                }
                if (this._acceptedContracts.Contains(this._contractToShowDetails))
                {
                    this._acceptedContracts.Remove(this._contractToShowDetails);
                    this._finishedContracts.Add(this._contractToShowDetails);  // If accepted and then rejected add to finished?
                }
            }
            ImGui.PopStyleColor(3);
        }

        private void DrawAcceptButton(Brutal.Numerics.float2 rightPanelRegionSize)
        {
            if (this._contractToShowDetails == null) { return; }
            if (this._contractToShowDetails.status != ContractStatus.Offered) { return; }

            // Show accept button
            ImGui.SameLine();
            var style = ImGui.GetStyle();
            float buttonWidthReject = ImGui.CalcTextSize("Reject Contract").X + style.FramePadding.X * 2.0f;
            float buttonWidthAccept = ImGui.CalcTextSize("Accept Contract").X + style.FramePadding.X * 2.0f;
            float resizeTriangleWidth = 25.0f;
            ImGui.SetCursorPosX(ImGui.GetCursorPosX() + rightPanelRegionSize.X - buttonWidthReject - buttonWidthAccept - resizeTriangleWidth);
            ImGui.PushStyleColor(ImGuiCol.Button, new Brutal.Numerics.float4 { X = 0.2f, Y = 0.5f, Z = 0.2f, W = 1.0f });
            ImGui.PushStyleColor(ImGuiCol.ButtonHovered, new Brutal.Numerics.float4 { X = 0.35f, Y = 0.75f, Z = 0.35f, W = 1.0f });
            ImGui.PushStyleColor(ImGuiCol.ButtonActive, new Brutal.Numerics.float4 { X = 0.2f, Y = 0.75f, Z = 0.2f, W = 1.0f });
            if (ImGui.Button("Accept Contract"))
            {
                // TODO: only call accept if it is possible to accept by checking maxNumberOfAcceptedContracts
                this._contractToShowDetails.AcceptOfferedContract(Program.GetPlayerTime());
                this._offeredContracts.Remove(this._contractToShowDetails);
                this._acceptedContracts.Add(this._contractToShowDetails);
            }
            ImGui.PopStyleColor(3);
        }
    }
}
