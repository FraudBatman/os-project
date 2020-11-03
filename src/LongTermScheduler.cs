using System;
using System.Collections.Generic;

namespace os_project
{
    public static class LongTermScheduler
    {
        /// <summary>
        /// Runs the long term scheduler tasks.
        /// Disposes of used memory by terminated processes.
        /// Allocates memory for processes in the 'NEW' queue.
        /// Adds PCB to the 'READY' queue.
        /// </summary>
        public static void Execute()
        {
            __init();

            System.Console.WriteLine("Scheduling " + Queue.New.Count + " programs...");
            bool isMemFull = false;

            while (!isMemFull)
            {
                System.Console.WriteLine("Schedule PCB: " + Queue.New.First.Value.ProcessID);
                PCB currentPCB = Queue.New.First.Value;

                // Buffer Size = (Input + Output + Temp) + (Job word count) + (Data word count)
                System.Console.WriteLine("Memory needed = " + currentPCB.TotalBufferSize);
                
                // Allocate the pages need in RAM for the PCB
                MMU.AllocateMemory(currentPCB, currentPCB.TotalBufferSize);
                Console.WriteLine("Page Count = " + MMU.OpenPages);
                
                // Get instructions from disk
                var jobList = Disk.ReadFromDisk(currentPCB.ProcessID)[0];
                var dataList = Disk.ReadFromDisk(currentPCB.ProcessID)[1];
                var bufferList = BuildBufferList(currentPCB);
                var words = ConcatWordLists(jobList, dataList, bufferList);
                var sections = SectionWordList(words, MMU.getPages(currentPCB).Length);

                // Loads the sections to RAM
                System.Console.WriteLine("Loading program to memory...");
                var pageSectionIdx = 0;
                foreach(var page in MMU.getPages(currentPCB))
                {
                    System.Console.Write("Load to page: " + page);
                    MMU.WritePage(page, currentPCB, sections[pageSectionIdx]);
                    pageSectionIdx++;
                    System.Console.WriteLine(" | Page " + page + " mapped");
                }

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
        /// Sections out the list of words into parts for writing to pages
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
        /// Concats all the 
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

            // Concat all the lists togehter
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
                MMU.DeallocateMemory(pcb);

                // TODO: Figure out if we need to remove the PCB from the queue

                // Remove from the terminated queue
                Queue.Terminated.RemoveLast();
                if (Queue.Terminated.Count == 0)
                    break;
            }

            if (Queue.Terminated.Count != 0)
                throw new Exception("Terminated PCBs were not disposed");
        }
    }
}