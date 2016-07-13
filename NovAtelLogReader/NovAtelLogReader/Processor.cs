using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using NovAtelLogReader.DataPoints;
using NovAtelLogReader.LogData;
using NLog;

namespace NovAtelLogReader
{
    class Processor
    {
        private IReader _reader;
        private IPublisher _publisher;
        private ILogRecordFormat _logRecordFormat;
        private List<DataPointRange> _dataPointsRange;
        private List<DataPointSatvis> _dataPointsSatvis;
        private List<DataPointPsrpos> _dataPointsPsrpos;
        private List<DataPointIsmrawtec> _dataPointsIsmrawtec;
        private List<DataPointIsmredobs> _dataPointsIsmredobs;
        private Timer _timer;
        private readonly object _locker = new object();
        private Logger _logger = LogManager.GetCurrentClassLogger();

        public Processor(IReader reader, IPublisher publisher, ILogRecordFormat logRecordFormat)
        {
            _dataPointsRange = new List<DataPointRange>();
            _dataPointsSatvis = new List<DataPointSatvis>();
            _dataPointsPsrpos = new List<DataPointPsrpos>();
            _dataPointsIsmrawtec = new List<DataPointIsmrawtec>();
            _dataPointsIsmredobs = new List<DataPointIsmredobs>();
            _publisher = publisher;
            _reader = reader;
            _logRecordFormat = logRecordFormat;
            _timer = new Timer((s) => PublishDataPoints(), null, Timeout.InfiniteTimeSpan, Timeout.InfiniteTimeSpan);
        }

        public void Start()
        {
            _logger.Info("Запуск процессора сообщений.");
            _timer.Change(TimeSpan.Zero, TimeSpan.FromMilliseconds(Properties.Settings.Default.PublishRate));
            _publisher.Open();
            _reader.Open(_logRecordFormat);
            _reader.DataReceived += (s, e) => Task.Run(() => ProcessLogRecord(e.LogRecord));
        }

        public void Stop()
        {
            _logger.Info("Закрытие процессора сообщений.");
            _timer.Change(Timeout.InfiniteTimeSpan, Timeout.InfiniteTimeSpan);
            _timer.Dispose();
            _reader.Close();
            _publisher.Close();
        }

