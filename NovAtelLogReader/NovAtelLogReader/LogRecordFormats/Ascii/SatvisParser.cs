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
    [Parser(Name = "SATVIS", Id = 48, Fromat = ParserFromat.Ascii)]
    class SatvisParser : AbstractAsciiParser
    {
        public override void Parse(string[] body, LogRecord record)
        {
            long nOfObservations = Int64.Parse(body[2]);

            long offset = 3;
            long rangeFields = 7;
            long maxIndex = rangeFields * nOfObservations + offset;

            while (offset < maxIndex)
            {
                var data = new LogDataSatvis()
                {
                    SatVis = String.Compare("TRUE", body[0], true) == 0,
                    Prn = uint.Parse(body[offset]),
                    GloFreq = int.Parse(body[offset + 1]),
                    Health = ulong.Parse(body[offset + 2]),
                    Elev = double.Parse(body[offset + 3], CultureInfo.InvariantCulture),
                    Az = double.Parse(body[offset + 4], CultureInfo.InvariantCulture)
                };

                data.NavigationSystem = Util.GetNavigationSystemByPrn(data.Prn);

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
