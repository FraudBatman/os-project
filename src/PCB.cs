using System;
using System.Collections.Generic;

namespace os_project 
{
    public class PCB
    {
        #region Job Data
        public enum STATUS {New, Ready, Waiting, Run, Terminate}
        private int jobID;
        private int jobInstructNum;
        private int jobPriortyNum;
        private int addToDiskPriority;
        private bool inMemory;
        public STATUS jobStatus;
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
            jobID = _jobID;
            jobInstructNum = _jobInstructNum;
            jobPriortyNum = _jobPriortyNum;

            jobStatus = STATUS.New;
            inMemory = false;
        }
        #endregion

        #region Getters & Setter
        //Job ID
        public int getJobID()
        {
            return jobID;
        }

        public void setJobID(int _jobID)
        {
            jobID = _jobID;
        }

        //Job Instruction Number
        public int getJobInstructNum()
        {
            return jobInstructNum;
        }

        public void setJobInstructNum(int _jobInstructNum)
        {
            jobInstructNum = +jobInstructNum;
        }

        //Job Priority Number
        public int getJobPriorityNum()
        {
            return jobPriortyNum;
        }

        public void setJobPriorityNum(int _jobPriortyNum)
        {
            jobPriortyNum = _jobPriortyNum;
        }

        //Job inMemory
        public bool isInMemory()
        {
            return inMemory;
        }

        public void setInMemory(bool memoryStatus)
        {
            inMemory = memoryStatus;
        }

        //Job Status
        public STATUS getJobStatus()
        {
            return jobStatus;
        }

        public void setJobStatus(STATUS newStatus)
        {
            jobStatus = newStatus;
        }
        #endregion

        #region Functions
        //toString
        public string toString()
        {
            string output = String.Format("Job ID: {0} \nJob Instruction Number: {1} \nJob Priority Number: {2} \nJob InMemory: {3} \nJob Status: {4}",
                jobID, jobInstructNum, jobPriortyNum, inMemory, jobStatus);

            return output;
        }
        #endregion
    }
}
