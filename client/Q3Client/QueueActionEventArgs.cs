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

    public class QueueMessageEventArgs  : EventArgs
    {
        private readonly int queueId;
        private readonly User sender;
        private readonly string message;

        public QueueMessageEventArgs(int queueId, string message)
        {
            this.queueId = queueId;
            this.message = message;
        }

        public QueueMessageEventArgs(int queueId, User sender, string message)
        {
            this.queueId = queueId;
            this.sender = sender;
            this.message = message;
        }


        public string Message { get { return message; } }

        public int QueueId
        {
            get { return queueId; }
        }

        public User Sender
        {
            get { return sender; }
        }
    }
}
