using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Runtime.InteropServices;

namespace os_project
{
    public partial class Driver
    {
        // Implement these once the CPU is complete
        #region Shared Resource Semaphores
        public static SemaphoreSlim _QueueLock = new SemaphoreSlim(1);
        public static SemaphoreSlim _MMULock = new SemaphoreSlim(1);
        public static SemaphoreSlim _DiskLock = new SemaphoreSlim(1);
        #endregion

        #region CPU Configurations
        public static CPU[] Cores;
        static bool isMultiCPU;
        public static bool IsMultiCPU { get { return isMultiCPU; } }
        #endregion


        #region Job File Configurations
        static int completetionStatus;
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
                Directory.CreateDirectory(@"./data");
                dataDir = System.IO.Directory.GetCurrentDirectory() + @"/data/";
            }
        }
        #endregion


        #region Main thread
        public static void Main(string[] args)
        {
            // Cross-platform compatibility
            SetOSPlatform(RuntimeInformation.IsOSPlatform(OSPlatform.Windows));

            // Ask for single-core
            // Start CPUs - false == single | true == multi
            Console.WriteLine("Type 1 for single-core, anything else for multi-core");
            // if (Console.ReadLine() == "1")
                // StartCPUs(false);
            // else
                StartCPUs(true);

            // Ask for policy
            Console.WriteLine("Type 1 for FIFO, anything else for priority");
            // if (Console.ReadLine() == "1")
                ShortTermScheduler.POLICY = SchedulerPolicy.FIFO;
            // else
                // ShortTermScheduler.POLICY = SchedulerPolicy.Priority;

            // Start of the cpu simulation
            System.Console.WriteLine("----- START OS SIMULATION ------");

            // Loader utilizes the disk lock for writing to disk
            _DiskLock.Wait();
            Loader load = new Loader(jobFile);
            load.LoadInstructions();
            _DiskLock.Release();

            // Validates if job completed successfully
            var completionStatus = 0;

            // Run the programs on the cores
            // if (isMultiCPU)
                completionStatus = RunMultiCore();
            // else
                // completionStatus = RunSingleCore();

            // Validate the program finished successully
            if (completetionStatus == 0)
                System.Console.WriteLine("----- OS SIMULATION PASSED ------");
            else
                System.Console.WriteLine("----- OS SIMULATION FAILED ------\n");

            System.Console.WriteLine("------ EXPORT METRICS ------");

            // Wait time export
            // Metrics.ExportWaitTime("Wait Times: Single Core | FIFO Policy");
            // Metrics.ExportWaitTime("Wait Times: Single Core | PRIORITY Policy");
            // Metrics.ExportWaitTime("Wait Times: Multi Core | FIFO Policy");
            // Metrics.ExportWaitTime("Wait Times: Multi Core | PRIORITY Policy");

            // Completion time export
            // Metrics.ExportCompletionTime("Completion Times: Single Core | FIFO Policy");
            // Metrics.ExportCompletionTime("Completion Times: Single Core | PRIORITY Policy");
            // Metrics.ExportCompletionTime("Completion Times: Multi Core | FIFO Policy");
            // Metrics.ExportCompletionTime("Completion Times: Multi Core | PRIORITY Policy");

            // CPU used
            // Metrics.ExportCPUUsed("CPU Used: Single Core | FIFO Policy");
            // Metrics.ExportCPUUsed("CPU Used: Single Core | PRIORITY Policy");
            // Metrics.ExportCPUUsed("CPU Used: Multi Core | FIFO Policy");
            // Metrics.ExportCPUUsed("CPU Used: Multi Core | PRIORITY Policy");

            // iOExecution Used
            // Metrics.ExportIOExecutionCounts("IO Execution Count");

            // Export disk information
            // WriteToFile("diskdata", DISKDUMP().ToString());

            // Export memory information
            // Get the total memory usage
        }

        static int RunSingleCore()
        {
            while (Queue.New.First != null)
            {
                // Acquires the mmu semaphore for reading and writing to memory
                LongTermScheduler.Execute();
                ShortTermScheduler.Start();

                // Waits for the core thread to run
                System.Console.WriteLine("Running PCB: " + Cores[0].ActiveProgram.ProcessID);
                Cores[0].Run().Wait();
                WriteToFile("ramdata", RAMDUMP());
            }

            if (Queue.Terminated.Count != 30 || Queue.New.First != null)
                return -1;

            return 0;
        }

        static int RunMultiCore()
        {
            // While there are programs in the job queue
            while (Queue.New.First != null)
            {
                // Load to memory from disk - requires the mmu lock to access
                LongTermScheduler.Execute();

                // While the ready queue is not empty thread the CPU threads
                while (Queue.Ready.First != null)
                {
                    // Dispatch to CPUs
                    ShortTermScheduler.Start();

                    foreach(var core in Cores)
                    {
                        if (!core.isWaiting && core.ActiveProgram != null)
                        {
                            // Wait for the core thread to start and the CPU acquires the MMU lock in its critical section
                            System.Console.WriteLine("Running PCB: " + core.ActiveProgram.ProcessID);
                            core.Run().Wait();
                        }
                    }
                }
            }

            if (Queue.Terminated.Count != 30 || Queue.New.First != null)
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

        /// <summary>
        /// Formats the disk as a JSON String to write to file
        /// </summary>
        /// <returns></returns>
        public static StringWriter DISKDUMP()
        {
            StringWriter writer = new StringWriter();
            foreach(var partition in Disk.diskPartitions)
            {
                System.Console.WriteLine(partition.Key);
                writer.Write("Disk Partiton For Program " + (partition.Key + 1).ToString() + "\n");
                var programData = Disk.ReadFromDisk(partition.Key);
                
                writer.Write("// JOBS\n");
                for (int i = 0; i < programData[0].Length; i++)
                    writer.Write(programData[0][i].Value + "\n");

                writer.Write("\n// DATA\n");
                for (int i = 0; i < programData[1].Length; i++)
                    writer.Write(programData[1][i].Value + "\n");

                writer.Write('\n');
            }

            System.Console.WriteLine(writer);
            writer.Close();
            return writer;
        }

        /// <summary>
        /// Dumps RAM into a string for writing to a file
        /// </summary>
        /// <param name="comment"></param>
        /// <returns></returns>
        public static string RAMDUMP(string comment = null)
        {
            bool alreadyCalled = false;
            int counter = 0;
            int target = -1;
            string[] programFile = File.ReadAllLines(jobFile);
            string returnValue = $"COMMENT: {comment}\n";
            for (int i = 0; i < RAM.RAM_SIZE; i++)
            {
                //CHECK FOR TRANSITION FROM JOB TO DATA
                if (counter == target)
                {
                    returnValue += "//DATA\n";
                }

                //already reading info
                if (alreadyCalled)
                {
                    //allocated
                    if (MMU.used[i] != -1)
                    {
                        returnValue += RAM.data[i].Value + "\n";
                        counter++;
                    }
                    //unallocated
                    else
                    {
                        returnValue += "//END OF JOB\n";
                        returnValue += "UNALLOCATED\n";
                        alreadyCalled = false;
                        counter = 0;
                        target = -1;
                    }
                }

                // not reading info
                else
                {
                    // allocated
                    if (MMU.used[i] != -1)
                    {
                        returnValue += $"//JOB PID: {Utilities.DecToHex(MMU.used[i])}\n";
                        returnValue += RAM.data[i].Value + "\n";
                        counter++;
                        alreadyCalled = true;

                        //get job info from jobs-file.txt 
                        foreach (string line in programFile)
                        {
                            if (line.Contains($"// JOB {Utilities.DecToHex(MMU.used[i])}"))
                            {
                                int[] numbers = Utilities.parseControlCard(line.Substring(3));
                                target = numbers[1];
                                break;
                            }
                        }
                    }
                    //unallocated
                    else
                    {
                        returnValue += "UNALLOCATED\n";
                    }
                }
            }
            return returnValue;
        }

        public static int LargestProgram
        { 
            get { return FindLargestProgram(); }
        }

        /// <summary>
        /// Finds the largest program size for the cache in the CPU
        /// </summary>
        /// <returns></returns>
        public static int FindLargestProgram()
        {
            int max = 0;
            foreach (var pcb in Queue.New)
            {
                if (pcb.ProgramSize > max)
                    max = pcb.ProgramSize;
            }
            return max;
        }
        #endregion

        #region Percentage RAM

        #endregion
    }
}
