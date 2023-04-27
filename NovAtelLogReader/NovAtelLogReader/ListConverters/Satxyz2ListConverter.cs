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
    [ListConverter(Name = "SATXYZ2")]
    class Satxyz2ListConverter : IListConverter
    {
        public IEnumerable<object> ToList(LogRecord record)
        {
            return record.Data.Where(data => data is LogDataSatxyz2).Select(data =>
            {
                var satxyz2 = data as LogDataSatxyz2;
                return new DataPointSatxyz2()
                {
                    Timestamp = record.Header.Timestamp,
                    NavigationSystem = satxyz2.NavigationSystem,
                    Prn = satxyz2.Prn,
                    Satellite = String.Format("{0}{1}", satxyz2.NavigationSystem, satxyz2.Prn),
                    X = satxyz2.X,
                    Y = satxyz2.Y,
                    Z = satxyz2.Z
                };
            });
        }
    }
}
