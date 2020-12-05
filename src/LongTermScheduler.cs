using System;
using System.Threading;
using System.Collections.Generic;

namespace os_project
{
    public static class LongTermScheduler
    {
        // Pointer for the pcb's instructions being loaded from disk to memory
        static PCB currentPointer;

        // Long term schedulers load based on programming degreee of the jobs
        static int degree = 0;

        /// <summary>
        /// Loads the memory as long as the LT_Scheduler has the queue lock and memory has enough space
        /// --> Make async
        /// </summary>
        public static void Execute()
        {
            // Acquires the lock for the new queue
            __init();

            // Multi core cpu configuration
            if (Driver.IsMultiCPU)
            {
                // Sets the memory switch false to check if allocation is possible
                bool isMemFullSwitch = false;
                bool programsLoaded = false;

                // While the memory is not full continue loading program data from disk to RAM with the MMU
                while (!isMemFullSwitch && !programsLoaded)
                {
                    // Allocate memory for the program disk instructions and buffer sizes if available
                    var allocationStartAddress = MMU.AllocateMemory(currentPointer);

                    // Validate if the MMU allocated memory in RAM for the program's instructions break if so
                    if (allocationStartAddress == -1)
                        isMemFullSwitch = true;
                    else
                    {
                        // Write the disk instructions and buffers to the allocated RAM for the PCB.ProgramSize count
                        WriteToMemory(allocationStartAddress);
                        SetPCBStartEndAddresses(allocationStartAddress);

                        // Move the pcb to the ready queue if it was loaded properly to memory
                        Queue.New.Remove(currentPointer);
                        Queue.Ready.AddLast(currentPointer);

                        if (Queue.New.First != null)
                            currentPointer = Queue.New.First.Value;
                        else
                            programsLoaded = true;
                    }
                }
            }

            // Single core cpu configuration
            else
            {
                // Allocate memory for the program disk instructions and buffer sizes if available
                var allocationStartAddress = MMU.AllocateMemory(currentPointer);

                // Validate if the MMU allocated memory in RAM for the program's instructions break if so
                if (allocationStartAddress == -1)
                    throw new Exception("Long term scheduler could not load from disk to memory on single core configuration");
                else
                {
                    // Write the disk instructions and buffers to the allocated RAM for the PCB.ProgramSize count
                    WriteToMemory(allocationStartAddress);
                    SetPCBStartEndAddresses(allocationStartAddress);

                    // Move the pcb to the ready queue if it was loaded properly to memory
                    Queue.New.Remove(currentPointer);
                    Queue.Ready.AddLast(currentPointer);
                }
            }
        }

        /// <summary>
        /// Acquires the queue lock for the new queue and validates the new queue is not empty
        /// --> Make async if Queue lock
        /// </summary>
        static void __init()
        {
            // For single core priority since the MMU only gets loaded one program at a time, need to select based on priority
            if (degree == 0 && ShortTermScheduler.POLICY == SchedulerPolicy.Priority)
            {
                Driver._QueueLock.Wait();
                var toSort = Queue.New;
                Queue.New = ShortTermScheduler.InsertSort(toSort);
                Driver._QueueLock.Release();
                degree = 1;
            }

            // What he said but for SJF
            if (degree == 0 && ShortTermScheduler.POLICY == SchedulerPolicy.SJF)
            {
                Driver._QueueLock.Wait();
                var toSort = Queue.New;
                Queue.New = ShortTermScheduler.InsertSortTheOtherOne(toSort);
                Driver._QueueLock.Release();
                degree = 1;
            }

            if (Driver._QueueLock.CurrentCount != 0)
            {
                if (Queue.New.First != null)
                {
                    currentPointer = Queue.New.First.Value;
                }
            }
        }

        /// <summary>
        /// Acquires the lock for the MMU then executes its write to functionality
        /// --> Make async if MMU lock
        /// </summary>
        /// <returns>True if the memory was written</returns>
        static void WriteToMemory(int allocationStartAddress)
        {
            Driver._MMULock.Wait();
            var allProgramData = ReadFromDisk;

            for (int i = 0; i < allProgramData.Length; i++)
                MMU.WriteWord(allocationStartAddress + i, currentPointer, allProgramData[i]);
            Driver._MMULock.Release();
        }

        /// <summary>
        /// Set the current pointed to PCB's job, data, and buffer addresses
        /// </summary>
        static bool SetPCBStartEndAddresses(int allocationStartAddress)
        {
            // Set ==> PCB data start & end addresses
            currentPointer.JobStartAddress = allocationStartAddress;
            currentPointer.JobEndAddress = allocationStartAddress + Disk.ReadFromDisk(currentPointer.DiskAddress)[0].Length - 1;

            // Set ==> PCB intput buffer start & end addresses
            currentPointer.InputBufferStartAddr = currentPointer.JobEndAddress + 1;
            currentPointer.InputBufferEndAddr = currentPointer.InputBufferStartAddr + currentPointer.InputBufferSize - 1;

            // Set ==> PCB output buffer start & end addresses
            currentPointer.OutputBufferStartAddr = currentPointer.InputBufferEndAddr + 1;
            currentPointer.OutputBufferEndAddr = currentPointer.OutputBufferStartAddr + currentPointer.OutputBufferSize - 1;

            // Set ==> PCB temp buffer start & end addresses
            currentPointer.TempStartBufferAddr = currentPointer.OutputBufferEndAddr + 1;
            currentPointer.TempEndBufferAddr = currentPointer.TempStartBufferAddr + currentPointer.TempBufferSize - 1;

            return false;
        }

        /// <summary>
        /// Read the program data from disk for the write to memory
        /// --> Make async if disk lock 
        /// </summary>
        /// <returns></returns>
        static Word[] ReadFromDisk
        {
            get
            {
                // Total program data: job, data, input buffers, output buffers, temp buffers
                var programData = new Word[currentPointer.ProgramSize];

                // Read the program data (job and data words) from disk
                var diskWords = Disk.ReadFromDisk(currentPointer.DiskAddress);

                // Get the list of program's disk read job attributes
                var jobWords = diskWords[0];

                // Get the list of program's disk read data attributes
                var dataWords = diskWords[1];
                var bufferData = new Word[dataWords.Length];

                for (int i = 0; i < dataWords.Length; i++)
                {
                    bufferData[i] = dataWords[i];
                }

                // Concat all the words into one list for the WriteToMemory function
                int j = 0;
                foreach (var word in ConcatWordLists(jobWords, bufferData))
                {
                    programData[j] = word;
                    j++;
                }

                return programData;
            }
        }

        /// <summary>
        /// Concats all the program job and data for buffers words into one array
        /// </summary>
        /// <returns></returns>
        static Word[] ConcatWordLists(Word[] jobList, Word[] dataList)
        {
            // Throws error if any of the lists are null which indicates non-created lists
            if (jobList == null && dataList == null)
                throw new Exception("Instruction lists are null, expected words");

            // Concat all the lists together
            var concatedWords = new Word[jobList.Length + dataList.Length];
            jobList.CopyTo(concatedWords, 0);
            dataList.CopyTo(concatedWords, jobList.Length);

            // Returns the list of concated words
            return concatedWords;
        }
    }
}