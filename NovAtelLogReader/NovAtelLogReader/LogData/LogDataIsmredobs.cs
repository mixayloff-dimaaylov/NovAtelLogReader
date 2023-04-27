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

namespace NovAtelLogReader.LogData
{
    class LogDataIsmredobs : LogDataBase
    {
        public double AverageCmc { get; set; }
        public double CmcStdDev { get; set; }
        public double TotalS4 { get; set; }
        public double CorrS4 { get; set; }
        public double PhaseSigma1Second { get; set; }
        public double PhaseSigma30Second { get; set; }
        public double PhaseSigma60Second { get; set; }
    }
}

