using Brutal.ImGuiApi;
using ContractManager.Contract;
using ContractManager.ContractBlueprint;
using System;
using System.Collections.Generic;
using System.Text;

namespace ContractManager.GUI
{
    internal class ActiveContractsWindow
    {
        private List<Contract.Contract> _acceptedContracts { get { return ContractManager.data.acceptedContracts; } }

        public ActiveContractsWindow() { }

        public Contract.Contract? DrawActiveContractsWindow()
        {
            Contract.Contract? contractToShowDetails = null;
            ImGui.SetNextWindowSizeConstraints(
                new Brutal.Numerics.float2 { X = 350.0f, Y = 300.0f },
                new Brutal.Numerics.float2 { X = 600.0f, Y = float.PositiveInfinity }  // no max size
            );
            if (ImGui.Begin("Active Contracts", ImGuiWindowFlags.None))
            {
                if (this._acceptedContracts.Count == 0)
                {
                    ImGui.Text("No active contracts");
                }
                else
                {
                    contractToShowDetails = this.DrawActiveContracts();
                }
            }
            ImGui.End();  // End of Active Contracts Window

            return contractToShowDetails;
        }

        private Contract.Contract? DrawActiveContracts()
        {
            Contract.Contract? contractToShowDetails = null;
            var style = ImGui.GetStyle();
            float buttonWidthDetails = ImGui.CalcTextSize("Details").X + style.FramePadding.X * 2.0f;
            float windowAvailableWidth = ImGui.GetContentRegionAvail().X;
            float triangleWidth = 25.0f;  // Width of the tree node triangle to fold the node.
            float maxTitleWidth = windowAvailableWidth - buttonWidthDetails - triangleWidth;
            foreach (Contract.Contract contract in this._acceptedContracts)
            {
                // Ensure the title can fit in a way to leave space for the details button
                string titleForNode = contract._contractBlueprint.title;
                float textSize = ImGui.CalcTextSize(titleForNode).X + style.FramePadding.X * 2.0f;
                while (textSize > maxTitleWidth)
                {
                    titleForNode = titleForNode[0..^3] + "..";
                    textSize = ImGui.CalcTextSize(titleForNode).X + style.FramePadding.X * 2.0f;
                }
                // Draw tree node for contract
                if (ImGui.TreeNodeEx(contract.contractUID, ImGuiTreeNodeFlags.DrawLinesToNodes, titleForNode))
                {
                    ImGui.SameLine();
                    ImGui.SetCursorPosX(ImGui.GetCursorPosX() + windowAvailableWidth - textSize - buttonWidthDetails - triangleWidth);
                    if (ImGui.SmallButton("Details"))
                    {
                        contractToShowDetails = contract;
                    }

                    ImGui.Text(String.Format("Complete {0} requirement{1}", contract._contractBlueprint.completionCondition, contract._contractBlueprint.completionCondition == CompletionCondition.All ? "s" : ""));
                    foreach (Contract.TrackedRequirement trackedRequirement in contract.trackedRequirements)
                    {
                        this.DrawRequirement(contract, trackedRequirement);
                    }

                    ImGui.TreePop();
                }
            }
            return contractToShowDetails;
        }

