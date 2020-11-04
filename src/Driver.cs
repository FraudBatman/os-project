using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace os_project
{
    public partial class Driver
    {
        #region CPU Configurations
        public static CPU[] Cores;
        static bool isMultiCPU;
        public static bool IsMultiCPU { get { return isMultiCPU; } }
        #endregion


        #region Job File Configurations
        static string jobFile;

        static void SetOSPlatform(bool isWin)
        {
            if (isWin)
                jobFile = System.IO.Directory.GetCurrentDirectory() + @"\resources\jobs-file.txt";
            else
                jobFile = System.IO.Directory.GetCurrentDirectory() + @"/resources/jobs-file.txt";
        }
        #endregion


        #region Main thread
        public static void Main(string[] args)
        {
            // Validate you can build :D
            System.Console.WriteLine("You built your project, good job!\n");

            // Cross-platform compatibility
            SetOSPlatform(RuntimeInformation.IsOSPlatform(OSPlatform.Windows));

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
            // ShortTermScheduler.Start();
            System.Console.Write("Ready Queue = " + Queue.Ready.Count);

            
            while (Queue.Ready.First != null)
            {
                System.Console.WriteLine("\n- CPU -");
                ShortTermScheduler.Start();
                Cores[0].Run();
            }


            System.Console.WriteLine("Terminated Queue = " + Queue.Terminated.Count + "\n");
        }


        /// <summary>
        /// Starts the CPU cores
        /// </summary>
        /// <param name="isMulti">Toggle between single and multi core configuration</param>
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
                    Cores[i] = new CPU(i);
            }
        }
        
        /// <summary>
        /// ERASE THIS BEFORE THE PROJECT IS DUE
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
