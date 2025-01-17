﻿/*
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
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Pipes;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Topshelf;

namespace NovAtelRunner
{
class Program
    {
        public static void Main()
        {

            HostFactory.Run(host =>
            {
                host.Service<NovAtelService>(service =>
                {
                    service.ConstructUsing(name => new NovAtelService());
                    service.WhenStarted(x => Task.Run(() => x.Start()));
                    service.WhenStopped(x => x.Stop());
                });
                
                host.RunAsLocalSystem();
                host.SetDescription("Сервис чтения данных со спутникового приемника NovAtel");
                host.SetDisplayName("Сервис NovAtel");
                host.SetServiceName("NovAtelService");
            });
        }
    }

    internal class NovAtelService
    {
        private volatile bool _running;
        private Process _process;
        private NamedPipeServerStream _pipe;

        private string _pipeName = "novatel-log-reader";
        private string _readerFileName = "NovAtelLogReader.exe";

        public NovAtelService()
        {
            _running = true;
            _pipe = new NamedPipeServerStream(_pipeName, PipeDirection.InOut);
        }

        public void Start()
        {
            while (_running)
            {
                _process = new Process()
                {
                    StartInfo = new ProcessStartInfo(_readerFileName)
                };

                _process.Start();
                _pipe.WaitForConnectionAsync();

                _process.WaitForExit();

                if (_pipe.IsConnected)
                {
                    _pipe.Disconnect();
                }

                _process.Dispose();
                _process = null;
            }
            
        }

        public void Stop()
        {
            _running = false;

            if (_pipe != null &&  _pipe.IsConnected)
            {
                try
                {
                    using (var writer = new StreamWriter(_pipe))
                    {
                        writer.AutoFlush = true;
                        writer.WriteLine("stop");
                    }

                    Thread.Sleep(5000);
                }
                catch (Exception)
                {
                    Console.WriteLine("Failed to send stop command via pipe");
                }
            }

            if (_process != null && _process.HasExited == false)
            {
                _process.Kill();
            }
        }
    }
}
