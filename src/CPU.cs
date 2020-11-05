using System;
using System.Threading.Tasks;


namespace os_project
{
    // This class needs to be object oriented -> encapuslate as much as possible
    // One way communication from dispatcher to driver, driver sends commands to the CPU
    public class CPU
    {
        #region Program Attributes
        PCB activeProgram;
        public PCB ActiveProgram
        {
            get { return activeProgram; }
            set
            {
                // Sets the active program to the CPU
                activeProgram = value;

                // Move Program
                Queue.Ready.Remove(activeProgram);
                Queue.Running.AddLast(activeProgram);
                activeProgram.State = PCB.PROCESS_STATE.RUNNING;

                // Sets the program count at 0 at initialization
                // Might need to be refactored
                PC = 0;
                registers = new Word[16];
                cache = new Word[activeProgram.GetCacheSize()];

                for (int i = 0; i < registers.Length; i++)
                {
                    registers[i] = new Word();
                }

                var startAddr = activeProgram.JobStartAddress;
                var jobPage = activeProgram.JobStartAddress.Substring(0,3);
                string offset = "00";            

                for (int i = 0; i < activeProgram.InstructionCount; i++)
                {
                    offset = Utilities.DecToHexAddr(i);
                    cache[i] = MMU.ReadWord(jobPage + offset, activeProgram);
                }

                acc = new Word(activeProgram.ProcessID, "0x00000000");
                registers[1] = new Word(activeProgram.ProcessID, "0x00000001");
            }
        }
        #endregion


        #region CPU Attributes
        const string NOPCODE = "13";

        //accumulator, naming it just acc due to my former crippling addiction to Shenzhen IO
        int PC;

        // Acc to word        
        Word acc;

        // CPU Memory
        Word[] registers;
        Word[] cache;

        // Save memory by not assigning if CPU is never instantiated
        int id;
        public int ID { get { return id; } }

        // Sets the active to program to notify it is waiting for a program
        public CPU(int id = 0)
        {
            this.id = id;
            activeProgram = null;
        }
        #endregion

        #region Threads
        public int Run()
        {
            while (PC < activeProgram.InstructionCount)
            {
                // Fetch data
                var instruction = Fetch();

                System.Console.WriteLine(instruction);

                // Decode data
                Decode(instruction);

                // Execute data
                Execute();
            }

            EndProcess();

            // Decode
            return 0;
        }

        // Ends the process
        private void EndProcess()
        {
            // Adds the pcb to the terminated queue
            var pcb = activeProgram;
            Queue.Running.Remove(pcb);
            Queue.Terminated.AddLast(pcb);

            /*
             * Timer: Stop the waiting time
             */
            Metrics.Stop(pcb);

            // Clears the CPU & PCB attributes
            this.registers = null;
            this.activeProgram = null;
            this.cache = null;
            this.PC = 0;
            this.OPCODE = -1;
        }

        #endregion

        #region Fetch Module
        public Word Fetch()
        {
            // Grab the instruction from memory
            var instruction = cache[PC];

            if (instruction.Value == "" || instruction.Value == null)
                throw new Exception("Invalid address, validate effective address for PC");

            PC++;

            return instruction;
        }

        string GetPhysicalAddress(bool isDirect, int pageNumber, string offset = "00")
        {
            if (isDirect)
            {
                // Converts the logical addresses to physical
                string directAddress =
                    "0x" +
                    Utilities.DecToHex(pageNumber) +
                    offset
                ;

                return directAddress;
            }
            return null;
        }

        string EffectiveAddress()
        {
            var pageNumbers = MMU.getPages(this.activeProgram);
            var page = pageNumbers[0];
            var offset = Utilities.DecToHexAddr(addr);


            if (addr >= MMU.PAGE_SIZE)
            {
                page = pageNumbers[1];
                offset = Utilities.DecToHexAddr(addr - MMU.PAGE_SIZE);
            }

            var wordValue = MMU.ReadWord("0x" + Utilities.DecToHex(page) + offset,
                this.activeProgram
            );

            // (i % MMU.PAGE_SIZE == 0 && i != 0)
            return wordValue.Value;
        }
        #endregion


