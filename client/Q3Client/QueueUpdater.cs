using NLog;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace Q3Client
{
    class QueueUpdater
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();

        private readonly Hub hub;
        private readonly ObservableCollection<Queue> queues = new ObservableCollection<Queue>();
        private Dictionary<int, Queue> queuesById = new Dictionary<int, Queue>();
        private User user;

        private QueueList queueList;
        private DisplayTimer alertDisplayTimer;
        private GroupsCache groupsCache;


        public QueueUpdater(Hub hub, User user, GroupsCache groupsCache)
        {
            this.hub = hub;
            this.user = user;

            this.groupsCache = groupsCache;           

            queueList = new QueueList(hub, groupsCache);
            queueList.Show();

            alertDisplayTimer = new DisplayTimer(queueList);
        }

        public ObservableCollection<Queue> Queues
        {
            get { return queues; }
        }

        public async Task RefreshALl()
        {
            try
            {
                logger.Debug(nameof(RefreshALl));
                var serverQueues = await hub.ListQueues();


                foreach (var q in serverQueues.OrderBy(q => q.Id))
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

                        queueList.Dispatcher.Invoke(() =>
                        {
                            var window =
                                queueList.QueuesPanel.Children.OfType<QueueNotification>()
                                    .FirstOrDefault(w => w.Queue.Id == q.Id);
                            if (window != null)
                            {
                                queueList.QueuesPanel.Children.Remove(window);
                            }
                        });
                    }
                }
            }
            catch (Exception e)
            {
                logger.Error(e, "RefreshAll Failed");
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
            logger.Debug(nameof(AddQueue));

            if (user.EmailAddress.EndsWith("softwire.com") && !string.IsNullOrWhiteSpace(queue.RestrictToGroup) && !groupsCache.UserIsInGroup(queue.RestrictToGroup))
            {
                logger.Info("Queue ignored. User is not in group " + queue.RestrictToGroup);
                return;
            }

            queueList.Dispatcher.Invoke(() =>
            {

                queue.User = user;
                queues.Add(queue);
                queuesById.Add(queue.Id, queue);

                var window = new QueueNotification(queue);
                window.JoinQueue += (s, e) => hub.JoinQueue(queue.Id);
                window.LeaveQueue += (s, e) => hub.LeaveQueue(queue.Id);
                window.ActivateQueue += (s, e) => hub.ActivateQueue(queue.Id);
                window.DeactivateQueue += (s, e) => hub.DeactivateQueue(queue.Id);
                window.CloseQueue += (s, e) => hub.CloseQueue(queue.Id);
                window.NagQueue += (s, e) => hub.NagQueue(queue.Id);
                window.SendMessage += (sender, args) => hub.MessageQueue(queue.Id, args.Message);
                queueList.QueuesPanel.Children.Insert(0, window);
            });

            alertDisplayTimer.ShowAlert();
        }

        public void UpdateQueue(Queue serverQueue)
        {
            logger.Debug(nameof(UpdateQueue));
            if (queuesById.ContainsKey(serverQueue.Id))
            {
                MergeChanges(queuesById[serverQueue.Id], serverQueue);
            }
        }

        public void UpdateQueueStatus(Queue queue)
        {
            logger.Debug(nameof(UpdateQueueStatus));
            if (queue.Members.Contains(user) && queue.Status == QueueStatus.Activated)
            {
                alertDisplayTimer.ShowAlert(true);
            }
            UpdateQueue(queue);
        }

        public void AddQueueMessage(int queueId, User sender, string message, DateTimeOffset timestamp)
        {
            logger.Debug(nameof(AddQueueMessage));
            if (queuesById.ContainsKey(queueId))
            { 
                var q = queuesById[queueId];
                q.AddMessage(new Queue.Message() {Sender = sender, Content = message, Timestamp = timestamp});
            }
        }

        public void NagQueue(int queueId)
        {
            logger.Debug(nameof(NagQueue));
            if (queuesById.ContainsKey(queueId))
            {
                var q = queuesById[queueId];
                if (!q.Members.Contains(user))
                {
                    alertDisplayTimer.ShowAlert();
                    q.Nag();
                }
            }
        }
    }
}
