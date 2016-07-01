using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace NovAtelLogReader.DataPoints
{
    [DataContract]
    [Serializable]
    public class DataPointPsrpos
    {
        [DataMember]
        public long Timestamp { get; set; }
        //[DataMember]
        //public NavigationSystem NavigationSystem { get; set; }
        //[DataMember]
        //public SignalType SignalType { get; set; }
        //[DataMember]
        //public string Satellite { get; set; }
        //[DataMember]
        //public uint Prn { get; set; }
        //[DataMember]
        //public int GloFreq { get; set; }
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
