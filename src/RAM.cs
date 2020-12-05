using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace os_project
{
    // Acts as the middle man between the system schedulers and RAM by managing the logical addresses
    public static class MemoryManager
    {
        private static RAM _RAM = new RAM();

        public static bool IsFull()
        {
            return _RAM.AllocatedSpace() == 0 ? false : true;
        }
    }

    // Just a class container that has a bank of register objects stored in an array
    // No class has acess to registers other than RAM 
    public class RAM
    {
        public const int RAM_SIZE = 1024;
        Register[] PHYSICAL_RAM = new Register[RAM_SIZE];

        // Creates a bank of registers for the memory
        public RAM()
        {
            for (int i = 0; i < PHYSICAL_RAM.Length; i++)
            {
                PHYSICAL_RAM[i] = new Register(i);
            }
        }

        // Read the instruction at the physical address in the register
        public string ReadFromRegister(int p_address)
        {
            return PHYSICAL_RAM[p_address].Instruction.Value;
        }

        // Write the instruction to the register in RAM at the register
        public void WriteToRegister(int p_address, Word word)
        {
            PHYSICAL_RAM[p_address].Register_Owner = word.OwnerOfWord;
            PHYSICAL_RAM[p_address].Instruction = word;
        }

        public int AllocatedSpace()
        {
            var count = 0;
            foreach (var reg in PHYSICAL_RAM)
            {
                if (reg.Instruction != null)
                    count++;
            }

            System.Console.WriteLine(count);
            return count;
        }

        public int EmptySpace()
        {
            var count = 0;
            foreach (var reg in PHYSICAL_RAM)
            {
                if (reg.Instruction == null)
                    count++;
            }
            return count;
        }

        private class Register
        {
            private int? reg_owner;
            private int p_address;
            private Word word;

            public Register(int physical_address, int? reg_owner = null)
            {
                this.reg_owner = reg_owner;
                this.p_address = physical_address;
                this.word = null;
            }

            public int? Register_Owner
            {
                get { return this.reg_owner; }
                set { this.reg_owner = value; }
            }

            public int P_Address
            {
                get { return this.p_address; }
                set { this.p_address = value; }
            }

            public Word Instruction
            {
                get { return this.word; }
                set { this.word = value; }
            }
        }
    }
}