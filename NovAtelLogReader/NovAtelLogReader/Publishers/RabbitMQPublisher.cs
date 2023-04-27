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

using Microsoft.Hadoop.Avro;
using NLog;
using NovAtelLogReader.DataPoints;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace NovAtelLogReader.Publishers
{
    class RabbitMQPublisher : AbstractGenericPublisher
    {
        private ConnectionFactory factory;
        private IConnection connection;
        private IModel channel;
        private Dictionary<Type, string> _queues = new Dictionary<Type, string>();
        private Dictionary<Type, object> _serializers = new Dictionary<Type, object>();
        private Logger _logger = LogManager.GetCurrentClassLogger();

        public override void Close()
        {
            _logger.Info("Закрытие RabbitMQ Publisher");
            _queues.Clear();
            _serializers.Clear();

            if (channel != null)
            {
                channel.Close();
                channel = null;
            }

            if (connection != null)
            {
                connection.Close();
                connection = null;
            }
        }

        private object CreateSerializer<T>()
        {
            return AvroSerializer.Create<List<T>>();
        }

        public override void Open()
        {
            _logger.Info("Открытие RabbitMQ Publisher");
            var connectionString = Properties.Settings.Default.RabbitConnectionString;
           
            factory = new ConnectionFactory();
            factory.Uri = new Uri(connectionString);
            connection = factory.CreateConnection();
            channel = connection.CreateModel();

            foreach (var type in Util.GetTypesWithAttribute<DataPointAttribute>())
            {
                var queue = type.GetCustomAttribute<DataPointAttribute>().Queue;
                var serializer = typeof(RabbitMQPublisher)
                    .GetMethod("CreateSerializer", BindingFlags.NonPublic | BindingFlags.Instance)
                    .MakeGenericMethod(type)
                    .Invoke(this, null);

                _queues.Add(type, queue);
                _serializers.Add(type, serializer);
                channel.QueueDeclare(queue, true, false, false, null);
            }
        }

        public override void Publish<T>(List<T> value)
        {
            using (var buffer = new MemoryStream())
            {
                if (_serializers.ContainsKey(typeof(T)) && _queues.ContainsKey(typeof(T)))
                {
                    (_serializers[typeof(T)] as IAvroSerializer<List<T>>).Serialize(buffer, value);
                    channel.BasicPublish(String.Empty, _queues[typeof(T)], null, buffer.ToArray());
                }
            }
        }
    }
}
