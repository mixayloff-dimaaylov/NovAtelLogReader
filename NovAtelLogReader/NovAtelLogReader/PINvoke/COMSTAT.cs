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
    internal struct COMSTAT
    {
        internal const uint fCtsHold = 0x1;
        internal const uint fDsrHold = 0x2;
        internal const uint fRlsdHold = 0x4;
        internal const uint fXoffHold = 0x8;
        internal const uint fXoffSent = 0x10;
        internal const uint fEof = 0x20;
        internal const uint fTxim = 0x40;
        internal UInt32 Flags;
        internal UInt32 cbInQue;
        internal UInt32 cbOutQue;
    }
}
