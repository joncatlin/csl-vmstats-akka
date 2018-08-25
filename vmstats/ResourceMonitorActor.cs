using System;
using Akka.Actor;
using Akka.Event;
using System.Diagnostics;

namespace vmstats
{
    class ResourceMonitorActor : ReceiveActor
    {
        #region Message classes
        public class CheckResources { }
        #endregion

        #region Local variables
        private TimeSpan previousCPUTime = TimeSpan.Zero;
        private DateTime previousClockTime = DateTime.MinValue;
        private int numProcessors = Environment.ProcessorCount;
        #endregion

        private ILoggingAdapter _log;


        public ResourceMonitorActor ()
        {
            _log = Context.GetLogger();

            // Initialize the previous times
            var proc = Process.GetCurrentProcess();
            previousCPUTime = proc.TotalProcessorTime;
            previousClockTime = DateTime.Now;

            Receive<CheckResources>(msg => ProcessCheckResources());
        }


        // Find any files in the directory
        private void ProcessCheckResources()
        {
            var proc = Process.GetCurrentProcess();
            var threads = proc.Threads;
            var maxWorkingSet = proc.MaxWorkingSet;

            var curPhysicalMem = proc.WorkingSet64 / 1024 / 1024 / 1024;
            var maxPhysicalMem = proc.MaxWorkingSet;

            var currPagedMem = proc.PagedMemorySize64 / 1024 / 1024 / 1024;
            var currVirtMem = proc.VirtualMemorySize64 / 1024 / 1024 / 1024;

//            Console.WriteLine($" maxWorkingSet = {maxWorkingSet}");
            Console.WriteLine($"|\tcurPhysicalMem = {curPhysicalMem} GB");
//            Console.WriteLine($" maxPhysicalMem = {maxPhysicalMem}");
            Console.WriteLine($"|\tcurrPagedMem = {currPagedMem} GB");
            Console.WriteLine($"|\tcurrVirtMem = {currVirtMem} GB");

            //            Console.WriteLine("My process used working set {0:n3} K of working set and CPU {1:n} msec", mem / 1024.0, cpu.TotalMilliseconds);
            var currentCPUTime = proc.TotalProcessorTime;
            var currentClockTime = DateTime.Now;

            var timeDiff = new TimeSpan(currentClockTime.Ticks - previousClockTime.Ticks + 1);
            var CPUdiff = currentCPUTime.TotalMilliseconds - previousCPUTime.TotalMilliseconds + 1;
            var rawCPUPercentage = CPUdiff / timeDiff.TotalMilliseconds;
            var totalCPUPercentage = (rawCPUPercentage * 100) / (numProcessors * 100);

//            Console.WriteLine($"currentCPUTime = {currentCPUTime}");
//            Console.WriteLine($"previousCPUTime = {previousCPUTime}");
//            Console.WriteLine($"currentClockTime = {currentClockTime.ToLongTimeString()}");
//            Console.WriteLine($"previousClockTime = {previousClockTime.ToLongTimeString()}");
//            Console.WriteLine($"timeDiff.TotalMilliseconds = {timeDiff.TotalMilliseconds}");
//            Console.WriteLine($"CPUdiff = {CPUdiff}");
//            Console.WriteLine($"rawCPUPercentage = {rawCPUPercentage}");
            Console.WriteLine($"|\ttotalCPUPercentage = {totalCPUPercentage:P}");

            // Store the current values for next time
            previousCPUTime = currentCPUTime;
            previousClockTime = currentClockTime;

//            foreach (var aProc in Process.GetProcesses())
//                Console.WriteLine("Proc {0,30}  CPU {1,-20:n} msec", aProc.ProcessName, cpu.TotalMilliseconds);
        }
    }
}
