using System;
using System.Collections.Generic;

namespace os_project
{

    // 1. The Scheduler loads programs from the disk into RAM according to the given scheduling policy.
    // 2. The scheduler must note in the PCB, which physical addresses in RAM each program/job begins, and ends.
    // 3. This ‘start’ address must be stored in the base-register(or program-counter) of the job). 
    // 4. The Scheduler module must also use the Input/Output buffer size information(extracted from the control cards) for
    //    allocating spaces in RAM for the input and output data.
    // 5. It may be instructive to store the start addresses of the input-buffer and output-buffer 
    //    spaces allocated in RAM as well. (Note that a job’s program-counter, 
    //    which is a component of the PCB, is different from the virtual CPU’s program-counter
    //    – see below). 
    // 6. The Scheduler module either loads one program or multiple programs at a time(in a
    //    multiprogramming system). 
    // 7. Thus, the Scheduler works closely with the Memory manager and the
    // 8. Effective-Address method to load jobs into RAM.

    // Long term scheduler execution instructions
    public partial class LongTermScheduler
    {
        // Executes the long term shceduler instructions - HAPPENS ONLY ONE TIME AT CALL
        public void Execute()
        {   
            System.Console.WriteLine("Scheduling " + Queue.New.Count + " programs...");

            System.Console.WriteLine(MemoryManager.IsFull());

            System.Console.WriteLine("Scheduler execution complete");
        }
    }

    // Disk Reader Controller
    public partial class LongTermScheduler
    {
        // Read in one job
        string[][] ReadFromDisk(int partitionID)
        {
            var job_i = Disk.diskPartitions[partitionID]["Job_Instructions"];
            var data_i = Disk.diskPartitions[partitionID]["Data_Instructions"];
            string[][] readInstructions = new string[2][];
            readInstructions[0] = ParseInstructionList(job_i);
            readInstructions[1] = ParseInstructionList(data_i);
            return readInstructions;
        }

        // Parse the list of words by getting the values
        string[] ParseInstructionList(List<Word> instructions)
        {
            string[] instruction_arr = new string[instructions.Count];
            var data_i = 0;
            foreach (var i in instructions)
            {
                instruction_arr[data_i] = i.Value;
                data_i++;
            }
            return instruction_arr;
        }
    }

    // TODO: NEED TO ENCAPUSLATE THE RAM FROM EVERYTHING OTHER THAN LONGTERM SCHEDULER
    // RAM Loader Controller - NEED TO ENCAPSULATE THE RAM FROM EVERYTHING ELSE!
    public partial class LongTermScheduler
    {
        // Load into RAM one job
        /// <summary>
        /// Loads info taken from disk into RAM in order of
        /// JOB, DATA, I BUFFER, O BUFFER
        /// </summary>
        /// <param name="job">the job taken from the disk</param>
        /// <param name="data">the data taken from disk (word.value as string)</param>
        // void LoadMemory(int processID, string[] job, string[] data)
        // {
        //     //clear the ram
        //     for (int i = 0; i < RAM.RAM_SIZE; i++)
        //     {
        //         string address = "0x" + Utilities.BinToHex(Utilities.DecToBin(i));
        //         Word emptyWord = new Word(0, "0x00000000");
        //         RAM.Memory(RWFlag.Write, address, emptyWord);
        //     }

        //     //write the new ram
        //     var numer = job.GetEnumerator();
        //     int currentWord = 0;
        //     int startWord = currentWord;

        //     //job info first
        //     foreach (string jobline in job)
        //     {
        //         string address = "0x" + Utilities.BinToHex(Utilities.DecToBin(currentWord));
        //         RAM.Memory(RWFlag.Write, address, new Word(currentWord, jobline));
        //         currentWord++;
        //     }

        //     int dataStart = currentWord;

        //     //data info second
        //     foreach (string dataline in data)
        //     {
        //         string address = "0x" + Utilities.BinToHex(Utilities.DecToBin(currentWord));
        //         RAM.Memory(RWFlag.Write, address, new Word(currentWord, dataline));
        //         currentWord++;
        //     }

        //     int dataEnd = currentWord;

        //     //update PCB with the starting points gathered on our magical journey
        //     UpdatePCB(LoadPCB(), startWord, dataStart, dataEnd);
        // }
    }

    // PCB Adaptation Controller
    public partial class LongTermScheduler
    {
        // Mingle with the PCB

        /// <summary>
        /// loads the first PCB from READY_QUEUE
        /// </summary>
        /// <returns></returns>
        PCB LoadPCB()
        {
            var returnValue = Queue.Ready.First.Value;
            Queue.Ready.RemoveFirst();
            return returnValue;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pcb"></param>
        /// <param name="jobStart"></param>
        /// <param name="dataStart"></param>
        /// <param name="outputStart"></param>
        void UpdatePCB(PCB pcb, int jobStart, int dataStart, int outputStart)
        {
            pcb.ProgramCount = jobStart;
            pcb.InputBufferStart = dataStart;
            pcb.OutputBufferStart = outputStart;
        }
    }
}