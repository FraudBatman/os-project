using System;
using System.Collections.Generic;

namespace os_project
{
    /// <summary>
    /// Manages the queues that will be used throughout the CPU simulator
    /// 
    /// Each one is created as a LinkedList which contains the necessary pointers pre-loaded
    /// </summary>
    public static class Queue
    {
        public static LinkedList<PCB> New = new LinkedList<PCB>();
        public static LinkedList<PCB> Ready = new LinkedList<PCB>();
        public static LinkedList<PCB> Running = new LinkedList<PCB>();
        public static LinkedList<PCB> Waiting = new LinkedList<PCB>();
        public static LinkedList<PCB> Terminated = new LinkedList<PCB>();
    }
}