using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NovAtelLogReader.LogData
{
    class LogDataBase
    {
        public NavigationSystem NavigationSystem { get; set; }
        public SignalType SignalType { get; set; }
        public uint Prn { get; set; }
        public int GloFreq { get; set; }
    }
}
