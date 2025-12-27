using ContractManager.ContractBlueprint;
using KSA;
using System;
using System.Collections.Generic;
using System.Text;

namespace ContractManager.Contract
{

    public class Contract
    {
        // Class for containing and handling contracts that are instantiated from a ContractBlueprint.
        public string blueprintUID { get; }

        private ContractBlueprint.ContractBlueprint _contractBlueprint { get; }

        public ContractStatus status { get; private set; }
        
        public double offeredTimeS { get; }
        public double acceptedTimeS { get; set; }

        public List<TrackedRequirement> trackedRequirements { get; set; } = new List<TrackedRequirement>();

        // The vehicle(s) for which the contract is tracked with.
        // NOTE: for now only allow one vehicle to do a contract with. Later add option to blueprint to allow multi-vehicle contracts.
        public List<string> trackedVehicleName { get; set; }

        public Contract(
            in ContractBlueprint.ContractBlueprint contractBlueprint,
            double playerTime
        )
        {
            this.blueprintUID = contractBlueprint.uid;
            this._contractBlueprint = contractBlueprint;
            this.offeredTimeS = playerTime;
            this.status = ContractStatus.Offered;

            foreach (Requirement blueprintRequirement in contractBlueprint.requirements)
            {
                // TODO: add action to be passed to constructor if `onRequirementX` is implemented
                // Construct a tracked requirement from blueprint.
                if (blueprintRequirement.type == RequirementType.Orbit)
                {
                    this.trackedRequirements.Add(new TrackedOrbit(in blueprintRequirement));
                }
            }
        }

        public void UpdateStateWithVehicle(in KSA.Vehicle vehicle) {
            // Only update if this tracked contract is tracking this vehicle.
            if ( this.trackedVehicleName.Count > 0 && !this.trackedVehicleName.Contains(vehicle.Id)) { return; }

            Utils.UpdateStateWithVehicle(in vehicle, this.trackedRequirements);
            return;
        }

        public void AcceptOfferedContract()
        {
            if (this.status == ContractStatus.Offered)
            {
                this.status = ContractStatus.Accepted;
                // Utils.TriggerAction(this._contractBlueprint.actions, ContractBlueprint.Action.TriggerType.OnContractAccept);
            }
        }

        public void RejectContract()
        {
            if (this.status is ContractStatus.Offered or ContractStatus.Accepted)
            {
                this.status = ContractStatus.Rejected;
                // Utils.TriggerAction(this._contractBlueprint.actions, ContractBlueprint.Action.TriggerType.OnContractReject);
            }
        }

        private void FailAcceptedContract()
        {
            if (this.status == ContractStatus.Accepted)
            {
                this.status = ContractStatus.Failed;
                Utils.TriggerAction(this._contractBlueprint.actions, ContractBlueprint.Action.TriggerType.OnContractFail);
            }
        }

        private void CompleteAcceptedContract()
        {
            if (this.status == ContractStatus.Accepted)
            {
                this.status = ContractStatus.Completed;
                Utils.TriggerAction(this._contractBlueprint.actions, ContractBlueprint.Action.TriggerType.OnContractComplete);
            }
        }

        public bool Update()
        {
            // TODO: Check if offered contract expired -> Rejected

            // Only check if status is Accepted, because that is the only situation the status can change through tracked requirements.
            if (this.status != ContractStatus.Accepted) { return false; }

            ContractStatus previousStatus = this.status;
            // TODO: Check if accepted contract expired -> Failed

            // Update the tracked requirements, e.g. change the status
            Utils.UpdateTrackedRequirements(this.trackedRequirements);

            // Update the contract
            TrackedRequirementStatus worstRequirementStatus = Utils.CheckTrackedRequirementsStatus(this.trackedRequirements);

            // Do actions
            if (worstRequirementStatus == TrackedRequirementStatus.FAILED)
            {
                this.FailAcceptedContract();
            }
            else
            if (worstRequirementStatus == TrackedRequirementStatus.ACHIEVED)
            {
                this.CompleteAcceptedContract();
            }

            return previousStatus != this.status;  // return true if the status changed (to let the manager know)
        }
        
        // TODO: Add GUI function(s) here
    }
    
    public enum ContractStatus
    {
        Offered,
        Rejected,
        Accepted,
        Completed,
        Failed
    }
}
