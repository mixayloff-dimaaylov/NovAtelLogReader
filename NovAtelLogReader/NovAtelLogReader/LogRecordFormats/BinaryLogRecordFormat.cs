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

using NLog;
using NovAtelLogReader.LogData;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace NovAtelLogReader.LogRecordFormats
{
    class BinaryLogRecordFormat : ILogRecordFormat
    {
        private Logger _logger = LogManager.GetCurrentClassLogger();
        private Dictionary<int, IParser> _parsers = new Dictionary<int, IParser>();

        public BinaryLogRecordFormat()
        {
            foreach (var type in Util.GetTypesWithAttribute<ParserAttribute>())
            {
                var attr = type.GetCustomAttribute<ParserAttribute>();

                if (attr.Fromat == ParserFromat.Binary)
                {
                    _parsers.Add(attr.Id, (IParser)Activator.CreateInstance(type));
                }
            }
        }

        public LogRecord ExtrcatLogRecord(byte[] data)
        {
            var messageId = BitConverter.ToUInt16(data, 4);
            LogRecord record = new LogRecord();
            record.Header = new LogHeader();
            record.Data = new List<LogDataBase>();
            record.Header.Timestamp = Util.GpsToUtcTime(BitConverter.ToUInt16(data, 14), BitConverter.ToUInt32(data, 16));
            _parsers[messageId].Parse(data, record);
            _logger.Trace("Новое сообщение {0} @ {1}", messageId, record.Header.Timestamp);
            return record;
        }
    }
}
