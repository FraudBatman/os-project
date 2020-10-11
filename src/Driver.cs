using System;

namespace os_project
{
    #region Driver functionality
    public partial class Driver 
    {
        
    }
    #endregion


    #region Main thread
    public partial class Driver 
    {
        static string jobFile = System.IO.Directory.GetCurrentDirectory() + @"\resources\jobs-file.txt";

        public static void Main(string[] args)
        {
            // Validate you can build :D
            System.Console.WriteLine("You built your project, good job bitch!");

            // Loader output
            // Loader load = new Loader(jobFile);
            // load.LoadInstructions();
            
        }
    }
    #endregion
}
