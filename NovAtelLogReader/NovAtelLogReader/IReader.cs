using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NovAtelLogReader
{
    interface IReader
    {
        void Open(ILogRecordFormat recordFormat);
        void Close();
        event EventHandler<ReceiveEventArgs> DataReceived;
    }
}
