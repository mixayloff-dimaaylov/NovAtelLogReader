using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NovAtelLogReader
{
    interface IPublisher
    {
        void Open();
        void Close();
        void Publish(List<DataPoint> dataPoints);
    }
}
