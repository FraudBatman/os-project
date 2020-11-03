using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace os_project
{
    #region Main thread
    public partial class Driver
    {
        static string jobFile;
        static CPU[] cpus;

        public static void Main(string[] args)
        {
            // Cross-platform compatibility
            bool isWin = RuntimeInformation.IsOSPlatform(OSPlatform.Windows);

            if (isWin)
            {
                jobFile = System.IO.Directory.GetCurrentDirectory() + @"\resources\jobs-file.txt";
            }
            else
            {
                jobFile = System.IO.Directory.GetCurrentDirectory() + @"/resources/jobs-file.txt";
            }

            // Validate you can build :D
            System.Console.WriteLine("You built your project, good job!\n");

            // Loader
            System.Console.WriteLine("- LOADER -");
            Loader load = new Loader(jobFile);
            load.LoadInstructions();

            // Scheduler
            System.Console.WriteLine("\n- SCHEDULER -");
            
            while(Queue.New.First != null)
            {
                LongTermScheduler.Execute();
                TerminateProcesses();
            }
        }
        
        /// <summary>
        /// Used for proof of concept to terminate processes 
        /// for running through the scheduler
        /// </summary>
        static void TerminateProcesses()
        {
            bool isDone = false;
            while (!isDone)
            {
                var pcb = Queue.Ready.First;
                Queue.Ready.RemoveFirst();
                Queue.Terminated.AddLast(pcb);

                if (Queue.Ready.First == null)
                    isDone = true;
            }
        }
    }
    #endregion
}
