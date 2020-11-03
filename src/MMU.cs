using System.Collections;
using System.Collections.Generic;

namespace os_project
{
    public static class MMU
    {
        #region Constants

        /// <summary>
        /// Returns the size of each page in words
        /// </summary>
        public const int PAGE_SIZE = 64;

        /// <summary>
        /// Total number of pages in memory
        /// </summary>
        public const int PAGE_COUNT = 16;

        #endregion

        #region Members

        /// <summary>
        /// Indexes for which pages are used that corelate to the index in pageList
        /// </summary>
        static int[] pageList = new int[PAGE_COUNT];

        /// <summary>
        /// Array of PIDs to be used for page determination
        /// </summary>
        static int[] processIDS = new int[PAGE_COUNT];

        /// <summary>
        /// bool to make sure MMU is initialized prior to use
        /// </summary>
        static bool initialized = false;

        #endregion

        #region Functions
        /// <summary>
        /// Allocates a proper number of pages to a program and correctly sets it in the page table
        /// </summary>
        /// <param name="pcb">PCB of the program to allocate</param>
        /// <param name="size">The total size of that needs to be allocated in terms of words</param>
        /// <returns>True if successfully initialized</returns>
        public static bool AllocateMemory(PCB pcb, int size)
        {
            //needs to be called prior to any use of the MMU
            __init();

            //checks to make sure there's enough space to allocate successfully
            if (OpenMemory < size)
                return false;

            //converts the size into pages to determine how many pages are needed
            int pagesToAllocate = (size / PAGE_SIZE) + (size % PAGE_SIZE == 0 ? 0 : 1);

            //adds the PCB to the list
            int programIndex = addProgram(pcb);

            //begin allocation
            for (int i = 0; i < PAGE_COUNT; i++)
            {
                if (!pageIsAllocated(i))
                {
                    pageList[i] = programIndex;
                    pagesToAllocate--;
                    if (pagesToAllocate == 0)
                        break;
                }
            }

            return true;
        }

        /// <summary>
        /// Deallocates memory of a particular program
        /// </summary>
        /// <param name="pcb">The PCB of the program to deallocate</param>
        public static void DeallocateMemory(PCB pcb)
        {
            //needs to be called prior to any use of the MMU
            __init();

            //get the PID's index in processIDs
            int PIDindex = getProcessIDLocation(pcb);

            //scan for pages to deallocate
            for (int i = 0; i < PAGE_COUNT; i++)
            {
                if (pageList[i] == processIDS[PIDindex])
                    pageList[i] = -1;
            }

            //remove the PID from processIDs
            processIDS[PIDindex] = -1;
        }

        /// <summary>
        /// Reads a word at a logical address
        /// </summary>
        /// <param name="address">An address formatted 0xHII, where H is hex for the logical page, and II is the offset on the page (last two bits ignored)</param>
        /// <param name="program">The PCB of the program connected to the logical address</param>
        /// <returns>The word read</returns>
        public static Word ReadWord(string address, PCB program)
        {
            //needs to be called prior to any use of the MMU
            __init();

            //get the physical address of the info provided
            int physical = logicalToPhysical(address, program);

            if (physical == -1)
                throw new System.Exception(
                    "Physical page address was not assigned\n" +
                    "Expected: 0x" + physical + "00 | Actual " + physical
                );

            //return the word at that address
            return RAM.Read(physical);
        }

        /// <summary>
        /// Reads a complete page
        /// </summary>
        /// <param name="page">The logical page number</param>
        /// <param name="program">The PCB of the program connected to the logical address</param>
        /// <returns>A word array of size PAGE_SIZE</returns>
        public static Word[] ReadPage(int page, PCB program)
        {
            //needs to be called prior to any use of the MMU
            __init();

            Word[] returnValue = new Word[PAGE_SIZE];

            //convert the page number into a string
            string pageaddress = $"0x{Utilities.DecToHex(page)}00";

            //get the physical address of the page
            int pagePhys = logicalToPhysical(pageaddress, program);

            if (pagePhys == -1)
                throw new System.Exception(
                    "Physical page address was not assigned\n" +
                    "Expected: 0x" + page + "00 | Actual " + pagePhys
                );

            //for every RAM location in the page, add its word to the returnvalue
            for (int i = 0; i < PAGE_SIZE; i++)
            {
                returnValue[i] = RAM.Read(pagePhys + i);
            }

            return returnValue;
        }

        /// <summary>
        /// Writes a word at a logical address
        /// </summary>
        /// <param name="address">The logical address of the word</param>
        /// <param name="program">The PCB of the program connected to the logical address</param>
        /// <param name="value">The value of the word to be written</param>
        public static void WriteWord(string address, PCB program, Word value)
        {
            //needs to be called prior to any use of the MMU
            __init();

            //get the physical address of the info provided
            int physical = logicalToPhysical(address, program);

            //write the word at that address
            RAM.Write(physical, value);
        }

