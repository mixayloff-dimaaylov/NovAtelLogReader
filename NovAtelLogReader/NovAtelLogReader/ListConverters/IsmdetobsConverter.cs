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
    [ListConverter(Name = "ISMDETOBS")]
    class IsmdetobsListConverter : IListConverter
    {
        private static IEnumerable<object> GetIsmdetDataPoints(LogHeader header, LogDataBase data)
        {
            var ismdet = data as LogDataIsmdetobs;
            var timestamp = header.Timestamp;

            foreach (var power in ismdet.Powers)
            {
                yield return new DataPointIsmdetobs()
                {
                    Timestamp = timestamp,
                    Satellite = String.Format("{0}{1}", ismdet.NavigationSystem, ismdet.Prn),
                    NavigationSystem = ismdet.NavigationSystem,
                    GloFreq = ismdet.GloFreq,
                    Prn = ismdet.Prn,
                    SignalType = ismdet.SignalType,
                    Power = power
                };

                timestamp += 20;
            }
        }

        public IEnumerable<object> ToList(LogRecord record)
        {
            return record.Data.Where(data => data is LogDataIsmdetobs).SelectMany(data => GetIsmdetDataPoints(record.Header, data));
        }
    }
}
