using System.Collections.Generic;

namespace os_project
{
    public class ShortTermScheduler
    {
        const SchedulerPolicy POLICY = SchedulerPolicy.FIFO;
        public void Start()
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
        private void load_FIFO()
        {
            sendToDispatch(Queue.New);
        }

        /// <summary>
        /// Sorts the list by priority, then send them to dispatcher (prolly with a foreach again lmao)
        /// </summary>
        private void load_PRIO()
        {
            var toSort = Queue.New;

            basicBitchInsertSort(toSort);

            sendToDispatch(toSort);
        }

        /// <summary>
        /// foreaches the sent list to the dispatcher
        /// </summary>
        /// <param name="kachow">the list to send</param>
        private void sendToDispatch(LinkedList<PCB> kachow)
        {
            foreach (PCB pcb in kachow)
            {
                Dispatcher.Dispatch(pcb);
            }
        }

        /// <summary>
        /// Oh god please avert your eyes from this monstrosity
        /// </summary>
        private LinkedList<PCB> basicBitchInsertSort(LinkedList<PCB> sortee)
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

    enum SchedulerPolicy
    {
        FIFO,
        Priority
    }
}