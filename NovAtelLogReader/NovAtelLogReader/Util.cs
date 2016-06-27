using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        /// <param name="tracking"></param>
        /// <returns></returns>
        public static SignalType GetSignalType(UInt32 tracking)
        {
            NavigationSystem system = GetNavigationSystem(tracking);
            uint code = tracking >> 21 & 0x1f;

            switch (system)
            {
                case NavigationSystem.GPS:
                    switch (code)
                    {
                        case GPS_SIGNAL_L1_CA:
                            return SignalType.L1;
                        case GPS_SIGNAL_L2_C:
                        case GPS_SIGNAL_L2_P:
                        case GPS_SIGNAL_L2_P_CODELESS:
                            return SignalType.L2;
                        case GPS_SIGNAL_L5_Q:
                            return SignalType.L5;
                        default:
                            return SignalType.Unknown;
                    }
                case NavigationSystem.GLONASS:
                    switch (code)
                    {
                        case GLONASS_SIGNAL_L1_CA:
                            return SignalType.L1;
                        case GLONASS_SIGNAL_L2_CA:
                        case GLONASS_SIGNAL_L2_P:
                            return SignalType.L2;
                        default:
                            return SignalType.Unknown;
                    }
                default:
                    return SignalType.Unknown;
            }
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
        public static uint GetActualGlonassFrequency(uint freq)
        {
            return (freq > 7) ? freq - 7 : freq;
        }

        public static long GpsToUtcTime(int gpsWeek, long ms)
        {
            DateTime datum = new DateTime(1980, 1, 6, 0, 0, 0);
            DateTime week = datum.AddDays(gpsWeek * 7);
            DateTime time = week.AddMilliseconds(ms);

            return new DateTimeOffset(time).ToUnixTimeMilliseconds();
        }
    }
}
