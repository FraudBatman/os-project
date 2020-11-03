using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace os_project
{
    #region Main thread
    public partial class Driver
    {
        static string jobFile;

        public static void Main(string[] args)
        {
            //cross-platform compatibility
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
            System.Console.WriteLine("You built your project, good job!");

            // Loader
            Loader load = new Loader(jobFile);
            load.LoadInstructions();
            System.Console.WriteLine("- LOADER -\n");

            // Scheduler
            System.Console.WriteLine("- SCHEDULER -");
            
            while(Queue.New.First != null)
            {
                System.Console.WriteLine("Open Pages " + MMU.OpenPages);
                LongTermScheduler.Execute();
                TerminateProcesses();
            }
        }

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
