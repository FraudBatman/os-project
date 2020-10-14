using System;

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

    // Disk Reader Controller
    public partial class LongTermScheduler
    {
        // Read in one job
        public void ReadFromDisk()
        {
            var jobData = Disk.diskPartitions[0]["Job_Instructions"];
            var dataData = Disk.diskPartitions[0]["Data_Instructions"];

            foreach (var x in dataData)
            {
                System.Console.WriteLine(x.ToString());
            }
        }

    }

    // RAM Loader Controller
    public partial class LongTermScheduler
    {
        static int firstword = 0;
        // Load into RAM one job
        /// <summary>
        /// Loads info taken from disk into RAM
        /// </summary>
        /// <param name="data">the data taken from disk</param>
        public void LoadMemory(string[] data)
        {

        }
    }

    // PCB Adaptation Controller
    public partial class LongTermScheduler
    {
        // Mingle with the PCB
    }
}