        // Draw tree node for requirement
        private void DrawRequirement(Contract.Contract contract, Contract.TrackedRequirement trackedRequirement)
        {
            if (trackedRequirement._blueprintRequirement.isHidden && trackedRequirement.status == TrackedRequirementStatus.NOT_STARTED) { return;}
            var colorText = Colors.grayDark;
            var colorHover = Colors.greenLight;
            var colorActive = Colors.green;
            if (trackedRequirement.status == TrackedRequirementStatus.NOT_STARTED)
            {
                colorText = Colors.grayDark;
                colorHover = Colors.grayLight;
                colorActive = Colors.gray;
            }
            else
            if (trackedRequirement.status == TrackedRequirementStatus.TRACKED)
            {
                colorText = Colors.orangeDark;
                colorHover = Colors.orangeLight;
                colorActive = Colors.orange;
            }
            else
            if (trackedRequirement.status == TrackedRequirementStatus.MAINTAINED)
            {
                colorText = Colors.yellowDark;
                colorHover = Colors.yellowLight;
                colorActive = Colors.yellow;
            }
            else
            if (trackedRequirement.status == TrackedRequirementStatus.ACHIEVED)
            {
                colorText = Colors.greenDark;
                colorHover = Colors.greenLight;
                colorActive = Colors.green;
            }
            else
            if (trackedRequirement.status == TrackedRequirementStatus.FAILED)
            {
                colorText = Colors.redDark;
                colorHover = Colors.redLight;
                colorActive = Colors.red;
            }
            
            ImGuiTreeNodeFlags requirementTreeNodeFlags = ImGuiTreeNodeFlags.DrawLinesToNodes;
            if (trackedRequirement.status == TrackedRequirementStatus.TRACKED)
            {
                // Open by default when tracked.
                requirementTreeNodeFlags |= ImGuiTreeNodeFlags.DefaultOpen;
            }
            ImGui.PushStyleColor(ImGuiCol.HeaderHovered, colorHover);
            ImGui.PushStyleColor(ImGuiCol.HeaderActive, colorActive);
            ImGui.PushStyleColor(ImGuiCol.Text, colorText);
            bool poppedStyleColor = false;
            if (ImGui.TreeNodeEx(
                String.Format("{0}_{1}", contract.contractUID, trackedRequirement.requirementUID),
                requirementTreeNodeFlags,
                trackedRequirement._blueprintRequirement.title
            ))
            {
                ImGui.PopStyleColor(3);
                poppedStyleColor = true;
                if (trackedRequirement._blueprintRequirement.synopsis != string.Empty)
                {
                    ImGui.TextWrapped(trackedRequirement._blueprintRequirement.synopsis);
                }
                if (trackedRequirement._blueprintRequirement.completeInOrder)
                {
                    ImGui.Text("Complete in order");
                }
                if (!trackedRequirement._blueprintRequirement.isCompletedOnAchievement)
                {
                    ImGui.Text("Maintain untill all achieved.");
                }
                
                // Show the status of the requirement
                if (trackedRequirement.status == TrackedRequirementStatus.NOT_STARTED)
                {
                    ImGui.TextColored(Colors.gray, "Status: Not yet started...");
                }
                else
                if (trackedRequirement.status == TrackedRequirementStatus.TRACKED)
                {
                    ImGui.TextColored(Colors.orange, "Status: Not yet achieved...");
                }
                else
                if (trackedRequirement.status == TrackedRequirementStatus.MAINTAINED)
                {
                    var style = ImGui.GetStyle();
                    float nodeAvailableWidth = ImGui.GetContentRegionAvail().X;
                    if (nodeAvailableWidth > ImGui.CalcTextSize("Status: Maintain until other requirements achieved").X + style.FramePadding.X * 2.0f) {
                        ImGui.TextColored(Colors.yellow, "Status: Maintain until other requirements achieved");
                    }
                    else
                    if (nodeAvailableWidth > ImGui.CalcTextSize("Status: Maintain until all achieved").X + style.FramePadding.X * 2.0f) {
                        ImGui.TextColored(Colors.yellow, "Status: Maintain until all achieved");
                    }
                    else
                    {
                        ImGui.TextColored(Colors.yellow, "Status: Maintain");
                    }
                }
                else
                if (trackedRequirement.status == TrackedRequirementStatus.ACHIEVED)
                {
                    ImGui.TextColored(Colors.green, "Status: Achieved!");
                }
                else
                if (trackedRequirement.status == TrackedRequirementStatus.FAILED)
                {
                    ImGui.TextColored(Colors.red, "Status: Failed!");
                }

                // Draw requirement type specific fields
                if (trackedRequirement._blueprintRequirement.type == RequirementType.Orbit)
                {
                    this.DrawRequiredOrbit(trackedRequirement);
                }
                else
                if (trackedRequirement._blueprintRequirement.type == RequirementType.Group)
                {
                     foreach (Contract.TrackedRequirement childTrackedRequirement in ((TrackedGroup)trackedRequirement).trackedRequirements)
                     {
                        this.DrawRequirement(contract, childTrackedRequirement);
                     }
                }
                ImGui.TreePop();
            }
            if (!poppedStyleColor)
            {
                ImGui.PopStyleColor(3);
            }
        }

