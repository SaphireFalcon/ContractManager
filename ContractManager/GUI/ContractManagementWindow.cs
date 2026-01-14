using Brutal.ImGuiApi;
using ContractManager.Contract;
using ContractManager.ContractBlueprint;
using ContractManager.Mission;
using KSA;
using System;
using System.Collections.Generic;

namespace ContractManager.GUI
{

    internal class LeftPanelListItem
    {
        private readonly ContractManagementWindow? _window = null; // ToDo: remove, it's static available.
        private readonly string _title = string.Empty;
        private readonly string _uid = string.Empty;
        private readonly string _guid = string.Empty;
        private readonly RightPanelDetailType _rightPanelDetailType = RightPanelDetailType.NONE;
        private readonly ColorTriplet? _colors = null;
        
        internal LeftPanelListItem(ContractManagementWindow window, Contract.Contract contract)
        { 
            this._window = window;
            this._title = contract._contractBlueprint.title;
            this._uid = contract.contractUID;
            this._guid = $"contract_{this._uid}";
            this._rightPanelDetailType = RightPanelDetailType.CONTRACT;
            this._colors = Colors.GetContractStatusColor(contract.status);
        }
        internal static List<LeftPanelListItem> GetLeftPanelListItems(ContractManagementWindow window, List<Contract.Contract> contracts)
        {
            List<LeftPanelListItem> leftPanelListItems = new List<LeftPanelListItem>();
            foreach (Contract.Contract contract in contracts)
            {
                leftPanelListItems.Add(new LeftPanelListItem(window, contract));
            }
            return leftPanelListItems;
        }

        internal LeftPanelListItem(ContractManagementWindow window, Mission.Mission mission)
        { 
            this._window = window;
            this._title = mission._missionBlueprint.title;
            this._uid = mission.missionUID;
            this._guid = $"mission_{this._uid}";
            this._rightPanelDetailType = RightPanelDetailType.MISSION;
            this._colors = Colors.GetMissionStatusColor(mission.status);
        }
        internal static List<LeftPanelListItem> GetLeftPanelListItems(ContractManagementWindow window, List<Mission.Mission> missions)
        {
            List<LeftPanelListItem> leftPanelListItems = new List<LeftPanelListItem>();
            foreach (Mission.Mission mission in missions)
            {
                leftPanelListItems.Add(new LeftPanelListItem(window, mission));
            }
            return leftPanelListItems;
        }
        internal LeftPanelListItem(ContractManagementWindow window, ContractBlueprint.ContractBlueprint contractBlueprint)
        { 
            this._window = window;
            this._title = contractBlueprint.title;
            this._uid = contractBlueprint.uid;
            this._guid = $"contract_{this._uid}";
            this._rightPanelDetailType = RightPanelDetailType.CONTRACTBLUEPRINT;
        }
        internal static List<LeftPanelListItem> GetLeftPanelListItems(ContractManagementWindow window, List<ContractBlueprint.ContractBlueprint> contractBlueprints)
        {
            List<LeftPanelListItem> leftPanelListItems = new List<LeftPanelListItem>();
            foreach (ContractBlueprint.ContractBlueprint contractBlueprint in contractBlueprints)
            {
                leftPanelListItems.Add(new LeftPanelListItem(window, contractBlueprint));
            }
            return leftPanelListItems;
        }

        internal LeftPanelListItem(ContractManagementWindow window, Mission.MissionBlueprint missionBlueprint)
        { 
            this._window = window;
            this._title = missionBlueprint.title;
            this._uid = missionBlueprint.uid;
            this._guid = $"mission_{this._uid}";
            this._rightPanelDetailType = RightPanelDetailType.MISSIONBLUEPRINT;
        }
        internal static List<LeftPanelListItem> GetLeftPanelListItems(ContractManagementWindow window, List<Mission.MissionBlueprint> missionBlueprints)
        {
            List<LeftPanelListItem> leftPanelListItems = new List<LeftPanelListItem>();
            foreach (Mission.MissionBlueprint missionBlueprint in missionBlueprints)
            {
                leftPanelListItems.Add(new LeftPanelListItem(window, missionBlueprint));
            }
            return leftPanelListItems;
        }
        
        internal LeftPanelListItem(ContractManagementWindow window, ContractBlueprint.Prerequisite prerequisite)
        { 
            this._window = window;
            this._title = prerequisite.type.ToString();
            this._uid = prerequisite.uid;
            this._guid = $"mission_{this._uid}";
            this._rightPanelDetailType = RightPanelDetailType.PREREQUISITE;
        }
        internal static List<LeftPanelListItem> GetLeftPanelListItems(ContractManagementWindow window, List<ContractBlueprint.Prerequisite> prerequisites)
        {
            List<LeftPanelListItem> leftPanelListItems = new List<LeftPanelListItem>();
            foreach (ContractBlueprint.Prerequisite prerequisite in prerequisites)
            {
                leftPanelListItems.Add(new LeftPanelListItem(window, prerequisite));
            }
            return leftPanelListItems;
        }
        
        internal LeftPanelListItem(ContractManagementWindow window, ContractBlueprint.Requirement requirement)
        { 
            this._window = window;
            this._title = requirement.title;
            this._uid = requirement.uid;
            this._guid = $"mission_{this._uid}";
            this._rightPanelDetailType = RightPanelDetailType.REQUIREMENT;
        }
        internal static List<LeftPanelListItem> GetLeftPanelListItems(ContractManagementWindow window, List<ContractBlueprint.Requirement> requirements)
        {
            List<LeftPanelListItem> leftPanelListItems = new List<LeftPanelListItem>();
            foreach (ContractBlueprint.Requirement requirement in requirements)
            {
                leftPanelListItems.Add(new LeftPanelListItem(window, requirement));
            }
            return leftPanelListItems;
        }
        
        internal LeftPanelListItem(ContractManagementWindow window, ContractBlueprint.Action action)
        { 
            this._window = window;
            this._title = action.trigger.ToString() + " " + action.type.ToString();
            this._uid = action.uid;
            this._guid = $"mission_{this._uid}";
            this._rightPanelDetailType = RightPanelDetailType.ACTION;
        }
        internal static List<LeftPanelListItem> GetLeftPanelListItems(ContractManagementWindow window, List<ContractBlueprint.Action> actions)
        {
            List<LeftPanelListItem> leftPanelListItems = new List<LeftPanelListItem>();
            foreach (ContractBlueprint.Action action in actions)
            {
                leftPanelListItems.Add(new LeftPanelListItem(window, action));
            }
            return leftPanelListItems;
        }

