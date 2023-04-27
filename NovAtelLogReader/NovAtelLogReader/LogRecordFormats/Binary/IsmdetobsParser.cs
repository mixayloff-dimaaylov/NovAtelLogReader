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

            var navigationSystem = (NavigationSystem)BitConverter.ToUInt32(data, HeaderLength);
            var nChans = BitConverter.ToUInt32(data, HeaderLength + 4);

            for (int idx = 0; idx < nChans; idx++)
            {
                var powers = new List<double>();
                var offset = HeaderLength + idx * 212;
                
                var basePower = BitConverter.ToUInt32(data, offset + 20);
                ulong bts = 0;

                powers.Add(basePower);

                for (int deltaIdx = 0; deltaIdx < 49; ++deltaIdx)
                {
                    bts = (BitConverter.ToUInt32(data, offset + 24 + 4 * deltaIdx) >> 20) & 0xfff;

                    if ((bts >> 11) != 0)
                    {
                        powers.Add(basePower * (double)((bts & 0x7ff) + 1) / 2048.0);
                    }
                    else
                    {
                        powers.Add(basePower * 2048.0 / ((bts & 0x7ff) + 1));
                    }
                }

                record.Data.Add(new LogDataIsmdetobs()
                {
                    Powers = powers,
                    Prn = data[offset + 8],
                    GloFreq = (sbyte) data[offset + 9],
                    NavigationSystem = navigationSystem,
                    SignalType = Util.GetSignalTypeIsm(navigationSystem, data[offset + 10])
                });
            }
        }
    }
}
