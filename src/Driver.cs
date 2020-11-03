using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace os_project
{
    public partial class Driver
    {
        #region CPU Configurations
        public static CPU[] Cores;
        static bool isMultiCPU = false;
        public static bool IsMultiCPU { get { return isMultiCPU; } }
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

            // Start CPUs - false == single | true == multi
            StartCPUs(false);

            // Loader
            System.Console.WriteLine("- LOADER -");
            Loader load = new Loader(jobFile);
            load.LoadInstructions();

            // Long-term Scheduler
            System.Console.WriteLine("\n- LONG-TERM SCHEDULER -");
            
            //
            // Uncommenting this schedules all the processes
            // - dont do it -
            //
            // while(Queue.New.First != null)
            // {
                LongTermScheduler.Execute();
                // TerminateProcesses();
            // }

            // Short-term Scheduler -> FIFO policy
            System.Console.WriteLine("- SHORT-TERM SCHEDULER -");
            ShortTermScheduler.Start();
        }

        static void StartCPUs(bool isMulti)
        {
            isMultiCPU = isMulti;

            if (!IsMultiCPU) // => Single CPU 1 core
            {
                // Sets 1 cpu with empty active program
                Cores = new CPU[1];
                Cores[0] = new CPU();
            }
            else // => Multi CPU 4 Cores
            {
                // Sets 4 cpus with empty active programs
                Cores = new CPU[4];
                for(int i = 0; i<Cores.Length; i++)
                    Cores[i] = new CPU();
            }
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
