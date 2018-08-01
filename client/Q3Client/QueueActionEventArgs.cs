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

        public QueueMessageEventArgs(int queueId, string message) : base(queueId)
        {
            Message = message;
        }

        public QueueMessageEventArgs(int queueId, User sender, string message, DateTimeOffset timestamp) : base(queueId)
        {
            Sender = sender;
            Message = message;
            Timestamp = timestamp;
        }

        public string Message { get; }
        public User Sender { get; }
        public DateTimeOffset Timestamp { get; }
    }
}
