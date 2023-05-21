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
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using NovAtelLogReader.DataPoints;
using NLog;
using System.Reflection;
using NovAtelLogReader.ListConverters;
using NovAtelLogReader.Readers;
using NovAtelLogReader.Publishers;
using NovAtelLogReader.LogRecordFormats;
using System.IO;

namespace NovAtelLogReader
{
    class Processor : IDisposable
    {
        private IReader _reader;
        private IPublisher _publisher;
        private ILogRecordFormat _logRecordFormat;
        private Timer _timer;
        private int _messageCounter;
        private readonly object _locker = new object();
        private Logger _logger = LogManager.GetCurrentClassLogger();
        private Dictionary<String, Type> _logTypes = new Dictionary<String, Type>();
        private Dictionary<String, IListConverter> _logListConverters = new Dictionary<String, IListConverter>();
        private Dictionary<String, List<object>> _logQueues = new Dictionary<string, List<object>>();

        /*
         * For SATXYZ2 interpolation
         * Grouped by sigcomb
         *
         * TODO: rewrite to standalone class
         */
        private Dictionary<String, DataPointSatxyz2> satxyz2lastSeenPoints = new Dictionary<String, DataPointSatxyz2>();

        public event EventHandler<ErrorEventArgs> UnrecoverableError;

        public Processor(IReader reader, IPublisher publisher, ILogRecordFormat logRecordFormat)
        {
            _publisher = publisher;
            _reader = reader;
            _logRecordFormat = logRecordFormat;
            _timer = new Timer((s) => PublishDataPoints(), null, Timeout.InfiniteTimeSpan, Timeout.InfiniteTimeSpan);

            foreach (var type in Util.GetTypesWithAttribute<DataPointAttribute>())
            {
                var name = type.GetCustomAttribute<DataPointAttribute>().Name;
                _logTypes.Add(name, type);
                _logQueues.Add(name, new List<object>());
            }

            foreach (var type in Util.GetTypesWithAttribute<ListConverterAttribute>())
            {
                var name = type.GetCustomAttribute<ListConverterAttribute>().Name;
                _logListConverters.Add(name, (IListConverter)Activator.CreateInstance(type));
            }
        }

        public void Start()
        {
            _logger.Info("Запуск процессора сообщений");
            _messageCounter = 0;
            _reader.DataReceived += (s, e) => Task.Run(() => ProcessMessage(e.Data));
            _reader.ReadError += (s, e) => UnrecoverableError?.Invoke(this, e);

            try
            {
                _reader.Open(_logRecordFormat);
                _publisher.Open();
                _timer.Change(TimeSpan.Zero, TimeSpan.FromMilliseconds(Properties.Settings.Default.PublishRate));
            }
            catch (Exception ex)
            {
                UnrecoverableError?.Invoke(this, new ErrorEventArgs(ex));
            }
        }

        public void Stop()
        {
            _logger.Info("Остановка процессора сообщений");

            lock (_locker)
            {
                _timer.Change(Timeout.InfiniteTimeSpan, Timeout.InfiniteTimeSpan);

                foreach (var queue in _logQueues)
                {
                    queue.Value.Clear();
                }

                try { _reader.Close(); } catch (Exception) { }
                try { _publisher.Close(); } catch (Exception) { }
            }
        }

        private void ProcessMessage(byte[] message)
        {
            var record = _logRecordFormat.ExtrcatLogRecord(message);
            var name = record.Header.Name;

            if (_logListConverters.ContainsKey(name))
            {
                lock (_locker)
                {
                    _logQueues[name].AddRange(_logListConverters[name].ToList(record));
                }
            }
            else
            {
                _logger.Warn("Получены данные для лога {0}, для которого нет обработчика", name);
            }
        }

