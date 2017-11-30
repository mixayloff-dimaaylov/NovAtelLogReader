﻿using NLog;
using System;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using NovAtelLogReader.LogRecordFormats;

namespace NovAtelLogReader.Readers
{
    class TextFileReader : IReader
    {
        public event EventHandler<ReceiveEventArgs> DataReceived;
        public event EventHandler<EventArgs> ReadError;

        private StreamReader _file;
        private CancellationTokenSource _cts;
        private ILogRecordFormat _recordFormat;
        private Logger _logger = LogManager.GetCurrentClassLogger();

        public void Close()
        {
            _logger.Info("Закрытие файла.");

            _cts.Cancel();
            _cts.Dispose();

            DataReceived = null;
            ReadError = null;

            if (_file != null)
            {
                _file.Dispose();
            }
        }

        public void Open(ILogRecordFormat recordFormat)
        {
            var fileName = Properties.Settings.Default.TextFilePath;
            _logger.Info("Открытие файла {0}", fileName);

            try
            {
                _recordFormat = recordFormat;
                _cts = new CancellationTokenSource();
                _file = new StreamReader(fileName);

                var readTask = Task.Run(async () =>
                {
                    _logger.Info("Запуск потока чтения файла");

                    string line;
                    while ((line = _file.ReadLine()) != null)
                    {
                        DataReceived?.Invoke(this, new ReceiveEventArgs() { Data = Encoding.ASCII.GetBytes(line) });
                        await Task.Delay(10, _cts.Token);
                    }
                }, _cts.Token);
                
                readTask.ContinueWith(t => ReadError?.Invoke(this, EventArgs.Empty), TaskContinuationOptions.OnlyOnFaulted);
            }
            catch(Exception ex)
            {
                _logger.Error(ex, "Ошибка при обработке файла {0}", fileName);
                ReadError?.Invoke(this, EventArgs.Empty);
            }
        }
    }
}