        internal void DrawPanelListItem(Brutal.Numerics.float2 buttonSize, bool colorItemsByStatus = false)
        {
            if (this._window == null) { return; }

            // Ensure the title can fit in the fix-width button
            var style = ImGui.GetStyle();
            string titleForButton = this._title;
            float textSize = ImGui.CalcTextSize(titleForButton).X + style.FramePadding.X * 2.0f;
            while (textSize > buttonSize.X)
            {
                titleForButton = titleForButton[0..^3] + "..";
                textSize = ImGui.CalcTextSize(titleForButton).X + style.FramePadding.X * 2.0f;
            }

            // Change the color for the button if it is currently selected for details.
            bool showAsActive = (this._window.rightPanelDetailType == this._rightPanelDetailType && this._window.rightPanelDetailUID == this._uid);
            if (showAsActive)
            {
                ImGui.PushStyleColor(ImGuiCol.Button, Colors.blueDefaultLight);
            }
            else
            // Change to color bases on status
            if (colorItemsByStatus && this._colors != null) {
                ImGui.PushStyleColor(ImGuiCol.Button, this._colors.dark);
                ImGui.PushStyleColor(ImGuiCol.ButtonHovered, this._colors.light);
                ImGui.PushStyleColor(ImGuiCol.ButtonActive, this._colors.normal);
            }
            if (ImGui.Button(titleForButton + String.Format("##{0}", this._guid), buttonSize))
            {
                // Toggle what to show
                if (this._window.rightPanelDetailSubType == this._rightPanelDetailType)
                {
                    if (this._window.rightPanelDetailSubUID == this._uid)
                    {
                        this._window.rightPanelDetailSubUID = string.Empty;
                    }
                    else
                    {
                        this._window.rightPanelDetailSubUID = this._uid;
                    }
                }
                else
                if (this._window.rightPanelDetailType == this._rightPanelDetailType && this._window.rightPanelDetailUID == this._uid )
                {
                    this._window.rightPanelDetailType = RightPanelDetailType.NONE;
                    this._window.rightPanelDetailUID = string.Empty;
                }
                else
                {
                    this._window.rightPanelDetailType = this._rightPanelDetailType;
                    this._window.rightPanelDetailUID = this._uid;
                }
            }
            if (showAsActive)
            {
                ImGui.PopStyleColor(1);
            }
            else
            if (colorItemsByStatus && this._colors != null)
            {
                ImGui.PopStyleColor(3);
            }
        }
    }

    internal enum RightPanelDetailType
    {
        NONE,
        CONTRACT,
        MISSION,
        CONTRACTBLUEPRINT,
        MISSIONBLUEPRINT,
        PREREQUISITE,
        REQUIREMENT,
        ACTION,
    }

    internal class ContractManagementWindow
    {
        // TODO: move this to a static data class?
        // What to show on the right side, one of CONTRACTBLUEPRINT, CONTRACT, MISSIONBLUEPRINT, MISSION, 
        internal RightPanelDetailType rightPanelDetailType { get; set; } = RightPanelDetailType.NONE;
        // The UID to show on the right, one of contract, contractBlueprint, mission, missionBlueprint uids for corresponding rightPanelDetailType.
        internal string rightPanelDetailUID { get; set; } = string.Empty;
        // What to show on the right side as a sub-type, one of PREREQUISITE, REQUIREMENT, ACTION
        internal RightPanelDetailType rightPanelDetailSubType { get; set; } = RightPanelDetailType.NONE;
        // The UID to show on the right, if rightPanelDetailType is a subtype such as prerequisite, requirement, action
        internal string rightPanelDetailSubUID { get; set; } = string.Empty;
        
        private readonly PlannerPanel _plannerPanel;

        public ContractManagementWindow()
        {
            this._plannerPanel = new PlannerPanel();
        }

        public void DrawContractManagementWindow(Contract.Contract? contractToShowDetails)
        {
            // FIXME: make this a callable function to set.
            if (contractToShowDetails != null)
            {
                this.rightPanelDetailUID = contractToShowDetails.contractUID;
                this.rightPanelDetailType = RightPanelDetailType.CONTRACT;
            }

            // Contract Management Window with two panels: left fixed-width, right flexible
            ImGui.SetNextWindowSizeConstraints(
                new Brutal.Numerics.float2 { X = 600.0f, Y = 300.0f },
                new Brutal.Numerics.float2 { X = float.PositiveInfinity, Y = float.PositiveInfinity }  // no max size
            );

            if (ImGui.Begin("Mission & Contract Management", ImGuiWindowFlags.None))
            {
                
                if (ImGui.BeginTabBar("ModeTabs", ImGuiTabBarFlags.None))
                {
                    if (ImGui.BeginTabItem("Mission Planner"))
                    {
                        // Draw left panel
                        this._plannerPanel.DrawPlannerLeftPanel();
                        this._plannerPanel.DrawPlannerRightPanel();
                        ImGui.EndTabItem();
                    }
                    if (ImGui.BeginTabItem("Mission & Contract Management"))
                    {
                        // Draw left panel
                        this.DrawManagementLeftPanel();

                        // Draw right panel
                        this.DrawManagementRightPanel();

                        ImGui.EndTabItem();
                    }
                    if (ImGui.BeginTabItem("Configuration"))
                    {
                        // ToDo
                        ImGui.EndTabItem();
                    }
                    ImGui.EndTabBar();
                }
            }
            ImGui.End();  // End of Contract Management Window
        }

        private Mission.Mission? GetRelatedMission()
        {
            Mission.Mission? relatedMission = null;
            if (this.rightPanelDetailType == RightPanelDetailType.CONTRACT && this.rightPanelDetailUID != string.Empty)
            {
                Contract.Contract? contractToShow = ContractUtils.FindContractFromContractUID(this.rightPanelDetailUID);
                if (contractToShow != null && contractToShow.missionUID != string.Empty) {
                    relatedMission = MissionUtils.FindMissionFromMissionUID(contractToShow.missionUID);
                }
            }
            if (this.rightPanelDetailType == RightPanelDetailType.MISSION && this.rightPanelDetailUID != string.Empty)
            {
                relatedMission = MissionUtils.FindMissionFromMissionUID(this.rightPanelDetailUID);
            }
            return relatedMission;
        }

