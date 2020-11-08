using System;
using System.IO;
using System.Collections.Generic;

namespace os_project
{
    public class Loader
    {
        string absolutePath;
        string[] programFile;
        string[] instructionSet;

        public Loader(string path = null)
        {
            if (path == null)
                throw new FileNotFoundException("File not found, expected path to not be {0}", path);

            try 
            {
                absolutePath = path;
                programFile = File.ReadAllLines(absolutePath);
            }   
            catch(FileNotFoundException e)
            {
                System.Console.WriteLine(e.Message);
            }

            instructionSet = RemoveSpecialCharacters(programFile);
        }

        public void LoadInstructions() 
        {
            var isLoaded = false;
            var currentJobPointer = 0;
            var printJobNumber = 0; 
            var currentInstructionPointer = 0;
            string instruction = "";
            List<Word> data = new List<Word>();
            
            // Builds the PCB & Program data based on the control card attribute ranges
            Dictionary<string, Dictionary<string, int>> PCB_Builder = 
                new Dictionary<string, Dictionary<string, int>>();

            // ["Job_Data"], ["Data_Data"]
            Dictionary<string, List<Word>> Data_Builder =
                new Dictionary<string, List<Word>>();

            System.Console.WriteLine("Loading Programs...");

            var counter = 0;
            while (isLoaded == false)
            {
                if (currentJobPointer == instructionSet.Length)
                {
                    isLoaded = true;
                } 
                else
                {
                    instruction = instructionSet[currentJobPointer];

                    if (instruction.Contains("JOB")) // => Job
                    {
                        // Initialize job data list
                        data = new List<Word>();

                        PCB_Builder.Add("JobAttributes", JobHandler(instruction));
                        counter++;
                    }
                    else if (instruction.Contains("Data")) // => Data
                    {   
                        currentInstructionPointer = 0;

                        // Insert the job data then initialize the data list
                        Data_Builder.Add("Job_Instructions", data);

                        // Initialize job data list
                        data = new List<Word>();

                        PCB_Builder.Add("DataAttributes", DataHandler(instruction));
                    }
                    else if (!instruction.Contains("END")) // => Instruction
                    {
                        // Build data list
                        data.Add(new Word(instructionSet[currentJobPointer]));
                        currentInstructionPointer++;
                    }
                    else // => End - need to write the program data to the disk
                    {
                        // Add the start disk address to the current PCB
                        PCB_Builder.Add("DiskAttributes", InstructionHandler(currentJobPointer));
                        Data_Builder.Add("Data_Instructions", data);

                        // Add program to the PCB linked list
                        Queue.New.AddLast(new PCB(
                            PCB_Builder["JobAttributes"]["processID"],
                            PCB_Builder["JobAttributes"]["instructionCount"],
                            PCB_Builder["JobAttributes"]["priority"],
                            PCB_Builder["DataAttributes"]["inputBufferSize"],
                            PCB_Builder["DataAttributes"]["outputBufferSize"],
                            PCB_Builder["DataAttributes"]["temporaryBufferSize"],
                            PCB_Builder["DiskAttributes"]["startDiskAddr"]
                        ));

                        /*
                         * Timer: Start the waiting time
                         */
                        // Metrics.Start(Queue.New.Last.Value);

                        // Load Job instructions to disk
                        Disk.WriteToDisk(
                            printJobNumber,
                            Data_Builder
                        );

                        // Re-initialize the builders for the next program
                        PCB_Builder = new Dictionary<string, Dictionary<string, int>>();
                        Data_Builder = new Dictionary<string, List<Word>>();

                        // Log program result
                        printJobNumber++;
                        currentInstructionPointer = 0;
                    }
                }
                instruction = "";
                currentJobPointer++;
            }
            
            // Print the loading complete once done
            System.Console.WriteLine("Loading Complete: " + Queue.New.Count + " programs added to the job queue");
        }

        public void ReadJobFile()
        {
            foreach (string line in programFile)
            {
                Console.WriteLine("\t" + line);
            }
        }

        private string[] RemoveSpecialCharacters(string[] instructions)
        {
            for (int i = 0; i < instructions.Length; i++)
            {
                if (instructions[i].Contains("JOB") || instructions[i].Contains("Data") || instructions[i].Contains("END"))
                {
                    instructions[i] = instructions[i].Substring(3);
                }
            }
            return instructions;
        }

        // Loads job control card attributes into PCB
        // Extract Job ID, instruction word count, priority
        private Dictionary<string, int> JobHandler(string _job)
        {
            int[] job = Utilities.parseControlCard(_job);   
            Dictionary<string, int> job_attrs = new Dictionary<string, int>();
            job_attrs.Add("processID", job[0]);
            job_attrs.Add("instructionCount", job[1]);
            job_attrs.Add("priority", job[2]);
            return job_attrs;
        }

        // Loads job control card attributes into PCB
        // Extract input, output, and temp buffer sizes
        private Dictionary<string, int> DataHandler(string _data) 
        {
            int[] data = Utilities.parseControlCard(_data);
            Dictionary<string, int> data_attrs = new Dictionary<string, int>();
            data_attrs.Add("inputBufferSize", data[0]);
            data_attrs.Add("outputBufferSize", data[1]);
            data_attrs.Add("temporaryBufferSize", data[2]);
            return data_attrs;
        }

        // Loads the start of the data address for disk
        private Dictionary<string, int> InstructionHandler(int _instructionAddr)
        {
            Dictionary<string, int> disk_attrs = new Dictionary<string, int>();
            disk_attrs.Add("startDiskAddr", _instructionAddr);
            return disk_attrs;
        }
    }
}