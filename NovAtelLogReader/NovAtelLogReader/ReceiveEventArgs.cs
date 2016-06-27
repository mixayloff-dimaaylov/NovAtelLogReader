using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NovAtelLogReader
{
    class ReceiveEventArgs : EventArgs
    {
        public LogRecord LogRecord { get; set; }
    }
}
