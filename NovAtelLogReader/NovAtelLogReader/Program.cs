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

using System;
using NLog;
using NovAtelLogReader.Readers;
using NovAtelLogReader.LogRecordFormats;
using NovAtelLogReader.Publishers;
using System.Threading;
using System.Threading.Tasks;
using System.IO;
using System.IO.Pipes;

namespace NovAtelLogReader
{
    class NovAtelService
    {
        private Processor _processor;
        private CancellationTokenSource _cancellation;
        private Logger _logger = LogManager.GetCurrentClassLogger();
        private ManualResetEvent _signal = new ManualResetEvent(false);

        private IReader _reader = (IReader)Activator.CreateInstance(Type.GetType(Properties.Settings.Default.Reader));
        private IPublisher _publisher = (IPublisher)Activator.CreateInstance(Type.GetType(Properties.Settings.Default.Publisher));
        private ILogRecordFormat _format = (ILogRecordFormat)Activator.CreateInstance(Type.GetType(Properties.Settings.Default.Format));

        private void UnrecoverableErrorHandler(object sender, ErrorEventArgs eventArgs)
        {
            var exception = eventArgs.GetException();
            _logger.Fatal(exception);

            // Fail fast
            Environment.Exit(1);

            //if (exception is FatalException)
            //{
            //    _logger.Fatal("Аварийная остановка");
            //    Environment.Exit(1);
            //}
            //else
            //{
            //    _signal.Set();
            //}
        }

        private async void Loop()
        {
            using (_processor = new Processor(_reader, _publisher, _format))
            {
                _processor.UnrecoverableError += UnrecoverableErrorHandler;

                while (!_cancellation.IsCancellationRequested)
                {
                    try
                    {
                        _processor.Start();
                        _signal.WaitOne();
                        _signal.Reset();
                        _processor.Stop();
                    }
                    catch (Exception ex)
                    {
                        _logger.Fatal(ex);
                    }

                    await Task.Delay(1000, _cancellation.Token);
                }
            }
        }

        public void Start()
        {
            _logger.Info("Запуск службы");
            _cancellation = new CancellationTokenSource();
            Task.Run(() => Loop(), _cancellation.Token);
        }

        public void Stop()
        {
            _logger.Info("Остановка службы");
            _cancellation.Cancel();
            _signal.Set();
        }
    }


    class Program
    {
        public static void Main()
        {
            ComPortReader.GetAvailablePortNames();
            
            var pipe = new NamedPipeClientStream(".", "novatel-log-reader", PipeDirection.InOut);
            var service = new NovAtelService();

            service.Start();
            pipe.Connect();

            using (var reader = new StreamReader(pipe))
            {
                reader.ReadLine();
            }

            service.Stop();
        }
    }
}
