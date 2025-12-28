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
        [XmlElement("contractUID")]
        public string contractUID { get; set; } = string.Empty;

        // Unique identifier for which blueprint the contract was instantiated from.
        [XmlElement("blueprintUID")]
        public string blueprintUID { get; set; } = string.Empty;
        
        // Status of the contract.
        [XmlElement("status")]
        public ContractStatus status { get; set; }
        
        // In-game (player) time when contract was offered.
        [XmlElement("offeredTime")]
        public double offeredTimeS { get; set; }
        // In-game (player) time when contract was accepted.
        [XmlElement("acceptedTime")]
        public double acceptedTimeS { get; set; }
        // In-game (player) time when contract was finished, i.e. rejected, completed or failed .
        [XmlElement("finishedTime")]
        public double finishedTimeS { get; set; }

        // list of tracked requirements
        [XmlArray("trackedRequirements")]
        public List<TrackedRequirement> trackedRequirements { get; set; } = new List<TrackedRequirement>();

        // The vehicle(s) for which the contract is tracked with.
        // NOTE: for now only allow one vehicle to do a contract with. Later add option to blueprint to allow multi-vehicle contracts.
        [XmlArray("trackedVehicleNames")]
        public List<string> trackedVehicleNames { get; set; } = new List<string>();

        // Constructor for XML deserialization.
        public Contract() { }

        // Set blueprint from uid. Used when contract is deserialized from XML. (because the blueprint is not serialized)
        public bool SetBlueprintFromUID(List<ContractBlueprint.ContractBlueprint> contractBlueprints)
        {
            //  Verify the data loaded.
            if (this.blueprintUID == string.Empty) { return false; }  // blueprintUID is not set, so cannot set blueprint
            // Add verification of the status
            // Add verification of the offered (and other?) time

            bool setBlueprint = this._contractBlueprint == null;
            if (setBlueprint) { return setBlueprint; }  // already set
            foreach (ContractBlueprint.ContractBlueprint contractBlueprint in contractBlueprints)
            {
                setBlueprint = contractBlueprint.uid == this.blueprintUID;
                if (setBlueprint)
                {
                    this._contractBlueprint = contractBlueprint;
                    // Set the tracked requirements.
                    
                    foreach (TrackedRequirement trackedRequirement in this.trackedRequirements)
                    {
                        setBlueprint = trackedRequirement.SetBlueprintRequirementFromUID(contractBlueprint.requirements);
                    }
                    break;
                }
            }
            return setBlueprint;
        }

        // Constructor to instantiate a contract from a blueprint. Used when a contract is offered.
        public Contract(in ContractBlueprint.ContractBlueprint contractBlueprint, double playerTime)
        {
            Console.WriteLine($"[CM] Contract({contractBlueprint.uid}, {playerTime})");
            this.blueprintUID = contractBlueprint.uid;
            this._contractBlueprint = contractBlueprint;
            this.offeredTimeS = playerTime;
            this.status = ContractStatus.Offered;
            this.contractUID = String.Format("{0}_{1:N3}", contractBlueprint.uid, playerTime);

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
        public bool Update(double playerTime)
        {
            // TODO: Check if offered contract expired -> Rejected

            // Only check if status is Accepted, because that is the only situation the status can change through tracked requirements.
            if (this.status != ContractStatus.Accepted) { return false; }

            ContractStatus previousStatus = this.status;
            // TODO: Check if accepted contract expired -> Failed

            // Update the tracked requirements, e.g. change the status
            ContractUtils.UpdateTrackedRequirements(this.trackedRequirements);

            // TODO: Add vehicleName to the trackedVehicleNames when the first requirement is achieved

            // Update the contract
            TrackedRequirementStatus worstRequirementStatus = ContractUtils.GetWorstTrackedRequirementStatus(this.trackedRequirements);

            // Do actions
            if (worstRequirementStatus == TrackedRequirementStatus.FAILED)
            {
                Console.WriteLine($"[CM] Contract.Update() Failed");
                this.FailAcceptedContract(playerTime);
            }
            else
            if (worstRequirementStatus is TrackedRequirementStatus.MAINTAINED or TrackedRequirementStatus.ACHIEVED)
            {
                Console.WriteLine($"[CM] Contract.Update() Achieved");
                this.CompleteAcceptedContract(playerTime);
            }

            return previousStatus != this.status;  // return true if the status changed (to let the manager know)
        }

        //  Accept offered contract, to be called from GUI accept button.
        public void AcceptOfferedContract(double playerTime)
        {
            Console.WriteLine($"[CM] Contract.AcceptOfferedContract({playerTime})");
            if (this.status == ContractStatus.Offered)
            {
                this.status = ContractStatus.Accepted;
                this.acceptedTimeS = playerTime;
                // Utils.TriggerAction(this._contractBlueprint.actions, ContractBlueprint.Action.TriggerType.OnContractAccept);
            }
        }
        
        //  Reject offered contract, to be called from GUI accept button, or on expire.
        public void RejectContract(double playerTime)
        {
            Console.WriteLine($"[CM] Contract.RejectContract({playerTime})");
            if (this.status is ContractStatus.Offered or ContractStatus.Accepted)
            {
                this.status = ContractStatus.Rejected;
                this.finishedTimeS = playerTime;
                // Utils.TriggerAction(this._contractBlueprint.actions, ContractBlueprint.Action.TriggerType.OnContractReject);
            }
        }

        // Fail accepted contract, to be called on expire or requirement failing.
        private void FailAcceptedContract(double playerTime)
        {
            Console.WriteLine($"[CM] Contract.FailAcceptedContract({playerTime})");
            if (this.status == ContractStatus.Accepted)
            {
                this.status = ContractStatus.Failed;
                this.finishedTimeS = playerTime;
                ContractUtils.TriggerAction(this._contractBlueprint.actions, ContractBlueprint.Action.TriggerType.OnContractFail);
            }
        }

        // Complete accepted contract, to be called when all requirements are achieved.
        private void CompleteAcceptedContract(double playerTime)
        {
            Console.WriteLine($"[CM] Contract.CompleteAcceptedContract({playerTime})");
            if (this.status == ContractStatus.Accepted)
            {
                this.status = ContractStatus.Completed;
                this.finishedTimeS = playerTime;
                ContractUtils.TriggerAction(this._contractBlueprint.actions, ContractBlueprint.Action.TriggerType.OnContractComplete);
            }
        }
        
        // TODO: Add GUI function(s) here
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