        private void DrawManagementLeftPanel()
        {
            // Left panel: fixed width and fill available height so it becomes scrollable when content overflows
            var style = ImGui.GetStyle();
            float leftPanelWidth = 260.0f;
            Brutal.Numerics.float2 leftPanelSize = new Brutal.Numerics.float2 { X = leftPanelWidth, Y = 0.0f };
            ImGui.PushStyleVar(ImGuiStyleVar.WindowPadding, new Brutal.Numerics.float2 { X = 0.0f, Y = 0.0f });
            if (ImGui.BeginChild("ManagementLeftPanel", leftPanelSize, ImGuiChildFlags.None, ImGuiWindowFlags.None))
            {
                // Retrieve if a related mission is selected
                Mission.Mission? relatedMission = this.GetRelatedMission();
                
                // Draw Mission section
                var tabLeftPanelRegionSize = ImGui.GetContentRegionAvail();
                float leftMissionPanelHeightRatio = 0.3f;
                
                Brutal.Numerics.float2 leftMissionPanelSize = new Brutal.Numerics.float2 {
                    X = leftPanelWidth,
                    Y = (tabLeftPanelRegionSize.Y * leftMissionPanelHeightRatio) - style.FramePadding.Y,
                };
                if (ImGui.BeginChild("ManagementLeftMissionPanel", leftMissionPanelSize, ImGuiChildFlags.Borders, ImGuiWindowFlags.None))
                {
                    ImGui.SeparatorText("Missions");

                    // Tabs in the left panel
                    if (ImGui.BeginTabBar("LeftMissionTabs", ImGuiTabBarFlags.None))
                    {
                        this.DrawTabItemList(
                            LeftPanelListItem.GetLeftPanelListItems(this, ContractManager.data.offeredMissions),
                            "Offered",
                            "management_missions",
                            relatedMission != null && ContractManager.data.offeredMissions.Contains(relatedMission)
                        );
                        this.DrawTabItemList(
                            LeftPanelListItem.GetLeftPanelListItems(this, ContractManager.data.acceptedMissions),
                            "Accepted",
                            "management_missions",
                            relatedMission != null && ContractManager.data.acceptedMissions.Contains(relatedMission)
                        );
                        this.DrawTabItemList(
                            LeftPanelListItem.GetLeftPanelListItems(this, ContractManager.data.finishedMissions),
                            "Finished",
                            "management_missions",
                            relatedMission != null && ContractManager.data.finishedMissions.Contains(relatedMission)
                        );

                        ImGui.EndTabBar();
                    }
                    ImGui.EndChild();  // End of LeftContractPanel
                }

                // Draw Contract section
                Brutal.Numerics.float2 leftContractPanelSize = new Brutal.Numerics.float2 {
                    X = leftPanelWidth,
                    Y = (tabLeftPanelRegionSize.Y * (1.0f - leftMissionPanelHeightRatio)) - style.FramePadding.Y
                };
                if (ImGui.BeginChild("ManagementLeftContractPanel", leftContractPanelSize, ImGuiChildFlags.Borders, ImGuiWindowFlags.None))
                {
                    ImGui.SeparatorText("Contracts");

                    if (relatedMission != null)
                    {
                        // Show only related contracts
                        List<Contract.Contract> contractsRelatedToMission = new List<Contract.Contract>();
                        foreach (string contractUID in relatedMission.contractUIDs)
                        {
                            Contract.Contract? contract = ContractUtils.FindContractFromContractUID(contractUID);
                            if (contract != null)
                            {
                                contractsRelatedToMission.Add(contract);
                            }
                        }
                        this.DrawItemList(LeftPanelListItem.GetLeftPanelListItems(this, contractsRelatedToMission), "management_contracts", true);
                        // Note what to do with contracts that haven't been offered? -> could create list items from contract blueprints.
                    }
                    else
                    {
                        // Show tabs in the left panel for all type of contracts
                        if (ImGui.BeginTabBar("LeftContractTabs", ImGuiTabBarFlags.None))
                        {
                            this.DrawTabItemList(LeftPanelListItem.GetLeftPanelListItems(this, ContractManager.data.offeredContracts), "Offered", "management_contracts");
                            this.DrawTabItemList(LeftPanelListItem.GetLeftPanelListItems(this, ContractManager.data.acceptedContracts), "Accepted", "management_contracts");
                            this.DrawTabItemList(LeftPanelListItem.GetLeftPanelListItems(this, ContractManager.data.finishedContracts), "Finished", "management_contracts");

                            ImGui.EndTabBar();
                        }
                    }

                    ImGui.EndChild();  // End of LeftContractPanel
                }

                ImGui.EndChild();  // End of LeftPanel
            }
            ImGui.PopStyleVar();
        }

        // Draw tab with list items, tabTitle has to be unique.
        internal void DrawTabItemList(List<LeftPanelListItem> listItems, string tabTitle, string guid, bool setTabSelected = false)
        {
            ImGuiTabItemFlags tabItemFlags = ImGuiTabItemFlags.None;
            if (setTabSelected) tabItemFlags |= ImGuiTabItemFlags.SetSelected;
            if (ImGui.BeginTabItem(String.Format("{0}##{1}", tabTitle, guid), tabItemFlags))
            {
                this.DrawItemList(listItems, String.Format("{0}_{1}_TabChild", tabTitle, guid));
                ImGui.EndTabItem();
            }
        }

