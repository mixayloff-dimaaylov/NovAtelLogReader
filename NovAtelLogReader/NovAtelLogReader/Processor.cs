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
                LogDataRange range = data as LogDataRange;
                return new DataPoint()
                {
                    Timestamp = logRecord.Header.Timestamp,
                    NavigationSystem = Util.GetNavigationSystem(range.Tracking),
                    SignalType = Util.GetSignalType(range.Tracking),
                    Prn = Util.GetActualPrn(range.Prn),
                    GloFreq = Util.GetActualGlonassFrequency(range.GloFreq),
                    Adr = range.Adr,
                    Psr = range.Psr,
                    CNo = range.CNo
                };
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
                    Console.WriteLine("Puplishing {0} points starting from {1}", _dataPoints.Count, DateTimeOffset.FromUnixTimeMilliseconds(_dataPoints[0].Timestamp));
                    _publisher.Publish(_dataPoints);
                    _dataPoints.Clear();
                }
            }
        }
    }
}
