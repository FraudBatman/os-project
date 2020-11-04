using System;
using System.Threading.Tasks;


namespace os_project
{
    // This class needs to be object oriented -> encapuslate as much as possible
    // One way communication from dispatcher to driver, driver sends commands to the CPU
    public class CPU
    {
        /* So look we all know that the opcode stuff isn't gonna be fun for anyone
        I'm just gonna write a bunch of snippets for each instruction
        And Jess can figure out how to implement them when we get there
        mkay? mkay. -Nic
        */

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
                activeProgram.ProgramCount = 0;
            }
        }
        #endregion


        #region CPU Attributes
        //accumulator, naming it just acc due to my former crippling addiction to Shenzhen IO
        Word acc;

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
            // Fetch();
            System.Console.WriteLine(Fetch().Value);

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

        #endregion

        #region Fetch Module
        Word Fetch()
        {
            // May need to be set outside of the method
            int pc = activeProgram.ProgramCount;

            // Grab the instruction from memory
            var physicalAddress = EffectiveAddress();

            if (physicalAddress == "" || physicalAddress == null)
                throw new Exception("Invalid address, validate effective address for PC");

            // Return the instruction
            return MMU.ReadWord(physicalAddress, activeProgram);
        }

        string EffectiveAddress()
        {
            // Converts the logical addresses to physical
            var pageNumber = ActiveProgram.ProgramCount.ToString();
            var offset = "00";

            string directAddress =
                "0x" +
                pageNumber +
                offset
            ;

            return directAddress;
        }
        #endregion


        #region Decode Module
        string Decode(Word instruction)
        {
            // //any variables called here are used for some snippets, just temporary for now
            // string outAddr = "";
            // string inAddr = "";
            // string jumpAddr = "";
            // Word[] sReg = new Word[2]; //seriously this shit be temporary idk how many sregs are required
            // Word dReg;
            // Word compare;

            // //current info for which word to read in I buffer or write in O buffer is probably a PCB thing
            // //will call it as PCB.In and PCB.Out
            // //Also PCB.ProgramCounter is the ProgramCounter for what step the CPU needs to run next;

            // //13: NOP
            // //Well that was easy.
            return "";
        }
        #endregion


        #region Execute Module
        public void ExecuteArith(int OPCode, int sReg0, int sReg1, int dReg)
        {
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
        public void ExecuteCondi(int OPCode, int bReg, int dReg, int addr)
        {
            // //02: ST
            // //requires address to store it to, string outAddr
            // //NOTE: in this snippet, it just writes the acc. addtional setup required to write from a different register
            // MMU.WriteWord(outAddr, activeProgram, acc);

            // //03: LW
            // //NOTE: in this snippet, it just reads to acc. additional setup required to send to a different register
            // acc = MMU.ReadWord(inAddr, activeProgram);

            // //0B: MOVI
            // //Not sure what the difference between 04 and 0B is, still skipping

            // //0C - 0E: ADDI - DIVI
            // //Cross the issue of 05-0A with the issue of 04 and you get one hell of a skip from me

            // //0F: LDI
            // //Imma be honest I have no clue here

            // //11: SLTI
            // //I'm calling this one "Salty". That's my nickname for it now.
            // //Salty requires another word to compare to
            // //also word comparison operators
            // dReg = (sReg[0] < compare ? 1 : 0);


            // //15-1A: BEQ - BLZ
            // //Branches probably just means jump to a spot for each of these functions
            // //Again, hard pass for right now
        }
        public void ExecuteUJump(int OPCode, int addr)
        {
            // //12: HLT
            // //End of the program. This would require a lot of info going everywhere, so it'd probably be best if we put this in its own function

            // //14: JMP
            // //Takes the value given and sets the PCB's prog. count. to it
            // PCB.ProgramCounter = jumpAddr;
        }
        public void ExecuteIO(int OPCode, int Reg0, int Reg1, int addr)
        {
            // //OPCD 00: INSTR RD
            // //NOTE: in this snippet, it just reads to acc. additional setup required to send to a different register
            // acc = PCB.In;

            // //01: WR
            // PCB.Out = acc;
        }
        #endregion
    }
}
