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

    public class QueueIdEventArgs : EventArgs
    {
        private readonly int id;

        public QueueIdEventArgs(int id)
        {
            this.id = id;
        }

        public int QueueId { get { return id; } }
    }


    public class QueueMessageEventArgs  : QueueIdEventArgs
    {
        private readonly User sender;
        private readonly string message;

        public QueueMessageEventArgs(int queueId, string message) : base(queueId)
        {
            this.message = message;
        }

        public QueueMessageEventArgs(int queueId, User sender, string message) : base(queueId)
        {
            this.sender = sender;
            this.message = message;
        }

        public string Message { get { return message; } }

        public User Sender
        {
            get { return sender; }
        }
    }
}
