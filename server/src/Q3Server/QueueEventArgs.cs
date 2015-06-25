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

    public class QueueMessageEventArgs : QueueEventArgs
    {
        private readonly Queue.Message message;

        public QueueMessageEventArgs(Queue queue, Queue.Message message) : base(queue)
        {
            this.message = message;
        }

        public Queue.Message Message
        {
            get { return message; }
        }
    }
}