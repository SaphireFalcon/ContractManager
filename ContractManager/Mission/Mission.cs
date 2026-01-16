using ContractManager.Contract;
using ContractManager.ContractBlueprint;
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace ContractManager.Mission
{
    public class Mission
    {
        internal MissionBlueprint _missionBlueprint { get; set;} = null;

        // Serializable fields.
        // Unique identifier for the mission.
        // [RENAMED v0.2.4]
        //[XmlElement("missionUID", DataType = "string")]
        //public string missionUID { get; set; } = string.Empty;
        [XmlElement("uid", DataType = "string")]
        public string uid { get; set; } = string.Empty;

        // Unique identifier for which blueprint the mission was instantiated from.
        [XmlElement("blueprintUID", DataType = "string")]
        public string blueprintUID { get; set; } = string.Empty;

        // Status of the contract.
        [XmlElement("status")]
        public MissionStatus status { get; set; }
        
        // In-game (sim) time when mission was offered in seconds.
        [XmlElement("offeredTime", DataType = "double")]
        public double offeredTimeS { get { return offeredSimTime.Seconds(); } set; } = double.NaN;

        // In-game (sim) time when mission was accepted in seconds.
        [XmlElement("acceptedTime", DataType = "double")]
        public double acceptedTimeS { get { return acceptedSimTime.Seconds(); } set; } = double.NaN;

        // In-game (sim) time when mission was finished in seconds, i.e. rejected, completed or failed.
        [XmlElement("finishedTime", DataType = "double")]
        public double finishedTimeS { get { return finishedSimTime.Seconds(); } set; } = double.NaN;

        // list of contractUID referencing the contracts created as part of this mission.
        [XmlArray("contractUIDs")]
        public List<string> contractUIDs { get; set; } = new List<string>();

        // internal variables
        internal KSA.SimTime offeredSimTime { get; set; } = new KSA.SimTime(double.NaN);
        internal KSA.SimTime acceptedSimTime { get; set; } = new KSA.SimTime(double.NaN);
        internal KSA.SimTime finishedSimTime { get; set; } = new KSA.SimTime(double.NaN);

        public Mission() { }

        // Clone, e.g after deserializing from a stream.
        internal Mission? Clone(List<MissionBlueprint> missionBlueprints)
        {
            var clonedMission = new Mission
            {
                uid = this.uid,
                blueprintUID = this.blueprintUID,
                status = this.status,
                offeredTimeS = this.offeredTimeS,
                acceptedTimeS = this.acceptedTimeS,
                finishedTimeS = this.finishedTimeS,
                offeredSimTime = new KSA.SimTime(this.offeredTimeS),
                acceptedSimTime = new KSA.SimTime(this.acceptedTimeS),
                finishedSimTime = new KSA.SimTime(this.finishedTimeS),
            };
            foreach (string contractUID in this.contractUIDs)
            {
                clonedMission.contractUIDs.Add(contractUID);
            }
            clonedMission._missionBlueprint = MissionUtils.FindMissionBlueprintFromUID(missionBlueprints, this.blueprintUID);
            if (clonedMission._missionBlueprint == null)
            {
                Console.WriteLine($"[CM] [ERROR] Mission '{this.uid}' could not find MissionBlueprint matching uid '{this.blueprintUID}'");
                return null;
            }

            return clonedMission;
        }

        // Constructor to instantiate a mission from a blueprint. Used when a misson is offered.
        public Mission(in MissionBlueprint missionBlueprint, KSA.SimTime simTime)
        {
            this.blueprintUID = missionBlueprint.uid;
            this._missionBlueprint = missionBlueprint;
            this.offeredSimTime = simTime;
            this.status = MissionStatus.Offered;
            this.uid = String.Format("{0}_{1:F0}", missionBlueprint.uid, this.offeredSimTime.Seconds());

            // Should we create contracts here? We should just allow the "normal" offer procedure do that.
        }
        
        // Update the mission. To be called in game-loop.
        public bool Update(KSA.SimTime simTime)
        {
            // Check if offered mission expired -> Rejected
            if (this.status == MissionStatus.Offered && !Double.IsPositiveInfinity(this._missionBlueprint.expiration))
            {
                KSA.SimTime expireOnSimTime = this.offeredSimTime + this._missionBlueprint.expiration;
                if (expireOnSimTime < simTime)
                {
                    this.ExpireOfferedMission(simTime);
                    return true;
                }
            }

            // Return early if status is not accepted.
            if (this.status != MissionStatus.Accepted) { return false; }

            MissionStatus previousStatus = this.status;
            // Check if accepted mission expired -> Failed
            if (!Double.IsPositiveInfinity(this._missionBlueprint.deadline))
            {
                KSA.SimTime failOnSimTime = this.acceptedSimTime + this._missionBlueprint.deadline;
                if (failOnSimTime < simTime)
                {
                    this.FailAcceptedMission(simTime);
                    return true;
                }
            }

            // Check if the mission can be completed
            // Use >= because a contract can fail and re-offered.
            bool allContractsFinished = this.contractUIDs.Count >= this._missionBlueprint.contractBlueprintUIDs.Count;
            if (!allContractsFinished) { return false; }
            // Check if all needed contracts are done
            foreach (string contractUID in this.contractUIDs)
            {
                Contract.Contract? contract = Contract.ContractUtils.FindContractFromContractUID(
                    ContractManager.data.finishedContracts,
                    contractUID
                );
                if (!(contract != null && contract.status == ContractStatus.Completed))
                {
                    allContractsFinished = false;
                    Console.WriteLine($"[CM] Mission.Update() contract '{contract._contractBlueprint.title}' not completed!");
                    break;
                }
            }
            if (!allContractsFinished) { return false; }

            // Do actions
            Console.WriteLine($"[CM] Mission.Update() All contracts are completed!");
            this.CompleteAcceptedMission(simTime);

            return previousStatus != this.status;  // return true if the status changed (to let the manager know)
        }
        
        //  Accept offered mission, to be called from GUI accept button.
        public void AcceptOfferedMission(KSA.SimTime simTime)
        {
            if (this.status == MissionStatus.Offered)
            {
                this.status = MissionStatus.Accepted;
                this.acceptedSimTime = simTime;
                 MissionUtils.TriggerAction(this, ContractBlueprint.TriggerType.OnMissionAccept);
            }
        }
        
        //  Reject mission, to be called from GUI reject button.
        public void RejectMission(KSA.SimTime simTime)
        {
            if (this.status is MissionStatus.Offered or MissionStatus.Accepted)
            {
                this.status = MissionStatus.Rejected;
                this.finishedSimTime = simTime;
                 MissionUtils.TriggerAction(this, ContractBlueprint.TriggerType.OnMissionReject);
            }
        }
        
        //  Expire offered mission, to be called on expire.
        public void ExpireOfferedMission(KSA.SimTime simTime)
        {
            if (this.status is MissionStatus.Offered)
            {
                this.status = MissionStatus.Rejected;
                this.finishedSimTime = simTime;
                 MissionUtils.TriggerAction(this, ContractBlueprint.TriggerType.OnMissionExpire);
            }
        }

        // Fail accepted mission, to be called on expire or requirement failing.
        private void FailAcceptedMission(KSA.SimTime simTime)
        {
            if (this.status == MissionStatus.Accepted)
            {
                this.status = MissionStatus.Failed;
                this.finishedSimTime = simTime;
                MissionUtils.TriggerAction(this, ContractBlueprint.TriggerType.OnMissionFail);
            }
        }

        // Complete accepted mission, to be called when all requirements are achieved.
        private void CompleteAcceptedMission(KSA.SimTime simTime)
        {
            if (this.status == MissionStatus.Accepted)
            {
                this.status = MissionStatus.Completed;
                this.finishedSimTime = simTime;
                MissionUtils.TriggerAction(this, ContractBlueprint.TriggerType.OnMissionComplete);
            }
        }
    }

    public enum MissionStatus
    {
        [XmlEnum("Failed")]
        Failed = 0,
        [XmlEnum("Rejected")]
        Rejected = 1,
        [XmlEnum("Offered")]
        Offered = 2,
        [XmlEnum("Accepted")]
        Accepted = 3,
        [XmlEnum("Completed")]
        Completed = 4,
    }
}