        private void ProcessLogRecord(LogRecord logRecord)
        {
            var pointsRange = logRecord.Data.Where(data => data is LogDataRange).Select(data =>
            {
                var range = data as LogDataRange;
                var dataPoint = new DataPointRange()
                {
                    Timestamp = logRecord.Header.Timestamp,
                    NavigationSystem = range.NavigationSystem,
                    SignalType = range.SignalType,
                    Prn = range.Prn,
                    GloFreq = range.GloFreq,
                    Adr = range.Adr,
                    Psr = range.Psr,
                    CNo = range.CNo
                };
                
                dataPoint.Satellite = String.Format("{0}{1}", dataPoint.NavigationSystem, dataPoint.Prn);
                return dataPoint;
            });
            lock (_locker)
            {
                _dataPointsRange.AddRange(pointsRange);
            }
            //------
            var pointsSatvis = logRecord.Data.Where(data => data is LogDataSatvis).Select(data =>
            {
                var satvis = data as LogDataSatvis;
                var dataPoint = new DataPointSatvis()
                {
                    Timestamp = logRecord.Header.Timestamp,
                    NavigationSystem = satvis.NavigationSystem,
                    Prn = satvis.Prn,
                    GloFreq = satvis.GloFreq,
                    SatVis = satvis.SatVis,
                    Health = satvis.Health,
                    Elev = satvis.Elev,
                    Az = satvis.Az
                };
                
                dataPoint.Satellite = String.Format("{0}{1}", dataPoint.NavigationSystem, dataPoint.Prn);
                return dataPoint;
            });
            lock (_locker)
            {
                _dataPointsSatvis.AddRange(pointsSatvis);
            }
            //------
            var pointsPsrpos = logRecord.Data.Where(data => data is LogDataPsrpos).Select(data =>
            {
                var psrpos = data as LogDataPsrpos;
                var dataPoint = new DataPointPsrpos()
                {
                    Timestamp = logRecord.Header.Timestamp,
                    Lat = psrpos.Lat,
                    Lon = psrpos.Lon,
                    Hgt = psrpos.Hgt,
                    LatStdDev = psrpos.LatStdDev,
                    LonStdDev = psrpos.LonStdDev,
                    HgtStdDev = psrpos.HgtStdDev
                };
                
                return dataPoint;
            });
            lock (_locker)
            {
                _dataPointsPsrpos.AddRange(pointsPsrpos);
            }
            //------
            var pointsIsmredobs = logRecord.Data.Where(data => data is LogDataIsmredobs).Select(data =>
            {
                var ismredobs = data as LogDataIsmredobs;
                var dataPoint = new DataPointIsmredobs()
                {
                    Timestamp = logRecord.Header.Timestamp,
                    NavigationSystem = ismredobs.NavigationSystem,
                    Prn = ismredobs.Prn,
                    GloFreq = ismredobs.GloFreq,
                    AverageCmc = ismredobs.AverageCmc,
                    SignalType = ismredobs.SignalType,
                    CmcStdDev = ismredobs.CmcStdDev,
                    TotalS4 = ismredobs.TotalS4,
                    CorrS4 = ismredobs.CorrS4,
                    PhaseSigma1Second = ismredobs.PhaseSigma1Second,
                    PhaseSigma30Second = ismredobs.PhaseSigma30Second,
                    PhaseSigma60Second = ismredobs.PhaseSigma60Second
                };

                dataPoint.Satellite = String.Format("{0}{1}", dataPoint.NavigationSystem, dataPoint.Prn);
                return dataPoint;
            });
            lock (_locker)
            {
                _dataPointsIsmredobs.AddRange(pointsIsmredobs);
            }
            //------
            var pointsIsmrawtec = logRecord.Data.Where(data => data is LogDataIsmrawtec).Select(data =>
            {
                var ismrawtec = data as LogDataIsmrawtec;
                var dataPoint = new DataPointIsmrawtec()
                {
                    Timestamp = logRecord.Header.Timestamp,
                    NavigationSystem = ismrawtec.NavigationSystem,
                    Prn = ismrawtec.Prn,
                    GloFreq = ismrawtec.GloFreq,
                    PrimarySignal = ismrawtec.PrimarySignal,
                    SecondarySignal = ismrawtec.SecondarySignal,
                    Tec = ismrawtec.Tec
                };

                dataPoint.Satellite = String.Format("{0}{1}", dataPoint.NavigationSystem, dataPoint.Prn);
                return dataPoint;
            });
            lock (_locker)
            {
                _dataPointsIsmrawtec.AddRange(pointsIsmrawtec);
            }
        }

        private void PublishDataPoints()
        {
            if (_dataPointsRange.Count > 0)
            {
                lock (_locker)
                {
                    if (_dataPointsRange.Count > 0)
                    {
                        _logger.Info("Отправка {0} точек по логу RANGE", _dataPointsRange.Count);
                        //Console.WriteLine("Publishing {0} points starting from {1}", _dataPointsRange.Count, DateTimeOffset.FromUnixTimeMilliseconds(_dataPointsRange[0].Timestamp));
                        _publisher.PublishRange(_dataPointsRange);
                        _dataPointsRange.Clear();
                    }
                }
            }
            if (_dataPointsSatvis.Count > 0)
            {
                lock (_locker)
                {
                    if (_dataPointsSatvis.Count > 0)
                    {
                        _logger.Info("Отправка {0} точек по логу SATVIS", _dataPointsSatvis.Count);
                        //Console.WriteLine("Publishing {0} points starting from {1}", _dataPointsRange.Count, DateTimeOffset.FromUnixTimeMilliseconds(_dataPointsRange[0].Timestamp));
                        _publisher.PublishSatvis(_dataPointsSatvis);
                        _dataPointsSatvis.Clear();
                    }
                }
            }
        }
    }
}
