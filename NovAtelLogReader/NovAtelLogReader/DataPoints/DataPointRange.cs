using System;
using System.Runtime.Serialization;
using NovAtelLogReader.LogData;

namespace NovAtelLogReader.DataPoints
{
    /*
     *
     * {
     *   "name": "NovAtelLogReader.DataPoints.DataPointRange",
     *   "type": "record",
     *   "fields": [
     *     {
     *       "name": "Timestamp",
     *       "type": "long"
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
     *       "name": "SignalType",
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
     *       "name": "Satellite",
     *       "type": "string"
     *     },
     *     {
     *       "name": "Prn",
     *       "type": "int"
     *     },
     *     {
     *       "name": "GloFreq",
     *       "type": "int"
     *     },
     *     {
     *       "name": "Psr",
     *       "type": "double"
     *     },
     *     {
     *       "name": "Adr",
     *       "type": "double"
     *     },
     *     {
     *       "name": "CNo",
     *       "type": "double"
     *     },
     *     {
     *       "name": "LockTime",
     *       "type": "double"
     *     },
     *     {
     *       "name": "Power",
     *       "type": "double"
     *     }
     *   ]
     * }
     *
     * */

    [DataContract]
    [Serializable]
    [DataPoint(Name = "RANGE", Queue = "datapoint-raw-range")]
    public class DataPointRange
    {
        [DataMember]
        public long Timestamp { get; set;  }
        [DataMember]
        public NavigationSystem NavigationSystem { get; set; }
        [DataMember]
        public SignalType SignalType { get; set; }
        [DataMember]
        public string Satellite { get; set; }
        [DataMember]
        public uint Prn { get; set; }
        [DataMember]
        public int GloFreq { get; set; }
        [DataMember]
        public double Psr { get; set; }
        [DataMember]
        public double Adr { get; set; }
        [DataMember]
        public double CNo { get; set; }
        [DataMember]
        public double LockTime { get; set; }
        [DataMember]
        public double Power { get; set; }

        public bool IsValid()
        {
            switch (NavigationSystem)
            {
                case NavigationSystem.GLONASS:
                    return Prn > 0 && Prn <= 27;
                case NavigationSystem.GPS:
                    return Prn > 0 && Prn <= 32;
                default:
                    return false;

            }
        }
    }
}
