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
using System.Runtime.InteropServices;

namespace NovAtelLogReader.PInvoke
{
    [StructLayout(LayoutKind.Sequential)]
    internal struct COMMPROP
    {
        internal UInt16 wPacketLength;
        internal UInt16 wPacketVersion;
        internal UInt32 dwServiceMask;
        internal UInt32 dwReserved1;
        internal UInt32 dwMaxTxQueue;
        internal UInt32 dwMaxRxQueue;
        internal UInt32 dwMaxBaud;
        internal UInt32 dwProvSubType;
        internal UInt32 dwProvCapabilities;
        internal UInt32 dwSettableParams;
        internal UInt32 dwSettableBaud;
        internal UInt16 wSettableData;
        internal UInt16 wSettableStopParity;
        internal UInt32 dwCurrentTxQueue;
        internal UInt32 dwCurrentRxQueue;
        internal UInt32 dwProvSpec1;
        internal UInt32 dwProvSpec2;
        internal Byte wcProvChar;
    }
}
