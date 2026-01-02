using ContractManager.ContractBlueprint;
using KSA;
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace ContractManager.Contract
{
    public class Contract
    {
        // Class for containing and handling contracts that are instantiated from a ContractBlueprint.
        
        // Internal handle to the blueprint the contract was instantiated from.
        internal ContractBlueprint.ContractBlueprint _contractBlueprint { get; set;} = null;

        // Serializable fields.
        // Unique identifier for the contract.
        [XmlElement("contractUID", DataType = "string")]
        public string contractUID { get; set; } = string.Empty;

        // Unique identifier for which blueprint the contract was instantiated from.
        [XmlElement("blueprintUID", DataType = "string")]
        public string blueprintUID { get; set; } = string.Empty;
        
        // Status of the contract.
        [XmlElement("status")]
        public ContractStatus status { get; set; }
        
        // In-game (sim) time when contract was offered in seconds.
        [XmlElement("offeredTime", DataType = "double")]
        public double offeredTimeS { get { return offeredSimTime.Seconds(); } set; } = double.NaN;
        // In-game (sim) time when contract was accepted in seconds.
        [XmlElement("acceptedTime", DataType = "double")]
        public double acceptedTimeS { get { return acceptedSimTime.Seconds(); } set; } = double.NaN;
        // In-game (sim) time when contract was finished in seconds, i.e. rejected, completed or failed.
        [XmlElement("finishedTime", DataType = "double")]
        public double finishedTimeS { get { return finishedSimTime.Seconds(); } set; } = double.NaN;
        
        internal KSA.SimTime offeredSimTime { get; set; } = new KSA.SimTime(double.NaN);
        internal KSA.SimTime acceptedSimTime { get; set; } = new KSA.SimTime(double.NaN);
        internal KSA.SimTime finishedSimTime { get; set; } = new KSA.SimTime(double.NaN);

        // list of tracked requirements
        [XmlArray("trackedRequirements")]
        public List<TrackedRequirement> trackedRequirements { get; set; } = new List<TrackedRequirement>();

        // The vehicle(s) for which the contract is tracked with.
        // NOTE: for now only allow one vehicle to do a contract with. Later add option to blueprint to allow multi-vehicle contracts.
        [XmlArray("trackedVehicleNames")]
        public List<string> trackedVehicleNames { get; set; } = new List<string>();

        // Constructor for XML deserialization.
        public Contract() { }

        // Clone, e.g after deserializing from a stream.
        internal Contract? Clone(List<ContractBlueprint.ContractBlueprint> contractBlueprints)
        {
            Contract clonedContract = new Contract
            {
                contractUID = this.contractUID,
                blueprintUID = this.blueprintUID,
                status = this.status,
                offeredTimeS = this.offeredTimeS,
                acceptedTimeS = this.acceptedTimeS,
                finishedTimeS = this.finishedTimeS,
                offeredSimTime = new KSA.SimTime(this.offeredTimeS),
                acceptedSimTime = new KSA.SimTime(this.acceptedTimeS),
                finishedSimTime = new KSA.SimTime(this.finishedTimeS)
            };
            foreach (string trackedVehicleName in trackedVehicleNames)
            {
                clonedContract.trackedVehicleNames.Add(trackedVehicleName);
            }
            clonedContract._contractBlueprint = ContractUtils.FindContractBlueprintFromUID(contractBlueprints, this.blueprintUID);
            if (clonedContract._contractBlueprint != null)
            {
                foreach (TrackedRequirement trackedRequirement in this.trackedRequirements)
                {
                    TrackedRequirement? clonedTrackedRequirement = trackedRequirement.Clone(clonedContract._contractBlueprint.requirements);
                    if (clonedTrackedRequirement != null) {
                        clonedContract.trackedRequirements.Add(clonedTrackedRequirement);
                    }
                    else
                    {
                        Console.WriteLine($"[CM] [ERROR] Contract could not clone trackedRequirement '{trackedRequirement.requirementUID}'");
                        return null;
                    }
                }
            }
            else
            {
                Console.WriteLine($"[CM] [ERROR] Contract could not find ContractBlueprint matching uid '{this.blueprintUID}'");
                return null;
            }
            return clonedContract;
        }

        // Constructor to instantiate a contract from a blueprint. Used when a contract is offered.
        public Contract(in ContractBlueprint.ContractBlueprint contractBlueprint, KSA.SimTime simTime)
        {
            this.blueprintUID = contractBlueprint.uid;
            this._contractBlueprint = contractBlueprint;
            this.offeredSimTime = simTime;
            this.status = ContractStatus.Offered;
            this.contractUID = String.Format("{0}_{1:F0}", contractBlueprint.uid, this.offeredSimTime.Seconds());

            foreach (Requirement blueprintRequirement in contractBlueprint.requirements)
            {
                // Construct a tracked requirement from blueprint.
                this.trackedRequirements.Add(TrackedRequirement.CreateFromBlueprintRequirement(blueprintRequirement));
            }
        }

        //  Update the tracked state of the requirements in the contract with the data from the vehicle. To be called in game-loop.
        public void UpdateStateWithVehicle(in KSA.Vehicle vehicle) {
            // Only update if contract accepted (shouldn't be called anyway, but to be sure).
            if (this.status != ContractStatus.Accepted) { return; }

            // Only update if this tracked contract is tracking this vehicle.
            if ( this.trackedVehicleNames.Count > 0 && !this.trackedVehicleNames.Contains(vehicle.Id)) { return; }

            ContractUtils.UpdateStateWithVehicle(in vehicle, this.trackedRequirements);

            return;
        }

        // Update the contract. To be called in game-loop.
        public bool Update(KSA.SimTime simTime)
        {
            // Check if offered contract expired -> Rejected
            if (!Double.IsPositiveInfinity(this._contractBlueprint.expiration))
            {
                KSA.SimTime expireOnSimTime = this.offeredSimTime + this._contractBlueprint.expiration;
                if (expireOnSimTime < simTime)
                {
                    this.ExpireOfferedContract(simTime);
                    return true;
                }
            }

            // Only check if status is Accepted, because that is the only situation the status can change through tracked requirements.
            if (this.status != ContractStatus.Accepted) { return false; }

            ContractStatus previousStatus = this.status;
            // Check if accepted contract expired -> Failed
            if (!Double.IsPositiveInfinity(this._contractBlueprint.deadline))
            {
                KSA.SimTime failOnSimTime = this.acceptedSimTime + this._contractBlueprint.deadline;
                if (failOnSimTime < simTime)
                {
                    this.FailAcceptedContract(simTime);
                    return true;
                }
            }

            // Update the tracked requirements, e.g. change the status
            ContractUtils.UpdateTrackedRequirements(this.trackedRequirements);

            // TODO: Add vehicleName to the trackedVehicleNames when the first requirement is achieved

            // Update the contract
            TrackedRequirementStatus worstRequirementStatus = ContractUtils.GetWorstTrackedRequirementStatus(this.trackedRequirements);

            // Do actions
            if (worstRequirementStatus == TrackedRequirementStatus.FAILED)
            {
                Console.WriteLine($"[CM] Contract.Update() Failed");
                this.FailAcceptedContract(simTime);
            }
            else
            if (worstRequirementStatus is TrackedRequirementStatus.MAINTAINED or TrackedRequirementStatus.ACHIEVED)
            {
                Console.WriteLine($"[CM] Contract.Update() Achieved");
                this.CompleteAcceptedContract(simTime);
            }

            return previousStatus != this.status;  // return true if the status changed (to let the manager know)
        }

        //  Accept offered contract, to be called from GUI accept button.
        public void AcceptOfferedContract(KSA.SimTime simTime)
        {
            if (this.status == ContractStatus.Offered)
            {
                this.status = ContractStatus.Accepted;
                this.acceptedSimTime = simTime;
                // Utils.TriggerAction(this, ContractBlueprint.Action.TriggerType.OnContractAccept);
            }
        }
        
        //  Reject contract, to be called from GUI reject button.
        public void RejectContract(KSA.SimTime simTime)
        {
            if (this.status is ContractStatus.Offered or ContractStatus.Accepted)
            {
                this.status = ContractStatus.Rejected;
                this.finishedSimTime = simTime;
                // Utils.TriggerAction(this, ContractBlueprint.Action.TriggerType.OnContractReject);
            }
        }
        
        //  Expire offered contract, to be called on expire.
        public void ExpireOfferedContract(KSA.SimTime simTime)
        {
            if (this.status is ContractStatus.Offered)
            {
                this.status = ContractStatus.Rejected;
                this.finishedSimTime = simTime;
                // Utils.TriggerAction(this, ContractBlueprint.Action.TriggerType.OnContractExpire);
            }
        }

        // Fail accepted contract, to be called on expire or requirement failing.
        private void FailAcceptedContract(KSA.SimTime simTime)
        {
            if (this.status == ContractStatus.Accepted)
            {
                this.status = ContractStatus.Failed;
                this.finishedSimTime = simTime;
                ContractUtils.TriggerAction(this, ContractBlueprint.TriggerType.OnContractFail);
            }
        }

        // Complete accepted contract, to be called when all requirements are achieved.
        private void CompleteAcceptedContract(KSA.SimTime simTime)
        {
            if (this.status == ContractStatus.Accepted)
            {
                this.status = ContractStatus.Completed;
                this.finishedSimTime = simTime;
                ContractUtils.TriggerAction(this, ContractBlueprint.TriggerType.OnContractComplete);
            }
        }
    }
    
    public enum ContractStatus
    {
        [XmlEnum("Offered")]
        Offered,
        [XmlEnum("Rejected")]
        Rejected,
        [XmlEnum("Accepted")]
        Accepted,
        [XmlEnum("Completed")]
        Completed,
        [XmlEnum("Failed")]
        Failed
    }
}
