using NovAtelLogReader.LogData;
using System;
using System.Runtime.Serialization;

namespace NovAtelLogReader.DataPoints
{

    /*
     *
     * {
     *   "name": "NovAtelLogReader.DataPoints.DataPointIsmdetobs",
     *   "type": "record",
     *   "fields": [
     *     {
     *       "name": "GloFreq",
     *       "type": "int"
     *     },
     *     {
     *       "name": "NavigationSystem",
     *       "type": {
     * 	"name": "NovAtelLogReader.LogData.NavigationSystem",
     * 	"type": "enum",
     * 	"symbols": [
     * 	  "GPS",
     * 	  "GLONASS",
     * 	  "SBAS",
     * 	  "Galileo",
     * 	  "BeiDou",
     * 	  "QZSS",
     * 	  "Reserved",
     * 	  "Other"
     * 	]
     *       }
     *     },
     *     {
     *       "name": "Power",
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
     * 	"name": "NovAtelLogReader.LogData.SignalType",
     * 	"type": "enum",
     * 	"symbols": [
     * 	  "Unknown",
     * 	  "L1CA",
     * 	  "L2C",
     * 	  "L2CA",
     * 	  "L2P",
     * 	  "L2P_codeless",
     * 	  "L2Y",
     * 	  "L5Q"
     * 	]
     *       }
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
    [DataPoint(Name = "ISMDETOBS", Queue = "datapoint-raw-ismdetobs")]
    public class DataPointIsmdetobs
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
        public double Power { get; set; }       
    }
}
