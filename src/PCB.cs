public class PCB {
    
    int cpuid = 0; // CPU ID, init to first CPU
    int program_counter = 0; // program counter in terms of words
    
    // TODO: state
    int code_size = 0; // size of the program in words as pulled from JOB
    Status status; // status of the job
    int priority = 0; // priority of the job, with 0 at max priority

    enum Status {
        Running, Ready, Blocked, New
    }

    //test comment
    //other test comment

}
