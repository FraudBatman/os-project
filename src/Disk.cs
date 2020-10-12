using System;
using System.Collections.Generic;

namespace os_project
{
    public class Disk
    {   
        public static Dictionary<int, Dictionary<string, List<string>>> diskPartition = 
            new Dictionary<int, Dictionary<string, List<string>>>(4096);

        public static string ReadFromDisk(bool jobOrDataType, int jobNum, int instructionNum)
        {
            return "";
        }

        public static void WriteToDisk(int jobNum, Dictionary<string, List<string>> instructionList)
        {
            diskPartition.Add(jobNum, instructionList);
        }
    }
}