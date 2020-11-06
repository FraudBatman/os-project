using System;
using System.Collections.Generic;

namespace os_project
{
    public static class LongTermScheduler
    {
        /// <summary>
        /// Used to point to each individual process as it runs through the scheduling
        /// </summary>
        static int processPointer = 1;

        /// <summary>
        /// Runs the long term scheduler tasks
        /// Disposes of used memory by terminated processes
        /// Allocates memory for processes in the 'NEW' queue
        /// Adds PCB to the 'READY' queue
        /// </summary>
        public static void Execute()
        {
            __init();

            bool isMemFull = false;

            while (!isMemFull && processPointer <= 30)
            {
                // Points to the first pcb in the new list
                PCB currentPCB = Queue.New.First.Value;

                // Allocate the pages need in RAM for the PCB
                MMU.AllocateMemory(currentPCB, currentPCB.TotalBufferSize);
                
                // Get instructions from disk
                var jobList = Disk.ReadFromDisk(currentPCB.ProcessID)[0];
                var dataList = Disk.ReadFromDisk(currentPCB.ProcessID)[1];
                var bufferList = BuildBufferList(currentPCB);
                var words = ConcatWordLists(jobList, dataList, bufferList);
                var sections = SectionWordList(words, MMU.getPages(currentPCB).Length);

                // Loads the sections to RAM
                var pageSectionIdx = 0;
                
                var offsetHandler = jobList.Length + dataList.Length;

                var inputDiff = 0;

                foreach(var page in MMU.getPages(currentPCB))
                {
                    MMU.WritePage(page, currentPCB, sections[pageSectionIdx]);

                    if (pageSectionIdx == 0)
                    {
                        // Grab the job addresses
                        currentPCB.JobStartAddress = "0x" + Utilities.DecToHex(page) + "00";
                        currentPCB.JobEndAddress = "0x" + Utilities.DecToHex(page) + Utilities.DecToHexAddr(jobList.Length - 1).ToString();

                        // Grab the data addresses if it doesn't go over pages
                        if (offsetHandler <= MMU.PAGE_SIZE)
                        {
                            currentPCB.DataStartAddress = "0x" + Utilities.DecToHex(page) +
                                Utilities.DecToHexAddr((jobList.Length))
                            ;
                            currentPCB.DataEndAddress = "0x" + Utilities.DecToHex(page) +
                                Utilities.DecToHexAddr((offsetHandler - 1))
                            ;
                        }
                        else
                        {
                            currentPCB.DataStartAddress = "0x" + Utilities.DecToHex(page) +
                                Utilities.DecToHexAddr(jobList.Length)
                            ;
                        }

                        if (currentPCB.ProcessID == 4)
                            System.Console.WriteLine();

                        // Grab the start-buffer addresses
                        if (offsetHandler < MMU.PAGE_SIZE)
                        {
                            inputDiff = MMU.PAGE_SIZE - offsetHandler;
                            currentPCB.InputBufferStartAddr = "0x" + Utilities.DecToHex(page) +
                                Utilities.DecToHexAddr(MMU.PAGE_SIZE - (inputDiff))
                            ;
                        }
                    }
                    else
                    {
                        var tempOffset = offsetHandler - MMU.PAGE_SIZE;

                        if (currentPCB.ProcessID == 4)
                            System.Console.WriteLine();

                        // Grab the overhead data addresses on next pages
                        if (offsetHandler > MMU.PAGE_SIZE)
                        {
                            currentPCB.DataEndAddress = "0x" + Utilities.DecToHex(page) +
                                Utilities.DecToHexAddr(tempOffset - 1)
                            ;

                            currentPCB.InputBufferStartAddr = "0x" + Utilities.DecToHex(page) +
                                Utilities.DecToHexAddr(tempOffset - 1)
                            ;

                            currentPCB.InputBufferEndAddr = "0x" + Utilities.DecToHex(page) +
                                Utilities.DecToHexAddr(tempOffset + currentPCB.InputBufferSize - 1)
                            ;
                        }
                        else
                        {
                            currentPCB.InputBufferEndAddr = "0x" + Utilities.DecToHex(page) +
                                Utilities.DecToHexAddr(tempOffset + currentPCB.InputBufferSize - inputDiff)
                            ;
                        }

                        // Grab the output buffer addresses on the next pages
                        currentPCB.OutputBufferStartAddr = "0x" + Utilities.DecToHex(page) +
                                Utilities.DecToHexAddr(tempOffset + currentPCB.InputBufferSize)
                            ;

                        currentPCB.OutputBufferEndAddr = "0x" + Utilities.DecToHex(page) +
                                Utilities.DecToHexAddr(tempOffset + currentPCB.InputBufferSize + currentPCB.OutputBufferSize)
                            ;

                        currentPCB.TempStartBufferAddr = "0x" + Utilities.DecToHex(page) +
                                Utilities.DecToHexAddr(tempOffset + currentPCB.InputBufferSize + currentPCB.OutputBufferSize + 1)
                            ;

                        currentPCB.tempEndBufferAddr = "0x" + Utilities.DecToHex(page) +
                                Utilities.DecToHexAddr(tempOffset + currentPCB.InputBufferSize + currentPCB.OutputBufferSize + currentPCB.TempBufferSize - 1)
                            ;
                    }

                    pageSectionIdx++;
                }

                // Queue handlers
                Queue.New.RemoveFirst();
                Queue.Ready.AddLast(currentPCB);
                currentPCB.State = PCB.PROCESS_STATE.READY;
                currentPCB.MemoryPages = MMU.getPages(currentPCB);

                // Scheduling handlers
                if (MMU.OpenPages == 0)
                    isMemFull = true;

                processPointer++;
            }
        }

