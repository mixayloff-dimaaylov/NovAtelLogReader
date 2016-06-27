using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NovAtelLogReader
{
    class LogDataIsmredobs : LogDataBase
    {
        public double AverageCmc { get; set; }
        public double CmcStdDev { get; set; }
        public double TotalS4 { get; set; }
        public double CorrS4 { get; set; }
        public double PhaseSigma1Second { get; set; }
        public double PhaseSigma30Second { get; set; }
        public double PhaseSigma60Second { get; set; }
    }
}

