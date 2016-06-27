using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace NovAtelLogReader
{
    class ComPortReader : IReader
    {
        public event EventHandler<ReceiveEventArgs> DataReceived;
        private SerialPort _serialPort;
        private CancellationTokenSource _cts;
        ILogRecordFormat _recordFormat;
        List<string> _commands = new List<string>()
        {
            "unlog all",
            "CNOUPDATE 20HZ",
            "DIFFCODEBIASCONTROL DISABLE",
            "EXTERNALCLOCK DISABLE",
            "DLLTIMECONSTA GPSL2P 0",
            "DLLTIMECONSTA GPSL2C 0",
            "DLLTIMECONSTA GLOL2P 0",
            "DLLTIMECONSTA GLOL2CA 0",
            "RTKDYNAMICS STATIC",
            "SETIONOTYPE L1L2",
            "ismbandwidth 1.0 0.0",
            "ismsignalcontrol GPSL2P enable enable",
            //"log psrposa ontime 1",
            //"log psrxyza ontime 1",
            //"log satxyz2a ontime 1",
            "log rangea ontime 0.02",
            //"log ismrawobsa onnew",
            //"log ismrawteca onnew",
            //"log satvis2a ontime 10",
            //"log ismdetobsa onnew",
            //"LOG ISMREDOBSa ONNEW"
        };

        public void Close()
        {
            _cts.Cancel();
            _cts.Dispose();
            _serialPort.Close();
            _serialPort.Dispose();
        }

        public void Open(ILogRecordFormat recordFormat)
        {
            _recordFormat = recordFormat;
            _cts = new CancellationTokenSource();
            _serialPort = new SerialPort(Properties.Settings.Default.SerialPort);
            _serialPort.ReadTimeout = 500;
            _serialPort.WriteTimeout = 500;
            _serialPort.BaudRate = Properties.Settings.Default.SerialPortSpeed;
            _serialPort.Open();

            Task.Run(() => Read(), _cts.Token);

        }

        public void Read()
        {
            // Инициализация приемника
            _commands.ForEach(_serialPort.WriteLine);
            _serialPort.DiscardInBuffer();
            _serialPort.DiscardOutBuffer();

            // Чтение строк
            while (!_cts.IsCancellationRequested)
            {
                try
                {
                    var line = _serialPort.ReadLine();
                    DataReceived?.Invoke(this, new ReceiveEventArgs() { LogRecord = _recordFormat.Parse(line) });
                }
                catch (Exception)
                {
                }
            }
        }
    }
}
