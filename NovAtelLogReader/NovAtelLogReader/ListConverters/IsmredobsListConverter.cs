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

using NovAtelLogReader.DataPoints;
using NovAtelLogReader.LogData;
using System;
using System.Collections.Generic;
using System.Linq;

namespace NovAtelLogReader.ListConverters
{
    [ListConverter(Name = "ISMREDOBS")]
    class IsmredobsListConverter : IListConverter
    {
        public IEnumerable<object> ToList(LogRecord record)
        {
            return record.Data.Where(data => data is LogDataIsmredobs).Select(data =>
            {
                var ismredobs = data as LogDataIsmredobs;
                return new DataPointIsmredobs()
                {
                    Timestamp = record.Header.Timestamp,
                    NavigationSystem = ismredobs.NavigationSystem,
                    Prn = ismredobs.Prn,
                    Satellite = String.Format("{0}{1}", ismredobs.NavigationSystem, ismredobs.Prn),
                    GloFreq = ismredobs.GloFreq,
                    AverageCmc = ismredobs.AverageCmc,
                    SignalType = ismredobs.SignalType,
                    CmcStdDev = ismredobs.CmcStdDev,
                    TotalS4 = ismredobs.TotalS4,
                    CorrS4 = ismredobs.CorrS4,
                    PhaseSigma1Second = ismredobs.PhaseSigma1Second,
                    PhaseSigma30Second = ismredobs.PhaseSigma30Second,
                    PhaseSigma60Second = ismredobs.PhaseSigma60Second
                };
            });
        }
    }
}
