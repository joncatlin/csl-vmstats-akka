using System;
using System.Collections.Generic;
using System.Text;

namespace vmstats
{
    public class MetricStore
    {
        public Dictionary<string, Metric> metrics { get; set; }
        public string vmName { get; set; }
        public string date { get; set; }

        public MetricStore(string vmName, string date)
        {
            this.vmName = vmName;
            this.date = date;
            this.metrics = new Dictionary<string, Metric>();
        }
    }

}
