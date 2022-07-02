using NovAtelLogReader.LogData;
using System;
using System.Runtime.Serialization;

namespace NovAtelLogReader.DataPoints
{
    /*
     *
     * {
     *   "name": "NovAtelLogReader.DataPoints.DataPointSatvis",
     *   "type": "record",
     *   "fields": [
     *     {
     *       "name": "Az",
     *       "type": "double"
     *     },
     *     {
     *       "name": "Elev",
     *       "type": "double"
     *     },
     *     {
     *       "name": "GloFreq",
     *       "type": "int"
     *     },
     *     {
     *       "name": "Health",
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
     *       "name": "Prn",
     *       "type": "int"
     *     },
     *     {
     *       "name": "Satellite",
     *       "type": "string"
     *     },
     *     {
     *       "name": "SatVis",
     *       "type": "boolean"
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
    [DataPoint(Name = "SATVIS", Queue = "datapoint-raw-satvis")]
    public class DataPointSatvis
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
        public bool SatVis { get; set; }
        [DataMember]
        public ulong Health { get; set; }
        [DataMember]
        public double Elev { get; set; }
        [DataMember]
        public double Az { get; set; }
    }
}
