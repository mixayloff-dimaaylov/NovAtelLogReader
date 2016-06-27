using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NovAtelLogReader
{
    class LogRecord
    {
        public LogHeader Header { get; set; }
        public List<LogDataBase> Data { get; set; }
        public UInt32 Checksum { get; set; }
        
    }
}
