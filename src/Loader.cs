using System;
using System.IO;

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

        // Loads job instruction into PCB
        private int JobHandler(string job)
        {
            

            return 0;
        }

        // Loads data instruction into PCB
        private int DataHandler(string data) 
        {


            return 0;
        }

        // Loads instruction into PCB
        private int InstructionHandler(string instruction)
        {
            

            return 0;
        }   
    }
}