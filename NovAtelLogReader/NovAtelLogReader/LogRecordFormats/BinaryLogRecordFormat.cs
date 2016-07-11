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
        private static readonly int HeaderLength = 28;

        public LogRecord Parse(byte[] data)
        {
            var messageId = BitConverter.ToUInt16(data, 4);
            switch (messageId)
            {
                case 43:
                    return ParseRange(data);
                default:
                    throw new InvalidOperationException("Unsupported log");
            }
        }

        private LogRecord ParseRange(byte[] data)
        {
            LogRecord logRecord = new LogRecord();
            logRecord.Header = new LogHeader();
            logRecord.Data = new List<LogDataBase>();
            logRecord.Header.Name = "RANGE";
            logRecord.Header.Timestamp = Util.GpsToUtcTime(BitConverter.ToUInt16(data, 14), BitConverter.ToUInt32(data, 16));

            var nOfObservations = BitConverter.ToUInt32(data, HeaderLength);

            for (int idx = 0; idx < nOfObservations; idx++)
            {
                var offset = HeaderLength + idx * 44;
                var tracking = BitConverter.ToUInt32(data, offset + 44);

                LogDataRange logDataRange = new LogDataRange()
                {
                    Prn = BitConverter.ToUInt32(data, offset + 4),
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

            return logRecord;
        }

        public LogRecord Parse(string data)
        {
            throw new NotImplementedException();
        }
    }
}
