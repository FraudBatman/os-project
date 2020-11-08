namespace os_project
{
    public static class Dispatcher
    {
        static int cacheSize = SetCacheSizeForCPUs();

        /// <summary>
        /// Dispatcher function that sends the PCB to the CPU(s)
        /// </summary>
        /// <param name="pcb">The PCB of the job to dispatch</param>
        /// <returns>-1 if no open cores are available</returns>
        public static int Dispatch(PCB pcb)
        {
            if (!Driver.IsMultiCPU) // => Single CPU dispatch
            {
                // Dispatch the first program from the short term scheduler
                if (Driver.Cores[0].ActiveProgram == null)
                {
                    Driver.Cores[0].ActiveProgram = pcb;
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
                Driver.Cores[openCoreId].Cache = new Word[cacheSize];
                Driver.Cores[openCoreId].ActiveProgram = pcb;

                if (Driver.Cores[openCoreId].ActiveProgram == null)
                    throw new System.Exception("Dispatcher never sent pcb to core, check dispatcher open core logic");

                return FindOpenCore();
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

        /// <summary>
        /// Finds the largest program size for the cache in the CPU
        /// </summary>
        /// <returns></returns>
        public static int SetCacheSizeForCPUs()
        {
            int max = 0;
            foreach(var pcb in Queue.New)
            {
                if (pcb.ProgramSize > max)
                    max = pcb.ProgramSize;
            }
            return max;
        }
    }
}
