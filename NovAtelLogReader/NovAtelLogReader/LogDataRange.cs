using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NovAtelLogReader
{
    class LogDataRange : LogDataBase
    {
        public double Psr { get; set; }
        public double PsrStd { get; set; }
        public double Adr { get; set; }
        public double AdrStd { get; set; }
        public double CNo { get; set; }
        public double LockTime { get; set; }
        public UInt32 Tracking { get; set; }
    }
}
