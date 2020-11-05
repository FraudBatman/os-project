using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Diagnostics;
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
        static string dataDir;

        static void SetOSPlatform(bool isWin)
        {
            if (isWin)
            {
                jobFile = System.IO.Directory.GetCurrentDirectory() + @"\resources\jobs-file.txt";
                Directory.CreateDirectory(@"\data");
                dataDir = System.IO.Directory.GetCurrentDirectory() + @"\data\";
            }
            else
            {
                jobFile = System.IO.Directory.GetCurrentDirectory() + @"/resources/jobs-file.txt";
                Directory.CreateDirectory(@"/data");
                dataDir = System.IO.Directory.GetCurrentDirectory() + @"/data/";
            }
        }
        #endregion


        #region Main thread
        public static void Main(string[] args)
        {
            // Validate you can build :D
            System.Console.WriteLine("You built your project, good job!\n");

            // Cross-platform compatibility
            SetOSPlatform(RuntimeInformation.IsOSPlatform(OSPlatform.Windows));

            // Ask for single-core
            // Start CPUs - false == single | true == multi
            Console.WriteLine("Type 1 for single-core, anything else for multi-core");
            if (Console.ReadLine() == "1")
                StartCPUs(false);
            else
                StartCPUs(true);

            // Ask for policy
            Console.WriteLine("Type 1 for FIFO, anything else for priority");
            if (Console.ReadLine() == "1")
                ShortTermScheduler.POLICY = SchedulerPolicy.FIFO;
            else
                ShortTermScheduler.POLICY = SchedulerPolicy.Priority;

            // Loader
            Loader load = new Loader(jobFile);
            load.LoadInstructions();

            // Run the programs on the cores
            if (isMultiCPU)
                RunMultiCore();
            else
                RunSingleCore();

            // Metrics.ExportWaitTime("Single Core / FIFO Wait Times");
            // Metrics.ExportWaitTime("Single Core / PRIO Wait Times");

            // Metrics.ExportCompletionTime("Single Core / FIFO Completion Times");
            // Metrics.ExportCompletionTime("Single Core / PRIO Completion Times");
            // Metrics.ExportCompletionTime("Multi Core / FIFO Completion Times");
            // Metrics.ExportCompletionTime("Multi Core / PRIO Completion Times");

            // IO Execution exports
            // Metrics.ExportIOExecutionCounts("Single Core / FIFO Completion Times");
            // Metrics.ExportIOExecutionCounts("Single Core / PRIO Completion Times");
            // Metrics.ExportIOExecutionCounts("Multi Core / FIFO Completion Times");
            // Metrics.ExportIOExecutionCounts("Multi Core / PRIO Completion Times");

            // RAM Percentage exports
            // Metrics.ExportPercentageRam("Single Core / FIFO RAM %");
            // Metrics.ExportPercentageRam("Single Core / PRIO RAM %");
            // Metrics.ExportPercentageRam("Multi Core / FIFO RAM %");
            // Metrics.ExportPercentageRam("Multi Core / PRIO RAM %");

            // Cache percentage exports
            // Metrics.ExportPercentageCache("Single Core / FIFO CACHE %");
            // Metrics.ExportPercentageCache("Single Core / PRIO CACHE %");
            // Metrics.ExportPercentageCache("Multi Core / FIFO CACHE %");
            // Metrics.ExportPercentageCache("Multi Core / PRIO CACHE %");

            // Core used exports
            // Metrics.ExportCPUUsed("Multi Core / FIFO Core Used");
            // Metrics.ExportCPUUsed("Multi Core / PRIO Core Used");

        }

        static int RunSingleCore()
        {
            while (Queue.New.First != null)
            {
                LongTermScheduler.Execute();

                while (Queue.Ready.First != null)
                {
                    ShortTermScheduler.Start();
                    System.Console.WriteLine("Running PCB: " + Cores[0].ActiveProgram.ProcessID);
                    Cores[0].Run();
                }
            }

            if (Queue.Terminated.Count != 30 && Queue.New.First != null)
                return -1;

            return 0;
        }

        static int RunMultiCore()
        {
            while (Queue.New.First != null)
            {
                // Load to memory
                LongTermScheduler.Execute();

                // Dispatch to CPUs
                while (Queue.Ready.First != null)
                {
                    ShortTermScheduler.Start();

                    if (Cores[0].ActiveProgram != null)
                    {
                        System.Console.WriteLine("Running PCB: " + Cores[0].ActiveProgram.ProcessID);
                        Cores[0].Run();
                    }

                    if (Cores[1].ActiveProgram != null)
                    {
                        System.Console.WriteLine("Running PCB: " + Cores[1].ActiveProgram.ProcessID);
                        Cores[1].Run();
                    }

                    if (Cores[2].ActiveProgram != null)
                    {
                        System.Console.WriteLine("Running PCB: " + Cores[2].ActiveProgram.ProcessID);
                        Cores[2].Run();
                    }

                    if (Cores[3].ActiveProgram != null)
                    {
                        System.Console.WriteLine("Running PCB: " + Cores[3].ActiveProgram.ProcessID);
                        Cores[3].Run();
                    }
                }
            }

            if (Queue.Terminated.Count != 30 && Queue.New.First != null)
                return -1;

            return 0;
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
                for (int i = 0; i < Cores.Length; i++)
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

        #region File Writing
        public static void WriteToFile(string fileName, string data)
        {
            FileStream fs = new FileStream(dataDir + fileName, FileMode.Append);
            StreamWriter sw = new StreamWriter(fs);
            sw.Write(data);
            sw.Flush();
            fs.Close();
        }
        #endregion

        #region Percentage RAM

        #endregion

    }
}
