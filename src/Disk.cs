using System;
using System.Collections.Generic;

namespace os_project
{
    public class Disk
    {
        public const int DISK_SIZE = 2048;

        public static Dictionary<int, Dictionary<string, List<Word>>> diskPartitions =
            new Dictionary<int, Dictionary<string, List<Word>>>(DISK_SIZE);

        public static Word[][] ReadFromDisk(int partitionID)
        {
            var job_i = Disk.diskPartitions[partitionID]["Job_Instructions"];
            var data_i = Disk.diskPartitions[partitionID]["Data_Instructions"];
            Word[][] readInstructions = new Word[2][];
            readInstructions[0] = ParseInstructionList(job_i);
            readInstructions[1] = ParseInstructionList(data_i);
            return readInstructions;
        }

        // Parse the list of words by getting the values
        static Word[] ParseInstructionList(List<Word> instructions)
        {
            Word[] instruction_arr = new Word[instructions.Count];
            var data_i = 0;
            foreach (var i in instructions)
            {
                instruction_arr[data_i] = i;
                data_i++;
            }
            return instruction_arr;
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