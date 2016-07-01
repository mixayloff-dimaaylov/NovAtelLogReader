using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NovAtelLogReader.LogData
{
    class LogDataIsmrawtec : LogDataBase
    {
        public SignalType PrimarySignal { get; set; }
        public SignalType SecondarySignal { get; set; }
        public double Tec { get; set; }
    }
}
