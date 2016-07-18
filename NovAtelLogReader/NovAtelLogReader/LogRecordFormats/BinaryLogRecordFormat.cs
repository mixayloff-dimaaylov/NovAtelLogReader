using NLog;
using NovAtelLogReader.LogData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NovAtelLogReader.LogRecordFormats
{
    class BinaryLogRecordFormat : ILogRecordFormat
    {
        private Logger _logger = LogManager.GetCurrentClassLogger();
        private static readonly int HeaderLength = 28;

        public LogRecord Parse(byte[] data)
        {
            var messageId = BitConverter.ToUInt16(data, 4);
            LogRecord logRecord = new LogRecord();
            logRecord.Header = new LogHeader();
            logRecord.Data = new List<LogDataBase>();
            logRecord.Header.Timestamp = Util.GpsToUtcTime(BitConverter.ToUInt16(data, 14), BitConverter.ToUInt32(data, 16));

            _logger.Info("Новое сообщение {0} @ {1}", messageId, logRecord.Header.Timestamp);

            switch (messageId)
            {
                case 43:
                    _logger.Info("Обработка Range");
                    ParseRange(data, logRecord);
                    break;
                case 47:
                    _logger.Info("Обработка Psrpos");
                    ParsePsrpos(data, logRecord);
                    break;
                case 48:
                    _logger.Info("Обработка Satvis");
                    ParseSatvis(data, logRecord);
                    break;
                case 1393:
                    _logger.Info("Обработка Ismredobs");
                    ParseIsmredobs(data, logRecord);
                    break;
                case 1390:
                    _logger.Info("Обработка Ismrawtec");
                    ParseIsmrawtec(data, logRecord);
                    break;
                case 1395:
                    _logger.Info("Обработка Ismdetobs");
                    ParseIsmdetobs(data, logRecord);
                    break;
                case 1451:
                    _logger.Info("Обработка Satxyz2");
                    ParseSatxyz2(data, logRecord);
                    break;
                default:
                    throw new InvalidOperationException("Unsupported log" + messageId.ToString());
            }
            return logRecord;
        }

       

        private void ParseRange(byte[] data, LogRecord logRecord)
        {
            logRecord.Header.Name = "RANGE";
            var nOfObservations = BitConverter.ToUInt32(data, HeaderLength);

            for (int idx = 0; idx < nOfObservations; idx++)
            {
                var offset = HeaderLength + idx * 44;
                var tracking = BitConverter.ToUInt32(data, offset + 44);

                LogDataRange logDataRange = new LogDataRange()
                {
                    Prn = BitConverter.ToUInt16(data, offset + 4),
                    GloFreq = BitConverter.ToInt16(data, offset + 6),
                    Psr = BitConverter.ToDouble(data, offset + 8),
                    PsrStd = BitConverter.ToSingle(data, offset + 16),
                    Adr = BitConverter.ToDouble(data, offset + 20),
                    AdrStd = BitConverter.ToSingle(data, offset + 28),
                    CNo = BitConverter.ToSingle(data, offset + 36),
                    LockTime = BitConverter.ToSingle(data, offset + 40),
                    NavigationSystem = Util.GetNavigationSystem(tracking),
                    SignalType = Util.GetSignalType(tracking),
                    Tracking = tracking
                };

                if (logDataRange.NavigationSystem == NavigationSystem.GLONASS)
                {
                    logDataRange.Prn = Util.GetActualPrn(logDataRange.Prn);
                    logDataRange.GloFreq = Util.GetActualGlonassFrequency(logDataRange.GloFreq);
                }

                logRecord.Data.Add(logDataRange);
            }
        }

        private void ParseSatxyz2(byte[] data, LogRecord logRecord)
        {
            logRecord.Header.Name = "SATXYZ2";
            var nOfObservations = BitConverter.ToUInt32(data, HeaderLength);
            _logger.Info("Количество измерений Satxyz2: {0}", nOfObservations);

            for (int idx = 0; idx < nOfObservations; idx++)
            {
                var offset = HeaderLength + idx * 72;
                LogDataSatxyz2 logDataSatxyz2 = new LogDataSatxyz2()
                {
                    NavigationSystem = (NavigationSystem)BitConverter.ToUInt32(data, offset + 4),
                    Prn = BitConverter.ToUInt16(data, offset + 8),
                    X = BitConverter.ToDouble(data, offset + 12),
                    Y = BitConverter.ToDouble(data, offset + 20),
                    Z = BitConverter.ToDouble(data, offset + 28)
                };
                _logger.Info("Получена точка для лога Satxyz2 с системой {0}, Prn = {1}", logDataSatxyz2.NavigationSystem, logDataSatxyz2.Prn);
                logRecord.Data.Add(logDataSatxyz2);
            }
        }
        private void ParseSatvis(byte[] data, LogRecord logRecord)
        {
            logRecord.Header.Name = "SATVIS";
            var nOfObservations = BitConverter.ToUInt32(data, HeaderLength + 8);
            for(int idx = 0; idx < nOfObservations; idx++)
            {
                var offset = HeaderLength + idx * 40;
                LogDataSatvis logDataSatvis = new LogDataSatvis()
                {
                    SatVis = BitConverter.ToInt32(data, HeaderLength) == 1,
                    Prn = BitConverter.ToUInt16(data, offset + 12),
                    GloFreq = BitConverter.ToUInt16(data, offset + 14),
                    Health = BitConverter.ToUInt32(data, offset + 16),
                    Elev = BitConverter.ToDouble(data, offset + 20),
                    Az = BitConverter.ToDouble(data, offset + 28)
                };
                logDataSatvis.NavigationSystem = Util.GetNavigationSystemByPrn(logDataSatvis.Prn);

                if (logDataSatvis.NavigationSystem == NavigationSystem.GLONASS)
                {
                    logDataSatvis.Prn = Util.GetActualPrn(logDataSatvis.Prn);
                    logDataSatvis.GloFreq = Util.GetActualGlonassFrequency(logDataSatvis.GloFreq);
                }

                logRecord.Data.Add(logDataSatvis);
            }
        }

        private void ParsePsrpos(byte[] data, LogRecord logRecord)
        {
            logRecord.Header.Name = "PSRPOS";
            logRecord.Data.Add(new LogDataPsrpos()
            {
                Lat = BitConverter.ToDouble(data, HeaderLength + 8),
                Lon = BitConverter.ToDouble(data, HeaderLength + 16),
                Hgt = BitConverter.ToDouble(data, HeaderLength + 24),
                LatStdDev = BitConverter.ToSingle(data, HeaderLength + 40),
                LonStdDev = BitConverter.ToSingle(data, HeaderLength + 44),
                HgtStdDev = BitConverter.ToSingle(data, HeaderLength + 48)
            });
        }

        private void ParseIsmredobs(byte[] data, LogRecord logRecord)
        {
            logRecord.Header.Name = "ISMREDOBS";
            var nOfObservations = BitConverter.ToUInt32(data, HeaderLength);

            for (int idx = 0; idx < nOfObservations; idx++)
            {
                var offset = HeaderLength + idx * 56;
                var navigationSystem = (NavigationSystem)data[offset + 6];
                logRecord.Data.Add(new LogDataIsmredobs()
                {
                    NavigationSystem = navigationSystem,
                    Prn = data[offset + 4],
                    GloFreq = data[offset + 5],
                    SignalType = Util.GetSignalTypeIsm(navigationSystem, data[offset + 7]),
                    AverageCmc = BitConverter.ToSingle(data, offset + 24),
                    CmcStdDev = BitConverter.ToSingle(data, offset + 28),
                    TotalS4 = BitConverter.ToSingle(data, offset + 32),
                    CorrS4 = BitConverter.ToSingle(data, offset + 36),
                    PhaseSigma1Second = BitConverter.ToSingle(data, offset + 40),
                    PhaseSigma30Second = BitConverter.ToSingle(data, offset + 52),
                    PhaseSigma60Second = BitConverter.ToSingle(data, offset + 56)
                });
            }
        }

        private void ParseIsmrawtec(byte[] data, LogRecord logRecord)
        {
            logRecord.Header.Name = "ISMRAWTEC";
            var nOfObservations = BitConverter.ToUInt32(data, HeaderLength);
            for (int idx = 0; idx < nOfObservations; idx++)
            {
                var offset = HeaderLength + idx * 16;
                var system = (NavigationSystem)data[offset + 6];
                LogDataIsmrawtec logDataIsmrawtec = new LogDataIsmrawtec()
                {
                    Prn = data[offset + 4],
                    GloFreq = data[offset + 5],
                    NavigationSystem = system,
                    PrimarySignal = Util.GetSignalTypeIsm(system, data[offset + 7]),
                    SecondarySignal = Util.GetSignalTypeIsm(system, data[offset + 8]),
                    Tec = BitConverter.ToSingle(data, offset + 12)
                };
                logRecord.Data.Add(logDataIsmrawtec);
            }
        }

        private static void ParseIsmdetobs(byte[] data, LogRecord logRecord)
        {
            logRecord.Header.Name = "ISMDETOBS";
            var powers = new List<double>();
            var basePower = BitConverter.ToUInt32(data, HeaderLength + 20);
            ulong bts;

            powers.Add(basePower);
            for (int i = 0; i < 49; ++i)
            {
                bts = BitConverter.ToUInt32(data, HeaderLength + 24 + 4 * i) & 0xfff;
                if ((bts >> 11) != 0)
                {
                    powers.Add(basePower * ((bts & 0x7ff) + 1) / 2048);
                }
                else
                {
                    powers.Add(basePower * 2048 / ((bts & 0x7ff) + 1));
                }
            }

            var navigationSystem = (NavigationSystem)BitConverter.ToUInt32(data, HeaderLength);
            LogDataIsmdetobs logDataIsmdetobs = new LogDataIsmdetobs()
            {
                Powers = powers,
                Prn = data[HeaderLength + 8],
                GloFreq = data[HeaderLength + 9],
                NavigationSystem = navigationSystem,
                SignalType = Util.GetSignalTypeIsm(navigationSystem, data[HeaderLength + 10])
            };

            logRecord.Data.Add(logDataIsmdetobs);
        }

        public LogRecord Parse(string data)
        {
            throw new NotImplementedException();
        }
    }
}
