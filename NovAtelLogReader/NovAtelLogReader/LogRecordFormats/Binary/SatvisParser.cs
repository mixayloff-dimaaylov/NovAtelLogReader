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
    [Parser(Name = "SATVIS", Id = 48, Fromat = ParserFromat.Binary)]
    class SatvisParser : AbstractBinaryParser
    {
        public override void Parse(byte[] data, LogRecord record)
        {
            record.Header.Name = "SATVIS";
            var nOfObservations = BitConverter.ToUInt32(data, HeaderLength + 8);
            for (int idx = 0; idx < nOfObservations; idx++)
            {
                var offset = HeaderLength + idx * 40;
                LogDataSatvis satvis = new LogDataSatvis()
                {
                    SatVis = BitConverter.ToInt32(data, HeaderLength) == 1,
                    Prn = BitConverter.ToUInt16(data, offset + 12),
                    GloFreq = BitConverter.ToUInt16(data, offset + 14),
                    Health = BitConverter.ToUInt32(data, offset + 16),
                    Elev = BitConverter.ToDouble(data, offset + 20),
                    Az = BitConverter.ToDouble(data, offset + 28)
                };
                satvis.NavigationSystem = Util.GetNavigationSystemByPrn(satvis.Prn);

                if (satvis.NavigationSystem == NavigationSystem.GLONASS)
                {
                    satvis.Prn = Util.GetActualPrn(satvis.Prn);
                    satvis.GloFreq = Util.GetActualGlonassFrequency(satvis.GloFreq);
                }

                record.Data.Add(satvis);
            }
        }
    }
}
