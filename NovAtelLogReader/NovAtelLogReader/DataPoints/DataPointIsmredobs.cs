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
using System.Runtime.Serialization;

namespace NovAtelLogReader.DataPoints
{
    /*
     *
     * {
     *   "name": "NovAtelLogReader.DataPoints.DataPointIsmredobs",
     *   "type": "record",
     *   "fields": [
     *     {
     *       "name": "Timestamp",
     *       "type": "long"
     *     },
     *     {
     *       "name": "NavigationSystem",
     *       "type": {
     *         "name": "NovAtelLogReader.LogData.NavigationSystem",
     *         "type": "enum",
     *         "symbols": [
     *           "GPS",
     *           "GLONASS",
     *           "SBAS",
     *           "Galileo",
     *           "BeiDou",
     *           "QZSS",
     *           "Reserved",
     *           "Other"
     *         ]
     *       }
     *     },
     *     {
     *       "name": "SignalType",
     *       "type": {
     *         "name": "NovAtelLogReader.LogData.SignalType",
     *         "type": "enum",
     *         "symbols": [
     *           "Unknown",
     *           "L1CA",
     *           "L2C",
     *           "L2CA",
     *           "L2P",
     *           "L2P_codeless",
     *           "L2Y",
     *           "L5Q"
     *         ]
     *       }
     *     },
     *     {
     *       "name": "Satellite",
     *       "type": "string"
     *     },
     *     {
     *       "name": "Prn",
     *       "type": "int"
     *     },
     *     {
     *       "name": "GloFreq",
     *       "type": "int"
     *     },
     *     {
     *       "name": "AverageCmc",
     *       "type": "double"
     *     },
     *     {
     *       "name": "CmcStdDev",
     *       "type": "double"
     *     },
     *     {
     *       "name": "TotalS4",
     *       "type": "double"
     *     },
     *     {
     *       "name": "CorrS4",
     *       "type": "double"
     *     },
     *     {
     *       "name": "PhaseSigma1Second",
     *       "type": "double"
     *     },
     *     {
     *       "name": "PhaseSigma30Second",
     *       "type": "double"
     *     },
     *     {
     *       "name": "PhaseSigma60Second",
     *       "type": "double"
     *     }
     *   ]
     * }
     *
     * */

    [DataContract]
    [Serializable]
    [DataPoint(Name = "ISMREDOBS", Queue = "datapoint-raw-ismredobs")]
    public class DataPointIsmredobs
    {
        [DataMember]
        public long Timestamp { get; set; }
        [DataMember]
        public NavigationSystem NavigationSystem { get; set; }
        [DataMember]
        public SignalType SignalType { get; set; }
        [DataMember]
        public string Satellite { get; set; }
        [DataMember]
        public uint Prn { get; set; }
        [DataMember]
        public int GloFreq { get; set; }
        [DataMember]
        public double AverageCmc { get; set; }
        [DataMember]
        public double CmcStdDev { get; set; }
        [DataMember]
        public double TotalS4 { get; set; }
        [DataMember]
        public double CorrS4 { get; set; }
        [DataMember]
        public double PhaseSigma1Second { get; set; }
        [DataMember]
        public double PhaseSigma30Second { get; set; }
        [DataMember]
        public double PhaseSigma60Second { get; set; }
    }
}