        private void PublishDataPoints()
        {
            if (_reader.MessageCounter == _messageCounter)
            {
                UnrecoverableError?.Invoke(this, new ErrorEventArgs(new FatalException("Число сообщений не изменилось")));
            }

            _messageCounter = _reader.MessageCounter;
            lock (_locker)
            {
                Func<DataPointSatxyz2, long> timestampGetter =
                    (x) => x.Timestamp;
                Func<DataPointSatxyz2, string> groupByFunc =
                    (x) => x.Satellite;
                Func<long, DataPointSatxyz2, DataPointSatxyz2, double, DataPointSatxyz2> interpolatorFunc =
                    (time, data1, data2, t) => {
                        var X = ResampleExt.Lerp(data1.X, data2.X, t);
                        var Y = ResampleExt.Lerp(data1.Y, data2.Y, t);
                        var Z = ResampleExt.Lerp(data1.Z, data2.Z, t);

                        var res = new DataPointSatxyz2
                        {
                            // The date is already interpolated for us.
                            Timestamp =        time,
                            NavigationSystem = data1.NavigationSystem,
                            Prn =              data1.Prn,
                            Satellite =        data1.Satellite,

                            // We must interpolate, just doing a simple linear interpolation here.
                            X = X,
                            Y = Y,
                            Z = Z
                        };

                        // Here we instantiate and return an output (resampled) data point.
                        return res;
                    };

                InterpolateSATXYZ2<DataPointSatxyz2>(
                    timestampGetter, groupByFunc, interpolatorFunc
                );
            }

            foreach (var queue in _logQueues)
            {
                lock (_locker)
                {
                    _logger.Info("Отправка {0} точек по логу {1}", queue.Value.Count, queue.Key);
                    _publisher.Publish(_logTypes[queue.Key], queue.Value);
                    queue.Value.Clear();
                }
            }
        }

        /*
         * For SATXYZ2 interpolation
         */
        private void InterpolateSATXYZ2<T>(
            Func<T, long> timestampGetter,
            Func<T, string> groupByFunc,
            Func<long, T, T, double, T> interpolatorFunc
        )
        {
            var queue = _logQueues["SATXYZ2"];
            var acc = new List<T>();

            _logger.Info("Интерполяция {0} точек по логу SATXYZ2", queue.Count);

            foreach (var sat in queue.Cast<T>().GroupBy(groupByFunc))
            {
                var max = timestampGetter(sat.Last());

                object lastSeen;
                try {
                    lastSeen = satxyz2lastSeenPoints[sat.Key];
                }
                // If no data, cannot interpolate. For next try
                catch (KeyNotFoundException) {
                    satxyz2lastSeenPoints.Remove(sat.Key);
                    satxyz2lastSeenPoints.Add(sat.Key, (DataPointSatxyz2) sat.Cast<object>().Last());
                    acc.AddRange(sat);
                    continue;
                }

                // Reset if cycle slip occured (period = 10 seconds)
                if ((max - timestampGetter((T) lastSeen)) > 20000)
                {
                    satxyz2lastSeenPoints.Remove(sat.Key);
                    satxyz2lastSeenPoints.Add(sat.Key, (DataPointSatxyz2) sat.Cast<object>().Last());
                    acc.AddRange(sat);
                    continue;
                }

                var prepended = sat.Prepend((T) lastSeen);
                var min = timestampGetter(prepended.First());

                var interpolated = prepended.Resample(
                    min, max, 20, timestampGetter, interpolatorFunc
                );

                satxyz2lastSeenPoints[sat.Key] = interpolated.Cast<DataPointSatxyz2>().Last();

                acc.AddRange(interpolated.Skip(1)); // was sent in previous itteration
            }

            _logger.Info("Total: [{0}]", acc.Count());
            _logQueues["SATXYZ2"].Clear();
            _logQueues["SATXYZ2"].AddRange(acc.Cast<object>());
            _logger.Info("Total in queue: [{0}]", _logQueues["SATXYZ2"].Count());
        }

        public void Dispose()
        {
            _timer.Dispose();
        }
    }
}
