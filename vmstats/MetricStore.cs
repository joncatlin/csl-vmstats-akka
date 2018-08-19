using System;
using System.Collections.Generic;
using System.Text;
using vmstats_shared;

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

        protected MetricStore(MetricStore another)
        {
            this.vmName = another.vmName;
            this.date = another.date;
            this.metrics = new Dictionary<string, Metric>();
            foreach (var entry in another.metrics)
            {
                this.metrics.Add(entry.Key, entry.Value);
            }
        }

        public MetricStore Clone()
        {
            return new MetricStore(this);
        }        
    }

}
