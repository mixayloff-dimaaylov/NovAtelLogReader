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
        private IAvroSerializer<List<DataPointRange>> avroSerializerRange;
        private IAvroSerializer<List<DataPointSatvis>> avroSerializerSatvis;
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
                channel.QueueDeclare(queueNameRange, true, false, false, null);
                channel.QueueDeclare(queueNameSatvis, true, false, false, null);
                avroSerializerRange = AvroSerializer.Create<List<DataPointRange>>();
                avroSerializerSatvis = AvroSerializer.Create<List<DataPointSatvis>>();
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
    }
}
