﻿using NLog;
using NovAtelLogReader.LogRecordFormats;
using NovAtelLogReader.PInvoke;
using System;
using System.Collections;
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
        public event EventHandler<ErrorEventArgs> ReadError;

        private Logger _logger = LogManager.GetCurrentClassLogger();

        // private SerialPort _serialPort;

        private PISerialPort _serialPort;
        private CancellationTokenSource _cts;
        ILogRecordFormat _recordFormat;

        private volatile int _messageCounter = 0;

        private readonly int HeaderLength = 28;
        private readonly byte[] HeaderStart = new byte[] { 0xaa, 0x44, 0x12 };

        private readonly List<string> _initCommands = new List<string>()
        {
            "UNLOGALL TRUE",
            "CNOUPDATE 20HZ",
            "DIFFCODEBIASCONTROL DISABLE",
            "EXTERNALCLOCK DISABLE",
            "DLLTIMECONSTA GPSL2P 0",
            "DLLTIMECONSTA GPSL2C 0",
            "DLLTIMECONSTA GLOL2P 0",
            "DLLTIMECONSTA GLOL2CA 0",
            "RTKDYNAMICS STATIC",
            "SETIONOTYPE L1L2",
            "ISMBANDWIDTH 1.0 0.0",
            "ISMSIGNALCONTROL GPSL2P ENABLE ENABLE",
            "LOG PSRPOSB ONTIME 1",
            "LOG RANGEB ONTIME 0.02",
            "LOG ISMRAWTECB ONNEW",
            "LOG ISMDETOBSB ONNEW",
            "LOG ISMREDOBSB ONNEW",
            "LOG SATXYZ2B ONTIME 10"
        };

        public int MessageCounter
        {
            get { return _messageCounter;  }
        }

        public void Close()
        {
            _logger.Info("Закрытие COM-порта");

            DataReceived = null;
            ReadError = null;

            if (_cts != null)
            {
                _cts.Cancel();
                _cts.Dispose();
                _cts = null;
            }

            if (_serialPort != null)
            {
                _serialPort.Close();
                _serialPort.Dispose();
                _serialPort = null;
            }            
        }

        public void Open(ILogRecordFormat recordFormat)
        {
            _recordFormat = recordFormat;
            _messageCounter = 0;

            InitReceiver();

            Task.Run(() => Read(), _cts.Token).ContinueWith(_ => ReadError?.Invoke(this, new ErrorEventArgs(_.Exception)), TaskContinuationOptions.OnlyOnFaulted);
        }

        private void InitReceiver()
        {
            var portName = Properties.Settings.Default.ComPortName;
            _logger.Info("Открытие COM-порта {0}", portName);
            _cts = new CancellationTokenSource();

            //_serialPort = new SerialPort(portName);
            //_serialPort.ReadTimeout = 1500;
            //_serialPort.WriteTimeout = 1500;
            //_serialPort.DtrEnable = true;
            //_serialPort.RtsEnable = true;
            //_serialPort.BaudRate = Properties.Settings.Default.ComPortSpeed;
            //_serialPort.Open();

            _serialPort = new PISerialPort();
            _serialPort.Open(portName, Properties.Settings.Default.ComPortSpeed);

            _logger.Info("Инициализация приемника");
            _initCommands.ForEach(_serialPort.WriteLine);

            Thread.Sleep(1500);

            _serialPort.DiscardInBuffer();
            _serialPort.DiscardOutBuffer();
        }

        public void Read()
        {
            byte[] buffer = new byte[HeaderLength];
            byte[] crc = new byte[4];
            int dataLength = 0;

            while (!_cts.IsCancellationRequested)
            {
                if (_serialPort.Read(buffer, 0, 1) > 0 && buffer[0] != 0xaa) continue;
                if (_serialPort.Read(buffer, 1, 1) > 0 && buffer[1] != 0x44) continue;
                if (_serialPort.Read(buffer, 2, 1) > 0 && buffer[2] != 0x12) continue;
                if (_serialPort.Read(buffer, 3, 1) > 0 && buffer[3] != HeaderLength) continue;

                _messageCounter++;

                _serialPort.Read(buffer, 4, HeaderLength - 4);
                dataLength = BitConverter.ToUInt16(buffer, 8);

                Array.Resize(ref buffer, HeaderLength + dataLength);
                _serialPort.Read(buffer, HeaderLength, dataLength);
                _serialPort.Read(crc, 0, 4);
                
                if (Util.CalculateBlockCRC32(buffer) == BitConverter.ToUInt32(crc, 0))
                {
                    DataReceived?.Invoke(this, new ReceiveEventArgs() { Data = buffer });
                }
                else
                {
                    _logger.Warn("Невверная контрольная сумма пакета данных");
                }
            }
        }
    }
}
