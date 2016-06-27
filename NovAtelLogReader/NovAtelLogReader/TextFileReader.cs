using System;

using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace NovAtelLogReader
{
    class TextFileReader : IReader
    {
        public event EventHandler<ReceiveEventArgs> DataReceived;
        private StreamReader _file;
        private CancellationTokenSource _cts;
        ILogRecordFormat _recordFormat;

        public void Close()
        {
            _cts.Cancel();
            _cts.Dispose();

            if (_file != null)
            {
                _file.Dispose();
            }
        }

        public void Open(ILogRecordFormat recordFormat)
        {
            _recordFormat = recordFormat;
            _file = new StreamReader(Properties.Settings.Default.PathForReading);
            _cts = new CancellationTokenSource();
            Task.Run(() =>
            {
                string line;
                while ((line = _file.ReadLine()) != null)
                {
                    DataReceived?.Invoke(this, new ReceiveEventArgs() { LogRecord = _recordFormat.Parse(line) });
                }
            }, _cts.Token);
        }
    }
}
