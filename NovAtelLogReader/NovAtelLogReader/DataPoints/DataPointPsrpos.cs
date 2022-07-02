using System;
using System.Runtime.Serialization;

namespace NovAtelLogReader.DataPoints
{
    /*
     *
     * {
     *   "name": "NovAtelLogReader.DataPoints.DataPointPsrpos",
     *   "type": "record",
     *   "fields": [
     *     {
     *       "name": "Hgt",
     *       "type": "double"
     *     },
     *     {
     *       "name": "HgtStdDev",
     *       "type": "double"
     *     },
     *     {
     *       "name": "Lat",
     *       "type": "double"
     *     },
     *     {
     *       "name": "LatStdDev",
     *       "type": "double"
     *     },
     *     {
     *       "name": "Lon",
     *       "type": "double"
     *     },
     *     {
     *       "name": "LonStdDev",
     *       "type": "double"
     *     },
     *     {
     *       "name": "Timestamp",
     *       "type": "long"
     *     }
     *   ]
     * }
     * */
    [DataContract]
    [Serializable]
    [DataPoint(Name = "PSRPOS", Queue = "datapoint-raw-psrpos")]
    public class DataPointPsrpos
    {
        [DataMember]
        public long Timestamp { get; set; }
        [DataMember]
        public double Lat { get; set; }
        [DataMember]
        public double Lon { get; set; }
        [DataMember]
        public double Hgt { get; set; }
        [DataMember]
        public double LatStdDev { get; set; }
        [DataMember]
        public double LonStdDev { get; set; }
        [DataMember]
        public double HgtStdDev { get; set; }
    }
}
