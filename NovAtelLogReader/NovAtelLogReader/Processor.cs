﻿using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using NovAtelLogReader.DataPoints;
using NLog;
using System.Reflection;
using NovAtelLogReader.ListConverters;
using NovAtelLogReader.Readers;
using NovAtelLogReader.Publishers;
using NovAtelLogReader.LogRecordFormats;

namespace NovAtelLogReader
{
    class Processor : IDisposable
    {
        private IReader _reader;
        private IPublisher _publisher;
        private ILogRecordFormat _logRecordFormat;
        private Timer _timer;
        private readonly object _locker = new object();
        private Logger _logger = LogManager.GetCurrentClassLogger();
        private Dictionary<String, Type> _logTypes = new Dictionary<String, Type>();
        private Dictionary<String, IListConverter> _logListConverters = new Dictionary<String, IListConverter>();
        private Dictionary<String, List<object>> _logQueues = new Dictionary<string, List<object>>();

        public event EventHandler<EventArgs> UnrecoverableError;

        public Processor(IReader reader, IPublisher publisher, ILogRecordFormat logRecordFormat)
        {
            _publisher = publisher;
            _reader = reader;
            _logRecordFormat = logRecordFormat;
            _timer = new Timer((s) => PublishDataPoints(), null, Timeout.InfiniteTimeSpan, Timeout.InfiniteTimeSpan);

            foreach (var type in Util.GetTypesWithAttribute<DataPointAttribute>())
            {
                var name = type.GetCustomAttribute<DataPointAttribute>().Name;
                _logTypes.Add(name, type);
                _logQueues.Add(name, new List<object>());
            }

            foreach (var type in Util.GetTypesWithAttribute<ListConverterAttribute>())
            {
                var name = type.GetCustomAttribute<ListConverterAttribute>().Name;
                _logListConverters.Add(name, (IListConverter)Activator.CreateInstance(type));
            }
        }

        public void Start()
        {
            _logger.Info("Запуск процессора сообщений");

            _reader.DataReceived += (s, e) => Task.Run(() => ProcessMessage(e.Data));
            _reader.ReadError += (s, e) => UnrecoverableError?.Invoke(this, EventArgs.Empty);
            _reader.Open(_logRecordFormat);
            _publisher.Open();

            _timer.Change(TimeSpan.Zero, TimeSpan.FromMilliseconds(Properties.Settings.Default.PublishRate));
        }

        public void Stop()
        {
            _logger.Info("Остановка процессора сообщений");
            _timer.Change(Timeout.InfiniteTimeSpan, Timeout.InfiniteTimeSpan);
            _reader.Close();
            _publisher.Close();
        }
        
        private void ProcessMessage(byte[] message)
        {
            var record = _logRecordFormat.ExtrcatLogRecord(message);
            var name = record.Header.Name;

            if (_logListConverters.ContainsKey(name))
            {
                lock (_locker)
                {
                    _logQueues[name].AddRange(_logListConverters[name].ToList(record));
                }
            }
            else
            {
                _logger.Warn("Получены данные для лога {0}, для которого нет обработчика", name);
            }
        }

        private void PublishDataPoints()
        {
            foreach (var queue in _logQueues)
            {
                lock (_locker)
                {
                    if (queue.Value.Count > 0)
                    {
                        _logger.Info("Отправка {0} точек по логу {1}", queue.Value.Count, queue.Key);
                        _publisher.Publish(_logTypes[queue.Key], queue.Value);
                        queue.Value.Clear();
                    }
                }
            }
        }

        public void Dispose()
        {
            _timer.Dispose();
        }
    }
}
