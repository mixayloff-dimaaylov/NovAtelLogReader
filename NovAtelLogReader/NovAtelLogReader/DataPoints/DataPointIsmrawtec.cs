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
     *   "name": "NovAtelLogReader.DataPoints.DataPointIsmrawtec",
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
     *       "name": "PrimarySignal",
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
     *       "name": "SecondarySignal",
     *       "type": "NovAtelLogReader.LogData.SignalType"
     *     },
     *     {
     *       "name": "Tec",
     *       "type": "double"
     *     }
     *   ]
     * }
     *
     * */

    [DataContract]
    [Serializable]
    [DataPoint(Name = "ISMRAWTEC", Queue = "datapoint-raw-ismrawtec")]
    public class DataPointIsmrawtec
    {
        [DataMember]
        public long Timestamp { get; set; }
        [DataMember]
        public NavigationSystem NavigationSystem { get; set; }
        [DataMember]
        public string Satellite { get; set; }
        [DataMember]
        public uint Prn { get; set; }
        [DataMember]
        public int GloFreq { get; set; }
        [DataMember]
        public SignalType PrimarySignal { get; set; }
        [DataMember]
        public SignalType SecondarySignal { get; set; }
        [DataMember]
        public double Tec { get; set; }
    }
}
