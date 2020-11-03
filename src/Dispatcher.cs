namespace os_project
{
    public static class Dispatcher
    {
        /// <summary>
        /// Base function that sends the PCB to the CPU(s)
        /// </summary>
        /// <param name="pcb">The PCB of the job to dispatch</param>
        public static void Dispatch()
        {
            if (Driver.CPUIsMulti) // => Multi CPU dispatch
            {
                
            }
            else // => Single CPU dispatch
            {
                
            }
        }
    }
}
