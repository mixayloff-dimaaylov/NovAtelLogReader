using NLog;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace NovAtelLogReader.Readers
{
    class ComPortReader : IReader
    {
        public event EventHandler<ReceiveEventArgs> DataReceived;
        private SerialPort _serialPort;
        private CancellationTokenSource _cts;
        ILogRecordFormat _recordFormat;
        private Logger _logger = LogManager.GetCurrentClassLogger();

        List<string> _commands = new List<string>()
        {
            "unlogall true",
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
            "log psrposb ontime 1",
            //"log psrxyza ontime 1",
            //"log satxyz2a ontime 1",
            //"log rangeb ontime 0.02",
            //"log ismrawobsa onnew",
            //"log ismrawteca onnew",
            "log satvisb ontime 10",
            //"log ismdetobsa onnew",
            //"LOG ISMREDOBSA ONNEW"
            "log satxyz2b ontime 10"
        };

        public void Close()
        {
            _logger.Info("Закрытие COM-порта");
            _cts.Cancel();
            _cts.Dispose();
            _serialPort.Close();
            _serialPort.Dispose();
        }

        public void Open(ILogRecordFormat recordFormat)
        {
            _recordFormat = recordFormat;

            InitReceiver();
            Task.Run(() => Read(), _cts.Token);
        }

        private void InitReceiver()
        {
            var portName = Properties.Settings.Default.SerialPort;
            _logger.Info("Открытие COM-порта {0}", portName);
            _cts = new CancellationTokenSource();
            _serialPort = new SerialPort(portName);
            _serialPort.ReadTimeout = 1500;
            _serialPort.WriteTimeout = 1500;
            _serialPort.BaudRate = Properties.Settings.Default.SerialPortSpeed;
            _serialPort.Open();

            _logger.Info("Инициализация приемника");
            _commands.ForEach(_serialPort.WriteLine);
            Thread.Sleep(1500);
            _serialPort.DiscardInBuffer();
            _serialPort.DiscardOutBuffer();
        }

        public void Read()
        {
            // Чтение строк
            while (!_cts.IsCancellationRequested)
            {
                try
                {
                    string line = String.Empty;

                    _serialPort.ReadTo(Encoding.ASCII.GetString(new byte[] { 0xaa, 0x44, 0x12 }));
                    var headerLen = (byte) _serialPort.ReadByte();
                    
                    byte[] message = new byte[4096];
                    
                    if (headerLen == 28)
                    {
                        _serialPort.Read(message, 4, headerLen - 4);
                        var dataLen = BitConverter.ToUInt16(message, 8);
                        _serialPort.Read(message, headerLen, dataLen);
                        DataReceived?.Invoke(this, new ReceiveEventArgs() { LogRecord = _recordFormat.Parse(message) });
                    }

                    //try
                    //{
                    //    line = _serialPort.ReadLine();
                    //}
                    //catch (Exception)
                    //{
                    //    Close();
                    //    Thread.Sleep(100);
                    //    InitReceiver();
                    //}

                    //if (!String.IsNullOrEmpty(line))
                    //{
                    //    DataReceived?.Invoke(this, new ReceiveEventArgs() { LogRecord = _recordFormat.Parse(line) });
                    //}
                }
                catch (Exception ex)
                {
                    _logger.Error(ex);
                }
            }
        }
    }
}
