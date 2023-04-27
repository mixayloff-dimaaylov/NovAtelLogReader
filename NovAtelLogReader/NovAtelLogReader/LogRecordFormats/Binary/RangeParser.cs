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
    [Parser(Name = "RANGE", Id = 43, Fromat = ParserFromat.Binary)]
    class RangeParser : AbstractBinaryParser
    {
        public override void Parse(byte[] data, LogRecord record)
        {
            record.Header.Name = "RANGE";
            var nOfObservations = BitConverter.ToUInt32(data, HeaderLength);

            for (int idx = 0; idx < nOfObservations; idx++)
            {
                var offset = HeaderLength + idx * 44;
                var tracking = BitConverter.ToUInt32(data, offset + 44);

                var range = new LogDataRange()
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

                if (range.NavigationSystem == NavigationSystem.GLONASS)
                {
                    range.Prn = Util.GetActualPrn(range.Prn);
                    range.GloFreq = Util.GetActualGlonassFrequency(range.GloFreq);
                }

                record.Data.Add(range);
            }
        }
    }
}
