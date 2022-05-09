using NovAtelLogReader.LogData;
using System;
using System.Runtime.Serialization;

namespace NovAtelLogReader.DataPoints
{
    /*
     *
     * {
     *   "name": "NovAtelLogReader.DataPoints.DataPointIsmrawtec",
     *   "type": "record",
     *   "fields": [
     *     {
     *       "name": "GloFreq",
     *       "type": "int"
     *     },
     *     {
     *       "name": "NavigationSystem",
     *       "type": {
     *         "name": "NovAtelLogReader.LogData.NavigationSystem",
     *         "type": "enum",
     *         "symbols": [
     *           "GPS",
     *           "GLONASS",
     *           "SBAS",
     *           "Galileo",
     *           "BeiDou",
     *           "QZSS",
     *           "Reserved",
     *           "Other"
     *         ]
     *       }
     *     },
     *     {
     *       "name": "PrimarySignal",
     *       "type": {
     *         "name": "NovAtelLogReader.LogData.SignalType",
     *         "type": "enum",
     *         "symbols": [
     *           "Unknown",
     *           "L1CA",
     *           "L2C",
     *           "L2CA",
     *           "L2P",
     *           "L2P_codeless",
     *           "L2Y",
     *           "L5Q"
     *         ]
     *       }
     *     },
     *     {
     *       "name": "Prn",
     *       "type": "int"
     *     },
     *     {
     *       "name": "Satellite",
     *       "type": "string"
     *     },
     *     {
     *       "name": "SecondarySignal",
     *       "type": "NovAtelLogReader.LogData.SignalType"
     *     },
     *     {
     *       "name": "Tec",
     *       "type": "double"
     *     },
     *     {
     *       "name": "Timestamp",
     *       "type": "long"
     *     }
     *   ]
     * }
     *
     * */
    [DataContract]
    [Serializable]
    [DataPoint(Name = "ISMRAWTEC", Queue = "datapoint-raw-ismrawtec")]
    public class DataPointIsmrawtec
    {
        [DataMember]
        public long Timestamp { get; set; }
        [DataMember]
        public NavigationSystem NavigationSystem { get; set; }
        [DataMember]
        public string Satellite { get; set; }
        [DataMember]
        public uint Prn { get; set; }
        [DataMember]
        public int GloFreq { get; set; }
        [DataMember]
        public SignalType PrimarySignal { get; set; }
        [DataMember]
        public SignalType SecondarySignal { get; set; }
        [DataMember]
        public double Tec { get; set; }
    }
}
