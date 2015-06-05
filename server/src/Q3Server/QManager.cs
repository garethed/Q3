using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace Q3Server
{
    public interface IQManager
    {
        Queue CreateQueue(string QueueName, string restrictToGroup, User creatingUser);
        Queue GetQueue(int QueueId);
        event EventHandler<QueueEventArgs> queueCreated;

        IEnumerable<Queue> ListQueues();
        void CloseQueue(int id);
    }

    public class QManager : IQManager
    {
        private object lockable = new object();
        private Dictionary<int, Queue> queues = new Dictionary<int, Queue>();
        private int lastQueueId = 0;

        public QManager(IQEventsListener eventListener)
        {
            this.queueCreated += eventListener.OnQueueCreated;
        }

        public Queue CreateQueue(string queueName, string restrictToGroup, User creatingUser)
        {
            lock (lockable)
            {
                if (queues.Any(q => q.Value.Name.Equals(queueName, StringComparison.InvariantCultureIgnoreCase)))
                {
                    throw new DuplicateQueueException();
                }

                var queue = new Queue(++lastQueueId, queueName, creatingUser, restrictToGroup);
                queues.Add(queue.Id, queue);

                if (queueCreated != null)
                {
                    queueCreated(this, new QueueEventArgs(queue));
                }

                return queue;               
            }
        }

        public Queue GetQueue(int queueId)
        {
            if (!queues.ContainsKey(queueId))
            {
                throw new QueueNotFoundException();
            }

            return queues[queueId];
        }

        public IEnumerable<Queue> ListQueues()
        {
            return queues.Values;
        }

        public void CloseQueue(int id)
        {
            lock (lockable)
            {
                if (queues.ContainsKey(id))
                {
                    var queueToRemove = queues[id];
                    queueToRemove.Close();
                    queues.Remove(id);                    
                }
            }
        }

        public event EventHandler<QueueEventArgs> queueCreated;
    }
}