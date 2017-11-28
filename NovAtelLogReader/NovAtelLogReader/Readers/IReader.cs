using NovAtelLogReader.LogRecordFormats;
using System;

namespace NovAtelLogReader.Readers
{
    interface IReader
    {
        void Open(ILogRecordFormat recordFormat);
        void Close();
        event EventHandler<ReceiveEventArgs> DataReceived;
        event EventHandler<EventArgs> ReadError;
    }
}
