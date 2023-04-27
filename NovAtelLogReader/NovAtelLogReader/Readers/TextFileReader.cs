/*
 * Copyright 2023 mixayloff-dimaaylov at github dot com
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *     http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

using NLog;
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
        public event EventHandler<ErrorEventArgs> ReadError;

        private StreamReader _file;
        private CancellationTokenSource _cts;
        private ILogRecordFormat _recordFormat;
        private Logger _logger = LogManager.GetCurrentClassLogger();

        private volatile int _messageCounter;

        public int MessageCounter
        {
            get { return _messageCounter; }
        }

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
            _recordFormat = recordFormat;
            _cts = new CancellationTokenSource();
            _file = new StreamReader(fileName);
            _messageCounter = 0;

            var readTask = Task.Run(async () =>
            {
                _logger.Info("Запуск потока чтения файла");

                string line;
                while ((line = _file.ReadLine()) != null)
                {
                    _messageCounter++;
                    DataReceived?.Invoke(this, new ReceiveEventArgs() { Data = Encoding.ASCII.GetBytes(line) });
                    await Task.Delay(10, _cts.Token);
                }
            }, _cts.Token);
                
            readTask.ContinueWith(_ => ReadError?.Invoke(this, new ErrorEventArgs(_.Exception)), TaskContinuationOptions.OnlyOnFaulted);
        }
    }
}
