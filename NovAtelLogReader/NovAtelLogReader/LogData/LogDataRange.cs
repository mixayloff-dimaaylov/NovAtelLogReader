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

using System;

namespace NovAtelLogReader.LogData
{
    class LogDataRange : LogDataBase
    {
        public double Psr { get; set; }
        public double PsrStd { get; set; }
        public double Adr { get; set; }
        public double AdrStd { get; set; }
        public double CNo { get; set; }
        public double LockTime { get; set; }
        public UInt32 Tracking { get; set; }
    }
}
