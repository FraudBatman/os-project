using System;
using System.Collections.Generic;

namespace os_project
{
    // PCB initialization
    public partial class PCB
    {
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

    // PCB: PCB controller
    public partial class PCB
    {
        public enum PROCESS_STATE
        {
            NEW,      // => Process: has been created, but not ready to run yet  
            READY,    // => Process: has been loaded into RAM and ready to run
            WAITING,  // => Process: is requring I/O execution to take place, waiting in queue
            RUNNING,  // => Process: process is being executed by the CPU
            TERMINATE // => Process: has finished its execution
        }

        private int programCount;
        private PROCESS_STATE state;

        public int ProgramCount
        {
            get { return programCount; }
            set { programCount = value; }
        }

        public PROCESS_STATE State
        {
            get { return state; }
            set { state = value; }
        }
    }

    // PCB: Job controller 
    public partial class PCB
    {
        int processID, instructionCount, priority, totalBufferSize;

        public int TotalBufferSize
        {
            get { return 
                this.BufferSize +
                (Disk.ReadFromDisk(this.ProcessID)[0].Length) +
                (Disk.ReadFromDisk(this.ProcessID)[1].Length);
            }
        }

        public int BufferSize
        {
            get
            {
                return inputBufferSize + outputBufferSize + tempBufferSize;
            }
        }

        public int ProcessID
        {
            get { return processID; }
            set { processID = value; }
        }

        public int InstructionCount
        {
            get { return instructionCount; }
            set { InstructionCount = value; }
        }

        public int Priority
        {
            get { return priority; }
        }
    }

    // PCB: Data controller
    public partial class PCB
    {
        private int inputBufferSize, outputBufferSize, tempBufferSize;
        private int inputBufferStart, outputBufferStart;

        public int InputBufferSize
        {
            get { return inputBufferSize; }
            set { inputBufferSize = value; }
        }

        public int OutputBufferSize
        {
            get { return outputBufferSize; }
            set { outputBufferSize = value; }
        }

        public int TempBufferSize
        {
            get { return tempBufferSize; }
            set { tempBufferSize = value; }
        }

        public int InputBufferStart
        {
            get { return inputBufferStart; }
            set { inputBufferStart = value; }
        }

        public int OutputBufferStart
        {
            get { return outputBufferStart; }
            set { outputBufferStart = value; }
        }
    }

    // PCB: Disk controller
    public partial class PCB
    {
        private int startDiskAddr, diskAddress;

        public int StartDiskAddr
        {
            get { return startDiskAddr; }
            set { startDiskAddr = value; }
        }

        public int DiskAddress 
        {
            get { return processID - 1; }
        }
    }
}
