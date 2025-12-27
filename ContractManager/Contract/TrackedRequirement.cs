using ContractManager.ContractBlueprint;
using System;
using System.Collections.Generic;
using System.Text;

namespace ContractManager.Contract
{
    public class TrackedRequirement
    {
        protected Requirement _blueprintRequirement { get; }
        public TrackedRequirementStatus status {  get; set; }
        public List<TrackedRequirement> trackedRequirements { get; set; } = new List<TrackedRequirement>();

        public RequirementType type { get { return this._blueprintRequirement.type; } }

        public TrackedRequirement(in Requirement blueprintRequirement)
        {
            this._blueprintRequirement = blueprintRequirement;
            if (blueprintRequirement.completeInOrder)
            {
                this.status = TrackedRequirementStatus.NOT_STARTED;
            } else {
                this.status = TrackedRequirementStatus.TRACKED;
            }
            
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
        public virtual void UpdateStateWithVehicle(in KSA.Vehicle vehicle) { return; }

        // Update the tracked requirement, e.g. change the status
        public void Update()
        {
            Utils.UpdateTrackedRequirements(this.trackedRequirements);
        }

        // TODO: Add GUI function(s) here
    }

    public enum TrackedRequirementStatus
    {
        // Order worst -> best status
        FAILED = 0,  // Failed, final state
        NOT_STARTED = 1,  // Not started yet -> e.g. it can only start if previous requirement has been 
        TRACKED = 2,  // Tracked until maintained, achieved, or failed.
        MAINTAINED = 3,  // Achieved, but tneed to hold the requirements until other requirements are achieved.
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
            this.orbitedBody = vehicle.Orbit.Parent.Id;  // TODO: Verify!
            this.apoapsis = vehicle.Orbit.Apoapsis;
            this.periapsis = vehicle.Orbit.Periapsis;
            return;
        }
        
        public void Update()
        {
            if (this.status is TrackedRequirementStatus.NOT_STARTED or TrackedRequirementStatus.ACHIEVED or TrackedRequirementStatus.FAILED) { return; }

            bool requirementAchieved = true;
            RequiredOrbit requiredOrbit = this._blueprintRequirement.orbit;
            if (requirementAchieved && requiredOrbit.targetBody != string.Empty && this.orbitedBody != string.Empty && this.orbitedBody != requiredOrbit.targetBody)
            {
                requirementAchieved = false;
            }
            if (requirementAchieved && requiredOrbit.minApoapsis != double.NaN && this.apoapsis != double.NaN && requiredOrbit.minApoapsis > this.apoapsis)
            {
                requirementAchieved = false;
            }
            if (requirementAchieved && requiredOrbit.maxApoapsis <= double.NaN && this.apoapsis != double.NaN && requiredOrbit.maxApoapsis < this.apoapsis)
            {
                requirementAchieved = false;
            }
            if (requirementAchieved && requiredOrbit.minPeriapsis != double.NaN && this.periapsis != double.NaN && requiredOrbit.minPeriapsis > this.periapsis)
            {
                requirementAchieved = false;
            }
            if (requirementAchieved && requiredOrbit.maxPeriapsis <= double.NaN && this.periapsis != double.NaN && requiredOrbit.maxPeriapsis < this.periapsis)
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
            if (this.status == TrackedRequirementStatus.MAINTAINED)
            {
                // requiremnt not achieved anymore, and requirement needs to be maintained, set back to tracked.
                this.status = TrackedRequirementStatus.TRACKED;
            }
        }

        // TODO: Add GUI function(s) here
    }
}
