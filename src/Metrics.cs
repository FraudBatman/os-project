using System;
using System.Xml;
using System.Timers;
using System.Collections.Generic;
using System.Diagnostics;

namespace os_project
{
    public static class Metrics
    {
        public static void Start(PCB pcb)
        {
            pcb.Timer.Start();
        }

        public static void Stop(PCB pcb)
        {
            pcb.Timer.Stop();
        }

        // Waiting module
        public static void ExportWaitTime(string title)
        {
            Driver.WriteToFile("waittime.txt", title + "\n");

            foreach(var pcb in Queue.Terminated)
            {
                Driver.WriteToFile("waittime.txt",  
                "Job Number: " + pcb.ProcessID.ToString() + " | Time (ms): "
                 + pcb.Export().ToString() + "\n");
            }
        }

        // Completion module
        public static void ExportCompletionTime(string title)
        {
            Driver.WriteToFile("completiontime.txt", title + "\n");

            foreach (var pcb in Queue.Terminated)
            {
                Driver.WriteToFile("completiontime.txt",
                "Job Number: " + pcb.ProcessID.ToString() + " | Time (ms): "
                 + pcb.Export().ToString() + "\n");
            }
        }

        // Number I/O 
        public static void ExportIOExecutionCounts(string title)
        {
            Driver.WriteToFile("ioexecutioncount.txt", title + "\n");

            foreach (var pcb in Queue.Terminated)
            {
                Driver.WriteToFile("ioexecutioncount.txt",
                "Job Number: " + pcb.ProcessID.ToString() + " | IO Count: "
                 + pcb.IOOperationCount.ToString() + "\n");
            }
        }

        // Percentage Ram
        public static void ExportPercentageRam(string title)
        {
            Driver.WriteToFile("percentageramused.txt", title + "\n");

            foreach (var pcb in Queue.Terminated)
            {
                Driver.WriteToFile("percentageramused.txt",
                "Job Number: " + pcb.ProcessID.ToString() + " | RAM %: "
                 + pcb.IOOperationCount.ToString() + "\n");
            }
        }

        // Cache 
        public static void ExportPercentageCache(string title)
        {
            Driver.WriteToFile("percentagecacheused.txt", title + "\n");

            foreach (var pcb in Queue.Terminated)
            {
                Driver.WriteToFile("percentagecacheused.txt",
                "Job Number: " + pcb.ProcessID.ToString() + "| CPU Used " + pcb.Core_Used +  " | Cache %: "
                 + pcb.CacheUsed.ToString() + "\n");
            }
        }

        // Measuring and comparing fifo priority policies
        public static void ExportCPUUsed(string title)
        {
            Driver.WriteToFile("cpuused.txt", title + "\n");

            foreach (var pcb in Queue.Terminated)
            {
                Driver.WriteToFile("cpuused.txt",
                "Job Number: " + pcb.ProcessID.ToString() + " | Core: "
                 + pcb.Core_Used.ToString() + "\n");
            }
        }

        // Data Dumping

        // Collect adn tabulate runs

    }
}