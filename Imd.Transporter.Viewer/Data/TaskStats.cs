using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Imd.Transporter.Viewer.Data
{
    public class TaskStats
    {
        public int Volume { get; set; }

        public string ServerName { get; set; }

        public string StatusText { get; set; }

        public double AverageTransferSpeedMbps { get; set; }

        public int WeekOfYear { get; set; }

        public int Year { get; set; }

    }
}