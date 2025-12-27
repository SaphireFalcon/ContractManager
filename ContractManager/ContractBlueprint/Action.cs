using System.Xml.Serialization;
using System.Collections.Generic;

namespace ContractManager.ContractBlueprint
{
    public class Action
    {

        public enum ActionType
        {
            [XmlEnum("showMessage")]
            ShowMessage
        }

        public enum TriggerType
        {
            [XmlEnum("onContractComplete")]
            OnContractComplete,
            [XmlEnum("onContractFail")]
            OnContractFail
        }

        // The trigger of the action.
        [XmlElement("trigger")]
        public TriggerType trigger { get; set; }

        // The type of the action.
        [XmlElement("type")]
        public ActionType type { get; set; }

        // Fields for specific action types.
        // type: showMessage - message to show when triggered.
        [XmlElement("showMessage")]
        public string showMessage { get; set; }

        public Action() { }

        public void WriteToConsole()
        {
            Console.WriteLine($"  - trigger: {trigger}");
            if (type == ActionType.ShowMessage)
            {
                Console.WriteLine($"    type: showMessage: {showMessage}");
            }
        }

        public void DoAction()
        {
            Console.WriteLine($"[CM] DoAction {this.type}");
            if (type == ActionType.ShowMessage) {
                this.ShowMessage();
            }
        }

        // Actions
        private void ShowMessage()
        {
            Console.WriteLine($"[CM] ShowMessage: '{this.showMessage}'");
        }
    }
}
