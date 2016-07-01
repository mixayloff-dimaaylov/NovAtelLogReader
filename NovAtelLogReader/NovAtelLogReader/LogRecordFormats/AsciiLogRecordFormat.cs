using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NovAtelLogReader.LogData;
using NLog;

namespace NovAtelLogReader.LogRecordFormats
{
    class AsciiLogRecordFormat : ILogRecordFormat
    {
        private Logger _logger = LogManager.GetCurrentClassLogger();
        public LogRecord Parse(byte[] data)
        {
            return Parse(Encoding.ASCII.GetString(data));
        }

        public LogRecord Parse(string data)
        {
            var parts = data.Split(new char[] { ';', '*' });

            if (parts.Length != 3 && !parts[0].StartsWith("#"))
            {
                _logger.Error("Неверный формат лога");
                throw new InvalidOperationException("Wrong log line format");
            }

            var header = parts[0].Split(',');
            var body = parts[1].Split(',');
            var checksum = parts[2];

            var logRecord = new LogRecord()
            {
                Header = new LogHeader(),
                Data = new List<LogDataBase>()
            };

            logRecord.Header.Name = header[0].Remove(0, 1);
            logRecord.Header.Port = header[1];
            logRecord.Header.Timestamp = Util.GpsToUtcTime(Int32.Parse(header[5]), Convert.ToInt64(Double.Parse(header[6], CultureInfo.InvariantCulture) * 1000));

            switch (logRecord.Header.Name)
            {
                case ("RANGEA"):
                    ParseRange(body, logRecord);
                    break;
                case ("ISMREDOBSA"):
                    ParseIsmredobs(body, logRecord);
                    break;
                case ("ISMDETOBSA"):
                    ParseIsmdetobs(body, logRecord);
                    break;
                case ("SATVISA"):
                    ParseSatvis(body, logRecord);
                    break;
                case ("ISMRAWTECA"):
                    ParseIsmrawtec(body, logRecord);
                    break;
                case ("PSRPOSA"):
                    ParsePsrpos(body, logRecord);
                    break;
                default:
                    break;
            }
            
            return logRecord;
        }

        private static void ParsePsrpos(string[] body, LogRecord logRecord)
        {
            logRecord.Data.Add(new LogDataPsrpos()
            {
                Lat = Double.Parse(body[2], CultureInfo.InvariantCulture),
                Lon = Double.Parse(body[3], CultureInfo.InvariantCulture),
                Hgt = Double.Parse(body[4], CultureInfo.InvariantCulture),
                LatStdDev = Double.Parse(body[7], CultureInfo.InvariantCulture),
                LonStdDev = Double.Parse(body[8], CultureInfo.InvariantCulture),
                HgtStdDev = Double.Parse(body[9], CultureInfo.InvariantCulture)
            });
        }

        private static void ParseIsmrawtec(string[] body, LogRecord logRecord)
        {
            long nOfObservations = Int64.Parse(body[0]);

            long offset = 1;
            long rangeFields = 10;
            long maxIndex = rangeFields * nOfObservations + offset;

            while (offset < maxIndex)
            {
                var system = (NavigationSystem)UInt32.Parse(body[offset + 2]);
                LogDataIsmrawtec logDataIsmrawtec = new LogDataIsmrawtec()
                {
                    Prn = uint.Parse(body[offset]),
                    GloFreq = int.Parse(body[offset + 1]),
                    NavigationSystem = system,
                    PrimarySignal = Util.GetSignalTypeIsm(system, uint.Parse(body[offset + 3])),
                    SecondarySignal = Util.GetSignalTypeIsm(system, uint.Parse(body[offset + 4])),
                    Tec = Double.Parse(body[offset + 8], CultureInfo.InvariantCulture)
                };
                
                logRecord.Data.Add(logDataIsmrawtec);
                offset += rangeFields;
            }
        }

        private static void ParseSatvis(string[] body, LogRecord logRecord)
        {
            long nOfObservations = Int64.Parse(body[1]);

            long offset = 2;
            long rangeFields = 7;
            long maxIndex = rangeFields * nOfObservations + offset;

            while (offset < maxIndex)
            {
                LogDataSatvis logDataSatvis = new LogDataSatvis()
                {
                    SatVis = String.Compare("TRUE", body[0], true) == 0,
                    Prn = uint.Parse(body[offset]),
                    GloFreq = int.Parse(body[offset + 1]),
                    Health = ulong.Parse(body[offset + 2]),
                    Elev = double.Parse(body[offset + 3], CultureInfo.InvariantCulture),
                    Az = double.Parse(body[offset + 4], CultureInfo.InvariantCulture)
                };

                logDataSatvis.NavigationSystem = Util.GetNavigationSystemByPrn(logDataSatvis.Prn);

                if(logDataSatvis.NavigationSystem == NavigationSystem.GLONASS)
                {
                    logDataSatvis.Prn = Util.GetActualPrn(logDataSatvis.Prn);
                    logDataSatvis.GloFreq = Util.GetActualGlonassFrequency(logDataSatvis.GloFreq);
                }

                logRecord.Data.Add(logDataSatvis);
                offset += rangeFields;
            }
        }

