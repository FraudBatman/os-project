# Memory and you: Nic's understanding of Logical and Physical memory

1) PCB stores physical memory location of start points for program, I/O buffers
- For example, a program that starts at 0, goes to 256, and needs 20 for both buffers would set start points at 0 (program), 257 (input buffer), and 277 (output buffer).

2) When CPU calls for an address, it takes the logical address and adds the start address to it
- Following the above example, if the CPU calls for [3] in the input buffer, it calls for the PCU's start address for the input buffer and adds the logical address, resulting in the CPU calling to address 260.