        // Drawlist items, guid has to be unique.
        internal void DrawItemList(List<LeftPanelListItem> listItems, string guid, bool colorItemsByStatus = false)
        {
            var style = ImGui.GetStyle();
            var availableRegionSize = ImGui.GetContentRegionAvail();
            Brutal.Numerics.float2 childRegionsSize = new Brutal.Numerics.float2 { X = 0.0f, Y = availableRegionSize.Y };
            // Wrap contents in a child to make it scrollable if needed
            if (ImGui.BeginChild(guid, childRegionsSize, ImGuiChildFlags.None, ImGuiWindowFlags.NoTitleBar))
            {
                // Fill available width for buttons
                var childAvailableRegionSize = ImGui.GetContentRegionAvail();
                Brutal.Numerics.float2 buttonSize = new Brutal.Numerics.float2 { X = childAvailableRegionSize.X, Y = 0.0f };
                // Left-align button text
                ImGui.PushStyleVar(ImGuiStyleVar.ButtonTextAlign, new Brutal.Numerics.float2 { X = 0.0f, Y = 0.5f });
                foreach (LeftPanelListItem listItem in listItems)
                {
                    listItem.DrawPanelListItem(buttonSize, colorItemsByStatus);
                }
                ImGui.PopStyleVar();
                ImGui.EndChild();
            }
        }


        private void DrawManagementRightPanel()
        {
            ImGui.SameLine();
            // Right panel: fills remaining space on the right
            if (ImGui.BeginChild("ManagementRightPanel"))
            {
                Brutal.Numerics.float2 rightPanelRegionSize = ImGui.GetContentRegionAvail();
                Brutal.Numerics.float2 rightPanelSize = new Brutal.Numerics.float2 { X = 0.0f, Y = rightPanelRegionSize.Y - 35.0f};
                // Wrap contents in a child to make it scrollable if needed
                if (ImGui.BeginChild("ManagementDetails", rightPanelSize, ImGuiChildFlags.None, ImGuiWindowFlags.None))
                {
                    if (this.rightPanelDetailType == RightPanelDetailType.NONE || this.rightPanelDetailUID == string.Empty)
                    {
                        ImGui.TextWrapped("Select an item from the left panel to view details here.");
                    }
                    else
                    if (this.rightPanelDetailType == RightPanelDetailType.CONTRACT)
                    {
                        Contract.Contract? contractToShow = ContractUtils.FindContractFromContractUID(this.rightPanelDetailUID);
                        if (contractToShow != null) {
                            this.DrawContractDetails(contractToShow);
                        }
                        else
                        {
                            // Something bad happened to the detail UID, it became corrupted.
                            this.rightPanelDetailType = RightPanelDetailType.NONE;
                            this.rightPanelDetailUID = string.Empty;
                        }
                    }
                    else
                    if (this.rightPanelDetailType == RightPanelDetailType.MISSION)
                    {
                        Mission.Mission? missionToShow = MissionUtils.FindMissionFromMissionUID(this.rightPanelDetailUID);
                        if (missionToShow != null) {
                            this.DrawMissionDetails(missionToShow);
                        }
                        else
                        {
                            // Something bad happened to the detail UID, it became corrupted.
                            this.rightPanelDetailType = RightPanelDetailType.NONE;
                            this.rightPanelDetailUID = string.Empty;
                        }
                    }

                    ImGui.EndChild();  // End of Management details child
                }
                // Draw the button region
                ImGui.Separator();
                if (this.rightPanelDetailType == RightPanelDetailType.CONTRACT && this.rightPanelDetailUID != string.Empty)
                {
                    Contract.Contract? contractToShow = ContractUtils.FindContractFromContractUID(this.rightPanelDetailUID);
                    if (contractToShow != null) {
                        this.DrawRejectContractButton(contractToShow);
                        this.DrawAcceptContractButton(contractToShow, rightPanelRegionSize);
                    }
                }
                if (this.rightPanelDetailType == RightPanelDetailType.MISSION && this.rightPanelDetailUID != string.Empty)
                {
                    Mission.Mission? missionToShow = MissionUtils.FindMissionFromMissionUID(this.rightPanelDetailUID);
                    if (missionToShow != null) {
                        this.DrawRejectMissionButton(missionToShow);
                        this.DrawAcceptMissionButton(missionToShow, rightPanelRegionSize);
                    }
                }
                ImGui.EndChild();  // End of RightPanel
            }
        }

        private void DrawContractDetails(Contract.Contract contract)
        {
            // Draw contract details
            ImGui.SeparatorText("Contract details: " + contract._contractBlueprint.title);

            if (contract.missionUID != string.Empty)
            {
                Mission.Mission? mission = MissionUtils.FindMissionFromMissionUID(contract.missionUID);
                if (mission != null)
                {
                    ImGui.Text(String.Format("Part of mission: {0}", mission._missionBlueprint.title));
                }
            }

            if (contract.status == ContractStatus.Rejected)
            {
                ImGui.Text("Status: Rejected.");
            }
            if (contract.status == ContractStatus.Completed)
            {
                ImGui.Text("Status: Completed.");
            }
            if (contract.status == ContractStatus.Failed)
            {
                ImGui.Text("Status: Failed.");
            }
                        
            if (contract._contractBlueprint.synopsis != string.Empty)
            {
                // TODO: make bold.
                ImGui.TextWrapped(contract._contractBlueprint.synopsis);
            }
                        
            if (contract._contractBlueprint.description != string.Empty)
            {
                ImGui.TextWrapped(contract._contractBlueprint.description);
            }

            if (!Double.IsPositiveInfinity(contract._contractBlueprint.expiration))
            {
                // Contract can expire
                if (contract.status == ContractStatus.Offered)
                {
                    KSA.SimTime simTime = Universe.GetElapsedSimTime();
                    KSA.SimTime expireOnSimTime = contract.offeredSimTime + contract._contractBlueprint.expiration;
                    KSA.SimTime expireInSimTime = expireOnSimTime - simTime;
                    ImGui.Text(String.Format("Expire offered contract on {0} in {1}", Utils.FormatSimTimeAsYearDayTime(expireOnSimTime), Utils.FormatSimTimeAsRelative(expireInSimTime, true)));
                }
            }
            else
            {
                ImGui.Text("Offered contract does not expire.");
            }

            if (!Double.IsPositiveInfinity(contract._contractBlueprint.deadline))
            {
                // Contract has a deadline
                if (contract.status == ContractStatus.Offered)
                {
                    KSA.SimTime deadlineSimTime = new KSA.SimTime(contract._contractBlueprint.deadline);
                    ImGui.Text(String.Format("Contract has a deadline of {0}", Utils.FormatSimTimeAsRelative(deadlineSimTime, true)));
                }
                else
                if (contract.status == ContractStatus.Accepted)
                {
                    KSA.SimTime simTime = Universe.GetElapsedSimTime();
                    KSA.SimTime deadlineOnSimTime = contract.acceptedSimTime + contract._contractBlueprint.deadline;
                    KSA.SimTime deadlineInSimTime = deadlineOnSimTime - simTime;
                    ImGui.Text(String.Format("Contract has a deadline on {0} in {1}", Utils.FormatSimTimeAsYearDayTime(deadlineOnSimTime), Utils.FormatSimTimeAsRelative(deadlineInSimTime, true)));
                }
            }
            else
            {
                ImGui.Text("Contract does not have a deadline.");
            }
                        
            ImGui.SeparatorText("Rewards");
            ImGui.Text("None implemented yet.");

            this.DrawRequirements(contract);
        }

