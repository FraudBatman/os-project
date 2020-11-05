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

        public static void Export()
        {
            // Export the xml information
        }

        // Waiting module
        

        // Completion module


        // Number I/O 


        // Percentage Ram

        
        // Measuring and comparing fifo priority policies

        
        // Collect adn tabulate runs

    }
}