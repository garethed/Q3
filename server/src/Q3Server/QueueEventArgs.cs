using System;

namespace Q3Server
{
    public class QueueEventArgs : EventArgs
    {
        private readonly Queue queue;

        public QueueEventArgs(Queue queue)
        {
            this.queue = queue;
        }

        public Queue Queue
        {
            get { return queue; }
        }
    }
}