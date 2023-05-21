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
using System.Collections.Generic;
using System.Reflection;

namespace NovAtelLogReader
{
    static class Util
    {
        // Коды типов сигналов GSM
        // OEM6 Firmware Reference Manual Rev 9, Table 130 (p.579)
        private const uint GPS_SIGNAL_L1_CA = 0;
        private const uint GPS_SIGNAL_L2_P = 5;
        private const uint GPS_SIGNAL_L2_P_CODELESS = 9;
        private const uint GPS_SIGNAL_L5_Q = 14;
        private const uint GPS_SIGNAL_L2_C = 17;

        // Коды типов сигналов GLONASS
        // OEM6 Firmware Reference Manual Rev 9, Table 130 (p.579)
        private const uint GLONASS_SIGNAL_L1_CA = 0;
        private const uint GLONASS_SIGNAL_L2_CA = 1;
        private const uint GLONASS_SIGNAL_L2_P = 5;

        // Коды типов сигналов GPS для логов ISM*
        // GPStation-6 User Manual Rev 2, Table 18 (p.61)
        private const uint ISM_GPS_SIGNAL_L1_CA = 1;
        private const uint ISM_GPS_SIGNAL_L2_Y = 4;
        private const uint ISM_GPS_SIGNAL_L2_C = 5;
        private const uint ISM_GPS_SIGNAL_L2_P = 6;
        private const uint ISM_GPS_SIGNAL_L5_Q = 7;

        // Коды типов сигналов GLONASS для логов ISM*
        // GPStation-6 User Manual Rev 2, Table 18 (p.61)
        private const uint ISM_GLONASS_SIGNAL_L1_CA = 1;
        private const uint ISM_GLONASS_SIGNAL_L2_CA = 3;
        private const uint ISM_GLONASS_SIGNAL_L2_P = 4;

        public static IEnumerable<Type> GetTypesWithAttribute<Attribute>()
        {
            foreach (Type type in Assembly.GetExecutingAssembly().GetTypes())
            {
                if (type.GetCustomAttributes(typeof(Attribute), true).Length > 0)
                {
                    yield return type;
                }
            }
        }

        /// <summary>
        /// Возвращает тип навигационной системы
        /// </summary>
        /// <param name="tracking"></param>
        /// <returns></returns>
        public static NavigationSystem GetNavigationSystem(UInt32 tracking)
        {
            uint system = (tracking >> 16) & 0x7;
            return (NavigationSystem)system;
        }

        /// <summary>
        /// Возвращает тип сигнала навигационной системы
        /// </summary>
        /// <param name="system">Навигационная система</param>
        /// <param name="code">Код сигнала</param>
        /// <returns>Тип сигнала</returns>
        public static SignalType GetSignalType(NavigationSystem system, uint code)
        {
            switch (system)
            {
                case NavigationSystem.GPS:
                    switch (code)
                    {
                        case GPS_SIGNAL_L1_CA:
                            return SignalType.L1CA;
                        case GPS_SIGNAL_L2_C:
                            return SignalType.L2C;
                        case GPS_SIGNAL_L2_P:
                            return SignalType.L2P;
                        case GPS_SIGNAL_L2_P_CODELESS:
                            return SignalType.L2P_codeless;
                        case GPS_SIGNAL_L5_Q:
                            return SignalType.L5Q;
                        default:
                            return SignalType.Unknown;
                    }
                case NavigationSystem.GLONASS:
                    switch (code)
                    {
                        case GLONASS_SIGNAL_L1_CA:
                            return SignalType.L1CA;
                        case GLONASS_SIGNAL_L2_CA:
                            return SignalType.L2CA;
                        case GLONASS_SIGNAL_L2_P:
                            return SignalType.L2P;
                        default:
                            return SignalType.Unknown;
                    }
                default:
                    return SignalType.Unknown;
            }
        }

