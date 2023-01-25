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
                InterpolateDataPoints();
            }

            foreach (var queue in _logQueues)
            {
                lock (_locker)
                {
                    if (queue.Value.Count > 0)
                    {
                        _logger.Info("Отправка {0} точек по логу {1}", queue.Value.Count, queue.Key);
                        _publisher.Publish(_logTypes[queue.Key], queue.Value);
                        queue.Value.Clear();
                    }
                }
            }
        }

        private void InterpolateDataPoints()
        {
            var queue = _logQueues["SATXYZ2"];
            var acc = new List<object>();

            foreach (var sat in queue.GroupBy(x => ((DataPointSatxyz2) x).Satellite))
            {
                var min = sat
                    .Select(x => ((DataPointSatxyz2) x).Timestamp).Min();
                var max = sat
                    .Select(x => ((DataPointSatxyz2) x).Timestamp).Max();

                acc.AddRange((List<object>) sat.Resample(
                    min, max, 20,
                    data => ((DataPointSatxyz2) data).Timestamp,
                    (time, data1, data2, t) =>
                    // Here we instantiate and return an output (resampled) data point.
                    (object) new DataPointSatxyz2
                    {
                        // The date is already interpolated for us.
                        Timestamp =        ((DataPointSatxyz2) data1).Timestamp,
                        NavigationSystem = ((DataPointSatxyz2) data1).NavigationSystem,
                        Prn =              ((DataPointSatxyz2) data1).Prn,
                        Satellite =        ((DataPointSatxyz2) data1).Satellite,

                        // We must interpolate, just doing a simple linear interpolation here.
                        X = ResampleExt.Lerp(((DataPointSatxyz2) data1).X, ((DataPointSatxyz2) data2).X, t),
                        Y = ResampleExt.Lerp(((DataPointSatxyz2) data1).Y, ((DataPointSatxyz2) data2).Y, t),
                        Z = ResampleExt.Lerp(((DataPointSatxyz2) data1).Z, ((DataPointSatxyz2) data2).Z, t)
                    }
                ));
            }

            _logQueues["SATXYZ2"] = acc;
        }

        public void Dispose()
        {
            _timer.Dispose();
        }
    }
}

// The function is an extension method, so it must be defined in a static class.
public static class ResampleExt
{
    // Resample an input time series and create a new time series between two
    // particular dates sampled at a specified time interval.
    public static IEnumerable<OutputDataT> Resample<InputValueT, OutputDataT>(

        // Input time series to be resampled.
        this IEnumerable<InputValueT> source,

        // Start date of the new time series.
        long startDate,

        // Date at which the new time series will have ended.
        long endDate,

        // The time interval between samples.
        long resampleInterval,

        // Function that selects a date/time value from an input data point.
        Func<InputValueT, long> dateSelector,

        // Interpolation function that produces a new interpolated data point
        // at a particular time between two input data points.
        Func<long, InputValueT, InputValueT, double, OutputDataT> interpolator
    )
    {
        // ... argument checking omitted ...

        //
        // Manually enumerate the input time series...
        // This is manual because the first data point must be treated specially.
        //
        var e = source.GetEnumerator();
        if (e.MoveNext())
        {
            // Initialize working date to the start date, this variable will be used to
            // walk forward in time towards the end date.
            var workingDate = startDate;

            // Extract the first data point from the input time series.
            var firstDataPoint = e.Current;

            // Extract the first data point's date using the date selector.
            var firstDate = dateSelector(firstDataPoint);

            // Loop forward in time until we reach either the date of the first
            // data point or the end date, which ever comes first.
            while (workingDate < endDate && workingDate <= firstDate)
            {
                // Until we reach the date of the first data point,
                // use the interpolation function to generate an output
                // data point from the first data point.
                yield return interpolator(workingDate, firstDataPoint, firstDataPoint, 0);

                // Walk forward in time by the specified time period.
                workingDate += resampleInterval;
            }

            //
            // Setup current data point... we will now loop over input data points and
            // interpolate between the current and next data points.
            //
            var curDataPoint = firstDataPoint;
            var curDate = firstDate;

            //
            // After we have reached the first data point, loop over remaining input data points until
            // either the input data points have been exhausted or we have reached the end date.
            //
            while (workingDate < endDate && e.MoveNext())
            {
                // Extract the next data point from the input time series.
                var nextDataPoint = e.Current;

                // Extract the next data point's date using the data selector.
                var nextDate = dateSelector(nextDataPoint);

                // Calculate the time span between the dates of the current and next data points.
                var timeSpan = nextDate - firstDate;

                // Loop forward in time until wwe have moved beyond the date of the next data point.
                while (workingDate <= endDate && workingDate < nextDate)
                {
                    // The time span from the current date to the working date.
                    var curTimeSpan = workingDate - curDate;

                    // The time between the dates as a percentage (a 0-1 value).
                    var timePct = curTimeSpan / timeSpan;

                    // Interpolate an output data point at the particular time between
                    // the current and next data points.
                    yield return interpolator(workingDate, curDataPoint, nextDataPoint, timePct);

                    // Walk forward in time by the specified time period.
                    workingDate += resampleInterval;
                }

                // Swap the next data point into the current data point so we can move on and continue
                // the interpolation with each subsqeuent data point assuming the role of
                // 'next data point' in the next iteration of this loop.
                curDataPoint = nextDataPoint;
                curDate = nextDate;
            }

            // Finally loop forward in time until we reach the end date.
            while (workingDate < endDate)
            {
                // Interpolate an output data point generated from the last data point.
                yield return interpolator(workingDate, curDataPoint, curDataPoint, 1);

                // Walk forward in time by the specified time period.
                workingDate += resampleInterval;
            }
        }
    }

    // The linear interpolation is defined as follows.
    public static double Lerp(double v1, double v2, double t)
    {
        return v1 + ((v2 - v1) * t);
    }
}
