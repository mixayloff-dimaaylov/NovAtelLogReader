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
    public class DataPointSatvis
    {
        [DataMember]
        public long Timestamp { get; set; }
        [DataMember]
        public NavigationSystem NavigationSystem { get; set; }
        //[DataMember]
        //public SignalType SignalType { get; set; }
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
