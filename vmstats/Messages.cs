namespace vmstats
{
    class Messages
    {
        #region funcational messages
        /// <summary>
        /// Message that contains some metrics to be processed
        /// </summary>
        public class MetricsToBeProcessed {
            public string vmName { get; set; }
            public string date { get; set; }
            public long time { get; set; }
            public float[] elements { get; set; }

            public MetricsToBeProcessed (string vmName, string date, long time, float[] elements)
            {
                this.vmName = vmName;
                this.date = date;
                this.time = time;
                this.elements = elements;
            }
        }

        /// <summary>
        /// Message that signals an actor is finished processing and is stopping
        /// </summary>
        public class Stopping { }

        /// <summary>
        /// Message that signals the current processing of the directory has found no files
        /// </summary>
        public class NoMoreMetrics { }

        /// <summary>
        /// Message that signals the processing for a specific file is complete
        /// </summary>
        public class FileComplete
        {
            public string name { get; set; }
            public string[] headings { get; set; }

            public FileComplete(string name, string[] headings)
            {
                this.name = name;
                this.headings = headings;
            }
        }
        #endregion
    }
}
