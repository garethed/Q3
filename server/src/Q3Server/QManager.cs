using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace Q3
{
    public interface IQManager
    {
        Queue CreateQueue(string QueueName);
        Queue GetQueue(string QueueName);
        event EventHandler<QueueCreatedEventArgs> queueCreated;

        IEnumerable<string> ListQueues();
    }

    public class QManager : IQManager
    {
        public QManager(QEventsListener eventListener)
        {
            this.queueCreated += eventListener.OnQueueCreated;
        }

        ConcurrentDictionary<string, Queue> queues = new ConcurrentDictionary<string, Queue>();

        public Queue CreateQueue(string queueName)
        {
            var queue = new Queue(queueName);
            if (queues.TryAdd(queueName, queue))
            {
                if (queueCreated != null)
                {
                    queueCreated(this, new QueueCreatedEventArgs() { Queue = queue });
                }

                return queue;
            }

            throw new DuplicateQueueException();
        }

        public Queue GetQueue(string queueName)
        {
            var queue = queues[queueName];

            if (queue == null)
            {
                throw new QueueNotFoundException();
            }

            return queue;
        }

        public IEnumerable<string> ListQueues()
        {
            return queues.Keys;
        }

        public event EventHandler<QueueCreatedEventArgs> queueCreated;
    }

    public class QueueCreatedEventArgs
    {
        public Queue Queue;
    }
}