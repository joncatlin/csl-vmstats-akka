using System;
using System.Collections.Generic;
using System.Text;
using Akka.Actor;
using Serilog;
using System.IO;
using System.Text.RegularExpressions;

namespace vmstats
{
    public class FileReaderActor : ReceiveActor
    {

        #region Message classes
        public class Process
        {
            public Process(string name)
            {
                fileName = name;
            }

            public string fileName { get; private set; }
        }
        #endregion

        static readonly string SEPERATOR = ",";

        #region Instance variables
        // Definitions for pattern used to scan text for metrics
        private readonly string pattern = @"[\w.]+";
        Regex rgxElements;
        Regex rgxVmNamePattern;

        // Dispatcher for metric processing
        private IActorRef _metricAccumulatorDispatcherActor;
        #endregion

        public FileReaderActor(String vmNamePattern, IActorRef metricDispatcher)
        {
             _metricAccumulatorDispatcherActor = metricDispatcher;

            // Initialize the patterns used to scan the file for metrics
            rgxElements = new Regex(pattern, RegexOptions.Compiled);
            rgxVmNamePattern = new Regex(vmNamePattern, RegexOptions.Compiled);

            Receive<Process>(msg => ProcessFile(msg));
        }


        private void ProcessFile(Process msg)
        {
            int lines = 0;
            string[] headings = null;
            bool firstLine = true;

            // Read each line of the file
            using (StreamReader sr = File.OpenText(msg.fileName))
            {
                string s = String.Empty;

                // Read the first line for the heading information for each metric
                //var headings = ParseLine(sr.ReadLine());

                while ((s = sr.ReadLine()) != null)
                {
                    lines++;
                    if (firstLine)
                    {
                        // The first line of the file should contain headings. Save them minus the first few columns
                        // which contain none metric information.
                        firstLine = false;
                        string[] tempHeadings = ParseLine(s);
                        headings = new string[tempHeadings.Length - 3];
                        Array.Copy(tempHeadings, 3, headings, 0, headings.Length);
                    }
                    else
                    {
                        // Decompose each line of the file
                        var elements = ParseLine(s);

                        // Only process lines that match the vm name pattern
                        if (rgxVmNamePattern.IsMatch(elements[0]))
                        {
                            ProcessLine(headings, elements);
                        }
                    }
                }
            }

            Log.Information("Processed fileName={0}, with {1} lines", msg.fileName, lines++);

            

            // Move the file to the processed directory so we do not process it again
            string nameOnly = Path.GetFileName(msg.fileName);
            string directory = Path.GetDirectoryName(msg.fileName);

            // To move a file or folder to a new location:
            try
            {
                System.IO.File.Move(msg.fileName, directory + "\\ProcessedFiles\\" + nameOnly);
            } catch (IOException e)
            {
                Log.Error("Cannot move file name={0} to processed directory. Error={1}. File will have to be manually moved.", msg.fileName, e.Message);
            }
        }

       
        private string[] ParseLine (string s)
        {
            string[] elements = s.Split(SEPERATOR);
            for (int index = 0; index < elements.Length; index++) 
            {
                elements[index] = elements[index].Replace("\"","");
            }
            return elements;
        }


        private void ProcessLine (string[] headings, string[] elements)
        {
            // This assumes a fixed format for the file where the fields are in the order of
            // vmName, date, time, etc

            // Remove special characters from the vmName, date and time fields
            string vmName = elements[0].Replace("/", "-");
            string date = elements[1].Replace("/", "-");
            string time = elements[2].Replace("/", "-");

            // Create a subset of the array to set, removing the first columns vmName, date and time and convert
            // each element to a float
            int newLength = elements.Length - 3;
            float[] newElements = new float[newLength];
            
            for (int index = 0; index < newLength; index++)
            {
                newElements[index] = float.Parse(elements[3 + index]);
            }

            // Convert the time to a long in milliseconds
            string dateToConvert = date + " " + time;
            DateTime dt = DateTime.ParseExact(dateToConvert, "MM-dd-yyyy HH:mm:ss", null);
            long timestamp = dt.Ticks;

            // Send the metrics to the correct actor via the dispatcher
            _metricAccumulatorDispatcherActor.Tell(new Messages.MetricsToBeProcessed(vmName, date, timestamp, newElements));
        }
    }
}
