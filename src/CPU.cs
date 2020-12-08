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

                // Acquire the queue lock and remove the cpu
                Driver._QueueLock.Wait();
                Queue.Ready.Remove(activeProgram);
                Queue.Running.AddLast(activeProgram);
                activeProgram.State = PCB.PROCESS_STATE.RUNNING;
                Driver._QueueLock.Release();

                // Set the CPU to not waiting
                isWaiting = false;

                // Sets the program count at 0 at initialization
                // Might need to be refactored
                PC = 0;
                registers = new Word[16];
                cache = new Word[activeProgram.ProgramSize];

                for (int i = 0; i < registers.Length; i++)
                {
                    registers[i] = new Word();
                }

                acc = registers[0];
            }
        }
        #endregion

        #region CPU Attributes
        const string NOPCODE = "13";

        //accumulator, naming it just acc due to my former crippling addiction to Shenzhen IO
        int PC;

        // Indicates if the CPU is waiting or trapped
        public bool isWaiting = true;
        public bool hitTrap = false;

        // Acc to word        
        Word acc;

        // CPU Memory
        Word[] registers;
        Word[] cache;

        public Word[] Registers
        {
            get { return registers; }
            set { registers = value; }
        }

        public Word[] Cache
        {
            get { return cache; }
            set { cache = value; }
        }

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

        int dumpcount = 1;

        public void Run()
        {
            Driver.WriteToFile($"newDumpsP2S1/MCSJF/Dump{dumpcount}", Driver.RAMDUMP());
            dumpcount++;

            while (PC < activeProgram.InstructionCount && !isWaiting)
            {
                // Fetch data
                var instruction = Fetch();

                // Decode data
                Decode(instruction);

                // Execute data
                Execute();
            }

            EndProcess();
        }

        // Ends the process
        private void EndProcess()
        {
            // Deallocate the memory
            MMU.DeallocateMemory(activeProgram);

            // Adds the pcb to the terminated queue
            activeProgram.Core_Used = ID;

            // Remove the program from the running queue
            Driver._QueueLock.Wait();
            var pcb = activeProgram;
            Queue.Running.Remove(pcb);
            Queue.Terminated.AddLast(pcb);
            Driver._QueueLock.Release();

            // Clears the CPU & PCB attributes
            this.registers = null;
            this.activeProgram = null;
            this.PC = 0;
            this.OPCODE = -1;
            this.isWaiting = false;

            // Clear the cache
            this.cache = null;

            // Stop the running time
            Metrics.Stop(pcb);
        }

        #endregion

        #region Fetch Module
        public Word Fetch()
        {
            // Grab the instruction from the cache
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
            return MMU.ReadWord(addr, this.activeProgram).Value;
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

            // Gets the opcode. god knows how
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
            // Console.WriteLine($"ARITH: OPCODE {OPCODE} | {dReg} = {sReg0} OP {sReg1}");
        }

        private void ExecuteCondi()
        {
            string second;
            int preAddr = addr;
            addr /= 4;

            if (dReg == 0)
            {
                second = Utilities.WordFill(Utilities.DecToHex(preAddr));
            }
            else
            {
                second = MMU.ReadWord(addr, activeProgram).Value;
            }

            switch (OPCODE)
            {
                case 2: // 02: ST
                    Driver._MMULock.Wait();
                    DMA.IOExecution(false, this, bReg, addr, false).GetAwaiter().GetResult();
                    Driver._MMULock.Release();
                    break;
                case 3: // 03: LW
                    Driver._MMULock.Wait();
                    DMA.IOExecution(true, this, dReg, addr, false).GetAwaiter().GetResult();
                    Driver._MMULock.Release();
                    break;
                case 11: // 0B: MOVI
                    registers[dReg].Value = Utilities.WordFill(second);
                    break;
                case 12: // 0C: ADDI
                    registers[dReg] = registers[dReg] + MMU.ReadWord(addr, activeProgram);
                    break;
                case 13: // 0D: MULI
                    registers[dReg] = registers[dReg] * MMU.ReadWord(addr, activeProgram);
                    break;
                case 14: // 0E: DIVI
                    registers[dReg] = registers[dReg] / MMU.ReadWord(addr, activeProgram);
                    break;
                case 15: // 0F: LDI
                    registers[dReg] = new Word(second);
                    break;
                case 17: // 11: SLTI
                    registers[dReg].Value = Utilities.DecToHexFullAddr(registers[sReg0].ValueAsInt < addr ? 1 : 0);
                    break;
                case 21: // 15: BEQ
                    PC = registers[bReg] == registers[dReg] ? preAddr : PC;
                    break;
                case 22: // 16: BNE
                    PC = registers[bReg] != registers[dReg] ? preAddr : PC;
                    break;
                case 23: // 17: BEZ
                    PC = registers[bReg].ValueAsInt == 0 ? preAddr : PC;
                    break;
                case 24: // 18: BNZ
                    PC = registers[bReg].ValueAsInt != 0 ? preAddr : PC;
                    break;
                case 25: // 19: BGZ
                    PC = registers[bReg].ValueAsInt > 0 ? preAddr : PC;
                    break;
                case 26: // 1A: BLZ
                    PC = registers[bReg].ValueAsInt < 0 ? preAddr : PC;
                    break;
                default:
                    throw new Exception("OPCode invalid, check the hex to dec conversion: " + OPCODE);
            }
            // Console.WriteLine($"CONDI: OPCODE {OPCODE} | B = {bReg} | D = {dReg} | ADDR = {addr} | SECOND = {second}");
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
            // Console.WriteLine($"UJUMP: OPCODE {OPCODE} | ADDR {addr}");
        }
        private async Task ExecuteIO()
        {
            bool reg2ORAddress = false;
            int fourthValue = addr;

            //if the address is 0, set the addr
            if (addr == 0)
            {
                reg2ORAddress = true;
                fourthValue = reg2;
            }

            switch (OPCODE)
            {
                case 0: // 00: RD
                    // //NOTE: in this snippet, it just reads to acc. additional setup required to send to a different register
                    Driver._MMULock.Wait();
                    await DMA.IOExecution(true, this, reg1, fourthValue, reg2ORAddress);
                    Driver._MMULock.Release();
                    break;
                case 1: // 01: WR
                    isWaiting = true;
                    await DMA.IOExecution(false, this, reg1, fourthValue, reg2ORAddress);
                    Driver._MMULock.Release();
                    break;
                default:
                    throw new Exception("OPCode invalid, check the hex to dec conversion: " + OPCODE);

            }
            // Console.WriteLine($"IO: OPCODE {OPCODE} | REG1 : {reg1} | REG2 : {reg2} |ADDR {addr}");
        }
        #endregion
    }
}
