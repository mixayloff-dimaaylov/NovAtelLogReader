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
        private List<DataPointIsmdetobs> _dataPointsIsmdetobs;
        private List<DataPointSatxyz2> _dataPointsSatxyz2;
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
            _dataPointsIsmdetobs = new List<DataPointIsmdetobs>();
            _dataPointsSatxyz2 = new List<DataPointSatxyz2>();
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

        private IEnumerable<DataPointIsmdetobs> GetIsmdetDataPoints(LogHeader logHeader, LogDataBase data)
        {
            var ismdet = data as LogDataIsmdetobs;
            var timestamp = logHeader.Timestamp;

            foreach (var power in ismdet.Powers)
            {
                yield return new DataPointIsmdetobs()
                {
                    Timestamp = timestamp,
                    Satellite = String.Format("{0}{1}", ismdet.NavigationSystem, ismdet.Prn),
                    NavigationSystem = ismdet.NavigationSystem,
                    GloFreq = ismdet.GloFreq,
                    Prn = ismdet.Prn,
                    SignalType = ismdet.SignalType,
                    Power = power
                };

                timestamp += 50;
            }
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
               // _publisher.PublishRange(pointsRange.ToList());
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
                //_publisher.PublishSatvis(pointsSatvis.ToList());
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
                // _publisher.PublishPsrpos(pointsPsrpos.ToList());
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

            // -----

            var pointIsmdetobs = logRecord.Data.Where(data => data is LogDataIsmdetobs).SelectMany(data => GetIsmdetDataPoints(logRecord.Header, data));

            lock (_locker)
            {
                _dataPointsIsmdetobs.AddRange(pointIsmdetobs);
            }

            //------
            var pointsSatxyz2 = logRecord.Data.Where(data => data is LogDataSatxyz2).Select(data =>
            {
                var satxyz2 = data as LogDataSatxyz2;
                var dataPoint = new DataPointSatxyz2()
                {
                    Timestamp = logRecord.Header.Timestamp,
                    NavigationSystem = satxyz2.NavigationSystem,
                    Prn = satxyz2.Prn,
                    X = satxyz2.X,
                    Y = satxyz2.Y,
                    Z = satxyz2.Z
                };
                dataPoint.Satellite = String.Format("{0}{1}", dataPoint.NavigationSystem, dataPoint.Prn);
                return dataPoint;
            });

            lock (_locker)
            {
                // _publisher.PublishSatxyz2(pointsSatxyz2.ToList());
                _dataPointsSatxyz2.AddRange(pointsSatxyz2);
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
                        _publisher.PublishSatvis(_dataPointsSatvis);
                        _dataPointsSatvis.Clear();
                    }
                }
            }
            if (_dataPointsPsrpos.Count > 0)
            {
                lock (_locker)
                {
                    if (_dataPointsPsrpos.Count > 0)
                    {
                        _logger.Info("Отправка {0} точек по логу PSRPOS", _dataPointsPsrpos.Count);
                        _publisher.PublishPsrpos(_dataPointsPsrpos);
                        _dataPointsPsrpos.Clear();
                    }
                }
            }
            if(_dataPointsSatxyz2.Count > 0)
            {
                lock(_locker)
                {
                    if(_dataPointsSatxyz2.Count > 0)
                    {
                        _logger.Info("Отправка {0} точек по логу SATXYZ2", _dataPointsSatxyz2.Count);
                        _publisher.PublishSatxyz2(_dataPointsSatxyz2);
                        _dataPointsSatxyz2.Clear();
                    }
                }
            }
            if (_dataPointsIsmredobs.Count > 0)
            {
                lock (_locker)
                {
                    if (_dataPointsIsmredobs.Count > 0)
                    {
                        _logger.Info("Отправка {0} точек по логу ISMREDOBS", _dataPointsIsmredobs.Count);
                        _publisher.PublishIsmredobs(_dataPointsIsmredobs);
                        _dataPointsIsmredobs.Clear();
                    }
                }
            }
            if (_dataPointsIsmrawtec.Count > 0)
            {
                lock (_locker)
                {
                    if (_dataPointsIsmrawtec.Count > 0)
                    {
                        _logger.Info("Отправка {0} точек по логу ISMRAWTEC", _dataPointsIsmrawtec.Count);
                        _publisher.PublishIsmrawtec(_dataPointsIsmrawtec);
                        _dataPointsIsmrawtec.Clear();
                    }
                }
            }
            if (_dataPointsIsmdetobs.Count > 0)
            {
                lock (_locker)
                {
                    if (_dataPointsIsmdetobs.Count > 0)
                    {
                        _logger.Info("Отправка {0} точек по логу ISMDETOBS", _dataPointsIsmdetobs.Count);
                        _publisher.PublishIsmdetobs(_dataPointsIsmdetobs);
                        _dataPointsIsmdetobs.Clear();
                    }
                }
            }
        }
    }
}
