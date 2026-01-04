using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Xml.Serialization;

namespace ContractManager.ContractBlueprint
{
    public class Requirement
    {
        // The unique identifier for the requirement.
        [XmlElement("uid", DataType = "string")]
        public string uid { get; set; }

        // Type of the requirement.
        [XmlElement("type")]
        public RequirementType type { get; set; }

        // The title of the requirement.
        [XmlElement("title", DataType = "string")]
        public string title { get; set; }

        // A brief synopsis of the requirement.
        [XmlElement("synopsis", DataType = "string")]
        public string synopsis { get; set; } = string.Empty;

        // Detailed description of the requirement.
        [XmlElement("description", DataType = "string")]
        public string description { get; set; } = string.Empty;

        // Flag if the requirement is completed upon achievement.
        [XmlElement("isCompletedOnAchievement", DataType = "boolean")]
        public bool isCompletedOnAchievement { get; set; } = true;

        // Flag if the requirement is hidden until previous requirement was achieved.
        [XmlElement("isHidden", DataType = "boolean")]
        public bool isHidden { get; set; } = false;

        // Flag if the requirement can only be completed after previous requirement was achieved.
        [XmlElement("completeInOrder", DataType = "boolean")]
        public bool completeInOrder { get; set; } = true;

        // Fields for specific requirement types.
        // type: orbit - field for orbit requirement type.
        [XmlElement("Orbit")]
        public RequiredOrbit? orbit { get; set; } = null;
        
        // type: group - field for group requirement type.
        [XmlElement("Group")]
        public RequiredGroup? group { get; set; } = null;

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
            if (type == RequirementType.Orbit && orbit != null)
            {
                Console.WriteLine($"{indent}  Required Orbit:");
                Console.WriteLine($"{indent}    targetBody: {orbit.targetBody}");
                Console.WriteLine($"{indent}    type: {orbit.type}");
                Console.WriteLine($"{indent}    minApoapsis: {orbit.minApoapsis}");
                Console.WriteLine($"{indent}    maxApoapsis: {orbit.maxApoapsis}");
                Console.WriteLine($"{indent}    minPeriapsis: {orbit.minPeriapsis}");
                Console.WriteLine($"{indent}    maxPeriapsis: {orbit.maxPeriapsis}");

                Console.WriteLine($"{indent}    minEccentricity: {orbit.minEccentricity}");
                Console.WriteLine($"{indent}    maxEccentricity: {orbit.maxEccentricity}");
                Console.WriteLine($"{indent}    minPeriod: {orbit.minPeriod}");
                Console.WriteLine($"{indent}    maxPeriod: {orbit.maxPeriod}");
                
                Console.WriteLine($"{indent}    minLongitudeOfAscendingNode: {orbit.minLongitudeOfAscendingNode}");
                Console.WriteLine($"{indent}    maxLongitudeOfAscendingNode: {orbit.maxLongitudeOfAscendingNode}");
                Console.WriteLine($"{indent}    minInclination: {orbit.minInclination}");
                Console.WriteLine($"{indent}    maxInclination: {orbit.maxInclination}");
                Console.WriteLine($"{indent}    minArgumentOfPeriapsis: {orbit.minArgumentOfPeriapsis}");
                Console.WriteLine($"{indent}    maxArgumentOfPeriapsis: {orbit.maxArgumentOfPeriapsis}");
            }
            if (type == RequirementType.Group && group != null)
            {
                Console.WriteLine($"{indent}  completionCondition: {group.completionCondition}");
                Console.WriteLine($"{indent}  child requirements: {group.requirements.Count}");
                foreach (Requirement requirement in group.requirements)
                {
                    requirement.WriteToConsole(hierachyLevel + 1);
                }
            }
        }

