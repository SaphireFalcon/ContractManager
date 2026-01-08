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
        [XmlElement("missionUID", DataType = "string")]
        public string missionUID { get; set; } = string.Empty;

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
        internal KSA.SimTime offeredSimTime { get; set; } = new KSA.SimTime(double.NaN);
        internal KSA.SimTime acceptedSimTime { get; set; } = new KSA.SimTime(double.NaN);
        internal KSA.SimTime finishedSimTime { get; set; } = new KSA.SimTime(double.NaN);

        // list of contractUID referencing the contracts created as part of this mission.
        [XmlArray("contractUIDs")]
        public List<string> contractUIDs { get; set; } = new List<string>();

        public Mission() { }

        // Clone, e.g after deserializing from a stream.
        internal Mission? Clone(List<MissionBlueprint> missionBlueprints, List<ContractBlueprint.ContractBlueprint> contractBlueprints)
        {
            // TODO!!!
            return new Mission();
        }

        // Constructor to instantiate a mission from a blueprint. Used when a misson is offered.
        public Mission(in MissionBlueprint missionBlueprint, KSA.SimTime simTime)
        {
            this.blueprintUID = missionBlueprint.uid;
            this._missionBlueprint = missionBlueprint;
            this.offeredSimTime = simTime;
            this.status = MissionStatus.Offered;
            this.missionUID = String.Format("{0}_{1:F0}", missionBlueprint.uid, this.offeredSimTime.Seconds());

            // Should we create contracts here? We should just allow the "normal" offer procedure do that.
        }
    }

    public enum MissionStatus
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
