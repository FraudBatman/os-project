namespace os_project
{
    public static class Dispatcher
    {
        static int cacheSize;

        /// <summary>
        /// Dispatcher function that sends the PCB to the CPU(s)
        /// </summary>
        /// <param name="pcb">The PCB of the job to dispatch</param>
        /// <returns>-1 if no open cores are available</returns>
        public static int Dispatch(PCB pcb)
        {
            if (Driver.LargestProgram == 0)
                Driver.LargestProgram = Driver.FindLargestProgram();

            if (!Driver.IsMultiCPU) // => Single CPU dispatch
            {
                // Dispatch the first program from the short term scheduler
                if (Driver.Cores[0].ActiveProgram == null)
                {
                    // Load in the PCB 
                    Driver.Cores[0].ActiveProgram = pcb;

                    // Load the instructions from memory into the CPU cache
                    LoadCPUCache(Driver.Cores[0]);

                    return FindOpenCore();
                }

                return 0;
            }
            else // => Multi CPU dispatch
            {
                var openCoreId = FindOpenCore();

                if (openCoreId == -1)
                    return -1;

                // Dispatch to the CPU
                Driver.Cores[openCoreId].ActiveProgram = pcb;

                if (Driver.Cores[openCoreId].ActiveProgram == null)
                    throw new System.Exception("Dispatcher never sent pcb to core, check dispatcher open core logic");

                // Load the instructions from memory into the CPU cache
                LoadCPUCache(Driver.Cores[openCoreId]);
                return FindOpenCore();
            }
        }

        /// <summary>
        /// Sets the cache for the CPU passed in with size matching the largest program in the job file
        /// </summary>
        /// <param name="core">Core the cache is set for</param>
        static void LoadCPUCache(CPU core)
        {
            core.Cache = new Word[Driver.LargestProgram];
            for (int i = 0; i < core.ActiveProgram.ProgramSize; i++)
            {
                core.Cache[i] = MMU.ReadWord(i, core.ActiveProgram);
            }
        }

        /// <summary>
        /// Finds the first open core in a linear search of O(n)
        /// </summary>
        /// <returns>-1 if no open cores are available</returns>
        static int FindOpenCore()
        {
            foreach(var core in Driver.Cores)
            {
                if (core.ActiveProgram == null)
                    return core.ID; 
            }

            return -1;
        }
    }
}
