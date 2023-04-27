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

using System;
using System.Runtime.Serialization;

namespace NovAtelLogReader.DataPoints
{
    /*
     *
     * {
     *   "name": "NovAtelLogReader.DataPoints.DataPointPsrpos",
     *   "type": "record",
     *   "fields": [
     *     {
     *       "name": "Timestamp",
     *       "type": "long"
     *     },
     *     {
     *       "name": "Lat",
     *       "type": "double"
     *     },
     *     {
     *       "name": "Lon",
     *       "type": "double"
     *     },
     *     {
     *       "name": "Hgt",
     *       "type": "double"
     *     },
     *     {
     *       "name": "LatStdDev",
     *       "type": "double"
     *     },
     *     {
     *       "name": "LonStdDev",
     *       "type": "double"
     *     },
     *     {
     *       "name": "HgtStdDev",
     *       "type": "double"
     *     }
     *   ]
     * }
     *
     * */

    [DataContract]
    [Serializable]
    [DataPoint(Name = "PSRPOS", Queue = "datapoint-raw-psrpos")]
    public class DataPointPsrpos
    {
        [DataMember]
        public long Timestamp { get; set; }
        [DataMember]
        public double Lat { get; set; }
        [DataMember]
        public double Lon { get; set; }
        [DataMember]
        public double Hgt { get; set; }
        [DataMember]
        public double LatStdDev { get; set; }
        [DataMember]
        public double LonStdDev { get; set; }
        [DataMember]
        public double HgtStdDev { get; set; }
    }
}