        /// <summary>
        /// Sections out the list of words into parts for writing to pages
        /// Does this at O(n^(n-1)) 
        /// </summary>
        /// <param name="words">List of concated words</param>
        /// <param name="size">Page count determines the amount of sections</param>
        /// <returns></returns>
        static Word[][] SectionWordList(Word[] words, int size)
        {
            Word[][] sections = new Word[size][];
            var pageIdx = 0;
            for (int i = 0; i <= words.Length; i++) 
            {
                if (i % MMU.PAGE_SIZE == 0 && i != 0) {
                    sections[pageIdx] = new Word[MMU.PAGE_SIZE];
                    Array.Copy(
                        words,
                        i * pageIdx,
                        sections[pageIdx],
                        0,
                        MMU.PAGE_SIZE
                    );

                    pageIdx++;
                }
                else if (i == words.Length)
                {
                    sections[pageIdx] = new Word[i % MMU.PAGE_SIZE];
                    Array.Copy(
                        words,
                        (words.Length) - (i % MMU.PAGE_SIZE),
                        sections[pageIdx],
                        0,
                        (i % MMU.PAGE_SIZE)
                    );
                }
            }
            return sections;
        }
        
        /// <summary>
        /// Concats all the words needed to load to memory
        /// </summary>
        /// <param name="jobList">The job instruction's list of words</param>
        /// <param name="dataList">The data instruction's list of words</param>
        /// <param name="bufferList">The buffers null words that are used for writing to</param>
        /// <returns></returns>
        static Word[] ConcatWordLists(Word[] jobList, Word[] dataList, Word[] bufferList)
        {
            // Throws error if any of the lists are null which indicates non-created lists
            if (jobList == null && dataList == null)
                throw new Exception("Instruction lists are null, expected words");

            // Concat all the lists together
            var concatedWords = new Word[jobList.Length + dataList.Length + bufferList.Length];
            jobList.CopyTo(concatedWords, 0);
            dataList.CopyTo(concatedWords, jobList.Length);
            bufferList.CopyTo(concatedWords, (jobList.Length + dataList.Length));

            // Returns the list of concated words
            return concatedWords;
        }
        
        /// <summary>
        /// Creates a list of null words for the pcb's required buffers
        /// </summary>
        /// <param name="pcb"></param>
        /// <returns></returns>
        static Word[] BuildBufferList(PCB pcb)
        {
            Word[] bufferWords = new Word[pcb.BufferSize];

            if (pcb.BufferSize == 0)
                return null;

            for (int i = 0; i < pcb.BufferSize; i++)
                bufferWords[i] = new Word(pcb.ProcessID);


            return bufferWords;
        }

        /// <summary>
        /// Disposes the terminated PCBs page allocation and removes it from the queue?
        /// </summary>
        static void __init()
        {
            if (Queue.Terminated.Count == 0)
                return;

            foreach (PCB pcb in Queue.Terminated)
            {
                // Dispoases of the memory
                if (MMU.getPages(pcb).Length != 0)
                    MMU.DeallocateMemory(pcb);
            }
        }
    }
}