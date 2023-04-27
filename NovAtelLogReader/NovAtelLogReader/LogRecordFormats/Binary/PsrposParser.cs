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
    [Parser(Name = "PSRPOS", Id = 47, Fromat = ParserFromat.Binary)]
    class PsrposParser : AbstractBinaryParser
    {
        public override void Parse(byte[] data, LogRecord record)
        {
            record.Header.Name = "PSRPOS";
            record.Data.Add(new LogDataPsrpos()
            {
                Lat = BitConverter.ToDouble(data, HeaderLength + 8),
                Lon = BitConverter.ToDouble(data, HeaderLength + 16),
                Hgt = BitConverter.ToDouble(data, HeaderLength + 24),
                LatStdDev = BitConverter.ToSingle(data, HeaderLength + 40),
                LonStdDev = BitConverter.ToSingle(data, HeaderLength + 44),
                HgtStdDev = BitConverter.ToSingle(data, HeaderLength + 48)
            });
        }
    }
}
