using System.Collections.Generic;

namespace os_project
{
    public static class ShortTermScheduler
    {
        public static SchedulerPolicy POLICY = SchedulerPolicy.FIFO;
        public static void Start()
        {
            switch (POLICY)
            {
                case SchedulerPolicy.FIFO:
                    load_FIFO();
                    break;
                case SchedulerPolicy.Priority:
                    load_PRIO();
                    break;

                default:
                    throw new System.Exception("Dear you,\nWhat the fuck is this scheduler policy???\n-ShortTermScheduler.Start()");
            }
        }

        /// <summary>
        /// Sends to dispatcher with First In, First Out (so basically just a foreach lmao)
        /// </summary>
        static void load_FIFO()
        {
            SendToDispatcher(Queue.Ready);
        }

        /// <summary>
        /// Sorts the list by priority, then send them to dispatcher (prolly with a foreach again lmao)
        /// </summary>
        static void load_PRIO()
        {
            Driver._QueueLock.Wait();
            var toSort = Queue.Ready;
            Queue.Ready = InsertSort(toSort);
            Driver._QueueLock.Release();
            SendToDispatcher(Queue.Ready);
        }

        /// <summary>
        /// Iterates sending pcbs from the ready queue to the dispatcher for loading to cores
        /// </summary>
        /// <param name="queuedList">the list to send</param>
        static void SendToDispatcher(LinkedList<PCB> queuedList)
        {
            // Multi core configuration
            if (Driver.IsMultiCPU)
            {
                var status = 0;

                while (status != -1)
                {
                    if (queuedList.First == null)
                        return;

                    status = Dispatcher.Dispatch(queuedList.First.Value);
                }
            }

            // Single core configuration
            else
            {
                if (queuedList.First == null)
                    return;

                Dispatcher.Dispatch(queuedList.First.Value);
            }
        }

        /// <summary>
        /// Sorts the queue based on priority from min to max values by insertion
        /// - O(n^2) complexity
        /// </summary>
        public static LinkedList<PCB> InsertSort(LinkedList<PCB> sortee)
        {

            //Jess, if you're looking at this, I apolgize for what you're about to see.
            //It's not pretty. I'm not proud.

            var returnValue = new LinkedList<PCB>();
            var runCount = sortee.Count;

            for (int run = 0; run < runCount; run++)
            {
                //incrementing index for the foreach
                int currentIndex = 0;

                //index of highest priority
                int HPindex = 0;    //Also known as the HP Reverb G2 gotemm

                //shitty highest priority tracker
                //I'm so sorry for your eyes
                PCB highestPriority = new PCB(0, 0, 9999, 0, 0, 0, 0);

                //find the highest priority PCB
                foreach (PCB pcb in sortee)
                {
                    if (pcb.Priority < highestPriority.Priority)
                    {
                        HPindex = currentIndex;
                        highestPriority = pcb;
                    }
                    currentIndex++;
                }

                //add it to the new queue and kill it from the old queue
                returnValue.AddLast(highestPriority);
                sortee.Remove(highestPriority);

            }
            

            return returnValue;
        }
    }

    public enum SchedulerPolicy
    {
        FIFO,
        Priority
    }
}