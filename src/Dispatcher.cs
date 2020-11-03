using System.Timers;
using System.Diagnostics;

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
                int[] numbers = new int[100];

                Stopwatch timer = new Stopwatch();
                timer.Start();


                timer.Stop();
                var time = timer.Elapsed;
                System.Console.WriteLine(time);
            }
            else // => Single CPU dispatch
            {
                Utilities.StartTimer();
                Utilities.StopTimer();
            }
        }
    }
}
