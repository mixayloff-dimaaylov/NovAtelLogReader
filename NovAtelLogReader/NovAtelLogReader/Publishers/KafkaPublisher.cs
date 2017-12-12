﻿using System;
using System.Collections.Generic;
using Microsoft.Hadoop.Avro;
using Confluent.Kafka.Serialization;
using Confluent.Kafka;
using System.IO;
using System.Reflection;
using NovAtelLogReader.DataPoints;

namespace NovAtelLogReader.Publishers
{
    class DataPointListSerializer<T> : ISerializer<List<T>>
    {
        private IAvroSerializer<List<T>> avro;

        public DataPointListSerializer()
        {
            avro = AvroSerializer.Create<List<T>>();
        }

        public IEnumerable<KeyValuePair<string, object>> Configure(IEnumerable<KeyValuePair<string, object>> config, bool isKey)
        {
            return config;
        }

        public byte[] Serialize(string topic, List<T> data)
        {
            using (var buffer = new MemoryStream())
            {
                avro.Serialize(buffer, data);
                return buffer.ToArray();
            }
        }
    }

    class KafkaPublisher : AbstractGenericPublisher
    {
        private Dictionary<Type, string> _queues = new Dictionary<Type, string>();
        private Dictionary<Type, IDisposable> _producers = new Dictionary<Type, IDisposable>();

        public override void Close()
        {
            foreach (var producer in _producers)
            {
                try
                {
                    producer.Value.GetType().GetMethod("Flush", new Type[] { }).Invoke(producer.Value, null);
                }
                finally
                {
                    producer.Value.Dispose();
                }
            }

            _queues.Clear();
            _producers.Clear();
        }

        private object CreateProducer<T>(Dictionary<string, object> config)
        {
            return new Producer<Null, List<T>>(config, null, new DataPointListSerializer<T>());
        }

        public override void Open()
        {
            var config = new Dictionary<string, object> { { "bootstrap.servers", Properties.Settings.Default.KafkaBrokers } };

            _queues.Clear();
            _producers.Clear();

            foreach (var type in Util.GetTypesWithAttribute<DataPointAttribute>())
            {
                var queue = type.GetCustomAttribute<DataPointAttribute>().Queue;
                var producer = (IDisposable) typeof(KafkaPublisher)
                    .GetMethod("CreateProducer", BindingFlags.NonPublic | BindingFlags.Instance)
                    .MakeGenericMethod(type)
                    .Invoke(this, new object[] { config });

                _queues.Add(type, queue);
                _producers.Add(type,  producer);
            }
        }

        public override void Publish<T>(List<T> value)
        {
            if (_producers.ContainsKey(typeof(T)) && _queues.ContainsKey(typeof(T)))
            {
                (_producers[typeof(T)] as Producer<Null, List<T>>).ProduceAsync(_queues[typeof(T)], null, value);
            }
        }
    }
}
