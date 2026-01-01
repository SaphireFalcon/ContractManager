using System.Xml.Serialization;
using System.Collections.Generic;

namespace ContractManager.ContractBlueprint
{
    public class Action
    {
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

        // Actions
        private void ShowMessage(Contract.Contract contract)
        {
            Console.WriteLine($"[CM] ShowMessage: '{this.showMessage}'");
            if (contract._contractBlueprint == null) { return; }

            ContractManager.data.popupWindows.Add(
                new GUI.PopupWindow
                {
                    title = contract._contractBlueprint.title,
                    messageToShow = this.showMessage,
                    popupType = this.type == ActionType.ShowMessage ? GUI.PopupType.Popup : GUI.PopupType.Modal
                }
            );
        }

        internal bool Validate()
        {
            if ((type is ActionType.ShowMessage or ActionType.ShowBlockingPopup) && String.IsNullOrEmpty(this.showMessage))
            {
                Console.WriteLine($"[CM] [WARNING] action type = '{type}' `showMessage` field can't be empty.");
                return false;
            }
            // ActionType and TriggerType don't need to be validated loading XML will throw an exception.
            return true;
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
        [XmlEnum("onContractComplete")]
        OnContractComplete,
        [XmlEnum("onContractFail")]
        OnContractFail
    }
}
