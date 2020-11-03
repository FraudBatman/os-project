namespace os_project
{
    public static class Dispatcher
    {
        //I dunno figure this shit out later 
        /// <summary>
        /// Base function that sends the PCB to the CPU(s)
        /// </summary>
        /// <param name="pcb">The PCB of the job to dispatch</param>
        public static void Dispatch(PCB pcb)
        {
            if (Driver.IsMulti) // => Multi CPU dispatch
            {
                
            }
            else // => Single CPU dispatch
            {

            }
        }
    }
}
