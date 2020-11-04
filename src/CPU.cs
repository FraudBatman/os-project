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

                // Sets the program count at 0 at initialization
                // Might need to be refactored
                PC = 0;
                IOOperationCount = 0;
            }
        }
        #endregion


        #region CPU Attributes
        const string NOPCODE = "13";


        //accumulator, naming it just acc due to my former crippling addiction to Shenzhen IO
        int acc, PC, IOOperationCount;

        // CPU registers
        int?[] registers;

        // Save memory by not assigning if CPU is never instantiated
        int id;
        public int ID { get { return id; } }

        // Sets the active to program to notify it is waiting for a program
        public CPU(int id = 0)
        {
            this.id = id;
            activeProgram = null;

            // Initialize the registers
            acc = 0;
            registers = new int?[16];

            // Set the acc
            registers[0] = acc;

            // Set the zero register
            registers[1] = 0;
        }
        #endregion

        #region Threads
        public int Run()
        {
            Queue.Ready.Remove(activeProgram);
            Queue.Running.AddLast(activeProgram);
            activeProgram.State = PCB.PROCESS_STATE.RUNNING;

            while (PC < activeProgram.InstructionCount)
            {
                // Fetch data
                var instruction = Fetch();
                System.Console.WriteLine(instruction.Value);

                // Decode data
                Decode(instruction);

                // Execute data
               Execute();
            }

            EndProcess();

            // Decode
            return 0;
        }

        private async Task IOOperation(Word word)
        {
            // Add functionality for the DMA channel
        }

        private async Task Block()
        {
            // Blocking functionality for multi-core
        }

        // Ends the process
        private void EndProcess()
        {
            // Adds the pcb to the terminated queue
            var pcb = activeProgram;
            Queue.Running.Remove(pcb);
            Queue.Terminated.AddLast(pcb);

            // Clears the CPU & PCB attributes
            this.registers = null;
            this.activeProgram = null;
            this.PC = 0;
            this.ExecutionPointer = null;
            this.OPCODE = -1;
        }

        #endregion

        #region Fetch Module
        public Word Fetch()
        {   
            int[] pageNumbers = MMU.getPages(activeProgram);
            int page = pageNumbers[0];
            var offset = Utilities.DecToHexAddr(PC);

            if(PC >= MMU.PAGE_SIZE)
            {
                page = pageNumbers[1];
                offset = (PC - MMU.PAGE_SIZE).ToString();
            }

            // Grab the instruction from memory
            var physicalAddress = EffectiveAddress(true, page, offset);

            if (physicalAddress == "" || physicalAddress == null)
                throw new Exception("Invalid address, validate effective address for PC");

            PC++;

            return MMU.ReadWord(physicalAddress, activeProgram);
        }

        string EffectiveAddress(bool isDirect, int pageNumber, string offset = "00")
        {
            if(isDirect)
            {
                // Converts the logical addresses to physical
                string directAddress =
                    "0x" +
                    Utilities.DecToHex(pageNumber) +
                    offset
                ;

                return directAddress;
            }
            else
            {
                // Converts to physical indirect address
            }

            return null;
        }
        #endregion


        #region Decode Module
        private int OPCODE, sReg0, sReg1, dReg, addr, bReg, reg1, reg2;
        private string ExecutionPointer, jumpToAddr;

        /// <summary>
        /// Takes an instruction and converts it into a usable format before sending it to an execute
        /// </summary>
        /// <param name="instruction">The instruction to decode</param>
        public void Decode(Word instruction)
        {
            // Convert to binary
            string data = Utilities.HexToBin(instruction.Value);
            System.Console.WriteLine(data);
            
            // Parse the instruction type
            ExecutionPointer = data.Substring(0, 2);

            // Parse the opcode - always 6 bits
            OPCODE = Utilities.BinToDec(data.Substring(2, 6));
            System.Console.WriteLine(OPCODE);

            // If nopcode, do nothing, return
            if (OPCODE == Utilities.HexToDec(NOPCODE))
            {
                ExecutionPointer = null;
                return;
            }

            switch (ExecutionPointer)
            {
                case "00": // => Arithmetic
                    sReg0 = Utilities.BinToDec(data.Substring(8, 4));
                    sReg1 = Utilities.BinToDec(data.Substring(12, 4));
                    dReg = Utilities.BinToDec(data.Substring(16, 4));
                    break;
                case "01": // => Conditional
                    bReg = Utilities.BinToDec(data.Substring(8, 4));
                    dReg = Utilities.BinToDec(data.Substring(8, 4));
                    addr = Utilities.BinToDec(data.Substring(16));
                    break;
                case "10": // => Uncon. Jump -> may need to refactor
                    jumpToAddr = data.Substring(8);
                    break;
                case "11": // => IO
                    reg1 = Utilities.BinToDec(data.Substring(8, 4));
                    reg2 = Utilities.BinToDec(data.Substring(8, 4));
                    addr = Utilities.BinToDec(data.Substring(16));
                    break;
            }
        }
        #endregion


        #region Execute Module
        public void Execute(int opCode)
        {
            switch (ExecutionPointer)
            {
                case "00": // => Arithmetic
                    ExecuteArith();
                    break;
                case "01": // => Conditional
                    ExecuteCondi();
                    break;
                case "10": // => Uncon. Jump
                    ExecuteUJump();
                    break;
                case "11": // => IO
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
                    break;
                case 5:
                    System.Console.WriteLine();
                    break;
                case 6:
                    System.Console.WriteLine();
                    break;
                case 7:
                    System.Console.WriteLine();
                    break;
                case 8:
                    System.Console.WriteLine();
                    break;
                case 9:
                    System.Console.WriteLine();
                    break;
                case 10:
                    System.Console.WriteLine();
                    break;
                default:
                    throw new Exception("OPCode invalid, check the dec to hex conversion: " + OPCODE);
            }

            // //04: MOV
            // //Yeah you see the notes from before? those by like a million. skipping for now...

            // //05: ADD
            // //for 05-0A IDK if we'll have more than the two sRegs so keep that in mind
            // dReg = sReg[0] + sReg[1];

            // //06: SUB
            // dReg = sReg[0] - sReg[1];

            // //07: MUL
            // dReg = sReg[0] * sReg[1];

            // //08: DIV
            // dReg = sReg[0] / sReg[1];

            // //09: AND
            // dReg = sReg[0] & sReg[1];

            // //0A: OR
            // dReg = sReg[0] | sReg[1];

            // //10: SLT
            // //Ternaries make me wet
            // //Also we should probably get onto a prop for turning value into an int
            // dReg = (sReg[0].ValueToInt < 0 ? 1 : 0);
        }

        private void ExecuteCondi()
        {
            switch (OPCODE)
            {
                case 2: // 02: ST
                    System.Console.WriteLine();
                    break;
                case 3: // 03: LW
                    System.Console.WriteLine();
                    break;
                case 11: // 0B: MOVI
                    System.Console.WriteLine();
                    break;
                case 12: // 0C: ADDI
                    System.Console.WriteLine();
                    break;
                case 13: // 0D: MULI
                    System.Console.WriteLine();
                    break;
                case 14: // 0E: DIVI
                    System.Console.WriteLine();
                    break;
                case 15: // 0F: LDI
                    System.Console.WriteLine();
                    break;
                case 17: // 11: SLTI
                    System.Console.WriteLine();
                    break;
                case 21: // 15: BEQ
                    System.Console.WriteLine();
                    break;
                case 22: // 16: BNE
                    System.Console.WriteLine();
                    break;
                case 23: // 17: BEZ
                    System.Console.WriteLine();
                    break;
                case 24: // 18: BNZ
                    System.Console.WriteLine();
                    break;
                case 25: // 19: BGZ
                    System.Console.WriteLine();
                    break;
                case 26: // 1A: BLZ
                    System.Console.WriteLine();
                    break;
                default:
                    throw new Exception("OPCode invalid, check the dec to hex conversion: " + OPCODE);
            }
        }
        private void ExecuteUJump()
        {
            switch (OPCODE)
            {
                case 18: // 12: HLT
                    System.Console.WriteLine();
                    // //End of the program. This would require a lot of info going everywhere, so it'd probably be best if we put this in its own function
                    break;
                case 20: // 14: JMP
                    System.Console.WriteLine();
                    // //Takes the value given and sets the PCB's prog. count. to it
                    // PCB.ProgramCounter = jumpAddr;
                    break;
            }
        }
        private void ExecuteIO()
        {
            switch (OPCODE)
            {
                case 0: // 00: RD
                    System.Console.WriteLine();
                    // //NOTE: in this snippet, it just reads to acc. additional setup required to send to a different register
                    // acc = PCB.In;
                    break;
                case 1: // 01: WR
                    System.Console.WriteLine();
                    // PCB.Out = acc;
                    break;
                default:
                    throw new Exception("OPCode invalid, check the dec to hex conversion: " + OPCODE);
                
            }

        }
        #endregion
    }
}
