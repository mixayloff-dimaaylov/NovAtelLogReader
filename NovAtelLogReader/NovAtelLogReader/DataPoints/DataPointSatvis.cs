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
     *   "name": "NovAtelLogReader.DataPoints.DataPointSatvis",
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
     *       "name": "SatVis",
     *       "type": "boolean"
     *     },
     *     {
     *       "name": "Health",
     *       "type": "long"
     *     },
     *     {
     *       "name": "Elev",
     *       "type": "double"
     *     },
     *     {
     *       "name": "Az",
     *       "type": "double"
     *     }
     *   ]
     * }
     *
     * */

    [DataContract]
    [Serializable]
    [DataPoint(Name = "SATVIS", Queue = "datapoint-raw-satvis")]
    public class DataPointSatvis
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
        public bool SatVis { get; set; }
        [DataMember]
        public ulong Health { get; set; }
        [DataMember]
        public double Elev { get; set; }
        [DataMember]
        public double Az { get; set; }
    }
}