        #region Decode Module
        private int ExecutionPointer, OPCODE, sReg0, sReg1, dReg, addr, bReg, reg1, reg2;
        private string jumpToAddr;

        /// <summary>
        /// Takes an instruction and converts it into a usable format before sending it to an execute
        /// </summary>
        /// <param name="instruction">The instruction to decode</param>
        public void Decode(Word instruction)
        {
            string data = instruction.Value;

            //NOP check
            if (data.Substring(1) == NOPCODE)
            {
                // //13: NOP
                // //Well that was easy.
                return;
            }

            //0 check
            registers[1].Value = "0x00000000";

            //gets first 2 bits of data
            ExecutionPointer = Utilities.HexToDec(data.ToCharArray()[0].ToString()) / 4;

            //gets the opcode. god knows how
            OPCODE = (((Utilities.HexToDec(data.ToCharArray()[0].ToString()) % 4) * 16))
            + Utilities.HexToDec(data.ToCharArray()[1].ToString());

            // If nopcode, do nothing, return
            if (OPCODE == Utilities.HexToDec(NOPCODE))
            {
                return;
            }

            switch (ExecutionPointer)
            {
                case 0: // => Arithmetic
                    sReg0 = Utilities.HexToDec(data.ToCharArray()[2].ToString());
                    sReg1 = Utilities.HexToDec(data.ToCharArray()[3].ToString());
                    dReg = Utilities.HexToDec(data.ToCharArray()[4].ToString());
                    break;
                case 1: // => Conditional
                    bReg = Utilities.HexToDec(data.ToCharArray()[2].ToString());
                    dReg = Utilities.HexToDec(data.ToCharArray()[3].ToString());
                    addr = Utilities.HexToDec(data.Substring(4, 4));
                    break;
                case 2: // => Uncon. Jump -> may need to refactor
                    jumpToAddr = data.Substring(2);
                    break;
                case 3: // => IO
                    reg1 = Utilities.HexToDec(data.ToCharArray()[2].ToString());
                    reg2 = Utilities.HexToDec(data.ToCharArray()[3].ToString());
                    addr = Utilities.HexToDec(data.Substring(4, 4)) / 4;
                    break;
            }
        }
        #endregion


        #region Execute Module
        public void Execute()
        {
            switch (ExecutionPointer)
            {
                case 0: // => Arithmetic
                    ExecuteArith();
                    break;
                case 1: // => Conditional
                    ExecuteCondi();
                    break;
                case 2: // => Uncon. Jump
                    ExecuteUJump();
                    break;
                case 3: // => IO
                    ExecuteIO();
                    break;
                default:
                    return;
            }
        }

        private void ExecuteArith()
        {
            switch (OPCODE)
            {
                case 4: // MOV
                    registers[dReg] = registers[bReg];
                    break;
                case 5: // ADD
                    registers[dReg] = registers[sReg0];
                    registers[dReg] += registers[sReg1];
                    break;
                case 6: // SUB
                    registers[dReg] = registers[sReg0];
                    registers[dReg] -= registers[sReg1];
                    break;
                case 7: // MUL
                    registers[dReg] = registers[sReg0];
                    registers[dReg] *= registers[sReg1];
                    break;
                case 8: // DIV
                    registers[dReg] = registers[sReg0];
                    registers[dReg] /= registers[sReg1];
                    break;
                case 9: // AND
                    registers[dReg] = registers[sReg0] & registers[sReg1];
                    break;
                case 10: // OR
                    registers[dReg] = registers[sReg0] | registers[sReg1];
                    break;
                case 16: // SLT
                    registers[dReg].Value = registers[sReg0].ValueAsInt < registers[bReg].ValueAsInt ? "00000000" : "00000001";
                    break;
                default:
                    throw new Exception("OPCode invalid, check the hex to dec conversion: " + OPCODE);
            }
        }

