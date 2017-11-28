using System;
using NLog;
using NovAtelLogReader.Readers;
using NovAtelLogReader.LogRecordFormats;
using NovAtelLogReader.Publishers;

namespace NovAtelLogReader
{
    class Program
    {
        private static Logger _logger = LogManager.GetCurrentClassLogger();
        static void Main(string[] args)
        {
            while (true)
            {
                var _processor = new Processor(new TextFileReader(), new RabbitMQPublisher(), new AsciiLogRecordFormat());

                try
                {
                    _logger.Info("Запуск процессора обработки данных");
                    _processor.Start();
                }
                catch (Exception ex)
                {
                    _logger.Error(ex);
                }

                Console.ReadKey();

                _processor.Stop();
            }
        }
        
    }
}
