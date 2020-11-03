using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace os_project
{
    public partial class Driver
    {
        #region CPU Configurations
        public static CPU[] singleCPU = new CPU[1];
        public static CPU[] multiCPU = new CPU[4];
        static bool cpuIsMulti = false;
        public static bool CPUIsMulti { get { return cpuIsMulti; } }
        #endregion


        #region Job File Configurations
        static string jobFile;
        #endregion


        #region Main thread
        public static void Main(string[] args)
        {
            // Cross-platform compatibility
            bool isWin = RuntimeInformation.IsOSPlatform(OSPlatform.Windows);

            if (isWin)
            {
                jobFile = System.IO.Directory.GetCurrentDirectory() + @"\resources\jobs-file.txt";
            }
            else
            {
                jobFile = System.IO.Directory.GetCurrentDirectory() + @"/resources/jobs-file.txt";
            }

            // Validate you can build :D
            System.Console.WriteLine("You built your project, good job!\n");

            // Loader
            System.Console.WriteLine("- LOADER -");
            Loader load = new Loader(jobFile);
            load.LoadInstructions();

            // Long-term Scheduler
            System.Console.WriteLine("\n- LONG-TERM SCHEDULER -");
            
            // while(Queue.New.First != null)
            // {
                LongTermScheduler.Execute();
                // TerminateProcesses();
            // }

            // Short-term Scheduler
            System.Console.WriteLine("- SHORT-TERM SCHEDULER -");
            ShortTermScheduler.Start();

        }
        
        /// <summary>
        /// Used for proof of concept to terminate processes for running through the scheduler
        /// </summary>
        static void TerminateProcesses()
        {
            bool isDone = false;
            while (!isDone)
            {
                var pcb = Queue.Ready.First;
                Queue.Ready.RemoveFirst();
                Queue.Terminated.AddLast(pcb);

                if (Queue.Ready.First == null)
                    isDone = true;
            }
        }
        #endregion

    }
}
