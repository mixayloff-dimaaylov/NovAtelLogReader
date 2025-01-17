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
using System.Collections.Generic;

namespace NovAtelLogReader.LogRecordFormats.Ascii
{
    [Parser(Name = "ISMDETOBS", Id = 1395, Fromat = ParserFromat.Ascii)]
    class IsmdetobsParser : AbstractAsciiParser
    {
        public override void Parse(string[] body, LogRecord record)
        {
            var powers = new List<double>();
            var basePower = ulong.Parse(body[7]);
            ulong bts;

            powers.Add(basePower);
            for (int i = 8; i < 57; ++i)
            {
                bts = (Convert.ToUInt32(body[i], 16)) & 0xfff;
                if ((bts >> 11) != 0)
                {
                    powers.Add(basePower * ((bts & 0x7ff) + 1) / 2048);
                }
                else
                {
                    powers.Add(basePower * 2048 / ((bts & 0x7ff) + 1));
                }
            }

            var navigationSystem = (NavigationSystem)Enum.Parse(typeof(NavigationSystem), body[0]);
            var data = new LogDataIsmdetobs()
            {
                Powers = powers,
                Prn = UInt32.Parse(body[2]),
                GloFreq = Int32.Parse(body[3]),
                NavigationSystem = navigationSystem,
                SignalType = Util.GetSignalTypeIsm(navigationSystem, UInt32.Parse(body[4]))
            };

            record.Data.Add(data);
        }
    }
}
