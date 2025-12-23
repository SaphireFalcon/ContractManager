using System.Xml.Serialization;
using System.Collections.Generic;

namespace ContractManager.ContractBlueprint
{
    public class Requirement
    {
        // The unique identifier for the requirement.
        [XmlElement("uid")]
        public string uid { get; set; }

        // Type of the requirement.
        [XmlElement("type")]
        public string type { get; set; }

        // The title of the requirement.
        [XmlElement("title")]
        public string title { get; set; }

        // A brief synopsis of the requirement.
        [XmlElement("synopsis")]
        public string synopsis { get; set; }

        // Detailed description of the requirement.
        [XmlElement("description")]
        public string description { get; set; }

        // Flag if the requirement is completed upon achievement.
        [XmlElement("isCompletedOnAchievement")]
        public bool isCompletedOnAchievement { get; set; } = true;

        // Flag if the requirement is hidden until previous requirement was achieved.
        [XmlElement("isHidden")]
        public bool isHidden { get; set; } = false;

        // Flag if the requirement can only be completed after previous requirement was achieved.
        [XmlElement("completeInOrder")]
        public bool completeInOrder { get; set; } = true;

        // List of child requirements for the contract.
        [XmlElement("requirements")]
        public List<Requirement> requirements { get; set; } = new List<Requirement>();

        // Completion condition of the requirement based on its child requirements.
        [XmlElement("completionCondition")]
        public string completionCondition { get; set; } = "all";

        // Fields for specific requirement types.
        // type: orbit - field for orbit requirement type.
        [XmlElement("Orbit")]
        public RequiredOrbit orbit { get; set; } = null;

        public Requirement() { }
                
        public void WriteToConsole(int hierachyLevel = 1)
        {
            string indent = new string(' ', hierachyLevel * 2);
            Console.WriteLine($"{indent}- Requirement: {uid}");
            Console.WriteLine($"{indent}  type: {type}");
            Console.WriteLine($"{indent}  title: {title}");
            Console.WriteLine($"{indent}  synopsis: {synopsis}");
            Console.WriteLine($"{indent}  description: {description}");
            Console.WriteLine($"{indent}  isCompletedOnAchievement: {isCompletedOnAchievement}");
            Console.WriteLine($"{indent}  isHidden: {isHidden}");
            Console.WriteLine($"{indent}  completeInOrder: {completeInOrder}");
            Console.WriteLine($"{indent}  completionCondition: {completionCondition}");
            Console.WriteLine($"{indent}  child requirements: {requirements.Count}");
            foreach (Requirement requirement in requirements)
            {
                requirement.WriteToConsole(hierachyLevel + 1);
            }
        }
    }

    public class RequiredOrbit
    {
        // Fields needed for the orbit requirement type.

        // The celestial body to orbit.
        [XmlElement("targetBody")]
        public string targetBody { get; set; }

        // minimum apoapsis altitude in meters.
        [XmlElement("minApoapsis")]
        public double minApoapsis { get; set; } = 0;

        // maximum apoapsis altitude in meters.
        [XmlElement("maxApoapsis")]
        public double maxApoapsis { get; set; } = double.PositiveInfinity;

        // minimum periapsis altitude in meters.
        [XmlElement("minPeriapsis")]
        public double minPeriapsis { get; set; } = 0;

        // maximum periapsis altitude in meters.
        [XmlElement("maxPeriapsis")]
        public double maxPeriapsis { get; set; } = double.PositiveInfinity;

        public RequiredOrbit() { }
    }
}
