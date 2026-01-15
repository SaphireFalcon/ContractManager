using System.Xml.Serialization;
using System.Collections.Generic;
using ContractManager.Contract;
using System.Xml.Linq;

namespace ContractManager.ContractBlueprint
{
    public class Action
    {
        // The unique identifier for the action.
        [XmlElement("uid", DataType = "string")]
        public string uid { get; set; }

        // The trigger of the action.
        [XmlElement("trigger")]
        public TriggerType trigger { get; set; }

        // The type of the action.
        [XmlElement("type")]
        public ActionType type { get; set; }

        // Fields for specific action types.
        // type: [ShowMessage, ShowBlockingPopup] - message to show when triggered.
        [XmlElement("showMessage", DataType = "string")]
        public string showMessage { get; set; } = string.Empty;
        
        // trigger: onRequirement* type of trigger - which requirement should trigger this action.
        [XmlElement("onRequirement", DataType = "string")]
        public string onRequirement { get; set; }

        public Action() { }

        public void WriteToConsole()
        {
            Console.WriteLine($"  - trigger: {trigger}");
            if (type == ActionType.ShowMessage)
            {
                Console.WriteLine($"    type: showMessage: {showMessage}");
            }
        }

        public void DoAction(Contract.Contract contract)
        {
            Console.WriteLine($"[CM] DoAction {this.type}");
            if (type == ActionType.ShowMessage) {
                this.ShowMessage(contract);
            }
            if (type == ActionType.ShowBlockingPopup) {
                this.ShowMessage(contract);
            }
        }

        public void DoAction(Mission.Mission mission)
        {
            Console.WriteLine($"[CM] DoAction {this.type}");
            if (type == ActionType.ShowMessage) {
                this.ShowMessage(mission);
            }
            if (type == ActionType.ShowBlockingPopup) {
                this.ShowMessage(mission);
            }
        }

        // Actions
        private void ShowMessage(Contract.Contract contract)
        {
            Console.WriteLine($"[CM] ShowMessage: '{this.showMessage}'");
            if (contract._contractBlueprint == null) { return; }

            ContractManager.data.popupWindows.Add(
                new GUI.PopupWindow
                {
                    title = contract._contractBlueprint.title,
                    uid = String.Format("contract{0}_{1}", contract.contractUID, this.trigger),
                    messageToShow = this.showMessage,
                    popupType = this.type == ActionType.ShowMessage ? GUI.PopupType.Popup : GUI.PopupType.Modal
                }
            );
        }

        private void ShowMessage(Mission.Mission mission)
        {
            Console.WriteLine($"[CM] ShowMessage: '{this.showMessage}'");
            if (mission._missionBlueprint == null) { return; }

            ContractManager.data.popupWindows.Add(
                new GUI.PopupWindow
                {
                    title = mission._missionBlueprint.title,
                    uid = String.Format("mission{0}_{1}", mission.missionUID, this.trigger),
                    messageToShow = this.showMessage,
                    popupType = this.type == ActionType.ShowMessage ? GUI.PopupType.Popup : GUI.PopupType.Modal
                }
            );
        }

        internal bool Validate()
        {
            // The uid can't be empty
            if (String.IsNullOrEmpty(this.uid))
            {
                Console.WriteLine("[CM] [WARNING] action uid has be to be defined.");
                return false;
            }
            if ((type is ActionType.ShowMessage or ActionType.ShowBlockingPopup) && String.IsNullOrEmpty(this.showMessage))
            {
                Console.WriteLine($"[CM] [WARNING] action type = '{type}' `showMessage` field can't be empty.");
                return false;
            }
            // ActionType and TriggerType don't need to be validated loading XML will throw an exception.
            return true;
        }

        // Migrate function for v0.2.2 adding UID to Action.
        internal static void MigrateAddUID(ref XElement actionsElement, in string parentUID)
        {
            int actionElementCounter = 0;
            foreach (XElement actionElement in actionsElement.Elements())
            {
                actionElement.Add(new XElement("uid", String.Format("{0}_{1}", parentUID, actionElementCounter++)));
            }
        }
    }

    public enum ActionType
    {
        [XmlEnum("showMessage")]
        ShowMessage,
        [XmlEnum("showBlockingPopup")]
        ShowBlockingPopup,
    }

    public enum TriggerType
    {
        // Contract
        [XmlEnum("onContractOffer")]
        OnContractOffer,  // transition to ContractStatus.Offered
        [XmlEnum("onContractAccept")]
        OnContractAccept,  // transition to ContractStatus.Accepted
        [XmlEnum("onContractExpire")]
        OnContractExpire,  // transition to ContractStatus.Rejected when expire time passed
        [XmlEnum("onContractReject")]
        OnContractReject,  // transition to ContractStatus.Rejected when reject button pressed
        [XmlEnum("onContractComplete")]
        OnContractComplete,  // transition to ContractStatus.Completed
        [XmlEnum("onContractFail")]
        OnContractFail,  // transition to ContractStatus.Failed

        // Requirement
        [XmlEnum("onRequirementStarted")]
        OnRequirementTracked,  // transition to TrackedRequirementStatus.TRACKED
        [XmlEnum("onRequirementMaintained")]
        OnRequirementMaintained,  // transition to TrackedRequirementStatus.MAINTAINED
        [XmlEnum("onRequirementReverted")]
        OnRequirementReverted,  // transition from TrackedRequirementStatus.MAINTAINED back to TrackedRequirementStatus.TRACKED
        [XmlEnum("onRequirementAchieved")]
        OnRequirementAchieved,  // transition to TrackedRequirementStatus.ACHIEVED
        [XmlEnum("onRequirementFailed")]
        OnRequirementFailed,  // transition to TrackedRequirementStatus.FAILED
        
        // Mission
        [XmlEnum("onMissionOffer")]
        OnMissionOffer,  // transition to MissionStatus.Offered
        [XmlEnum("onMissionAccept")]
        OnMissionAccept,  // transition to MissionStatus.Accepted
        [XmlEnum("onMissionExpire")]
        OnMissionExpire,  // transition to MissionStatus.Rejected when expire time passed
        [XmlEnum("onMissionReject")]
        OnMissionReject,  // transition to MissionStatus.Rejected when reject button pressed
        [XmlEnum("onMissionComplete")]
        OnMissionComplete,  // transition to MissionStatus.Completed
        [XmlEnum("onMissionFail")]
        OnMissionFail,  // transition to MissionStatus.Failed
    }
}
