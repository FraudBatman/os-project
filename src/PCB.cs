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

            // => Set the cache
            // this.cache = new Word[];
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

        private int iOOperationCount = 0;

        private int programCount;
        private PROCESS_STATE state;
        private int[] memoryPages;

        public int ProgramCount
        {
            get { return programCount; }
            set { programCount = value; }
        }

        public int IOOperationCount
        {
            get { return iOOperationCount; }
            set { iOOperationCount = value; }
        }

        public PROCESS_STATE State
        {
            get { return state; }
            set { state = value; }
        }

        public int[] MemoryPages
        {
            get { return memoryPages; }
            set { memoryPages = value; }
        }
    }

    // PCB: Metrics controller
    public partial class PCB
    {
        private Stopwatch timer = new Stopwatch();

        public Stopwatch Timer
        {
            get { return timer; }
        }

        public double Export()
        {
            return timer.Elapsed.TotalMilliseconds;
        }
    }

    // PCB: Job controller 
    public partial class PCB
    {
        int processID, instructionCount, priority, totalMemorySize, core_used;
        string jobStartAddress, jobEndAddress;

        public string JobStartAddress
        {
            get { return jobStartAddress; }
            set { jobStartAddress = value; }
        }

        public string JobEndAddress
        {
            get { return jobEndAddress; }
            set { jobEndAddress = value; }
        }

        public int TotalMemorySize
        {
            get
            {
                return
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

        public int Core_Used
        {
            get { return core_used; }
            set { core_used = value; }
        }
    }

    // PCB: Buffer controller
    public partial class PCB
    {
        public string tempStartBufferAddr, tempEndBufferAddr;

        public string TempStartBufferAddr
        {
            get { return tempStartBufferAddr; }
            set { tempStartBufferAddr = value; }
        }

        public string TempEndBufferAddr
        {
            get { return tempEndBufferAddr; }
            set { tempEndBufferAddr = value; }
        }

        public string inputBufferStartAddr, endInputBufferAddr;

        public string InputBufferStartAddr
        {
            get { return inputBufferStartAddr; }
            set { inputBufferStartAddr = value; }
        }

        public string InputBufferEndAddr
        {
            get { return endInputBufferAddr; }
            set { endInputBufferAddr = value; }
        }

        public string outputBufferStartAddr, outputBufferEndAddr;

        public string OutputBufferStartAddr
        {
            get { return outputBufferStartAddr; }
            set { outputBufferStartAddr = value; }
        }

        public string OutputBufferEndAddr
        {
            get { return outputBufferEndAddr; }
            set { outputBufferEndAddr = value; }
        }
    }

    // PCB: Data controller
    public partial class PCB
    {
        private int inputBufferSize, outputBufferSize, tempBufferSize;
        private int inputBufferStart, outputBufferStart;
        private int inputBufferIndex, outputBufferIndex;
        string dataStartAddress, dataEndAddress;

        private Word[] cache;

        public int GetCacheSize()
        {
            return cache.Length;
        }

        public string DataStartAddress
        {
            get { return dataStartAddress; }
            set { dataStartAddress = value; }
        }

        public string DataEndAddress
        {
            get { return dataEndAddress; }
            set { dataEndAddress = value; }
        }

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

        /*COMMENTED OUT FOR PHASE 2

        /// <summary>
        /// Returns the next word in the input buffer, auto increments the read index
        /// </summary>
        /// <value></value>
        public Word In()
        {
            InputBufferStart = Utilities.HexToDec(inputBufferStartAddr.Remove(0, 2));
            return MMU.ReadWord(Utilities.DecToHexFullAddr(InputBufferStart + inputBufferIndex++), this);
        }

        /// <summary>
        /// Sets the next word in the output buffer, increments the write index
        /// </summary>
        /// <param name="writeValue">The value to replace at the current index</param>
        public void Out(Word writeValue)
        {
            OutputBufferStart = Utilities.HexToDec(OutputBufferStartAddr.Remove(0, 2));
            MMU.WriteWord(Utilities.DecToHexFullAddr(OutputBufferStart + outputBufferIndex++), this, writeValue);
        }
        */

        /// <summary>
        /// Returns the next word in the input buffer, auto increments the read index
        /// </summary>
        /// <value></value>
        public Word In()
        {
            InputBufferStart = Utilities.HexToDec(inputBufferStartAddr.Remove(0, 2));
            return MMU.ReadWord(InputBufferStart + inputBufferIndex++);
        }

        /// <summary>
        /// Sets the next word in the output buffer, increments the write index
        /// </summary>
        /// <param name="writeValue">The value to replace at the current index</param>
        public void Out(Word writeValue)
        {
            OutputBufferStart = Utilities.HexToDec(OutputBufferStartAddr.Remove(0, 2));
            MMU.WriteWord(OutputBufferStart = outputBufferIndex++, writeValue);
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
