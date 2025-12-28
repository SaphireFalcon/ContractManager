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

        // Constructor, used when deserializing from XML.
        public TrackedRequirement() { }

        // Factory function to create a TrackedRequirement (sub-class types) from a blueprint requirement.
        public static TrackedRequirement CreateFromBlueprintRequirement(Requirement blueprintRequirement)
        {
            // Construct a tracked requirement from blueprint.
            if (blueprintRequirement.type == RequirementType.Orbit)
            {
                return new TrackedOrbit(in blueprintRequirement);
            }
            else
            if (blueprintRequirement.type == RequirementType.Group)
            {
                return new TrackedGroup(blueprintRequirement);
            }
            return new TrackedRequirement(blueprintRequirement); 
        }

        // Set the blueprintRequirement, used when deserialized from XML.
        public virtual bool SetBlueprintRequirementFromUID(List<ContractBlueprint.Requirement> blueprintRequirements)
        {
            if (this.requirementUID == string.Empty) { return false; }  // requirementUID is not set, so cannot find matching blueprint requirement.
            bool setBlueprintRequirement = this._blueprintRequirement == null;
            if (setBlueprintRequirement) { return setBlueprintRequirement; }  // already set
            foreach (ContractBlueprint.Requirement blueprintRequirement in blueprintRequirements)
            {
                setBlueprintRequirement = blueprintRequirement.uid == this.requirementUID;
                if (setBlueprintRequirement)
                {
                    // Found matching blueprint requirement.
                    this._blueprintRequirement = blueprintRequirement;
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
        }

        //  Update the state with vehicle data, overwrite as needed by childs.
        public virtual void UpdateStateWithVehicle(in KSA.Vehicle vehicle) { }
        
        // Update the tracked requirement based on the tracked state (updated through UpdateStateX functions), overwrite as needed by childs.
        public virtual void Update() { }
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
            RequiredOrbit? requiredOrbit = this._blueprintRequirement.orbit;
            if (requiredOrbit == null) { return; }
            if (requirementAchieved && requiredOrbit.targetBody != string.Empty && this.orbitedBody != string.Empty && this.orbitedBody != requiredOrbit.targetBody)
            {
                requirementAchieved = false;
            }

            if (requirementAchieved && !Double.IsNaN(requiredOrbit.minApoapsis) && requiredOrbit.minApoapsis > this.apoapsis)
            {
                requirementAchieved = false;
            }
            if (requirementAchieved && !Double.IsNaN(requiredOrbit.maxApoapsis) && requiredOrbit.maxApoapsis < this.apoapsis)
            {
                requirementAchieved = false;
            }

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
                    this.status = TrackedRequirementStatus.ACHIEVED;
                }
                else
                {
                    this.status = TrackedRequirementStatus.MAINTAINED;
                }
            }
            else
            if (!requirementAchieved && this.status == TrackedRequirementStatus.MAINTAINED)
            {
                // requirement is not maintained anymore, set back to tracked.
                this.status = TrackedRequirementStatus.TRACKED;
            }
        }
    }

    public class TrackedGroup : TrackedRequirement
    {
        // List of tracked child requirements
        [XmlArray("trackedRequirements")]
        public List<TrackedRequirement> trackedRequirements { get; set; } = new List<TrackedRequirement>();

        public TrackedGroup(in Requirement requirement) : base(in requirement)
        {
            if (requirement.group == null) { return; }
            foreach (Requirement blueprintRequirement in requirement.group.requirements)
            {
                this.trackedRequirements.Add(TrackedRequirement.CreateFromBlueprintRequirement(blueprintRequirement));
            }
        }

        // Set the blueprintRequirement, used when deserialized from XML.
        public override bool SetBlueprintRequirementFromUID(List<ContractBlueprint.Requirement> blueprintRequirements)
        {
            bool setBlueprintRequirement = base.SetBlueprintRequirementFromUID(blueprintRequirements);
            //  Set childs
            if (this._blueprintRequirement.group != null)
            {
                foreach (TrackedRequirement trackedRequirement in this.trackedRequirements)
                {
                    setBlueprintRequirement &= trackedRequirement.SetBlueprintRequirementFromUID(this._blueprintRequirement.group.requirements);
                }
            }
            return setBlueprintRequirement;
        }

        public override void UpdateStateWithVehicle(in KSA.Vehicle vehicle)
        {
            foreach (TrackedRequirement trackedRequirement in this.trackedRequirements)
            {
                trackedRequirement.UpdateStateWithVehicle(vehicle);
            }
        }
        
        public override void Update()
        {
            foreach (TrackedRequirement trackedRequirement in this.trackedRequirements)
            {
                trackedRequirement.Update();
            }
            
            // Check childs status and update for
            TrackedRequirementStatus worstRequirementStatus = ContractUtils.GetWorstTrackedRequirementStatus(this.trackedRequirements);
            
            // TODO: for these two fixme, need GetBestTrackedRequirementStatus
            // FIXME: If completion condition is any, but all childs has status FAILED, than also fail...
            if (worstRequirementStatus == TrackedRequirementStatus.FAILED && this._blueprintRequirement.group.completionCondition == CompletionCondition.All)
            {
                this.status = TrackedRequirementStatus.FAILED;
            }
            else
            // FIXME: If completion condition is any, than only one achieved should be sufficient
            if (worstRequirementStatus is TrackedRequirementStatus.MAINTAINED or TrackedRequirementStatus.ACHIEVED)
            {
                this.status = TrackedRequirementStatus.ACHIEVED;
            }
        }
    }
}
