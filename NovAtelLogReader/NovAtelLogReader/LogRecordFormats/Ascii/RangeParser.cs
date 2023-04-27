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
using System.Globalization;

namespace NovAtelLogReader.LogRecordFormats.Ascii
{
    [Parser(Name = "RANGE", Id = 43, Fromat = ParserFromat.Ascii)]
    class RangeParser : AbstractAsciiParser
    {
        public override void Parse(string[] body, LogRecord record)
        {
            long nOfObservations = Int64.Parse(body[0]);

            long offset = 1;
            long rangeFields = 10;
            long maxIndex = rangeFields * nOfObservations + offset;

            while (offset < maxIndex)
            {
                var tracking = Convert.ToUInt32(body[offset + 9], 16);

                var data = new LogDataRange()
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

                if (data.NavigationSystem == NavigationSystem.GLONASS)
                {
                    data.Prn = Util.GetActualPrn(data.Prn);
                    data.GloFreq = Util.GetActualGlonassFrequency(data.GloFreq);
                }

                record.Data.Add(data);
                offset += rangeFields;
            }
        }
    }
}
