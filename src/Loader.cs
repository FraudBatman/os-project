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
            var currentPointer = 0;
            var currentJobNumber = 1; 
            string instruction = "";

            System.Console.WriteLine("Loading Instructions...");

            while (isLoaded == false)
            {
                if (currentPointer == instructionSet.Length - 1 )
                {
                    isLoaded = true;
                } 
                else
                {
                    instruction = instructionSet[currentPointer];

                    if (instruction.Contains("JOB")) // => Job
                        JobHandler(instruction);
                    else if (instruction.Contains("Data")) // => Data
                        DataHandler(instruction);
                    else if (!instruction.Contains("END")) // => Instruction
                        InstructionHandler(instruction);
                    else // => End 
                    {
                        // Add a PCB to the list
                        

                        System.Console.WriteLine("Processed: Job " + currentJobNumber);
                        currentJobNumber++;
                    }
                }
                instruction = "";
                currentPointer++;
            }

            System.Console.WriteLine("Loading Complete");
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

        // Loads instruction into PCB
        private int InstructionHandler(string instruction)
        {
            // Write instructions to disk based on job number

            return 0;
        }
    }
}