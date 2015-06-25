using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Q3Client
{
    public class QueueActionEventArgs : EventArgs
    {
        private readonly Queue queue;

        public QueueActionEventArgs(Queue queue)
        {
            this.queue = queue;
        }

        public Queue Queue { get { return queue;  } }
    }

    public class QueueMessageEventArgs : QueueActionEventArgs
    {
        private readonly string message;

        public QueueMessageEventArgs(Queue queue, string message) : base(queue)
        {
            this.message = message;
        }

        public string Message { get { return message; } }
    }
}
