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
    [ListConverter(Name = "SATVIS")]
    class SatvisConverter : IListConverter
    {
        public IEnumerable<object> ToList(LogRecord record)
        {
            return record.Data.Where(data => data is LogDataSatvis).Select(data =>
            {
                var satvis = data as LogDataSatvis;
                return new DataPointSatvis()
                {
                    Timestamp = record.Header.Timestamp,
                    NavigationSystem = satvis.NavigationSystem,
                    Prn = satvis.Prn,
                    Satellite = String.Format("{0}{1}", satvis.NavigationSystem, satvis.Prn),
                    GloFreq = satvis.GloFreq,
                    SatVis = satvis.SatVis,
                    Health = satvis.Health,
                    Elev = satvis.Elev,
                    Az = satvis.Az
                };
            });
        }
    }
}
