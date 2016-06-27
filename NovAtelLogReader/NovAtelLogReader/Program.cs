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

namespace NovAtelLogReader
{
    class Program
    {
        static void Main(string[] args)
        {
            var _processor = new Processor(new ComPortReader(), new RabbitMQPublisher(), new AsciiLogRecordFormat());
            _processor.Start();
            Console.WriteLine("Working...");
            Console.ReadLine();
            _processor.Stop();
        }
        
    }
}