        /// <summary>
        /// Возвращает тип сигнала навигационной системы для логов ISM*
        /// </summary>
        /// <param name="system">Навигационная система</param>
        /// <param name="code">Код сигнала</param>
        /// <returns>Тип сигнала</returns>
        public static SignalType GetSignalTypeIsm(NavigationSystem system, uint code)
        {
            switch (system)
            {
                case NavigationSystem.GPS:
                    switch (code)
                    {
                        case ISM_GPS_SIGNAL_L1_CA:
                            return SignalType.L1CA;
                        case ISM_GPS_SIGNAL_L2_Y:
                            return SignalType.L2Y;
                        case ISM_GPS_SIGNAL_L2_C:
                            return SignalType.L2C;
                        case ISM_GPS_SIGNAL_L2_P:
                            return SignalType.L2P;
                        case ISM_GPS_SIGNAL_L5_Q:
                            return SignalType.L5Q;
                        default:
                            return SignalType.Unknown;
                    }
                case NavigationSystem.GLONASS:
                    switch (code)
                    {
                        case ISM_GLONASS_SIGNAL_L1_CA:
                            return SignalType.L1CA;
                        case ISM_GLONASS_SIGNAL_L2_CA:
                            return SignalType.L2CA;
                        case ISM_GLONASS_SIGNAL_L2_P:
                            return SignalType.L2P;
                        default:
                            return SignalType.Unknown;
                    }
                default:
                    return SignalType.Unknown;
            }
        }

        /// <summary>
        /// Возвращает тип сигнала навигационной системы
        /// </summary>
        /// <param name="tracking"></param>
        /// <returns></returns>
        public static SignalType GetSignalType(UInt32 tracking)
        {
            NavigationSystem system = GetNavigationSystem(tracking);
            uint code = tracking >> 21 & 0x1f;
            return GetSignalType(system, code);
        }

        /// <summary>
        /// Возвращает номер слота с учетом сдвига для спутников GLONASS
        /// OEM6 Firmware Reference Manual Rev 9, GLONASS Slot and Frequency Numbers (p.31)
        /// </summary>
        /// <param name="prn">Исходный номер слота</param>
        /// <returns>Скорректированный номер слота</returns>
        public static uint GetActualPrn(uint prn)
        {
            return (prn >= 38 && prn <= 61) ? prn - 37 : prn;
        }

        /// <summary>
        /// Возвращает номер частоты GLONASS с учетом сдвига
        /// OEM6 Firmware Reference Manual Rev 9, GLONASS Slot and Frequency Numbers (p.31)
        /// </summary>
        /// <param name="freq">Исходный номер частоты</param>
        /// <returns>Скорректированный номер частоты</returns>
        public static int GetActualGlonassFrequency(int freq)
        {
            return freq - 7;
        }

        public static NavigationSystem GetNavigationSystemByPrn(uint prn)
        {
            if (1 <= prn && prn <= 32)
            {
                return NavigationSystem.GPS;
            }

            if (38 <= prn && prn <= 61)
            {
                return NavigationSystem.GLONASS;
            }

            return NavigationSystem.Other;
        }

        /// <summary>
        /// Преобразование времени GPS в Unix Timestamp
        /// </summary>
        /// <param name="gpsWeek">Неделя GPS</param>
        /// <param name="ms">GPS Timestamp</param>
        /// <returns>Unix Timestamp</returns>
        public static long GpsToUtcTime(int gpsWeek, long ms)
        {
            DateTime datum = new DateTime(1980, 1, 6, 0, 0, 0, DateTimeKind.Utc);
            DateTime week = datum.AddDays(gpsWeek * 7);
            DateTime time = week.AddMilliseconds(ms);

            return new DateTimeOffset(time).ToLocalTime().ToUnixTimeMilliseconds();
        }

        #region CRC 32
        private const ulong CRC32_POLYNOMIAL = 0xEDB88320L;

        private static ulong CRC32Value(ulong ulCRC)
        {
            int j;

            for (j = 8; j > 0; j--)
            {
                if ((ulCRC & 1) != 0)
                    ulCRC = (ulCRC >> 1) ^ CRC32_POLYNOMIAL;
                else
                    ulCRC >>= 1;
            }

            return ulCRC;
        }

        /// <summary>
        /// Алгоритм вычисления контрольной суммы бинарного пакета
        /// OEM6 Firmware Reference Manual Rev 9, 32-Bit CRC (p.34)
        /// </summary>
        /// <param name="ucBuffer"></param>
        /// <returns></returns>
        public static ulong CalculateBlockCRC32(byte[] ucBuffer)
        {
            ulong ulTemp1;
            ulong ulTemp2;
            ulong ulCRC = 0;
            long idx = 0;

            while (idx < ucBuffer.LongLength)
            {
                ulTemp1 = (ulCRC >> 8) & 0x00FFFFFFL;
                ulTemp2 = CRC32Value((ulCRC ^ ucBuffer[idx++]) & 0xff);
                ulCRC = ulTemp1 ^ ulTemp2;
            }

            return ulCRC;
        }
        #endregion
    }