        internal bool Validate()
        {
            // The title can't be empty
            if (String.IsNullOrEmpty(this.title))
            {
                Console.WriteLine("[CM] [WARNING] requirement title has be to be defined.");
                return false;
            }
            // The uid can't be empty
            if (String.IsNullOrEmpty(this.uid))
            {
                Console.WriteLine("[CM] [WARNING] requirement uid has be to be defined.");
                return false;
            }
            // Validate type and their fields.
            if (type == RequirementType.Group && this.group == null)
            {
                Console.WriteLine($"[CM] [WARNING] requirement type = '{type}' `group` field can't be empty.");
                return false;
            }
            else
            if (type == RequirementType.Orbit && this.orbit == null)
            {
                Console.WriteLine($"[CM] [WARNING] requirement type = '{type}' `orbit` field can't be empty.");
                return false;
            }
            if (type == RequirementType.Group && !this.group.Validate())
            {
                return false;
            }
            else
            if (type == RequirementType.Orbit && !this.orbit.Validate())
            {
                return false;
            }

            // RequirementType doesn't need to be validated, loading XML will throw an exception.
            return true;
        }
    }

    public enum RequirementType
    {
        [XmlEnum("orbit")]
        Orbit,
        [XmlEnum("group")]
        Group
    }
    
    public enum OrbitType
    {
        [XmlEnum("invalid")]
        Invalid = 0,
        [XmlEnum("elliptical")]
        Elliptical = 1,
        [XmlEnum("suborbit")]
        Suborbit = 2,
        [XmlEnum("escape")]
        Escape = 3
    }
    public class RequiredOrbit
    {
        // Fields needed for the orbit requirement type.
        // The celestial body to orbit.
        [XmlElement("targetBody", DataType = "string")]
        public string targetBody { get; set; } = string.Empty;

        // The orbit type.
        [XmlElement("type")]
        public OrbitType type { get; set; } = OrbitType.Invalid;

        // minimum apoapsis altitude in meters.
        [XmlElement("minApoapsis", DataType = "double")]
        public double minApoapsis { get; set; } = double.NaN;

        // maximum apoapsis altitude in meters.
        [XmlElement("maxApoapsis", DataType = "double")]
        public double maxApoapsis { get; set; } = double.NaN;

        // minimum periapsis altitude in meters.
        [XmlElement("minPeriapsis", DataType = "double")]
        public double minPeriapsis { get; set; } = double.NaN;

        // maximum periapsis altitude in meters.
        [XmlElement("maxPeriapsis", DataType = "double")]
        public double maxPeriapsis { get; set; } = double.NaN;

        // minimum Eccentricity (ratio)
        [XmlElement("minEccentricity", DataType = "double")]
        public double minEccentricity { get; set; } = double.NaN;
        
        // maximum Eccentricity (ratio)
        [XmlElement("maxEccentricity", DataType = "double")]
        public double maxEccentricity { get; set; } = double.NaN;

        // minimum Period in seconds
        [XmlElement("minPeriod", DataType = "double")]
        public double minPeriod { get; set; } = double.NaN;

        // maximum Period in seconds
        [XmlElement("maxPeriod", DataType = "double")]
        public double maxPeriod { get; set; } = double.NaN;

        // minimum LongitudeOfAscendingNode in degrees
        [XmlElement("minLongitudeOfAscendingNode", DataType = "double")]
        public double minLongitudeOfAscendingNode { get; set; } = double.NaN;

        // maximum LongitudeOfAscendingNode in degrees
        [XmlElement("maxLongitudeOfAscendingNode", DataType = "double")]
        public double maxLongitudeOfAscendingNode { get; set; } = double.NaN;

        // minimum Inclination in degrees
        [XmlElement("minInclination", DataType = "double")]
        public double minInclination { get; set; } = double.NaN;

        // maximum Inclination in degrees
        [XmlElement("maxInclination", DataType = "double")]
        public double maxInclination { get; set; } = double.NaN;

        // minimum ArgumentOfPeriapsis in degrees
        [XmlElement("minArgumentOfPeriapsis", DataType = "double")]
        public double minArgumentOfPeriapsis { get; set; } = double.NaN;

        // maximum ArgumentOfPeriapsis in degrees
        [XmlElement("maxArgumentOfPeriapsis", DataType = "double")]
        public double maxArgumentOfPeriapsis { get; set; } = double.NaN;

        public RequiredOrbit() { }
        
        internal bool Validate()
        {
            // The targetBody can't be empty
            if (String.IsNullOrEmpty(this.targetBody))
            {
                Console.WriteLine($"[CM] [WARNING] orbits targetBody has be to be defined.");
                return false;
            }
            if (this.minApoapsis > this.maxApoapsis)
            {
                Console.WriteLine($"[CM] [WARNING] orbits min Apoapsis has to be larger than max Apoapsis.");
                return false;
            }
            if (this.minPeriapsis > this.maxPeriapsis)
            {
                Console.WriteLine($"[CM] [WARNING] orbits min Periapsis has to be larger than max Periapsis.");
                return false;
            }
            if (this.minEccentricity > this.maxEccentricity)
            {
                Console.WriteLine($"[CM] [WARNING] orbits min eccentricity has to be larger than max eccentricity.");
                return false;
            }
            if (this.minPeriod > this.maxPeriod)
            {
                Console.WriteLine($"[CM] [WARNING] orbits min period has to be larger than max period.");
                return false;
            }
            if (this.minLongitudeOfAscendingNode > this.maxLongitudeOfAscendingNode)
            {
                Console.WriteLine($"[CM] [WARNING] orbits min longitude of ascending node has to be larger than max longitude of ascending node.");
                return false;
            }
            if (this.minInclination > this.maxInclination)
            {
                Console.WriteLine($"[CM] [WARNING] orbits min Inclination has to be larger than max Inclination.");
                return false;
            }
            if (this.minArgumentOfPeriapsis > this.maxArgumentOfPeriapsis)
            {
                Console.WriteLine($"[CM] [WARNING] orbits min argument of Periapsis has to be larger than max argument of Periapsis.");
                return false;
            }

            return true;
        }
    }

    public class RequiredGroup
    {
        // Completion condition of the requirement based on its child requirements.
        [XmlElement("completionCondition")]
        public CompletionCondition completionCondition { get; set; } = CompletionCondition.All;

        // List of child requirements for the contract.
        [XmlElement("requirements")]
        public List<Requirement> requirements { get; set; } = new List<Requirement>();

        public RequiredGroup() { }

        internal bool Validate()
        {
            bool isValidated = true;
            foreach (Requirement requirement in requirements) {
                isValidated &= requirement.Validate();
            }
            return isValidated;
        }
    }
}
