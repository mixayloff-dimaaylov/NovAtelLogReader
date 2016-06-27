using Microsoft.Hadoop.Avro;
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
        private IAvroSerializer<List<DataPoint>> avroSerializer; 
        public void Close()
        {
            channel.Close();
            connection.Close();
        }

        public void Open()
        {
            factory = new ConnectionFactory();
            factory.Uri = Properties.Settings.Default.RabbitConnectionString;
            connection = factory.CreateConnection();
            channel = connection.CreateModel();
            queueName = Properties.Settings.Default.QueueName;
            channel.QueueDeclare(queueName, true, false, false, null);
            avroSerializer = AvroSerializer.Create<List<DataPoint>>();
        }

        public void Publish(List<DataPoint> dataPoints)
        {
            using (var buffer = new MemoryStream())
            {
                avroSerializer.Serialize(buffer, dataPoints);
                channel.BasicPublish(String.Empty, queueName, null, buffer.ToArray());
            }
        }
    }
}
