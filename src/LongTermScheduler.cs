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

            bool isMemFull = false;
            int count = 0;

            while (!isMemFull)
            {
                // Add deallocation here for terminated processes

                System.Console.WriteLine("Schedule PCB: " + Queue.New.First.Value.ProcessID);
                PCB currentPCB = Queue.New.First.Value;

                // Buffer Size = (Input + Output + Temp) + (Job word count) + (Data word count)
                int bufferSize = currentPCB.TotalBufferSize                           
                               + Disk.ReadFromDisk(currentPCB.DiskAddress)[0].Length  
                               + Disk.ReadFromDisk(currentPCB.DiskAddress)[1].Length; 
                System.Console.WriteLine("Memory needed = " + bufferSize);
                
                // Allocate the pages need in RAM for the PCB
                MMU.AllocateMemory(currentPCB, bufferSize);
                System.Console.WriteLine("Page Count = " + MMU.OpenPages);
                
                // Write to memory

                // Queue handlers
                Queue.New.RemoveFirst();
                Queue.Ready.AddLast(currentPCB);

                // Scheduling handlers
                if (MMU.OpenPages == 0)
                    isMemFull = true;

                System.Console.WriteLine();
            }

            System.Console.WriteLine("Ready Queue Count = " + Queue.Ready.Count);
            System.Console.WriteLine("Scheduler execution complete");
        }
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