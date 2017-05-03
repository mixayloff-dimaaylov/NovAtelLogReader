using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NovAtelLogReader.DataPoints;
using Microsoft.Hadoop.Avro;
using Confluent.Kafka.Serialization;
using Confluent.Kafka;
using System.IO;

namespace NovAtelLogReader
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

    class KafkaPublisher : IPublisher
    {
        private Producer<Null, List<DataPointRange>> rangeProducer;
        private Producer<Null, List<DataPointIsmredobs>> ismredobsProducer;
        private Producer<Null, List<DataPointSatxyz2>> satxyz2Producer;
        private string queueNameRange;
        private string queueNameIsmredobs;
        private string queueNameSatxyz2;

        public void Close()
        {
            rangeProducer.Dispose();
            ismredobsProducer.Dispose();
        }

        public void Open()
        {
            var config = new Dictionary<string, object> { { "bootstrap.servers", Properties.Settings.Default.KafkaBrokers } };

            queueNameRange = Properties.Settings.Default.QueueNameRange;
            queueNameIsmredobs = Properties.Settings.Default.QueueNameIsmredobs;
            queueNameSatxyz2 = Properties.Settings.Default.QueueNameSatxyz2;

            rangeProducer = new Producer<Null, List<DataPointRange>>(config, null, new DataPointListSerializer<DataPointRange>());
            ismredobsProducer = new Producer<Null, List<DataPointIsmredobs>>(config, null, new DataPointListSerializer<DataPointIsmredobs>());
            satxyz2Producer = new Producer<Null, List<DataPointSatxyz2>>(config, null, new DataPointListSerializer<DataPointSatxyz2>());
        }

        public void PublishIsmdetobs(List<DataPointIsmdetobs> dataPoints)
        {
        }

        public void PublishIsmrawtec(List<DataPointIsmrawtec> dataPoints)
        {
        }

        public void PublishIsmredobs(List<DataPointIsmredobs> dataPoints)
        {
            Console.Write("*");
            ismredobsProducer.ProduceAsync(queueNameIsmredobs, null, dataPoints);
        }

        public void PublishPsrpos(List<DataPointPsrpos> dataPoints)
        {
        }

        public void PublishRange(List<DataPointRange> dataPoints)
        {
            Console.Write(".");
            rangeProducer.ProduceAsync(queueNameRange, null, dataPoints);
        }

        public void PublishSatvis(List<DataPointSatvis> dataPoints)
        {
        }

        public void PublishSatxyz2(List<DataPointSatxyz2> dataPoints)
        {
            Console.Write("o");
            satxyz2Producer.ProduceAsync(queueNameSatxyz2, null, dataPoints);
        }
    }
}
