﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NovAtelLogReader
{
    interface ILogRecordFormat
    {
        LogRecord Parse(string data);
        LogRecord Parse(byte[] data);
    }
}
