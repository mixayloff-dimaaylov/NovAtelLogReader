using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NovAtelLogReader
{
    class LogDataSatvis : LogDataBase
    {
        public bool SatVis { get; set; }
        public ulong Health { get; set; }
        public double Elev { get; set; }
        public double Az { get; set; }
    }
}
