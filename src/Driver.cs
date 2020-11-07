using System;
using System.IO;
using System.Threading;
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
            // Console.WriteLine("Type 1 for single-core, anything else for multi-core");
            // if (Console.ReadLine() == "1")
                StartCPUs(true);
            // else
                // StartCPUs(true);

            // Ask for policy
            // Console.WriteLine("Type 1 for FIFO, anything else for priority");
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

            // Run the programs on the cores
            if (isMultiCPU)
                completetionStatus = RunMultiCore();
            else
                completetionStatus = RunSingleCore();

            // Validate the program finished successully
            if (completetionStatus == 0)
                System.Console.WriteLine("----- OS SIMULATION PASSED ------");
            else
                System.Console.WriteLine("----- OS SIMULATION FAILED ------");
        }

        static int RunSingleCore()
        {
            while (Queue.New.First != null)
            {
                // Acquires the mmu semaphore for reading and writing to memory
                _MMULock.Wait();
                LongTermScheduler.Execute();
                ShortTermScheduler.Start();
                _MMULock.Release();

                // Acquires the semaphore for the MMU for reading and writing
                System.Console.WriteLine("Running PCB: " + Cores[0].ActiveProgram.ProcessID);
                _MMULock.Wait();
                Cores[0].Run();
                _MMULock.Release();
            }

            if (Queue.Terminated.Count != 30 || Queue.New.First != null)
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

                //not reading info
                else
                {
                    //allocated
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
        #endregion

        #region Percentage RAM

        #endregion

    }
}