        private static void ParseIsmdetobs(string[] body, LogRecord logRecord)
        {
            var powers = new List<double>();
            var basePower = ulong.Parse(body[7]);
            ulong bts;

            powers.Add(basePower);
            for (int i = 8; i < 57; ++i)
            {
                bts = (Convert.ToUInt32(body[i], 16)) & 0xfff;
                if ((bts >> 11) != 0)
                {
                    powers.Add(basePower * ((bts & 0x7ff) + 1) / 2048);
                }
                else
                {
                    powers.Add(basePower * 2048 / ((bts & 0x7ff) + 1));
                }
            }

            var navigationSystem = (NavigationSystem)Enum.Parse(typeof(NavigationSystem), body[0]);
            LogDataIsmdetobs logDataIsmdetobs = new LogDataIsmdetobs()
            {
                Powers = powers,
                Prn = UInt32.Parse(body[2]),
                GloFreq = Int32.Parse(body[3]),
                NavigationSystem = navigationSystem,
                SignalType = Util.GetSignalTypeIsm(navigationSystem, UInt32.Parse(body[4]))
            };

            logRecord.Data.Add(logDataIsmdetobs);
        }

        private static void ParseIsmredobs(string[] body, LogRecord logRecord)
        {
            long nOfObservations = Int64.Parse(body[0]);

            long offset = 1;
            long rangeFields = 17;
            long maxIndex = rangeFields * nOfObservations + offset;

            while (offset < maxIndex)
            {
                var navigationSystem = (NavigationSystem)UInt32.Parse(body[offset + 2]);

                logRecord.Data.Add(new LogDataIsmredobs()
                {
                    NavigationSystem = navigationSystem,
                    Prn = UInt32.Parse(body[offset]),
                    GloFreq = Int32.Parse(body[offset + 1]),
                    SignalType = Util.GetSignalTypeIsm(navigationSystem, UInt32.Parse(body[offset + 3])),
                    AverageCmc = Double.Parse(body[offset + 8], CultureInfo.InvariantCulture),
                    CmcStdDev = Double.Parse(body[offset + 9], CultureInfo.InvariantCulture),
                    TotalS4 = Double.Parse(body[offset + 10], CultureInfo.InvariantCulture),
                    CorrS4 = Double.Parse(body[offset + 11], CultureInfo.InvariantCulture),
                    PhaseSigma1Second = Double.Parse(body[offset + 12], CultureInfo.InvariantCulture),
                    PhaseSigma30Second = Double.Parse(body[offset + 15], CultureInfo.InvariantCulture),
                    PhaseSigma60Second = Double.Parse(body[offset + 16], CultureInfo.InvariantCulture)
                });

                offset += rangeFields;
            }
        }

        private static void ParseRange(string[] body, LogRecord logRecord)
        {
            long nOfObservations = Int64.Parse(body[0]);

            long offset = 1;
            long rangeFields = 10;
            long maxIndex = rangeFields * nOfObservations + offset;

            while (offset < maxIndex)
            {
                var tracking = Convert.ToUInt32(body[offset + 9], 16);

                LogDataRange logDataRange = new LogDataRange()
                {
                    Prn = UInt32.Parse(body[offset]),
                    GloFreq = Int32.Parse(body[offset + 1]),
                    Psr = Double.Parse(body[offset + 2], CultureInfo.InvariantCulture),
                    PsrStd = Double.Parse(body[offset + 3], CultureInfo.InvariantCulture),
                    Adr = Double.Parse(body[offset + 4], CultureInfo.InvariantCulture),
                    AdrStd = Double.Parse(body[offset + 5], CultureInfo.InvariantCulture),
                    CNo = Double.Parse(body[offset + 7], CultureInfo.InvariantCulture),
                    LockTime = Double.Parse(body[offset + 8], CultureInfo.InvariantCulture),
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

                offset += rangeFields;
            }
        }
    }
}
