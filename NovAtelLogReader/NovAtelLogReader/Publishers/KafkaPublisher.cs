using System;
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

        public byte[] Serialize(List<T> data)
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
                producer.Value.Dispose();
            }
        }

        private object CreateProducer<T>(Dictionary<string, object> config)
        {
            return new Producer<Null, List<T>>(config, null, new DataPointListSerializer<T>());
        }

        public override void Open()
        {
            var config = new Dictionary<string, object> { { "bootstrap.servers", Properties.Settings.Default.KafkaBrokers } };

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
            (_producers[typeof(T)] as Producer<Null, List<T>>).ProduceAsync(_queues[typeof(T)], null, value);
        }
    }
}
