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
        private string queueName;
        private IAvroSerializer<List<DataPointRange>> avroSerializerRange;
        private Logger _logger = LogManager.GetCurrentClassLogger();
        public void Close()
        {
            _logger.Info("Закрытие RabbitMQ publisher");
            channel.Close();
            connection.Close();
        }

        public void Open()
        {
            _logger.Info("Открытие RabbitMQ publisher");
            var connectionString = Properties.Settings.Default.RabbitConnectionString;
            try
            {
                factory = new ConnectionFactory();
                factory.Uri = connectionString;
                connection = factory.CreateConnection();
                channel = connection.CreateModel();
                queueName = Properties.Settings.Default.QueueName;
                channel.QueueDeclare(queueName, true, false, false, null);
                avroSerializerRange = AvroSerializer.Create<List<DataPointRange>>();
            }
            catch(Exception ex)
            {
                _logger.Error(ex);
            }
        }

        public void Publish(List<DataPointRange> dataPoints)
        {
            _logger.Info("Отправка данных в очередь");
            using (var buffer = new MemoryStream())
            {
                avroSerializerRange.Serialize(buffer, dataPoints);
                channel.BasicPublish(String.Empty, queueName, null, buffer.ToArray());
            }
        }
    }
}
