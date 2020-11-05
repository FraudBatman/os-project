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
            pcb.Timer = new Stopwatch();
        }

        public static void Stop(PCB pcb)
        {
            pcb.Timer.Stop();
        }

        // Waiting module
        public static void ExportWaitTime(string title)
        {
            Driver.WriteToFile("waittime.txt", title + "\n");
            // Export the xml information
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
            // Export the xml information
            foreach (var pcb in Queue.Terminated)
            {
                Driver.WriteToFile("completiontime.txt",
                "Job Number: " + pcb.ProcessID.ToString() + " | Time (ms): "
                 + pcb.Export().ToString() + "\n");
            }
        }


        // Number I/O 


        // Percentage Ram

        
        // Measuring and comparing fifo priority policies

        
        // Collect adn tabulate runs

    }
}