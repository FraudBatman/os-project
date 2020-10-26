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
            __init();
            return true;
        }

        /// <summary>
        /// Deallocates memory of a particular program
        /// </summary>
        /// <param name="pcb">The PCB of the program to deallocate</param>
        public static void DeallocateMemory(PCB pcb)
        {
            __init();
        }

        /// <summary>
        /// Reads a word at a logical address
        /// </summary>
        /// <param name="address">An address formatted 0xHII, where H is hex for the logical page, and II is the offset on the page (last two bits ignored)</param>
        /// <param name="program">The PCB of the program connected to the logical address</param>
        /// <returns>The word read</returns>
        public static Word ReadWord(string address, PCB program)
        {
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
        #endregion

        #region Properties
        /// <summary>
        /// The amount of space (in words) that is currently unallocated
        /// </summary>
        /// <value></value>
        public static int OpenMemory
        {
            //get {return PAGE_SIZE * OpenPages;}
            get { return 0; }
        }
        #endregion
    }
}
