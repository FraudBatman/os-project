using System;
using System.Collections.Generic;

namespace os_project 
{
    public class PCB
    {
        #region Job Data
        private int jobID;
        private int jobInstructNum;
        private int jobPriortyNum;
        private int addToDiskPriority;
        private bool inMemory;
        #endregion

        #region Disk & Memory Data
        #endregion

        #region Constructor
        // For example, the ‘// Job 1 17 2’ control card of Job1 is processed by discarding the ‘//’, noting that
        // the ‘1’ is the ID of the first job, noting that ‘17’ (or 23 in decimal) is the number of words that constitute
        // the instructions of Job 1, and ‘2’ is the priority-number(to be used for scheduling) of Job 1. All the
        // numbers are in hex.Following the Job-control card are the actual instructions – one instruction per line
        // in the program-file, which must also be extracted and stored in disk
        public PCB(int _jobID, int _jobInstructNum, int _jobPriortyNum)
        {
            jobID = jobID;

        }
        #endregion

        #region Getters & Setter
        #endregion

        #region Functions
        #endregion
    }
}