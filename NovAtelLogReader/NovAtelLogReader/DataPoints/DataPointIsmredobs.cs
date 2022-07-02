using NovAtelLogReader.LogData;
using System;
using System.Runtime.Serialization;

namespace NovAtelLogReader.DataPoints
{
    /*
     *
     * {
     *   "name": "NovAtelLogReader.DataPoints.DataPointIsmredobs",
     *   "type": "record",
     *   "fields": [
     *     {
     *       "name": "AverageCmc",
     *       "type": "double"
     *     },
     *     {
     *       "name": "CmcStdDev",
     *       "type": "double"
     *     },
     *     {
     *       "name": "CorrS4",
     *       "type": "double"
     *     },
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
     *       "name": "PhaseSigma1Second",
     *       "type": "double"
     *     },
     *     {
     *       "name": "PhaseSigma30Second",
     *       "type": "double"
     *     },
     *     {
     *       "name": "PhaseSigma60Second",
     *       "type": "double"
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
     *       "name": "Timestamp",
     *       "type": "long"
     *     },
     *     {
     *       "name": "TotalS4",
     *       "type": "double"
     *     }
     *   ]
     * }
     * */
    [DataContract]
    [Serializable]
    [DataPoint(Name = "ISMREDOBS", Queue = "datapoint-raw-ismredobs")]
    public class DataPointIsmredobs
    {
        [DataMember]
        public long Timestamp { get; set; }
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
        public double AverageCmc { get; set; }
        [DataMember]
        public double CmcStdDev { get; set; }
        [DataMember]
        public double TotalS4 { get; set; }
        [DataMember]
        public double CorrS4 { get; set; }
        [DataMember]
        public double PhaseSigma1Second { get; set; }
        [DataMember]
        public double PhaseSigma30Second { get; set; }
        [DataMember]
        public double PhaseSigma60Second { get; set; }
    }
}

