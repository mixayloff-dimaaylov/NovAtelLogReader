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
    [ListConverter(Name = "PSRPOS")]
    class PsrposConverter : IListConverter
    {
        public IEnumerable<object> ToList(LogRecord record)
        {
            return record.Data.Where(data => data is LogDataPsrpos).Select(data =>
            {
                var psrpos = data as LogDataPsrpos;
                var dataPoint = new DataPointPsrpos()
                {
                    Timestamp = record.Header.Timestamp,
                    Lat = psrpos.Lat,
                    Lon = psrpos.Lon,
                    Hgt = psrpos.Hgt,
                    LatStdDev = psrpos.LatStdDev,
                    LonStdDev = psrpos.LonStdDev,
                    HgtStdDev = psrpos.HgtStdDev
                };

                return dataPoint;
            });
        }
    }
}
