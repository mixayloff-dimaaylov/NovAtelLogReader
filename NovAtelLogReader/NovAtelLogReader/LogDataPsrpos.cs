using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NovAtelLogReader
{
    class LogDataPsrpos : LogDataBase
    {
        public double Lat { get; set; }
        public double Lon { get; set; }
        public double Hgt { get; set; }
        public double LatStdDev { get; set; }
        public double LonStdDev { get; set; }
        public double HgtStdDev { get; set; }
    }
}
