using System;
using System.Collections.Generic;

namespace os_project
{
    public class Disk
    {   
        public static Dictionary<int, Dictionary<string, List<Word>>> diskPartitions = 
            new Dictionary<int, Dictionary<string, List<Word>>>(4096);

        public static Word ReadFromDisk(bool jobOrDataType, int jobNum, int instructionNum, bool printIO = false)
        {
            if (jobOrDataType == true)
            {
                if (printIO)
                    System.Console.WriteLine("Read Disk Partition " + jobNum + ": Job Instruction " + instructionNum + " = " +
                        diskPartitions[jobNum]["Job_Instructions"][instructionNum]
                    );

                return diskPartitions[jobNum]["Job_Instructions"][instructionNum];
            }
            else
            {
                if (printIO)
                    System.Console.WriteLine("Read disk partition " + jobNum + ": Data instruction " + instructionNum + " = " +
                        diskPartitions[jobNum]["Job_Instructions"][instructionNum]
                    );
                return diskPartitions[jobNum]["Data_Instructions"][instructionNum];
            }
        }

        public static void WriteToDisk(int jobNum, Dictionary<string, List<Word>> instructionList)
        {
            diskPartitions.Add(jobNum, instructionList);
        }

        public static int GetPartitionCount()
        {
            return diskPartitions.Count;
        }
    }
}