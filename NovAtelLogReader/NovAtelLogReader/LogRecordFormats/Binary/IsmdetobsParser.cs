using NovAtelLogReader.LogData;
using System;
using System.Collections.Generic;

namespace NovAtelLogReader.LogRecordFormats.Binary
{
    [Parser(Name = "ISMDETOBS", Id = 1395, Fromat = ParserFromat.Binary)]
    class IsmdetobsParser : AbstractBinaryParser
    {
        public override void Parse(byte[] data, LogRecord record)
        {
            record.Header.Name = "ISMDETOBS";
            var powers = new List<double>();
            var basePower = BitConverter.ToUInt32(data, HeaderLength + 20);
            ulong bts;

            powers.Add(basePower);
            for (int i = 0; i < 49; ++i)
            {
                bts = (BitConverter.ToUInt32(data, HeaderLength + 24 + 4 * i) >> 20) & 0xfff;
                if ((bts >> 11) != 0)
                {
                    powers.Add((double)basePower * (double)((bts & 0x7ff) + 1) / 2048.0);
                }
                else
                {
                    powers.Add((double)basePower * 2048.0 / (double)((bts & 0x7ff) + 1));
                }
            }

            var navigationSystem = (NavigationSystem)BitConverter.ToUInt32(data, HeaderLength);
            record.Data.Add(new LogDataIsmdetobs()
            {
                Powers = powers,
                Prn = data[HeaderLength + 8],
                GloFreq = data[HeaderLength + 9],
                NavigationSystem = navigationSystem,
                SignalType = Util.GetSignalTypeIsm(navigationSystem, data[HeaderLength + 10])
            });
        }
    }
}
