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
    [Parser(Name = "ISMRAWTEC", Id = 1390, Fromat = ParserFromat.Binary)]
    class IsmrawtecParser : AbstractBinaryParser
    {
        public override void Parse(byte[] data, LogRecord record)
        {
            record.Header.Name = "ISMRAWTEC";
            var nOfObservations = BitConverter.ToUInt32(data, HeaderLength);
            for (int idx = 0; idx < nOfObservations; idx++)
            {
                var offset = HeaderLength + idx * 16;
                var system = (NavigationSystem)data[offset + 6];

                record.Data.Add(new LogDataIsmrawtec()
                {
                    Prn = data[offset + 4],
                    GloFreq = (sbyte) data[offset + 5],
                    NavigationSystem = system,
                    PrimarySignal = Util.GetSignalTypeIsm(system, data[offset + 7]),
                    SecondarySignal = Util.GetSignalTypeIsm(system, data[offset + 8]),
                    Tec = BitConverter.ToSingle(data, offset + 12)
                });
            }
        }
    }
}
