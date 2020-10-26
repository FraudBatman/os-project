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
            return null;
        }

        /// <summary>
        /// Reads a complete page
        /// </summary>
        /// <param name="page">The logical page number</param>
        /// <param name="program">The PCB of the program connected to the logical address</param>
        /// <returns>A word array of size PAGE_SIZE</returns>
        public static Word[] ReadPage(int page, PCB program)
        {
            return null;
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
        }

        /// <summary>
        /// Writes an entire page in one go
        /// </summary>
        /// <param name="page">The logical page number to write</param>
        /// <param name="program">The PCB of the program connected to the logical address</param>
        /// <param name="values">An array of size PAGE_SIZE or less of the words to be written</param>
        public static void WritePage(int page, PCB program, Word[] values)
        {

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
        /// checks if a certain page is allocated
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
                int i = 0;
                foreach (int page in pageList)
                {
                    if (!pageIsAllocated(page))
                        i++;
                }
                return i;
            }
        }

        #endregion
    }
}
