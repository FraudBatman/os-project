namespace os_project
{
    public static class Dispatcher
    {
        /// <summary>
        /// Base function that sends the PCB to the CPU(s)
        /// </summary>
        /// <param name="pcb">The PCB of the job to dispatch</param>
        public static void Dispatch(PCB pcb)
        {
            if (!Driver.IsMultiCPU) // => Single CPU dispatch
            {
                
            }
            else // => Multi CPU dispatch
            {
                
            }
        }
    }
}