        /// <summary>
        /// Writes an entire page in one go
        /// </summary>
        /// <param name="page">The logical page number to write</param>
        /// <param name="program">The PCB of the program connected to the logical address</param>
        /// <param name="values">An array of size PAGE_SIZE or less of the words to be written</param>
        public static void WritePage(int page, PCB program, Word[] values)
        {
            Word[] updatedValues = new Word[PAGE_SIZE];
            //convert values into the proper size if necessary
            if (values.Length != PAGE_SIZE)
            {
                for (int i = 0; i < PAGE_SIZE; i++)
                {
                    if (values.Length <= i)
                        updatedValues[i] = null;
                    else
                        updatedValues[i] = values[i];
                }
            }
            else
            {
                updatedValues = values;
            }

            //convert the page number into a string
            string pageaddress = $"0x{Utilities.DecToHex(page)}00";

            //get the physical address of the page
            int pagePhys = logicalToPhysical(pageaddress, program);

            //for every RAM location in the page, overwrite that location with a new word
            for (int i = 0; i < PAGE_SIZE; i++)
            {
                RAM.Write(pagePhys + i, updatedValues[i]);
            }
        }
        #endregion

        #region Private Functions

        /// <summary>
        /// Should call before any MMU usage, initializes MMU 
        /// </summary>
        static void __init()
        {
            //If init has happened already, abort immediately!
            if (initialized) return;

            //Sanity check to make sure all of RAM and no more is addressed by MMU
            if (PAGE_COUNT * PAGE_SIZE != RAM.RAM_SIZE)
                throw new System.Exception($"MMU doesn't address all RAM correctly! Please check RAM and MMU constants.");

            //set all the pages as unallocated
            for (int i = 0; i < PAGE_COUNT; i++)
            {
                pageList[i] = -1;
            }

            //set all process ids to unset
            for (int i = 0; i < PAGE_COUNT; i++)
            {
                processIDS[i] = -1;
            }

            //make sure init isn't called again
            initialized = true;
        }

        /// <summary>
        /// adds a program to the first available spot in processIDs
        /// </summary>
        /// <param name="pcb">The program to add</param>
        /// <returns>The location of the program in processIDs</returns>
        static int addProgram(PCB pcb)
        {
            for (int i = 0; i < PAGE_COUNT; i++)
            {
                if (processIDS[i] == -1)
                {
                    processIDS[i] = pcb.ProcessID;
                    return i;
                }
            }
            return -1;
        }

        /// <summary>
        /// Finds all pages allocated to a program
        /// </summary>
        /// <param name="pcb">The program to find pages for</param>
        /// <returns>An array of pages allocated to the program</returns>
        public static int[] getPages(PCB pcb)
        {
            //holds the size of the return value
            int pageCount = 0;
            int processLocation = getProcessIDLocation(pcb);

            //increases the pagecount for every page found to be allocated
            for (int i = 0; i < PAGE_COUNT; i++)
            {
                if (pageList[i] == processLocation)
                {
                    pageCount++;
                }
            }

            int[] returnValue = new int[pageCount];
            int returnValueIndex = 0;

            //adds all the allocated pages to the returnvalue
            for (int i = 0; i < PAGE_COUNT; i++)
            {
                if (pageList[i] == processLocation)
                {
                    returnValue[returnValueIndex] = i;
                    returnValueIndex++;
                }
            }
            return returnValue;
        }

        /// <summary>
        /// The index of a PCB as it connects to its PID in processIDS
        /// </summary>
        /// <param name="pcb">The pcb to check for</param>
        /// <returns>The index it is located at, or -1 if not found</returns>
        static int getProcessIDLocation(PCB pcb)
        {
            for (int i = 0; i < PAGE_COUNT; i++)
            {
                if (processIDS[i] == pcb.ProcessID)
                    return i;
            }
            return -1;
        }

        /// <summary>
        /// Takes a logical address and returns a physical address in RAM
        /// </summary>
        /// <param name="logical">The logical address formatted 0xHII</param>
        /// <param name="pcb">The PCB of the program</param>
        /// <returns>A physical address that cooresponds to the logical address provided</returns>
        static int logicalToPhysical(string logical, PCB pcb)
        {
            //get the pages of the program to use
            int[] programPages = getPages(pcb);

            //convert the logical address string into info we can use
            string purehex = logical.Substring(2);

            //first H in 0xHHH is the page number in hex
            int pageNumber = Utilities.HexToDec(purehex.ToCharArray()[0].ToString());
            int offset = Utilities.HexToDec(purehex.Substring(1)) / 4; // /4 accounts for the last two bits going unused.

            //find the page number, convert it to physical, then offset
            foreach (var page in programPages)
            {
                if (page == pageNumber)
                {
                    return (page * PAGE_SIZE) + offset;
                }
            }

            // Return -1 if the page was not found
            return -1;
        }

        /// <summary>
        /// Checks if a certain page is allocated
        /// </summary>
        /// <param name="index">The page number to check</param>
        /// <returns>true if allocated</returns>
        static bool pageIsAllocated(int index)
        {
            return pageList[index] >= 0;
        }

        #endregion

        #region Properties
        /// <summary>
        /// The amount of space (in words) that is currently unallocated
        /// </summary>
        public static int OpenMemory
        {
            get { return PAGE_SIZE * OpenPages; }
        }

        /// <summary>
        /// The amount of pages unallocated for use by programs
        /// </summary>
        public static int OpenPages
        {
            get
            {
                int returnValue = 0;
                for (int i = 0; i < PAGE_COUNT; i++)
                {
                    if (!pageIsAllocated(i))
                        returnValue++;
                }
                return returnValue;
            }
        }

        #endregion
    }
}
