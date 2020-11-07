using System;
using System.Collections.Generic;
using System.Diagnostics;

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

        #region PCB Attributes
        private int programCount, processID, priority, core_used;

        public int ProgramCount
        {
            get { return programCount; }
            set { programCount = value; }
        }

        public int ProgramSize
        {
            get
            {
                return
                BufferSize +
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

        public int Priority
        {
            get { return priority; }
        }

        public int Core_Used
        {
            get { return core_used; }
            set { core_used = value; }
        }
        #endregion

        #region State Attributes
        private PROCESS_STATE state;

        public enum PROCESS_STATE
        {
            NEW,      // => Process: has been created, but not ready to run yet  
            READY,    // => Process: has been loaded into RAM and ready to run
            WAITING,  // => Process: is requring I/O execution to take place, waiting in queue
            RUNNING,  // => Process: process is being executed by the CPU
            TERMINATE // => Process: has finished its execution
        }

        public PROCESS_STATE State
        {
            get { return state; }
            set { state = value; }
        }
        #endregion


        #region Job Attributes
        int instructionCount, jobStartAddress, jobEndAddress;

        public int InstructionCount
        {
            get { return instructionCount; }
            set { InstructionCount = value; }
        }


        public int JobStartAddress
        {
            get { return jobStartAddress; }
            set { jobStartAddress = value; }
        }

        public int JobEndAddress
        {
            get { return jobEndAddress; }
            set { jobEndAddress = value; }
        }

        #endregion


        #region Data Attributes
        int dataStartAddress, dataEndAddress;

        public int DataStartAddress
        {
            get { return dataStartAddress; }
            set { dataStartAddress = value; }
        }

        public int DataEndAddress
        {
            get { return dataEndAddress; }
            set { dataEndAddress = value; }
        }
        #endregion


        #region Disk Attributes
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
        #endregion


        #region Input Buffer Attributes
        private int inputBufferSize, inputBufferIndex, inputBufferStartAddr, endInputBufferAddr;

        public int InputBufferSize
        {
            get { return inputBufferSize; }
            set { inputBufferSize = value; }
        }

        public int InputBufferStartAddr
        {
            get { return inputBufferStartAddr; }
            set { inputBufferStartAddr = value; }
        }

        public int InputBufferEndAddr
        {
            get { return endInputBufferAddr; }
            set { endInputBufferAddr = value; }
        }

        #endregion


        #region Output Buffer Attributes
        public int outputBufferSize, outputBufferIndex, outputBufferStartAddr, outputBufferEndAddr;

        public int OutputBufferSize
        {
            get { return outputBufferSize; }
            set { outputBufferSize = value; }
        }

        public int OutputBufferStartAddr
        {
            get { return outputBufferStartAddr; }
            set { outputBufferStartAddr = value; }
        }

        public int OutputBufferEndAddr
        {
            get { return outputBufferEndAddr; }
            set { outputBufferEndAddr = value; }
        }
        #endregion


        #region Temp Buffer Attributes
        public int tempBufferSize, tempStartBufferAddr, tempEndBufferAddr;

        public int TempBufferSize
        {
            get { return tempBufferSize; }
            set { tempBufferSize = value; }
        }

        public int TempStartBufferAddr
        {
            get { return tempStartBufferAddr; }
            set { tempStartBufferAddr = value; }
        }

        public int TempEndBufferAddr
        {
            get { return tempEndBufferAddr; }
            set { tempEndBufferAddr = value; }
        }
        #endregion


        #region Cache Attributes
        private Word[] cache;

        public Word[] Cache
        {
            get { return cache; }
        }
        #endregion


        #region Metrics Attributes
        private int iOOperationCount = 0;

        public int IOOperationCount
        {
            get { return iOOperationCount; }
            set { iOOperationCount = value; }
        }

        private Stopwatch timer = new Stopwatch();

        public Stopwatch Timer
        {
            get { return timer; }
        }

        public double Export()
        {
            return timer.Elapsed.TotalMilliseconds;
        }
        #endregion
    }
}
