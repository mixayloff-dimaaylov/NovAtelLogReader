using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace NovAtelLogReader
{
    class Processor
    {
        private IReader _reader;
        private IPublisher _publisher;
        private ILogRecordFormat _logRecordFormat;
        private List<DataPoint> _dataPoints;
        private Timer _timer;
        private readonly object _locker = new object();

        public Processor(IReader reader, IPublisher publisher, ILogRecordFormat logRecordFormat)
        {
            _dataPoints = new List<DataPoint>();
            _publisher = publisher;
            _reader = reader;
            _logRecordFormat = logRecordFormat;
            _timer = new Timer((s) => PublishDataPoints(), null, Timeout.InfiniteTimeSpan, Timeout.InfiniteTimeSpan);
        }

        public void Start()
        {
            _timer.Change(TimeSpan.Zero, TimeSpan.FromMilliseconds(Properties.Settings.Default.PublishRate));
            _publisher.Open();
            _reader.Open(_logRecordFormat);
            _reader.DataReceived += (s, e) => ProcessLogRecord(e.LogRecord);
        }

        public void Stop()
        {
            _timer.Change(Timeout.InfiniteTimeSpan, Timeout.InfiniteTimeSpan);
            _timer.Dispose();
            _reader.Close();
            _publisher.Close();
        }

        private void ProcessLogRecord(LogRecord logRecord)
        {
            var points = logRecord.Data.Where(data => data is LogDataRange).Select(data =>
            {
                var range = data as LogDataRange;
                var dataPoint = new DataPoint()
                {
                    Timestamp = logRecord.Header.Timestamp,
                    NavigationSystem = Util.GetNavigationSystem(range.Tracking),
                    SignalType = Util.GetSignalType(range.Tracking),
                    Prn = range.Prn,
                    GloFreq = range.GloFreq,
                    Adr = range.Adr,
                    Psr = range.Psr,
                    CNo = range.CNo
                };

                if (dataPoint.NavigationSystem == NavigationSystem.GLONASS)
                {
                    dataPoint.Prn = Util.GetActualPrn(dataPoint.Prn);
                    dataPoint.GloFreq = Util.GetActualGlonassFrequency(dataPoint.GloFreq);
                }

                dataPoint.Satellite = String.Format("{0}{1}", dataPoint.NavigationSystem, dataPoint.Prn);
                return dataPoint;
            });

            lock (_locker)
            {
                _dataPoints.AddRange(points);
            }
        }

        private void PublishDataPoints()
        {
            if (_dataPoints.Count > 0)
            {
                lock (_locker)
                {
                    if (_dataPoints.Count > 0)
                    {
                        Console.WriteLine("Puplishing {0} points starting from {1}", _dataPoints.Count, DateTimeOffset.FromUnixTimeMilliseconds(_dataPoints[0].Timestamp));
                        _publisher.Publish(_dataPoints);
                        _dataPoints.Clear();
                    }
                }
            }
        }
    }
}