        private void ExecuteCondi()
        {
            switch (OPCODE)
            {
                case 2: // 02: ST
                    cache[dReg].Value = EffectiveAddress();
                    break;
                case 3: // 03: LW
                    registers[dReg] = cache[addr];
                    break;
                case 11: // 0B: MOVI
                    registers[dReg].Value = Utilities.WordFill(Utilities.DecToHex(addr));
                    break;
                case 12: // 0C: ADDI
                    registers[dReg] = registers[dReg] + new Word(0, Utilities.WordFill(Utilities.DecToHex(addr)));
                    break;
                case 13: // 0D: MULI
                    registers[dReg] = registers[dReg] * new Word(0, Utilities.WordFill(Utilities.DecToHex(addr)));
                    break;
                case 14: // 0E: DIVI
                    registers[dReg] = registers[dReg] / new Word(0, Utilities.WordFill(Utilities.DecToHex(addr)));
                    break;
                case 15: // 0F: LDI
                    registers[dReg] = new Word(activeProgram.ProcessID, Utilities.WordFill(Utilities.DecToHex(addr)));
                    break;
                case 17: // 11: SLTI
                    dReg = sReg0 < addr ? 1 : 0;
                    break;
                case 21: // 15: BEQ
                    PC = registers[bReg] == registers[dReg] ? addr : PC;
                    break;
                case 22: // 16: BNE
                    PC = registers[bReg] != registers[dReg] ? addr : PC;
                    break;
                case 23: // 17: BEZ
                    PC = registers[bReg].ValueAsInt == 0 ? addr : PC;
                    break;
                case 24: // 18: BNZ
                    PC = registers[bReg].ValueAsInt != 0 ? addr : PC;
                    break;
                case 25: // 19: BGZ
                    PC = registers[bReg].ValueAsInt > 0 ? addr : PC;
                    break;
                case 26: // 1A: BLZ
                    PC = registers[bReg].ValueAsInt < 0 ? addr : PC;
                    break;
                default:
                    throw new Exception("OPCode invalid, check the hex to dec conversion: " + OPCODE);
            }
        }
        private void ExecuteUJump()
        {
            switch (OPCODE)
            {
                case 18: // 12: HLT
                    //Sets the PC to the instruction count, which will immediately break out of the upper loop
                    PC = activeProgram.InstructionCount;
                    break;
                case 20: // 14: JMP
                    // //Takes the value given and sets the PCB's prog. count. to it
                    PC = Utilities.HexToDec(jumpToAddr);
                    break;
                default:
                    throw new Exception("OPCode invalid, check the hex to dec conversion: " + OPCODE);
            }
        }
        private void ExecuteIO()
        {
            switch (OPCODE)
            {
                case 0: // 00: RD
                    // //NOTE: in this snippet, it just reads to acc. additional setup required to send to a different register
                    DMA_IOExecution(true);
                    break;
                case 1: // 01: WR
                    DMA_IOExecution(false);
                    break;
                default:
                    throw new Exception("OPCode invalid, check the hex to dec conversion: " + OPCODE);

            }

        }
        #endregion

        #region DMA Channel Threads
        async void DMA_IOExecution(bool readOrWrite)
        {
            if (readOrWrite) // ==> Read
            {
                Task thread = Task.Factory.StartNew(() =>
                {
                    DMA_Block(true);
                    acc = activeProgram.In();
                    DMA_Block(false);
                });
                thread.Wait();
            }
            else // ==> Write
            {
                Task thread = Task.Factory.StartNew(() =>
                {
                    DMA_Block(true);

                    var outputBuffer = cache.Length + activeProgram.InstructionCount + activeProgram.InputBufferSize - 1;
                    
                    for (int i = outputBuffer; i < cache.Length; i++)
                    {
                        if (cache[outputBuffer].Value == "00000000" && cache[outputBuffer].Value == "null")
                        {
                            activeProgram.Out(cache[i]);
                        }
                    }

                    DMA_Block(false);
                });
                thread.Wait();
            }

            // Increment the IO count for the pcb
            activeProgram.IOOperationCount++;
        }

        async Task DMA_Block(bool movePCB)
        {
            if (movePCB) // move PCB to waiting queue
            {
                Task block = Task.Factory.StartNew(() =>
                {
                    Queue.Running.Remove(activeProgram);
                    Queue.Waiting.AddLast(activeProgram);
                });
                block.Wait();
            }
            else // Remove the pcb
            {
                Task block = Task.Factory.StartNew(() =>
                {
                    Queue.Running.AddLast(activeProgram);
                    Queue.Waiting.Remove(activeProgram);
                });
                block.Wait();
            }
        }
        #endregion
    }
}
