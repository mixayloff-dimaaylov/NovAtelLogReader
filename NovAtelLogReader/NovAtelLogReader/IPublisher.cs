﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NovAtelLogReader.DataPoints;

namespace NovAtelLogReader
{
    interface IPublisher
    {
        void Open();
        void Close();
        void PublishRange(List<DataPointRange> dataPoints);
        void PublishSatvis(List<DataPointSatvis> dataPoints);
    }
}
