namespace os_project
{

    public class CPU
    {
        /* So look we all know that the opcode stuff isn't gonna be fun for anyone
        I'm just gonna write a bunch of snippets for each instruction
        And Jess can figure out how to implement them when we get there
        mkay? mkay. -Nic
        */

        //accumulator, naming it just acc due to my former crippling addiction to Shenzhen IO
        Word acc;

        //PCB for program CPU is currently running
        PCB activeProgram;

        /*
        public void opcodeSnips()
        {
            //any variables called here are used for some snippets, just temporary for now
            string outAddr = "";
            string inAddr = "";
            string jumpAddr = "";
            Word[] sReg = new Word[2]; //seriously this shit be temporary idk how many sregs are required
            Word dReg;
            Word compare;

            //current info for which word to read in I buffer or write in O buffer is probably a PCB thing
            //will call it as PCB.In and PCB.Out
            //Also PCB.ProgramCounter is the ProgramCounter for what step the CPU needs to run next;

            //OPCD 00: INSTR RD
            //NOTE: in this snippet, it just reads to acc. additional setup required to send to a different register
            acc = PCB.In;

            //01: WR
            PCB.Out = acc;

            //02: ST
            //requires address to store it to, string outAddr
            //NOTE: in this snippet, it just writes the acc. addtional setup required to write from a different register
            MMU.WriteWord(outAddr, activeProgram, acc);

            //03: LW
            //NOTE: in this snippet, it just reads to acc. additional setup required to send to a different register
            acc = MMU.ReadWord(inAddr, activeProgram);

            //04: MOV
            //Yeah you see the notes from before? those by like a million. skipping for now...

            //05 - 0A: ADD - OR
            //This is gonna require additional operations for each of these, sounds like future us problem to me.

            //0B: MOVI
            //Not sure what the difference between 04 and 0B is, still skipping

            //0C - 0E: ADDI - DIVI
            //Cross the issue of 05-0A with the issue of 04 and you get one hell of a skip from me

            //10: SLT
            //Ternaries make me wet
            //Also we should probably get onto a prop for turning value into an int
            dReg = (sReg[0].ValueToInt < 0 ? 1 : 0);

            //11: SLTI
            //I'm calling this one "Salty". That's my nickname for it now.
            //Salty requires another word to compare to
            //also word comparison operators
            dReg = (sReg[0] < compare ? 1 : 0);

            //12: HLT
            //End of the program. This would require a lot of info going everywhere, so it'd probably be best if we put this in its own function

            //13: NOP
            //Well that was easy.

            //14: JMP
            //Takes the value given and sets the PCB's prog. count. to it
            PCB.ProgramCounter = jumpAddr;

            //15-1A: BEQ - BLZ
            //Branches probably just means jump to a spot for each of these functions
            //Again, hard pass for right now
        }
        */
    }
}