        private void DrawRequiredOrbit(Contract.TrackedRequirement trackedRequirement)
        {
            RequiredOrbit? requiredOrbit = trackedRequirement._blueprintRequirement.orbit;
            if (requiredOrbit == null) { return; }
            
            float nodeAvailableWidth = ImGui.GetContentRegionAvail().X;
            // Show requirement(s) and current tracked state
            // Apoapsis
            if (trackedRequirement.status == TrackedRequirementStatus.NOT_STARTED && !Double.IsNaN(requiredOrbit.minApoapsis))
            {
                ImGui.TextColored(Colors.gray, String.Format("Apoapsis < {0}", Utils.FormatDistance(requiredOrbit.minApoapsis)));
            }
            else
            if (trackedRequirement.status == TrackedRequirementStatus.NOT_STARTED && !Double.IsNaN(requiredOrbit.maxApoapsis))
            {
                ImGui.TextColored(Colors.gray, String.Format("Apoapsis > {0}", Utils.FormatDistance(requiredOrbit.maxApoapsis)));
            }
            else
            if (nodeAvailableWidth > 350 && !Double.IsNaN(requiredOrbit.minApoapsis) && !Double.IsNaN(requiredOrbit.maxApoapsis))
            {
                double apoapsis = ((TrackedOrbit)trackedRequirement).apoapsis;
                ImGui.Text("Apoapsis:");
                ImGui.SameLine();
                ImGui.TextColored(requiredOrbit.minApoapsis < apoapsis ? Colors.green : Colors.red, String.Format("{0} <", Utils.FormatDistance(requiredOrbit.minApoapsis)));
                ImGui.SameLine();
                ImGui.TextColored(requiredOrbit.minApoapsis < apoapsis && apoapsis < requiredOrbit.maxApoapsis ? Colors.green : Colors.orange, Utils.FormatDistance(apoapsis));
                ImGui.SameLine();
                ImGui.TextColored(requiredOrbit.maxApoapsis > apoapsis ? Colors.green : Colors.red, String.Format("< {0}", Utils.FormatDistance(requiredOrbit.maxApoapsis)));
            }
            else
            {
                if (!Double.IsNaN(requiredOrbit.minApoapsis))
                {
                    double apoapsis = ((TrackedOrbit)trackedRequirement).apoapsis;
                    ImGui.Text("Apoapsis:");
                    ImGui.SameLine();
                    ImGui.TextColored(
                        requiredOrbit.minApoapsis < apoapsis ? Colors.green : Colors.red,
                        String.Format("{0} <", Utils.FormatDistance(requiredOrbit.minApoapsis)));
                    ImGui.SameLine();
                    ImGui.TextColored(requiredOrbit.minApoapsis < apoapsis ? Colors.green : Colors.orange, Utils.FormatDistance(apoapsis));
                }
                if (!Double.IsNaN(requiredOrbit.maxApoapsis))
                {
                    double apoapsis = ((TrackedOrbit)trackedRequirement).apoapsis;
                    ImGui.Text("Apoapsis:");
                    ImGui.SameLine();
                    ImGui.TextColored(
                        requiredOrbit.maxApoapsis > apoapsis ? Colors.green : Colors.red,
                        String.Format("{0} >", Utils.FormatDistance(requiredOrbit.maxApoapsis)));
                    ImGui.SameLine();
                    ImGui.TextColored(requiredOrbit.maxApoapsis > apoapsis ? Colors.green : Colors.orange, Utils.FormatDistance(apoapsis));
                }
            }
            // Periapsis
            if (trackedRequirement.status == TrackedRequirementStatus.NOT_STARTED && !Double.IsNaN(requiredOrbit.minPeriapsis))
            {
                ImGui.TextColored(Colors.gray, String.Format("Periapsis < {0}", Utils.FormatDistance(requiredOrbit.minPeriapsis)));
            }
            else
            if (trackedRequirement.status == TrackedRequirementStatus.NOT_STARTED && !Double.IsNaN(requiredOrbit.maxPeriapsis))
            {
                ImGui.TextColored(Colors.gray, String.Format("Periapsis > {0}", Utils.FormatDistance(requiredOrbit.maxPeriapsis)));
            }
            else
            if (nodeAvailableWidth > 350 && !Double.IsNaN(requiredOrbit.minPeriapsis) && !Double.IsNaN(requiredOrbit.maxPeriapsis))
            {
                double periapsis = ((TrackedOrbit)trackedRequirement).periapsis;
                ImGui.Text("Periapsis:");
                ImGui.SameLine();
                ImGui.TextColored(requiredOrbit.minPeriapsis < periapsis ? Colors.green : Colors.red, String.Format("{0} <", Utils.FormatDistance(requiredOrbit.minPeriapsis)));
                ImGui.SameLine();
                ImGui.TextColored(requiredOrbit.minPeriapsis < periapsis && periapsis < requiredOrbit.maxPeriapsis ? Colors.green : Colors.orange, Utils.FormatDistance(periapsis));
                ImGui.SameLine();
                ImGui.TextColored(requiredOrbit.maxPeriapsis > periapsis ? Colors.green : Colors.red, String.Format("< {0}", Utils.FormatDistance(requiredOrbit.maxPeriapsis)));
            }
            else
            {
                if (!Double.IsNaN(requiredOrbit.minPeriapsis))
                {
                    double periapsis = ((TrackedOrbit)trackedRequirement).periapsis;
                    ImGui.Text("Periapsis:");
                    ImGui.SameLine();
                    ImGui.TextColored(
                        requiredOrbit.minPeriapsis < periapsis ? Colors.green : Colors.red,
                        String.Format("{0} <", Utils.FormatDistance(requiredOrbit.minPeriapsis)));
                    ImGui.SameLine();
                    ImGui.TextColored(requiredOrbit.minPeriapsis < periapsis ? Colors.green : Colors.orange, Utils.FormatDistance(periapsis));
                }
                if (!Double.IsNaN(requiredOrbit.maxPeriapsis))
                {
                    double periapsis = ((TrackedOrbit)trackedRequirement).periapsis;
                    ImGui.Text("Periapsis:");
                    ImGui.SameLine();
                    ImGui.TextColored(
                        requiredOrbit.maxPeriapsis > periapsis ? Colors.green : Colors.red,
                        String.Format("{0} >", Utils.FormatDistance(requiredOrbit.maxPeriapsis)));
                    ImGui.SameLine();
                    ImGui.TextColored(requiredOrbit.maxPeriapsis > periapsis ? Colors.green : Colors.orange, Utils.FormatDistance(periapsis));
                }
            }
        }
    }
}
