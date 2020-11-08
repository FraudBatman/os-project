using System;
using System.Threading.Tasks;

namespace os_project
{
    public static class DMA
    {
        static bool blocked = false;

        /// <summary>
        /// Does an IO instruction through the DMA
        /// </summary>
        /// <param name="readOrWrite">True for read, false for write</param>
        /// <param name="callingCPU">The CPU that's calling the DMA</param>
        /// <param name="reg1">Reg1 in the instructions</param>
        /// <param name="secondLocation">The second location to be called, whether it be register or address</param>
        /// <param name="reg2ORAddress">True for reg2, false for address as second location</param>
        /// <returns></returns>
        public static async Task IOExecution(bool readOrWrite, CPU callingCPU, int reg1, int secondLocation, bool reg2ORAddress)
        {
            /*
             * Start the timer
             */
            // Metrics.Stop(callingCPU.ActiveProgram);

            if (readOrWrite) // ==> Read
            {
                Task thread = Task.Factory.StartNew(() =>
                {
                    blocker().GetAwaiter().GetResult();

                    if (reg2ORAddress)
                    {
                        callingCPU.Registers[reg1] = callingCPU.Registers[secondLocation];
                    }
                    else
                    {
                        callingCPU.Registers[reg1] = MMU.ReadWord(secondLocation, callingCPU.ActiveProgram);
                    }

                    unlocker().GetAwaiter().GetResult();
                });
                thread.Wait();
            }
            else // ==> Write
            {
                Task thread = Task.Factory.StartNew(() =>
                {
                    blocker().GetAwaiter().GetResult();

                    if (reg2ORAddress)
                    {
                        callingCPU.Registers[secondLocation] = callingCPU.Registers[reg1];
                    }
                    else
                    {
                        MMU.WriteWord(secondLocation, callingCPU.ActiveProgram, callingCPU.Registers[reg1]);
                    }

                    unlocker().GetAwaiter().GetResult();
                });
                thread.Wait();
            }

            /*
             * Stop the waiting timer
             */
            // Metrics.Stop(callingCPU.ActiveProgram);

            // Increment the IO count for the pcb
            callingCPU.ActiveProgram.IOOperationCount++;
        }


        static async Task blocker()
        {
            while (blocked) ;
            blocked = true;
        }

        static async Task unlocker()
        {
            blocked = false;
        }
    }
}