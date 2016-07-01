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

