using System;
using System.Collections.Generic;

namespace os_project
{
    // Long term scheduler execution instructions
    public static class LongTermScheduler
    {
        // Executes the long term shceduler instructions - HAPPENS ONLY ONE TIME AT CALL

        public static void Execute()
        {
            var here = Queue.New.First;
            Queue.New.RemoveFirst();
            Queue.Terminated.AddLast(here);

            // Add deallocation here for terminated processes
            __init();
            

            System.Console.WriteLine("Scheduling " + Queue.New.Count + " programs...");

            bool isMemFull = false;

            while (!isMemFull)
            {
                System.Console.WriteLine("Schedule PCB: " + Queue.New.First.Value.ProcessID);
                PCB currentPCB = Queue.New.First.Value;

                // Buffer Size = (Input + Output + Temp) + (Job word count) + (Data word count)
                int bufferSize = currentPCB.TotalBufferSize                           
                               + Disk.ReadFromDisk(currentPCB.DiskAddress)[0].Length  
                               + Disk.ReadFromDisk(currentPCB.DiskAddress)[1].Length; 
                System.Console.WriteLine("Memory needed = " + bufferSize);
                
                // Allocate the pages need in RAM for the PCB
                MMU.AllocateMemory(currentPCB, bufferSize);
                Console.WriteLine("Page Count = " + MMU.OpenPages);
                
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


        /// <summary>
        /// Disposes the terminated PCBs page allocation and removes it from the queue?
        /// 
        /// Figure out if PCBs in the terminated queue actually need to be removed all together
        /// </summary>
        static void __init()
        {
            if (Queue.Terminated.Count == 0)
                return;

            foreach (PCB pcb in Queue.Terminated)
            {
                Queue.Terminated.RemoveLast();
                System.Console.WriteLine("Hello");

                if (Queue.Terminated.Count == 0)
                    break;
            }

            if (Queue.Terminated.Count != 0)
                throw new Exception("Terminated PCBs were not disposed");
        }
    

        // PCB Adaptation Controller

        // Mingle with the PCB

        /// <summary>
        /// loads the first PCB from READY_QUEUE
        /// </summary>
        /// <returns></returns>
        static PCB LoadPCB()
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
        static void UpdatePCB(PCB pcb, int jobStart, int dataStart, int outputStart)
        {
            pcb.ProgramCount = jobStart;
            pcb.InputBufferStart = dataStart;
            pcb.OutputBufferStart = outputStart;
        }
    }
}