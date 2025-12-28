using Brutal.ImGuiApi;
using ContractManager.Contract;
using ContractManager.ContractBlueprint;
using System;
using System.Collections.Generic;
using System.Text;

namespace ContractManager
{
    internal class ActiveContractsWindow
    {
        private List<Contract.Contract> _acceptedContracts { get; }

        public ActiveContractsWindow(List<Contract.Contract> acceptedContracts)
        {
            this._acceptedContracts = acceptedContracts;
        }

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
            // TODO: Add colors
            
            ImGuiTreeNodeFlags requirementTreeNodeFlags = ImGuiTreeNodeFlags.DrawLinesToNodes;
            if (trackedRequirement.status == TrackedRequirementStatus.TRACKED)
            {
                // Open by default when tracked.
                requirementTreeNodeFlags |= ImGuiTreeNodeFlags.DefaultOpen;
            }
            if (ImGui.TreeNodeEx(
                String.Format("{0}_{1}", contract.contractUID, trackedRequirement.requirementUID),
                requirementTreeNodeFlags,
                trackedRequirement._blueprintRequirement.title
            ))
            {
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
                // TODO: add color!
                if (trackedRequirement.status == TrackedRequirementStatus.NOT_STARTED)
                {
                    ImGui.Text(String.Format("Status: Not yet started..."));
                }
                else
                if (trackedRequirement.status == TrackedRequirementStatus.TRACKED)
                {
                    ImGui.Text(String.Format("Status: Not yet achieved..."));
                }
                else
                if (trackedRequirement.status == TrackedRequirementStatus.MAINTAINED)
                {
                    ImGui.Text(String.Format("Status: Maintain until other requirements are achieved."));
                }
                else
                if (trackedRequirement.status == TrackedRequirementStatus.ACHIEVED)
                {
                    ImGui.Text(String.Format("Status: Achieved!"));
                }
                else
                if (trackedRequirement.status == TrackedRequirementStatus.FAILED)
                {
                    ImGui.Text(String.Format("Status: Failed!"));
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
            RequiredOrbit? requiredOrbit = trackedRequirement._blueprintRequirement.orbit;
            if (requiredOrbit == null) { return; }
            
            float nodeAvailableWidth = ImGui.GetContentRegionAvail().X;
            // Show requirement(s) and current tracked state
            // Apoapsis
            if (trackedRequirement.status == TrackedRequirementStatus.NOT_STARTED && !Double.IsNaN(requiredOrbit.minApoapsis))
            {
                ImGui.Text(String.Format("Apoapsis < {0}", FormatDistance(requiredOrbit.minApoapsis)));
            }
            else
            if (trackedRequirement.status == TrackedRequirementStatus.NOT_STARTED && !Double.IsNaN(requiredOrbit.maxApoapsis))
            {
                ImGui.Text(String.Format("Apoapsis > {0}", FormatDistance(requiredOrbit.maxApoapsis)));
            }
            else
            if (nodeAvailableWidth > 350 && !Double.IsNaN(requiredOrbit.minApoapsis) && !Double.IsNaN(requiredOrbit.maxApoapsis))
            {
                ImGui.Text(
                    String.Format("Apoapsis {0} < {1} < {2}",
                    FormatDistance(requiredOrbit.minApoapsis),
                    FormatDistance(((TrackedOrbit)trackedRequirement).apoapsis),
                    FormatDistance(requiredOrbit.maxApoapsis)));
            }
            else
            if (!Double.IsNaN(requiredOrbit.minApoapsis))
            {
                ImGui.Text(String.Format(
                    "Apoapsis {0} < {1}",
                    FormatDistance(requiredOrbit.minApoapsis),
                    FormatDistance(((TrackedOrbit)trackedRequirement).apoapsis)));
            }
            else
            if (!Double.IsNaN(requiredOrbit.maxApoapsis))
            {
                ImGui.Text(
                    String.Format("Apoapsis {0} > {1}",
                    FormatDistance(requiredOrbit.maxApoapsis),
                    FormatDistance(((TrackedOrbit)trackedRequirement).apoapsis)));
            }
            // Periapsis
            if (trackedRequirement.status == TrackedRequirementStatus.NOT_STARTED && !Double.IsNaN(requiredOrbit.minPeriapsis))
            {
                ImGui.Text(String.Format("Periapsis < {0}", FormatDistance(requiredOrbit.minPeriapsis)));
            }
            else
            if (trackedRequirement.status == TrackedRequirementStatus.NOT_STARTED && !Double.IsNaN(requiredOrbit.maxPeriapsis))
            {
                ImGui.Text(String.Format("Periapsis > {0}", FormatDistance(requiredOrbit.maxPeriapsis)));
            }
            else
            if (nodeAvailableWidth > 350 && !Double.IsNaN(requiredOrbit.minPeriapsis) && !Double.IsNaN(requiredOrbit.maxPeriapsis))
            {
                ImGui.Text(
                    String.Format("Periapsis {0} < {1} < {2}",
                    FormatDistance(requiredOrbit.minPeriapsis),
                    FormatDistance(((TrackedOrbit)trackedRequirement).periapsis),
                    FormatDistance(requiredOrbit.maxPeriapsis)));
            }
            else
            if (!Double.IsNaN(requiredOrbit.minPeriapsis))
            {
                ImGui.Text(String.Format(
                    "Periapsis {0} < {1}",
                    FormatDistance(requiredOrbit.minPeriapsis),
                    FormatDistance(((TrackedOrbit)trackedRequirement).periapsis)));
            }
            else
            if (!Double.IsNaN(requiredOrbit.maxPeriapsis))
            {
                ImGui.Text(
                    String.Format("Periapsis {0} > {1}",
                    FormatDistance(requiredOrbit.maxPeriapsis),
                    FormatDistance(((TrackedOrbit)trackedRequirement).periapsis)));
            }
        }

        private string FormatDistance(double distanceInMeters, string format = "{0:N1}{1}")
        {
            string unit = "m";
            double distance = distanceInMeters;
            if (distanceInMeters >= 1e7d)
            {
                distance /= 1e6d;
                unit = "Gm";
            }
            else
            if (distanceInMeters >= 1e4d)
            {
                distance /= 1e3d;
                unit = "km";
            }
            if (Double.IsNaN(distance))
            {
                unit = "";
            }
            return String.Format(format, distance, unit);
        }
    }
}
