using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace NovAtelLogReader.DataPoints
{
    [DataContract]
    [Serializable]
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
        public double Power { get; set; }
    }
}
