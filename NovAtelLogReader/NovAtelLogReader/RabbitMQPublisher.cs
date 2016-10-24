using Microsoft.Hadoop.Avro;
using NLog;
using NovAtelLogReader.DataPoints;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace NovAtelLogReader
{
    class RabbitMQPublisher : IPublisher
    {
        private ConnectionFactory factory;
        private IConnection connection;
        private IModel channel;
        private string queueNameRange;
        private string queueNameSatvis;
        private string queueNamePsrpos;
        private string queueNameSatxyz2;
        private string queueNameIsmredobs;
        private string queueNameIsmrawtec;
        private IAvroSerializer<List<DataPointRange>> avroSerializerRange;
        private IAvroSerializer<List<DataPointSatvis>> avroSerializerSatvis;
        private IAvroSerializer<List<DataPointPsrpos>> avroSerializerPsrpos;
        private IAvroSerializer<List<DataPointSatxyz2>> avroSerializerSatxyz2;
        private IAvroSerializer<List<DataPointIsmredobs>> avroSerializerIsmredobs;
        private IAvroSerializer<List<DataPointIsmrawtec>> avroSerializerIsmrawtec;
        private Logger _logger = LogManager.GetCurrentClassLogger();
        public void Close()
        {
            _logger.Info("Закрытие RabbitMQ publisher.");
            channel.Close();
            connection.Close();
        }

        public void Open()
        {
            _logger.Info("Открытие RabbitMQ publisher.");
            var connectionString = Properties.Settings.Default.RabbitConnectionString;
            try
            {
                factory = new ConnectionFactory();
                factory.Uri = connectionString;
                connection = factory.CreateConnection();
                channel = connection.CreateModel();
                queueNameRange = Properties.Settings.Default.QueueNameRange;
                queueNameSatvis = Properties.Settings.Default.QueueNameSatvis;
                queueNamePsrpos = Properties.Settings.Default.QueueNamePsrpos;
                queueNameSatxyz2 = Properties.Settings.Default.QueueNameSatxyz2;
                queueNameIsmredobs = Properties.Settings.Default.QueueNameIsmredobs;
                queueNameIsmrawtec = Properties.Settings.Default.QueueNameIsmrawtec;
                channel.QueueDeclare(queueNameRange, true, false, false, null);
                channel.QueueDeclare(queueNameSatvis, true, false, false, null);
                channel.QueueDeclare(queueNamePsrpos, true, false, false, null);
                channel.QueueDeclare(queueNameSatxyz2, true, false, false, null);
                channel.QueueDeclare(queueNameIsmredobs, true, false, false, null);
                channel.QueueDeclare(queueNameIsmrawtec, true, false, false, null);
                avroSerializerRange = AvroSerializer.Create<List<DataPointRange>>();
                avroSerializerSatvis = AvroSerializer.Create<List<DataPointSatvis>>();
                avroSerializerPsrpos = AvroSerializer.Create<List<DataPointPsrpos>>();
                avroSerializerSatxyz2 = AvroSerializer.Create<List<DataPointSatxyz2>>();
                avroSerializerIsmredobs = AvroSerializer.Create<List<DataPointIsmredobs>>();
                avroSerializerIsmrawtec = AvroSerializer.Create<List<DataPointIsmrawtec>>();
            }
            catch(Exception ex)
            {
                _logger.Error(ex);
            }
        }

        public void PublishRange(List<DataPointRange> dataPoints)
        {
            Console.WriteLine("Отправка {0} точек", dataPoints.Count);
            _logger.Info("Отправка данных в очередь");
            using (var buffer = new MemoryStream())
            {
                avroSerializerRange.Serialize(buffer, dataPoints);
                channel.BasicPublish(String.Empty, queueNameRange, null, buffer.ToArray());
            }
        }
        public void PublishSatvis(List<DataPointSatvis> dataPoints)
        {
            Console.WriteLine("Отправка {0} точек", dataPoints.Count);
            _logger.Info("Отправка данных в очередь");
            using (var buffer = new MemoryStream())
            {
                avroSerializerSatvis.Serialize(buffer, dataPoints);
                channel.BasicPublish(String.Empty, queueNameSatvis, null, buffer.ToArray());
            }
        }
        public void PublishPsrpos(List<DataPointPsrpos> dataPoints)
        {
            Console.WriteLine("Отправка {0} точек", dataPoints.Count);
            _logger.Info("Отправка данных в очередь");
            using (var buffer = new MemoryStream())
            {
                avroSerializerPsrpos.Serialize(buffer, dataPoints);
                channel.BasicPublish(String.Empty, queueNamePsrpos, null, buffer.ToArray());
            }
        }
        public void PublishSatxyz2(List<DataPointSatxyz2> dataPoints)
        {
            Console.WriteLine("Отправка {0} точек", dataPoints.Count);
            _logger.Info("Отправка данных в очередь");
            using (var buffer = new MemoryStream())
            {
                avroSerializerSatxyz2.Serialize(buffer, dataPoints);
                channel.BasicPublish(String.Empty, queueNameSatxyz2, null, buffer.ToArray());
            }
        }
        public void PublishIsmredobs(List<DataPointIsmredobs> dataPoints)
        {
            Console.WriteLine("Отправка {0} точек", dataPoints.Count);
            _logger.Info("Отправка данных в очередь");
            using (var buffer = new MemoryStream())
            {
                avroSerializerIsmredobs.Serialize(buffer, dataPoints);
                channel.BasicPublish(String.Empty, queueNameIsmredobs, null, buffer.ToArray());
            }
        }
        public void PublishIsmrawtec(List<DataPointIsmrawtec> dataPoints)
        {
            Console.WriteLine("Отправка {0} точек", dataPoints.Count);
            _logger.Info("Отправка данных в очередь");
            using (var buffer = new MemoryStream())
            {
                avroSerializerIsmrawtec.Serialize(buffer, dataPoints);
                channel.BasicPublish(String.Empty, queueNameIsmrawtec, null, buffer.ToArray());
            }
        }
    }
}
