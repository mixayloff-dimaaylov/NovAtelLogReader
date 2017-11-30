using System;
using NLog;
using NovAtelLogReader.Readers;
using NovAtelLogReader.LogRecordFormats;
using NovAtelLogReader.Publishers;
using System.Threading;
using System.Threading.Tasks;
using Topshelf;
using Topshelf.Runtime;

namespace NovAtelLogReader
{
    class NovAtelService
    {
        private Processor _processor;
        private CancellationTokenSource _cancellation;
        private Logger _logger = LogManager.GetCurrentClassLogger();
        private ManualResetEvent _signal = new ManualResetEvent(false);

        private IReader _reader = (IReader)Activator.CreateInstance(Type.GetType(Properties.Settings.Default.Reader));
        private IPublisher _publisher = (IPublisher)Activator.CreateInstance(Type.GetType(Properties.Settings.Default.Publisher));
        private ILogRecordFormat _format = (ILogRecordFormat)Activator.CreateInstance(Type.GetType(Properties.Settings.Default.Format));

        private async void Loop()
        {
            using (_processor = new Processor(_reader, _publisher, _format))
            {
                _processor.UnrecoverableError += (s, e) => _signal.Set();

                while (!_cancellation.IsCancellationRequested)
                {
                    try
                    {
                        _processor.Start();
                    }
                    catch (Exception ex)
                    {
                        _logger.Error(ex);
                    }

                    _signal.WaitOne();
                    _signal.Reset();
                    _processor.Stop();

                    await Task.Delay(1000, _cancellation.Token);
                }
            }
        }

        public void Start()
        {
            _logger.Fatal("C A N C E L L A T I O N   I S   {0}", _cancellation == null);

            _cancellation = new CancellationTokenSource();
            Task.Run(() => Loop(), _cancellation.Token);
        }

        public void Stop()
        {
            _cancellation.Cancel();
            _signal.Set();
        }
    }


    class Program
    {
        public static void Main()
        {

            HostFactory.Run(host =>
            {
                host.Service<NovAtelService>(service =>
                {
                    service.ConstructUsing(name => new NovAtelService());
                    service.WhenStarted(x => x.Start());
                    service.WhenStopped(x => x.Stop());
                });
                
                host.RunAsLocalSystem();
                host.SetDescription("Сервис чтения данных со спутникового приемника NovAtel");
                host.SetDisplayName("Сервис NovAtel");
                host.SetServiceName("NovAtelService");
            });
        }
    }
}
