﻿/*
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
    [Parser(Name = "PSRPOS", Id = 47, Fromat = ParserFromat.Ascii)]
    class PsrposParser : AbstractAsciiParser
    {
        public override void Parse(string[] body, LogRecord record)
        {
            record.Data.Add(new LogDataPsrpos()
            {
                Lat = Double.Parse(body[2], CultureInfo.InvariantCulture),
                Lon = Double.Parse(body[3], CultureInfo.InvariantCulture),
                Hgt = Double.Parse(body[4], CultureInfo.InvariantCulture),
                LatStdDev = Double.Parse(body[7], CultureInfo.InvariantCulture),
                LonStdDev = Double.Parse(body[8], CultureInfo.InvariantCulture),
                HgtStdDev = Double.Parse(body[9], CultureInfo.InvariantCulture)
            });
        }
    }
}
