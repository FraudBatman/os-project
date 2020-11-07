using System;
using System.Threading.Tasks;

namespace os_project
{
    public static class DMA
    {
        /// <summary>
        /// Executes the IO process directed
        /// </summary>
        /// <param name="readOrWrite">True for read, false for write</param>
        /// <param name="callingCPU">The CPU calling for the IO process</param>
        /// <returns></returns>
        public static async void IOExecution(bool readOrWrite, CPU callingCPU)
        {
            if (readOrWrite) // ==> Read
            {
                Task thread = Task.Factory.StartNew(() =>
                {
                    Block(true, callingCPU.ActiveProgram);
                    throw new Exception("NEED TO FIX DMA IN OUT");
                    Block(false, callingCPU.ActiveProgram);
                });
                thread.Wait();
            }
            else // ==> Write
            {
                Task thread = Task.Factory.StartNew(() =>
                {
                    Block(true, callingCPU.ActiveProgram);

                    var outputBuffer = callingCPU.Cache.Length + callingCPU.ActiveProgram.InstructionCount + callingCPU.ActiveProgram.InputBufferSize - 1;

                    for (int i = outputBuffer; i < callingCPU.Cache.Length; i++)
                    {
                        if (callingCPU.Cache[outputBuffer].Value == "00000000" && callingCPU.Cache[outputBuffer].Value == "null")
                        {
                            throw new Exception("NEED TO FIX DMA IN OUT");
                            //--NEEDS TO BE THE DMA IN / OUT VERSIONS //callingCPU.ActiveProgram.Out(cache[i]);
                        }
                    }

                    Block(false, callingCPU.ActiveProgram);
                });
                thread.Wait();
            }

            // Increment the IO count for the pcb
            callingCPU.ActiveProgram.IOOperationCount++;
        }

        /// <summary>
        /// Moves PCB to waiting queue to start waiting for IO
        /// </summary>
        /// <param name="waitPCB">True to send to waiting, false to remove from waiting</param>
        /// <param name="activeProgram">The PCB of the active program</param>
        /// <returns></returns>
        static async Task Block(bool waitPCB, PCB activeProgram)
        {

            if (waitPCB) // move PCB to waiting queue
            {
                Task block = Task.Factory.StartNew(() =>
                {
                    // /*
                    // * Timer: Stop the waiting time
                    // */
                    // Metrics.Start(activeProgram);

                    /*
                    * Timer: Stop the waiting time
                    */
                    // Metrics.Stop(activeProgram);

                    Queue.Running.Remove(activeProgram);
                    Queue.Waiting.AddLast(activeProgram);
                });
                block.Wait();
            }
            else // Remove the pcb
            {
                Task block = Task.Factory.StartNew(() =>
                {
                    Queue.Running.AddLast(activeProgram);
                    Queue.Waiting.Remove(activeProgram);

                    /*
                    * Timer: Stop the waiting time
                    */
                    // Metrics.Start(activeProgram);

                    // /*
                    // * Timer: Stop the waiting time
                    // */
                    // Metrics.Stop(activeProgram);
                });
                block.Wait();
            }
        }
    }
}