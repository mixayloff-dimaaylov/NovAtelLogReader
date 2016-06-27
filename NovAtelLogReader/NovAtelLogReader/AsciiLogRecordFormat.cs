using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NovAtelLogReader
{
    class AsciiLogRecordFormat : ILogRecordFormat
    {
        public LogRecord Parse(byte[] data)

        {
            return Parse(Encoding.ASCII.GetString(data));
        }

        public LogRecord Parse(string data)
        {
            var parts = data.Split(new char[] { ';', '*' });

            if (parts.Length != 3 && !parts[0].StartsWith("#"))
            {
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

            if (logRecord.Header.Name == "RANGEA")
            {
                long nOfObservations = Int64.Parse(body[0]);
                
                long offset = 1;
                long rangeFields = 10;
                long maxIndex = rangeFields * nOfObservations + offset;

                while (offset < maxIndex)
                {
                    logRecord.Data.Add(new LogDataRange()
                    {
                        Prn = UInt32.Parse(body[offset]),
                        GloFreq = Int32.Parse(body[offset + 1]),
                        Psr = Double.Parse(body[offset + 2], CultureInfo.InvariantCulture),
                        PsrStd = Double.Parse(body[offset + 3], CultureInfo.InvariantCulture),
                        Adr = Double.Parse(body[offset + 4], CultureInfo.InvariantCulture),
                        AdrStd = Double.Parse(body[offset + 5], CultureInfo.InvariantCulture),
                        CNo = Double.Parse(body[offset + 7], CultureInfo.InvariantCulture),
                        LockTime = Double.Parse(body[offset + 8], CultureInfo.InvariantCulture),
                        Tracking = Convert.ToUInt32(body[offset + 9], 16)
                    });

                    offset += rangeFields;
                }
            }

            if(logRecord.Header.Name == "ISMREDOBSA")
            {
                long nOfObservations = Int64.Parse(body[0]);

                long offset = 1;
                long rangeFields = 17;
                long maxIndex = rangeFields * nOfObservations + offset;
                while (offset < maxIndex)
                {
                    logRecord.Data.Add(new LogDataIsmredobs()
                    {
                        Prn = UInt32.Parse(body[offset]),
                        GloFreq = Int32.Parse(body[offset + 1]),
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
      
            return logRecord;
        }
    }
}
