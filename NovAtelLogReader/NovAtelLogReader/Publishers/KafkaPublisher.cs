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
using System.Collections.Generic;
using Microsoft.Hadoop.Avro;
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

        public byte[] Serialize(List<T> data, SerializationContext context)
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

        private object CreateProducer<T>(Dictionary<string, string> config)
        {
            return new ProducerBuilder<Null, List<T>>(config)
                .SetValueSerializer(new DataPointListSerializer<T>())
                .Build();
        }

        public override void Open()
        {
            var config = new Dictionary<string, string> { { "bootstrap.servers", Properties.Settings.Default.KafkaBrokers } };

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
                (_producers[typeof(T)] as IProducer<Null, List<T>>).ProduceAsync(_queues[typeof(T)], new Message<Null, List<T>>(){Value = value});
            }
        }
    }
}
