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
     *       "name": "Timestamp",
     *       "type": "long"
     *     },
     *     {
     *       "name": "Lat",
     *       "type": "double"
     *     },
     *     {
     *       "name": "Lon",
     *       "type": "double"
     *     },
     *     {
     *       "name": "Hgt",
     *       "type": "double"
     *     },
     *     {
     *       "name": "LatStdDev",
     *       "type": "double"
     *     },
     *     {
     *       "name": "LonStdDev",
     *       "type": "double"
     *     },
     *     {
     *       "name": "HgtStdDev",
     *       "type": "double"
     *     }
     *   ]
     * }
     *
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