        // Draw tree of requirements
        private void DrawRequirements(Contract.Contract contract)
        {
            ImGui.SeparatorText("Requirements");
            
            ImGuiTreeNodeFlags requirementTreeNodeFlags = ImGuiTreeNodeFlags.DefaultOpen | ImGuiTreeNodeFlags.DrawLinesToNodes;
            if (ImGui.TreeNodeEx("Contract requirements:", requirementTreeNodeFlags))
            {
                ImGui.Text(String.Format("Complete {0} of these requirements", contract._contractBlueprint.completionCondition));
                foreach (Contract.TrackedRequirement trackedRequirement in contract.trackedRequirements)
                {
                    this.DrawRequirement(contract, trackedRequirement);
                }
                ImGui.TreePop();
            }
        }

        // Draw tree node for requirement
        private void DrawRequirement(Contract.Contract contract, Contract.TrackedRequirement trackedRequirement)
        {
            if (trackedRequirement._blueprintRequirement == null) { return; }

            if (ImGui.TreeNodeEx("Requirement " + trackedRequirement._blueprintRequirement.title, ImGuiTreeNodeFlags.DefaultOpen | ImGuiTreeNodeFlags.DrawLinesToNodes))
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
                    this.DrawRequiredOrbit(contract, trackedRequirement);
                }
                else
                if (trackedRequirement._blueprintRequirement.type == RequirementType.Group)
                {
                    
                    if (contract.status == ContractStatus.Accepted || contract.status == ContractStatus.Completed)
                    {
                        this.DrawTrackedRequiredStatus(trackedRequirement.status);
                    }
                    if (trackedRequirement._blueprintRequirement.group != null)
                    {
                        ImGui.Text(String.Format("Complete {0} of these requirements", trackedRequirement._blueprintRequirement.group.completionCondition));
                    }
                    foreach (Contract.TrackedRequirement childTrackedRequirement in ((TrackedGroup)trackedRequirement).trackedRequirements)
                    {
                       this.DrawRequirement(contract, childTrackedRequirement);
                    }
                }
                ImGui.TreePop();
            }
        }

        private void DrawRequiredOrbit(Contract.Contract contract, Contract.TrackedRequirement trackedRequirement)
        {
            if (trackedRequirement._blueprintRequirement.orbit == null) { return; }
            RequiredOrbit requiredOrbit = trackedRequirement._blueprintRequirement.orbit;

            if (contract.status == ContractStatus.Offered)
            {
                // Show the requirement details
                ImGui.Text(String.Format("Target body {0}", requiredOrbit.targetBody));
                if (requiredOrbit.type != OrbitType.Invalid)
                {
                    ImGui.Text(String.Format("Orbit type {0}", requiredOrbit.type));
                }
                if (!Double.IsNaN(requiredOrbit.minApoapsis))
                {
                    ImGui.Text(String.Format("Min apoapsis {0:F0} m altitude", requiredOrbit.minApoapsis));
                }
                if (!Double.IsNaN(requiredOrbit.maxApoapsis))
                {
                    ImGui.Text(String.Format("Max apoapsis {0:F0} m altitude", requiredOrbit.maxApoapsis));
                }
                if (!Double.IsNaN(requiredOrbit.minPeriapsis))
                {
                    ImGui.Text(String.Format("Min periapsis {0:F0} m altitude", requiredOrbit.minPeriapsis));
                }
                if (!Double.IsNaN(requiredOrbit.maxPeriapsis))
                {
                    ImGui.Text(String.Format("Max periapsis {0:F0} m altitude", requiredOrbit.maxPeriapsis));
                }
                if (!Double.IsNaN(requiredOrbit.minEccentricity))
                {
                    ImGui.Text(String.Format("Min eccentricity {0:F6}", requiredOrbit.minEccentricity));
                }
                if (!Double.IsNaN(requiredOrbit.maxEccentricity))
                {
                    ImGui.Text(String.Format("Max eccentricity {0:F6}", requiredOrbit.maxEccentricity));
                }
                if (!Double.IsNaN(requiredOrbit.minPeriod))
                {
                    ImGui.Text(String.Format("Max period {0}", Utils.FormatSimTimeAsRelative(new KSA.SimTime(requiredOrbit.minPeriod), true)));
                }
                if (!Double.IsNaN(requiredOrbit.maxPeriod))
                {
                    ImGui.Text(String.Format("Max period {0}", Utils.FormatSimTimeAsRelative(new KSA.SimTime(requiredOrbit.maxPeriod), true)));
                }
                if (!Double.IsNaN(requiredOrbit.minLongitudeOfAscendingNode))
                {
                    ImGui.Text(String.Format("Max longitude of ascending node {0:F1}°", requiredOrbit.minLongitudeOfAscendingNode));
                }
                if (!Double.IsNaN(requiredOrbit.maxLongitudeOfAscendingNode))
                {
                    ImGui.Text(String.Format("Max longitude of ascending node {0:F1}°", requiredOrbit.maxLongitudeOfAscendingNode));
                }
                if (!Double.IsNaN(requiredOrbit.minInclination))
                {
                    ImGui.Text(String.Format("Max inclination {0:F1}°", requiredOrbit.minInclination));
                }
                if (!Double.IsNaN(requiredOrbit.maxInclination))
                {
                    ImGui.Text(String.Format("Max inclination {0:F1}°", requiredOrbit.maxInclination));
                }
                if (!Double.IsNaN(requiredOrbit.minArgumentOfPeriapsis))
                {
                    ImGui.Text(String.Format("Max argument of perapsis {0:F1}°", requiredOrbit.minArgumentOfPeriapsis));
                }
                if (!Double.IsNaN(requiredOrbit.maxArgumentOfPeriapsis))
                {
                    ImGui.Text(String.Format("Max argument of perapsis {0:F1}°", requiredOrbit.maxArgumentOfPeriapsis));
                }
            }
            else
            if (contract.status == ContractStatus.Accepted || contract.status == ContractStatus.Completed)
            {
                this.DrawTrackedRequiredStatus(trackedRequirement.status);

                // Show requirement(s) and current tracked state
                var color = Colors.GetTrackedRequirementStatusColor(trackedRequirement.status);
                // target body
                ImGui.TextColored(color, String.Format("Target body {0}", requiredOrbit.targetBody));
                // orbit type
                if (requiredOrbit.type != OrbitType.Invalid)
                {
                    ImGui.TextColored(color, String.Format("Orbit type {0}", requiredOrbit.type));
                }
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
                // Eccentricity
                if (!Double.IsNaN(requiredOrbit.minEccentricity) && !Double.IsNaN(requiredOrbit.maxEccentricity))
                {
                    ImGui.TextColored(
                        color,
                        String.Format(
                            "Eccentricity {0:F6} < {1:F6} < {2:F6}",
                            requiredOrbit.minEccentricity,
                            ((TrackedOrbit)trackedRequirement).eccentricity,
                            requiredOrbit.maxEccentricity));
                }
                else
                if (!Double.IsNaN(requiredOrbit.minEccentricity))
                {
                    ImGui.TextColored(
                        color,
                        String.Format(
                            "Eccentricity {0:F6} < {1:F6}",
                            requiredOrbit.minEccentricity,
                            ((TrackedOrbit)trackedRequirement).eccentricity));
                }
                else
                if (!Double.IsNaN(requiredOrbit.maxEccentricity))
                {
                    ImGui.TextColored(
                        color,
                        String.Format(
                            "Eccentricity {0:F6} > {1:F6}",
                            requiredOrbit.maxEccentricity,
                            ((TrackedOrbit)trackedRequirement).eccentricity));
                }
                // Period
                if (!Double.IsNaN(requiredOrbit.minPeriod) && !Double.IsNaN(requiredOrbit.maxPeriod))
                {
                    ImGui.TextColored(
                        color,
                        String.Format(
                            "Period {0} < {1} < {2}",
                            Utils.FormatSimTimeAsRelative(new KSA.SimTime(requiredOrbit.minPeriod), true),
                            Utils.FormatSimTimeAsRelative(new KSA.SimTime(((TrackedOrbit)trackedRequirement).period), true),
                            Utils.FormatSimTimeAsRelative(new KSA.SimTime(requiredOrbit.maxPeriod), true)));
                }
                else
                if (!Double.IsNaN(requiredOrbit.minPeriod))
                {
                    ImGui.TextColored(
                        color,
                        String.Format(
                            "Period {0} < {1}",
                            Utils.FormatSimTimeAsRelative(new KSA.SimTime(requiredOrbit.minPeriod), true),
                            Utils.FormatSimTimeAsRelative(new KSA.SimTime(((TrackedOrbit)trackedRequirement).period), true)));
                }
                else
                if (!Double.IsNaN(requiredOrbit.maxPeriod))
                {
                    ImGui.TextColored(
                        color,
                        String.Format(
                            "Period {0} > {1}",
                            Utils.FormatSimTimeAsRelative(new KSA.SimTime(requiredOrbit.maxPeriod), true),
                            Utils.FormatSimTimeAsRelative(new KSA.SimTime(((TrackedOrbit)trackedRequirement).period), true)));
                }
                // Longitude of Ascending Node
                if (!Double.IsNaN(requiredOrbit.minLongitudeOfAscendingNode) && !Double.IsNaN(requiredOrbit.maxLongitudeOfAscendingNode))
                {
                    ImGui.TextColored(
                        color,
                        String.Format(
                            "Longitude of Ascending Node {0:F1}° < {1:F1}° < {2:F1}°",
                            requiredOrbit.minLongitudeOfAscendingNode,
                            ((TrackedOrbit)trackedRequirement).longitudeOfAscendingNode,
                            requiredOrbit.maxLongitudeOfAscendingNode));
                }
                else
                if (!Double.IsNaN(requiredOrbit.minLongitudeOfAscendingNode))
                {
                    ImGui.TextColored(
                        color,
                        String.Format(
                            "Longitude of Ascending Node {0:F1}° < {1:F1}°",
                            requiredOrbit.minLongitudeOfAscendingNode,
                            ((TrackedOrbit)trackedRequirement).longitudeOfAscendingNode));
                }
                else
                if (!Double.IsNaN(requiredOrbit.maxLongitudeOfAscendingNode))
                {
                    ImGui.TextColored(
                        color,
                        String.Format(
                            "Longitude of Ascending Node {0:F1}° > {1:F1}°",
                            requiredOrbit.maxLongitudeOfAscendingNode,
                            ((TrackedOrbit)trackedRequirement).longitudeOfAscendingNode));
                }
                // Inclination
                if (!Double.IsNaN(requiredOrbit.minInclination) && !Double.IsNaN(requiredOrbit.maxInclination))
                {
                    ImGui.TextColored(
                        color,
                        String.Format(
                            "Inclination {0:F1}° < {1:F1}° < {2:F1}°",
                            requiredOrbit.minInclination,
                            ((TrackedOrbit)trackedRequirement).inclination,
                            requiredOrbit.maxInclination));
                }
                else
                if (!Double.IsNaN(requiredOrbit.minInclination))
                {
                    ImGui.TextColored(
                        color,
                        String.Format(
                            "Inclination {0:F1}° < {1:F1}°",
                            requiredOrbit.minInclination,
                            ((TrackedOrbit)trackedRequirement).inclination));
                }
                else
                if (!Double.IsNaN(requiredOrbit.maxInclination))
                {
                    ImGui.TextColored(
                        color,
                        String.Format(
                            "Inclination {0:F1}° > {1:F1}°",
                            requiredOrbit.maxInclination,
                            ((TrackedOrbit)trackedRequirement).inclination));
                }
                // Argument of Periapsis
                if (!Double.IsNaN(requiredOrbit.minArgumentOfPeriapsis) && !Double.IsNaN(requiredOrbit.maxArgumentOfPeriapsis))
                {
                    ImGui.TextColored(
                        color,
                        String.Format(
                            "Argument of Periapsis {0:F1}° < {1:F1}° < {2:F1}°",
                            requiredOrbit.minArgumentOfPeriapsis,
                            ((TrackedOrbit)trackedRequirement).argumentOfPeriapsis,
                            requiredOrbit.maxArgumentOfPeriapsis));
                }
                else
                if (!Double.IsNaN(requiredOrbit.minArgumentOfPeriapsis))
                {
                    ImGui.TextColored(
                        color,
                        String.Format(
                            "Argument of Periapsis {0:F1}° < {1:F1}°",
                            requiredOrbit.minArgumentOfPeriapsis,
                            ((TrackedOrbit)trackedRequirement).argumentOfPeriapsis));
                }
                else
                if (!Double.IsNaN(requiredOrbit.maxArgumentOfPeriapsis))
                {
                    ImGui.TextColored(
                        color,
                        String.Format(
                            "Argument of Periapsis {0:F1}° > {1:F1}°",
                            requiredOrbit.maxArgumentOfPeriapsis,
                            ((TrackedOrbit)trackedRequirement).argumentOfPeriapsis));
                }
            }
        }

        private void DrawTrackedRequiredStatus(Contract.TrackedRequirementStatus status)
        {
            var color = Colors.GetTrackedRequirementStatusColor(status);
            // Show the status of the requirement
            if (status == TrackedRequirementStatus.NOT_STARTED)
            {
                ImGui.TextColored(color, String.Format("Status: Not yet started..."));
            }
            else
            if (status == TrackedRequirementStatus.TRACKED)
            {
                ImGui.TextColored(color, String.Format("Status: Not yet achieved..."));
            }
            else
            if (status == TrackedRequirementStatus.MAINTAINED)
            {
                ImGui.TextColored(color, String.Format("Status: Maintain until other requirements are achieved."));
            }
            else
            if (status == TrackedRequirementStatus.ACHIEVED)
            {
                ImGui.TextColored(color, String.Format("Status: Achieved!"));
            }
            else
            if (status == TrackedRequirementStatus.FAILED)
            {
                ImGui.TextColored(color, String.Format("Status: Failed!"));
            }
        }
        
        private void DrawMissionDetails(Mission.Mission mission)
        {
            // Draw mission details
            ImGui.SeparatorText("Mission details: " + mission._missionBlueprint.title);

            if (mission.status == MissionStatus.Rejected)
            {
                ImGui.Text("Status: Rejected.");
            }
            if (mission.status == MissionStatus.Completed)
            {
                ImGui.Text("Status: Completed.");
            }
            if (mission.status == MissionStatus.Failed)
            {
                ImGui.Text("Status: Failed.");
            }

            if (mission._missionBlueprint.synopsis != string.Empty)
            {
                // TODO: make bold.
                ImGui.TextWrapped(mission._missionBlueprint.synopsis);
            }
                        
            if (mission._missionBlueprint.description != string.Empty)
            {
                ImGui.TextWrapped(mission._missionBlueprint.description);
            }

            if (!Double.IsPositiveInfinity(mission._missionBlueprint.expiration))
            {
                // Mission can expire
                if (mission.status == MissionStatus.Offered)
                {
                    KSA.SimTime simTime = Universe.GetElapsedSimTime();
                    KSA.SimTime expireOnSimTime = mission.offeredSimTime + mission._missionBlueprint.expiration;
                    KSA.SimTime expireInSimTime = expireOnSimTime - simTime;
                    ImGui.Text(String.Format("Expire offered mission on {0} in {1}", Utils.FormatSimTimeAsYearDayTime(expireOnSimTime), Utils.FormatSimTimeAsRelative(expireInSimTime, true)));
                }
            }
            else
            {
                ImGui.Text("Offered mission does not expire.");
            }

            if (!Double.IsPositiveInfinity(mission._missionBlueprint.deadline))
            {
                // Mission has a deadline
                if (mission.status == MissionStatus.Offered)
                {
                    KSA.SimTime deadlineSimTime = new KSA.SimTime(mission._missionBlueprint.deadline);
                    ImGui.Text(String.Format("Mission has a deadline of {0}", Utils.FormatSimTimeAsRelative(deadlineSimTime, true)));
                }
                else
                if (mission.status == MissionStatus.Accepted)
                {
                    KSA.SimTime simTime = Universe.GetElapsedSimTime();
                    KSA.SimTime deadlineOnSimTime = mission.acceptedSimTime + mission._missionBlueprint.deadline;
                    KSA.SimTime deadlineInSimTime = deadlineOnSimTime - simTime;
                    ImGui.Text(String.Format("Mission has a deadline on {0} in {1}", Utils.FormatSimTimeAsYearDayTime(deadlineOnSimTime), Utils.FormatSimTimeAsRelative(deadlineInSimTime, true)));
                }
            }
            else
            {
                ImGui.Text("Mission does not have a deadline.");
            }
                        
            ImGui.SeparatorText("Rewards");
            ImGui.Text("None implemented yet.");

            // List all contracts as buttons to show details. -> WHY? they should be shown on the left panel!
        }
        
        private void DrawRejectContractButton(Contract.Contract contract)
        {
            if (contract == null) { return; }
            if (!(contract.status is ContractStatus.Offered or ContractStatus.Accepted)) { return; }

            // Show reject button
            if (contract._contractBlueprint.isRejectable)
            {
                ImGui.PushStyleColor(ImGuiCol.Button, Colors.redDark);
                ImGui.PushStyleColor(ImGuiCol.ButtonHovered, Colors.redLight);
                ImGui.PushStyleColor(ImGuiCol.ButtonActive, Colors.red);
            }
            else
            {
                ImGui.PushStyleColor(ImGuiCol.Button, Colors.grayDark);
                ImGui.PushStyleColor(ImGuiCol.ButtonHovered, Colors.grayDark);
                ImGui.PushStyleColor(ImGuiCol.ButtonActive, Colors.grayDark);
            }
            if (ImGui.Button("Reject Contract"))
            {
                if (contract._contractBlueprint.isRejectable)
                {
                    contract.RejectContract(Universe.GetElapsedSimTime());
                    if (ContractManager.data.offeredContracts.Contains(contract))
                    {
                        ContractManager.data.offeredContracts.Remove(contract);
                    }
                    if (ContractManager.data.acceptedContracts.Contains(contract))
                    {
                        ContractManager.data.acceptedContracts.Remove(contract);
                        ContractManager.data.finishedContracts.Add(contract);
                    }
                }
            }
            ImGui.PopStyleColor(3);
        }

        private void DrawAcceptContractButton(Contract.Contract contract, Brutal.Numerics.float2 rightPanelRegionSize)
        {
            if (contract == null) { return; }
            if (contract.status != ContractStatus.Offered) { return; }

            // Show accept button
            ImGui.SameLine();
            var style = ImGui.GetStyle();
            float buttonWidthReject = ImGui.CalcTextSize("Reject Contract").X + style.FramePadding.X * 2.0f;
            float buttonWidthAccept = ImGui.CalcTextSize("Accept Contract").X + style.FramePadding.X * 2.0f;
            float resizeTriangleWidth = 25.0f;
            ImGui.SetCursorPosX(ImGui.GetCursorPosX() + rightPanelRegionSize.X - buttonWidthReject - buttonWidthAccept - resizeTriangleWidth);
            if (ContractManager.data.maxNumberOfAcceptedContracts > ContractManager.data.acceptedContracts.Count)
            {
                ImGui.PushStyleColor(ImGuiCol.Button, Colors.greenDark);
                ImGui.PushStyleColor(ImGuiCol.ButtonHovered, Colors.greenLight);
                ImGui.PushStyleColor(ImGuiCol.ButtonActive, Colors.green);
            }
            else
            {
                ImGui.PushStyleColor(ImGuiCol.Button, Colors.grayDark);
                ImGui.PushStyleColor(ImGuiCol.ButtonHovered, Colors.grayDark);
                ImGui.PushStyleColor(ImGuiCol.ButtonActive, Colors.grayDark);
            }
            if (ImGui.Button("Accept Contract"))
            {
                if (ContractManager.data.maxNumberOfAcceptedContracts > ContractManager.data.acceptedContracts.Count)
                {
                    contract.AcceptOfferedContract(Universe.GetElapsedSimTime());
                    ContractManager.data.offeredContracts.Remove(contract);
                    ContractManager.data.acceptedContracts.Add(contract);
                }
            }
            ImGui.PopStyleColor(3);
        }
        
        private void DrawRejectMissionButton(Mission.Mission mission)
        {
            if (mission == null) { return; }
            if (!(mission.status is MissionStatus.Offered or MissionStatus.Accepted)) { return; }

            // Show reject button
            if (mission._missionBlueprint.isRejectable)
            {
                ImGui.PushStyleColor(ImGuiCol.Button, Colors.redDark);
                ImGui.PushStyleColor(ImGuiCol.ButtonHovered, Colors.redLight);
                ImGui.PushStyleColor(ImGuiCol.ButtonActive, Colors.red);
            }
            else
            {
                ImGui.PushStyleColor(ImGuiCol.Button, Colors.grayDark);
                ImGui.PushStyleColor(ImGuiCol.ButtonHovered, Colors.grayDark);
                ImGui.PushStyleColor(ImGuiCol.ButtonActive, Colors.grayDark);
            }
            if (ImGui.Button("Reject Mission"))
            {
                if (mission._missionBlueprint.isRejectable)
                {
                    mission.RejectMission(Universe.GetElapsedSimTime());
                    if (ContractManager.data.offeredMissions.Contains(mission))
                    {
                        ContractManager.data.offeredMissions.Remove(mission);
                    }
                    if (ContractManager.data.acceptedMissions.Contains(mission))
                    {
                        ContractManager.data.acceptedMissions.Remove(mission);
                        ContractManager.data.finishedMissions.Add(mission);
                    }
                }
            }
            ImGui.PopStyleColor(3);
        }

        private void DrawAcceptMissionButton(Mission.Mission mission, Brutal.Numerics.float2 rightPanelRegionSize)
        {
            if (mission == null) { return; }
            if (mission.status != MissionStatus.Offered) { return; }

            // Show accept button
            ImGui.SameLine();
            var style = ImGui.GetStyle();
            float buttonWidthReject = ImGui.CalcTextSize("Reject Mission").X + style.FramePadding.X * 2.0f;
            float buttonWidthAccept = ImGui.CalcTextSize("Accept Mission").X + style.FramePadding.X * 2.0f;
            float resizeTriangleWidth = 25.0f;
            ImGui.SetCursorPosX(ImGui.GetCursorPosX() + rightPanelRegionSize.X - buttonWidthReject - buttonWidthAccept - resizeTriangleWidth);
            if (ContractManager.data.maxNumberOfAcceptedMissions > ContractManager.data.acceptedMissions.Count)
            {
                ImGui.PushStyleColor(ImGuiCol.Button, Colors.greenDark);
                ImGui.PushStyleColor(ImGuiCol.ButtonHovered, Colors.greenLight);
                ImGui.PushStyleColor(ImGuiCol.ButtonActive, Colors.green);
            }
            else
            {
                ImGui.PushStyleColor(ImGuiCol.Button, Colors.grayDark);
                ImGui.PushStyleColor(ImGuiCol.ButtonHovered, Colors.grayDark);
                ImGui.PushStyleColor(ImGuiCol.ButtonActive, Colors.grayDark);
            }
            if (ImGui.Button("Accept Mission"))
            {
                if (ContractManager.data.maxNumberOfAcceptedMissions > ContractManager.data.acceptedMissions.Count)
                {
                    mission.AcceptOfferedMission(Universe.GetElapsedSimTime());
                    ContractManager.data.offeredMissions.Remove(mission);
                    ContractManager.data.acceptedMissions.Add(mission);
                }
            }
            ImGui.PopStyleColor(3);
        }

        // TODO Move to GUI.Utils
        static internal void DrawHelpTooltip(string text)
        {
            ImGui.TextDisabled("(?)");
            if (ImGui.BeginItemTooltip())
            {
                ImGui.PushTextWrapPos(ImGui.GetFontSize() * 35.0f);
                ImGui.Text(text);
                ImGui.PopTextWrapPos();
                ImGui.EndTooltip();
            }
        }
    }
}
