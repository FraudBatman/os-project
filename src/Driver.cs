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
            System.Console.WriteLine("You built your project, good job bitch!");

            Loader load = new Loader(jobFile);

            // load.ReadJobFile();
            load.LoadInstructions();
            System.Console.WriteLine("- LOADER -\n");

            // Scheduler
            System.Console.WriteLine("- SCHEDULER -");
            LongTermScheduler LT_Scheduler = new LongTermScheduler();

            // Read from the disk
            // Console.WriteLine(Disk.ReadFromDisk(false, 5, 8, false).ToString());

            // Get the partition count
            // System.Console.WriteLine("Disk Partition Count = " + Disk.GetPartitionCount());
        }
    }
    #endregion
}
