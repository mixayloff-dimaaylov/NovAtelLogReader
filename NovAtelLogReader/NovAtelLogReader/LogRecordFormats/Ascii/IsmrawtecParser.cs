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
    [Parser(Name = "ISMRAWTEC", Id = 1390, Fromat = ParserFromat.Ascii)]
    class IsmrawtecParser : AbstractAsciiParser
    {
        public override void Parse(string[] body, LogRecord record)
        {
            long nOfObservations = Int64.Parse(body[0]);

            long offset = 1;
            long rangeFields = 10;
            long maxIndex = rangeFields * nOfObservations + offset;

            while (offset < maxIndex)
            {
                var system = (NavigationSystem)UInt32.Parse(body[offset + 2]);

                record.Data.Add(new LogDataIsmrawtec()
                {
                    Prn = uint.Parse(body[offset]),
                    GloFreq = int.Parse(body[offset + 1]),
                    NavigationSystem = system,
                    PrimarySignal = Util.GetSignalTypeIsm(system, uint.Parse(body[offset + 3])),
                    SecondarySignal = Util.GetSignalTypeIsm(system, uint.Parse(body[offset + 4])),
                    Tec = Double.Parse(body[offset + 8], CultureInfo.InvariantCulture)
                });

                offset += rangeFields;
            }
        }
    }
}
