using ContractManager.ContractBlueprint;
using KSA;
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace ContractManager.Contract
{
    [XmlInclude(typeof(TrackedGroup)), XmlInclude(typeof(TrackedOrbit))]
    public class TrackedRequirement
    {
        // Internal handle to the blueprint requirement.
        internal Requirement? _blueprintRequirement { get; set; } = null;
        // Internal handle to the contract holding the requirement.
        internal Contract? _contract { get; set; } = null;
        
        // Serializable fields.
        // Unique identifier which blueprint requirement is being tracked.
        [XmlElement("requirementUID", DataType = "string")]
        public string requirementUID { get; set; } = string.Empty;

        // Status of the tracked requirement.
        [XmlElement("status")]
        public TrackedRequirementStatus status {  get; set; } = TrackedRequirementStatus.NOT_STARTED;

        // Constructor, used when deserializing from XML.
        public TrackedRequirement() { }

        // Clone, e.g after deserializing from a stream.
        internal TrackedRequirement? Clone(List<ContractBlueprint.Requirement> blueprintRequirements, in Contract contract)
        {
            ContractBlueprint.Requirement? blueprintRequirement = ContractUtils.FindRequirementFromUID(blueprintRequirements, this.requirementUID);
            if (blueprintRequirement == null)
            {
                Console.WriteLine($"[CM] [ERROR] TrackedRequirement could not find blueprint requirement matching uid '{this.requirementUID}'");
                return null;
            }

            if (blueprintRequirement.type == RequirementType.Orbit)
            {
                return ((TrackedOrbit)this).Clone(blueprintRequirement, contract);
            }
            else
            if (blueprintRequirement.type == RequirementType.Group)
            {
                return ((TrackedGroup)this).Clone(blueprintRequirement, contract);
            }
            Console.WriteLine($"[CM] [ERROR] Unhandled type: '{blueprintRequirement.type}'");
            return null;
        }

        // Factory function to create a TrackedRequirement (sub-class types) from a blueprint requirement.
        public static TrackedRequirement CreateFromBlueprintRequirement(Requirement blueprintRequirement, in Contract contract)
        {
            // Construct a tracked requirement from blueprint.
            if (blueprintRequirement.type == RequirementType.Orbit)
            {
                return new TrackedOrbit(in blueprintRequirement, in contract);
            }
            else
            if (blueprintRequirement.type == RequirementType.Group)
            {
                return new TrackedGroup(blueprintRequirement, in contract);
            }
            return new TrackedRequirement(blueprintRequirement, in contract); 
        }

        // Constructor, used when contract is offered.
        public TrackedRequirement(in Requirement blueprintRequirement, in Contract contract)
        {
            this._blueprintRequirement = blueprintRequirement;
            this._contract = contract;
            this.requirementUID = blueprintRequirement.uid;
            
            // Set the initial status of the tracked requirement.
            if (blueprintRequirement.completeInOrder)
            {
                this.status = TrackedRequirementStatus.NOT_STARTED;
            } else {
                this.status = TrackedRequirementStatus.TRACKED;
                ContractUtils.TriggerAction(contract, ContractBlueprint.TriggerType.OnRequirementTracked, this);
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
        MAINTAINED = 3,  // Achieved, but need to hold the requirements until other requirements are achieved.
        [XmlEnum("ACHIEVED")]
        ACHIEVED = 4  // Achieved, final state
    }

    public class TrackedOrbit : TrackedRequirement
    {
        // The body the vehicle is curently orbiting
        public string orbitedBody { get; set; } = string.Empty;
        // The orbit Apoapsis in meters
        public double apoapsis { get; set; } = double.NaN;
        // The orbit Periapsis in meters 
        public double periapsis { get; set; } = double.NaN;
        // The eccentricity of the orbit (ratio)
        public double eccentricity { get; set; } = double.NaN;
        // The period of the orbit in seconds
        public double period { get; set; } = double.NaN;
        // The longitude of the ascending node; angle in degrees from reference frame of the parent body to the ascending node in reference plane.
        public double longitudeOfAscendingNode { get; set; } = double.NaN;
        // The inclination of the orbit; the angle in degrees between the reference plane of the parent body and the orbital plane.
        public double inclination { get; set; } = double.NaN;
        // The argument of Periapsis; the angle in degrees between the ascending node and the periapsis in orbital plane.
        public double argumentOfPeriapsis { get; set; } = double.NaN;
        // The type of orbit.
        public OrbitType type { get; set; } = OrbitType.Invalid;
        
        // Note: SemiMajorAxis and SemiMinorAxis can be derived from the other parameters.
        //   Currently, the ease of use for the player seems limited, so not adding for now.
        
        // Constructor, used when deserializing from XML.
        public TrackedOrbit() { }

        internal TrackedOrbit? Clone(in ContractBlueprint.Requirement blueprintRequirement, in Contract contract)
        {
            TrackedOrbit clonedTrackedOrbut = new TrackedOrbit
            {
                requirementUID = this.requirementUID,
                status = this.status,
                _blueprintRequirement = blueprintRequirement,
                _contract = contract
            };
            return clonedTrackedOrbut;
        }

        public TrackedOrbit(in Requirement requirement, in Contract contract) : base(in requirement, contract) { }

        public override void UpdateStateWithVehicle(in KSA.Vehicle vehicle)
        {
            KSA.Orbit vehicleOrbit = vehicle.Orbit;
            this.orbitedBody = vehicle.Orbit.Parent.Id;
            this.apoapsis = vehicle.Orbit.Apoapsis - vehicle.Orbit.Parent.MeanRadius;  // subtract mean radius to reflect Apoapsis shown in-game.
            this.periapsis = vehicle.Orbit.Periapsis - vehicle.Orbit.Parent.MeanRadius;  // subtract mean radius to reflect Apoapsis shown in-game.
            this.eccentricity = vehicleOrbit.Eccentricity;
            this.period = vehicleOrbit.Period;
            this.longitudeOfAscendingNode = Double.RadiansToDegrees(vehicleOrbit.LongitudeOfAscendingNode);
            this.inclination = Double.RadiansToDegrees(vehicleOrbit.Inclination);
            this.argumentOfPeriapsis = Double.RadiansToDegrees(vehicleOrbit.ArgumentOfPeriapsis);
            if (vehicleOrbit.GetOrbitType() == KSA.Orbit.OrbitType.Invalid)
            {
                this.type = OrbitType.Invalid;
            }
            else
            if (vehicleOrbit.GetOrbitType() == KSA.Orbit.OrbitType.Elliptical)
            {
                if (vehicleOrbit.Periapsis < vehicle.Orbit.Parent.MeanRadius)
                {
                    this.type = OrbitType.Suborbit;
                }
                else
                {
                    this.type = OrbitType.Elliptical;
                }
            }
            if (
                (vehicleOrbit.GetOrbitType() is KSA.Orbit.OrbitType.Hyperbolic or KSA.Orbit.OrbitType.Parabolic) &&
                vehicle.FlightPlan.Patches.Count > 0 && vehicle.FlightPlan.Patches[0].EndTransition == PatchTransition.Escape  // not sure if this condition is needed.
            )
            {
                this.type = OrbitType.Escape;
            }

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
                    ContractUtils.TriggerAction(this._contract, ContractBlueprint.TriggerType.OnRequirementAchieved, this);
                }
                else
                {
                    this.status = TrackedRequirementStatus.MAINTAINED;
                    ContractUtils.TriggerAction(this._contract, ContractBlueprint.TriggerType.OnRequirementMaintained, this);
                }
            }
            else
            if (!requirementAchieved && this.status == TrackedRequirementStatus.MAINTAINED)
            {
                // requirement is not maintained anymore, set back to tracked.
                this.status = TrackedRequirementStatus.TRACKED;
                ContractUtils.TriggerAction(this._contract, ContractBlueprint.TriggerType.OnRequirementReverted, this);
            }
        }
    }

    public class TrackedGroup : TrackedRequirement
    {
        // List of tracked child requirements
        [XmlArray("trackedRequirements")]
        public List<TrackedRequirement> trackedRequirements { get; set; } = new List<TrackedRequirement>();

        public TrackedGroup() { }
        
        internal TrackedGroup? Clone(in ContractBlueprint.Requirement blueprintRequirement, in Contract contract)
        {
            TrackedGroup clonedTrackedGroup = new TrackedGroup
            {
                requirementUID = this.requirementUID,
                status = this.status,
                _blueprintRequirement = blueprintRequirement
            };
            foreach (TrackedRequirement childTrackedRequirement in this.trackedRequirements)
            {
                TrackedRequirement? clonedTrackedRequirement = childTrackedRequirement.Clone(blueprintRequirement.group.requirements, contract);
                if (clonedTrackedRequirement != null) {
                    clonedTrackedGroup.trackedRequirements.Add(clonedTrackedRequirement);
                }
                else
                {
                    Console.WriteLine($"[CM] [ERROR] TrackedGroup could not clone trackedRequirement '{childTrackedRequirement.requirementUID}'");
                    return null;
                }
            }
            return clonedTrackedGroup;
        }

        public TrackedGroup(in Requirement requirement, in Contract contract) : base(in requirement, contract)
        {
            if (requirement.group == null) { return; }
            foreach (Requirement blueprintRequirement in requirement.group.requirements)
            {
                this.trackedRequirements.Add(TrackedRequirement.CreateFromBlueprintRequirement(blueprintRequirement, in contract));
            }
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
                ContractUtils.TriggerAction(this._contract, ContractBlueprint.TriggerType.OnRequirementFailed, this);
            }
            else
            // FIXME: If completion condition is any, than only one achieved should be sufficient
            if (worstRequirementStatus is TrackedRequirementStatus.MAINTAINED or TrackedRequirementStatus.ACHIEVED)
            {
                this.status = TrackedRequirementStatus.ACHIEVED;
                ContractUtils.TriggerAction(this._contract, ContractBlueprint.TriggerType.OnRequirementAchieved, this);
            }
        }
    }
}
