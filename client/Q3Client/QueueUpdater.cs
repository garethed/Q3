using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Q3Client
{
    class QueueUpdater
    {
        private readonly Hub hub;
        private readonly ObservableCollection<Queue> queues = new ObservableCollection<Queue>();
        private Dictionary<int, Queue> queuesById = new Dictionary<int, Queue>();
        private string userId;

        private QueueList queueList;


        public QueueUpdater(Hub hub, string userId)
        {
            this.hub = hub;
            this.userId = userId;

            queueList = new QueueList();
            queueList.Show();

        }

        public ObservableCollection<Queue> Queues
        {
            get { return queues; }
        }

        public async Task RefreshALl()
        {
            var serverQueues = await hub.ListQueues();


            foreach (var q in serverQueues)
            {
                if (queuesById.ContainsKey(q.Id))
                {
                    MergeChanges(queuesById[q.Id], q);
                }
                else
                {
                    AddQueue(q);
                }
            }

            foreach (var q in queues.ToList())
            {
                if (!serverQueues.Any(sq => sq.Id == q.Id))
                {
                    queues.Remove(q);
                    queuesById.Remove(q.Id);
                }
            }
            
        }

        private void MergeChanges(Queue clientQueue, Queue serverQueue)
        {
            clientQueue.Name = serverQueue.Name;
            clientQueue.Status = serverQueue.Status;
            clientQueue.Members = serverQueue.Members;
        }

        public void AddQueue(Queue queue)
        {
            
            queues.Add(queue);
            queuesById.Add(queue.Id, queue);

            var window = new QueueNotification(queue, userId);
            window.JoinQueue += (s, e) => hub.JoinQueue(queue.Id);
            window.LeaveQueue += (s, e) => hub.LeaveQueue(queue.Id);
            queueList.QueuesPanel.Children.Add(window);
        }

        public void UpdateQueue(Queue serverQueue)
        {
            MergeChanges(queuesById[serverQueue.Id], serverQueue);

        }
    }
}
