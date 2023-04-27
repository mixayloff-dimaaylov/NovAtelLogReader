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

namespace NovAtelLogReader.LogRecordFormats.Binary
{
    [Parser(Name = "ISMREDOBS", Id = 1393, Fromat = ParserFromat.Binary)]
    class IsmredobsParser : AbstractBinaryParser
    {
        public override void Parse(byte[] data, LogRecord record)
        {
            record.Header.Name = "ISMREDOBS";
            var nOfObservations = BitConverter.ToUInt32(data, HeaderLength);

            for (int idx = 0; idx < nOfObservations; idx++)
            {
                var offset = HeaderLength + idx * 56;
                var navigationSystem = (NavigationSystem)data[offset + 6];

                record.Data.Add(new LogDataIsmredobs()
                {
                    NavigationSystem = navigationSystem,
                    Prn = data[offset + 4],
                    GloFreq = (sbyte) data[offset + 5],
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
    }
}