    // The function is an extension method, so it must be defined in a static class.
    public static class ResampleExt
    {
        // Resample an input time series and create a new time series between two
        // particular dates sampled at a specified time interval.
        public static IEnumerable<OutputDataT> Resample<InputValueT, OutputDataT>(

            // Input time series to be resampled.
            this IEnumerable<InputValueT> source,

            // Start date of the new time series.
            long startDate,

            // Date at which the new time series will have ended.
            long endDate,

            // The time interval between samples.
            long resampleInterval,

            // Function that selects a date/time value from an input data point.
            Func<InputValueT, long> dateSelector,

            // Interpolation function that produces a new interpolated data point
            // at a particular time between two input data points.
            Func<long, InputValueT, InputValueT, double, OutputDataT> interpolator
        )
        {
            // ... argument checking omitted ...

            //
            // Manually enumerate the input time series...
            // This is manual because the first data point must be treated specially.
            //
            var e = source.GetEnumerator();
            if (e.MoveNext())
            {
                // Initialize working date to the start date, this variable will be used to
                // walk forward in time towards the end date.
                var workingDate = startDate;

                // Extract the first data point from the input time series.
                var firstDataPoint = e.Current;

                // Extract the first data point's date using the date selector.
                var firstDate = dateSelector(firstDataPoint);

                // Loop forward in time until we reach either the date of the first
                // data point or the end date, which ever comes first.
                while (workingDate < endDate && workingDate <= firstDate)
                {
                    // LogManager.GetCurrentClassLogger().Info("BP1");
                    // Until we reach the date of the first data point,
                    // use the interpolation function to generate an output
                    // data point from the first data point.
                    yield return interpolator(workingDate, firstDataPoint, firstDataPoint, 0);

                    // Walk forward in time by the specified time period.
                    workingDate += resampleInterval;
                }

                //
                // Setup current data point... we will now loop over input data points and
                // interpolate between the current and next data points.
                //
                var curDataPoint = firstDataPoint;
                var curDate = firstDate;

                //
                // After we have reached the first data point, loop over remaining input data points until
                // either the input data points have been exhausted or we have reached the end date.
                //
                while (workingDate < endDate && e.MoveNext())
                {
                    // Extract the next data point from the input time series.
                    var nextDataPoint = e.Current;

                    // Extract the next data point's date using the data selector.
                    var nextDate = dateSelector(nextDataPoint);

                    // Calculate the time span between the dates of the current and next data points.
                    var timeSpan = nextDate - firstDate;

                    // Loop forward in time until wwe have moved beyond the date of the next data point.
                    while (workingDate <= endDate && workingDate < nextDate)
                    {
                        // The time span from the current date to the working date.
                        // LogManager.GetCurrentClassLogger().Info("BP2 {0} {1} {2}", workingDate, curDate, timeSpan);
                        var curTimeSpan = workingDate - curDate;

                        // The time between the dates as a percentage (a 0-1 value).
                        var timePct = ((double) curTimeSpan) / ((double) timeSpan);

                        // Interpolate an output data point at the particular time between
                        // the current and next data points.
                        yield return interpolator(workingDate, curDataPoint, nextDataPoint, timePct);

                        // Walk forward in time by the specified time period.
                        workingDate += resampleInterval;
                    }

                    // Swap the next data point into the current data point so we can move on and continue
                    // the interpolation with each subsqeuent data point assuming the role of
                    // 'next data point' in the next iteration of this loop.
                    curDataPoint = nextDataPoint;
                    curDate = nextDate;
                }

                // Finally loop forward in time until we reach the end date.
                while (workingDate < endDate)
                {
                    // LogManager.GetCurrentClassLogger().Info("BP3");
                    // Interpolate an output data point generated from the last data point.
                    yield return interpolator(workingDate, curDataPoint, curDataPoint, 1);

                    // Walk forward in time by the specified time period.
                    workingDate += resampleInterval;
                }
            }
        }

        // The linear interpolation is defined as follows.
        public static double Lerp(double v1, double v2, double t)
        {
            return v1 + ((v2 - v1) * t);
        }
    }
}
