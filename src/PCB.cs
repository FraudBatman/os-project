using System;
using System.Collections.Generic;

namespace os_project 
{
    public class PCB
    {
        // Generic PCB attributes
        public enum PROCESS_STATE { 
            NEW,      // => Process: has been created, but not ready to run yet  
            READY,    // => Process: has been loaded into RAM and ready to run
            WAITING,  // => Process: is requring I/O execution to take place, waiting in queue
            RUNNING,  // => Process: process is being executed by the CPU
            TERMINATE // => Process: has finished its execution
        }

        private int programCount;
        private PROCESS_STATE state;

        // Job PCB attributes
        private int processID, instructionCount, priority;

        // Data PCB attributes
        private int inputBufferSize, outputBufferSize, tempBufferSize;

        // Disk information - PCB needs the start address of the instructions stored on the disk
        private int startDiskAddr;

        public PCB(int processID, int instructionCount, int priority, int inputBufferSize, int outputBufferSize, int tempBufferSize, int startDiskAddr)
        {
            // => Initialize Generic PCB attributes
            this.state = PROCESS_STATE.NEW;

            // => Initialize job attributes
            this.processID = processID;
            this.instructionCount = instructionCount;
            this.priority = priority;

            // => Initialize data attributes
            this.inputBufferSize = inputBufferSize;
            this.outputBufferSize = outputBufferSize;
            this.tempBufferSize = tempBufferSize;

            // => Initialize disk attributes
            this.startDiskAddr = startDiskAddr;
        }

        
    }
}
