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

        public int getProgramCount()
        {
            return this.programCount;
        }

        public void setProgramCount(int programCount)
        {
            this.programCount = programCount;
        }

        public PROCESS_STATE getState()
        {
            return this.state;
        }

        public void setState(PROCESS_STATE state)
        {
            this.state = state;
        }
    }

    // PCB: Job controller 
    public partial class PCB
    {
        int processID;
        int instructionCount;
        int priority;

        public int getProcessID() { return this.processID; }

        public void setProcessID(int processID) { this.processID = processID; }

        public int getInstructionCount(){ return this.instructionCount; }

        public void setInstructionCount(int instructionCount) { this.instructionCount = instructionCount; }

        public int getPriority(){ return this.priority; }

        public void setPriority(int priority){ this.priority = priority; }
    }

    // PCB: Data controller
    public partial class PCB
    {
        private int inputBufferSize, outputBufferSize, tempBufferSize;

        public int getInputBufferSize() { return this.inputBufferSize; }

        public void setInputBufferSize(int inputBufferSize) { this.inputBufferSize = inputBufferSize; }

        public int getOutputBufferSize() { return this.outputBufferSize; }

        public void setOutputBufferSize(int outputBufferSize) { this.outputBufferSize = outputBufferSize; }

        public int getTempBufferSize() { return this.tempBufferSize; }

        public void setTempBufferSize(int tempBufferSize) { this.tempBufferSize = tempBufferSize; }
    }

    // PCB: Disk controller
    public partial class PCB
    {
        private int startDiskAddr;

        public int getStartDiskAddr() { return this.startDiskAddr; }

        public void setStartDiskAddr(int startDiskAddr) { this.startDiskAddr = startDiskAddr; }
    }
}
