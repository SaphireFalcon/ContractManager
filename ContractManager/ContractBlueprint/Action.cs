using System.Xml.Serialization;
using System.Collections.Generic;

namespace ContractManager.ContractBlueprint
{
    public class Action
    {
        // The trigger of the action.
        [XmlElement("trigger")]
        public string trigger { get; set; }

        [XmlElement("type")]
        public string type { get; set; }

        [XmlElement("showMessage")]
        public string showMessage { get; set; }

        public Action() { }

        public void WriteToConsole()
        {
            Console.WriteLine($"  - trigger: {trigger}");
            if (type == "showMessage")
            {
                Console.WriteLine($"    type: showMessage: {showMessage}");
            }
        }
    }
}
