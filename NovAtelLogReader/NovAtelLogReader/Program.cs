using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RabbitMQ.Client;
using Newtonsoft.Json;
using System.Threading;
using System.IO;
using System.Globalization;

using Microsoft.Hadoop.Avro;
using NLog;
using NovAtelLogReader.Readers;
using NovAtelLogReader.LogRecordFormats;

namespace NovAtelLogReader
{
    class Program
    {
        private static Logger _logger = LogManager.GetCurrentClassLogger();
        static void Main(string[] args)
        {
            var _processor = new Processor(new TextFileReader(), new RabbitMQPublisher(), new AsciiLogRecordFormat());

            try
            {
                _logger.Info("Запуск программы.");
                _processor.Start();
            }
            catch(Exception ex)
            {
                _logger.Error(ex);
            }
            Console.WriteLine("Working...");
            Console.ReadLine();
            try
            {
                _processor.Stop();
            }
            catch(Exception ex)
            {
                _logger.Error(ex);
            }
        }
        
    }
}
