using ContractManager.ContractBlueprint;
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace ContractManager.Contract
{
    public class TrackedRequirement
    {
        // Internal handle to the blueprint requirement.
        internal Requirement _blueprintRequirement { get; set; } = null;
        
        // Serializable fields.
        // Unique identifier which blueprint requirement is being tracked.
        [XmlElement("requirementUID")]
        public string requirementUID { get; set; }

        // Status of the tracked requirement.
        [XmlElement("status")]
        public TrackedRequirementStatus status {  get; set; }

        // List of tracked child requirements
        [XmlArray("trackedRequirements")]
        public List<TrackedRequirement> trackedRequirements { get; set; } = new List<TrackedRequirement>();

        // Constructor, used when deserializing from XML.
        public TrackedRequirement() { }

        // Set the blueprintRequirement, used when deserialized from XML.
        public bool SetBlueprintRequirementFromUID(List<ContractBlueprint.Requirement> blueprintRequirements)
        {
            if (this.requirementUID == string.Empty) { return false; }  // requirementUID is not set, so can set blueprint requirement.
            bool setBlueprintRequirement = this._blueprintRequirement == null;
            if (setBlueprintRequirement) { return setBlueprintRequirement; }  // already set
            foreach (ContractBlueprint.Requirement blueprintRequirement in blueprintRequirements)
            {
                setBlueprintRequirement = blueprintRequirement.uid == this.requirementUID;
                if (setBlueprintRequirement)
                {
                    this._blueprintRequirement = blueprintRequirement;
                    //  Set childs
                    foreach (TrackedRequirement trackedRequirement in this.trackedRequirements)
                    {
                        setBlueprintRequirement = trackedRequirement.SetBlueprintRequirementFromUID(blueprintRequirement.requirements);
                    }
                    break;
                }
            }
            return setBlueprintRequirement;
        }

        // Constructor, used when contract is offered.
        public TrackedRequirement(in Requirement blueprintRequirement)
        {
            this._blueprintRequirement = blueprintRequirement;
            this.requirementUID = blueprintRequirement.uid;
            
            // Set the initial status of the tracked requirement.
            if (blueprintRequirement.completeInOrder)
            {
                this.status = TrackedRequirementStatus.NOT_STARTED;
            } else {
                this.status = TrackedRequirementStatus.TRACKED;
            }
            
            // FIXME: This should only be done if `blueprintRequirement.type == Group`
            //  Create tracked requirements as defined by the blueprint requirement.
            foreach (Requirement req in blueprintRequirement.requirements)
            {
                // construct a tracked requirement
                if (req.type == RequirementType.Orbit)
                {
                    this.trackedRequirements.Add(new TrackedOrbit(in req));
                }
            }
        }

        //  Update the state with vehicle data, overwrite as needed by childs.
        public virtual void UpdateStateWithVehicle(in KSA.Vehicle vehicle)
        {
            Console.WriteLine("[CM] TrackedRequirement.UpdateStateWithVehicle()");
        }
        
        // Update the tracked requirement based on the tracked state (updated through UpdateStateX functions), overwrite as needed by childs.
        public virtual void Update()
        {
            Console.WriteLine("[CM] TrackedRequirement.Update()");
        }

        // TODO: Add GUI function(s) here
    }

    public enum TrackedRequirementStatus
    {
        // Order status from worst to best status.
        [XmlEnum("FAILED")]
        FAILED = 0,  // Failed, final state
        [XmlEnum("NOT_STARTED")]
        NOT_STARTED = 1,  // Not started yet -> e.g. it can only start if previous requirement has been achieved.
        [XmlEnum("TRACKED")]
        TRACKED = 2,  // Tracked until maintained, achieved, or failed.
        [XmlEnum("MAINTAINED")]
        MAINTAINED = 3,  // Achieved, but tneed to hold the requirements until other requirements are achieved.
        [XmlEnum("ACHIEVED")]
        ACHIEVED = 4  // Achieved, final state
    }

    public class TrackedOrbit : TrackedRequirement
    {
        // The body the vehicle is curently orbiting
        public string orbitedBody { get; set; } = string.Empty;
        // The orbit Apoapsis
        public double apoapsis { get; set; } = double.NaN;
        // The orbit Periapsis
        public double periapsis { get; set; } = double.NaN;
        // TODO: add all the other ones
        
        public TrackedOrbit(in Requirement requirement) : base(in requirement) { }

        public override void UpdateStateWithVehicle(in KSA.Vehicle vehicle)
        {
            this.orbitedBody = vehicle.Orbit.Parent.Id;
            this.apoapsis = vehicle.Orbit.Apoapsis - vehicle.Orbit.Parent.MeanRadius;  // subtract mean radius to reflect Apoapsis shown in-game.
            this.periapsis = vehicle.Orbit.Periapsis - vehicle.Orbit.Parent.MeanRadius;  // subtract mean radius to reflect Apoapsis shown in-game.
            return;
        }
        
        public override void Update()
        {
            if (this.status is TrackedRequirementStatus.NOT_STARTED or TrackedRequirementStatus.ACHIEVED or TrackedRequirementStatus.FAILED) { return; }

            bool requirementAchieved = true;
            RequiredOrbit requiredOrbit = this._blueprintRequirement.orbit;
            if (requirementAchieved && requiredOrbit.targetBody != string.Empty && this.orbitedBody != string.Empty && this.orbitedBody != requiredOrbit.targetBody)
            {
                requirementAchieved = false;
            }
            Console.WriteLine($"[CM] required apoapsis: {requiredOrbit.minApoapsis} < {this.apoapsis} < {requiredOrbit.maxApoapsis}");
            if (requirementAchieved && !Double.IsNaN(requiredOrbit.minApoapsis) && requiredOrbit.minApoapsis > this.apoapsis)
            {
                requirementAchieved = false;
            }
            if (requirementAchieved && !Double.IsNaN(requiredOrbit.maxApoapsis) && requiredOrbit.maxApoapsis < this.apoapsis)
            {
                requirementAchieved = false;
            }
            Console.WriteLine($"[CM] required periapsis: {requiredOrbit.minPeriapsis} < {this.periapsis} < {requiredOrbit.maxPeriapsis}");
            if (requirementAchieved && !Double.IsNaN(requiredOrbit.minPeriapsis) && requiredOrbit.minPeriapsis > this.periapsis)
            {
                requirementAchieved = false;
            }
            if (requirementAchieved && !Double.IsNaN(requiredOrbit.maxPeriapsis) && requiredOrbit.maxPeriapsis < this.periapsis)
            {
                requirementAchieved = false;
            }
            if (requirementAchieved && this.status == TrackedRequirementStatus.TRACKED)
            {
                // Update status because the requirement is achieved.
                if (this._blueprintRequirement.isCompletedOnAchievement)
                {
                    Console.WriteLine($"[CM] requirement {this.requirementUID} -> ACHIEVED");
                    this.status = TrackedRequirementStatus.ACHIEVED;
                }
                else
                {
                    Console.WriteLine($"[CM] requirement {this.requirementUID} -> MAINTAINED");
                    this.status = TrackedRequirementStatus.MAINTAINED;
                }
            }
            else
            if (!requirementAchieved && this.status == TrackedRequirementStatus.MAINTAINED)
            {
                // requirement is not maintained anymore, set back to tracked.
                Console.WriteLine($"[CM] requirement {this.requirementUID} -> TRACKED");
                this.status = TrackedRequirementStatus.TRACKED;
            }
        }

        // TODO: Add GUI function(s) here
    }
}
