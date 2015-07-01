using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNet.SignalR.Client;
using NLog;

namespace Q3Client
{
    public class Hub : INotifyPropertyChanged
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();

        private readonly User user;

        private HubConnection hubConnection;
        private IHubProxy hub;


        public Hub(User user)
        {
            this.user = user;
#if DEBUG
            hubConnection = new HubConnection("http://localhost:51442/");
#else
            hubConnection = new HubConnection("http://poolq3/");
#endif
            hub = hubConnection.CreateHubProxy("QHub");
            hub.On<Queue>("NewQueue", q => RaiseEvent("created", QueueCreated, q));
            hub.On<Queue>("QueueMembershipChanged", q => RaiseEvent("membershipchanged", QueueMembershipChanged, q));
            hub.On<Queue>("QueueStatusChanged", q => RaiseEvent("statuschanged", QueueStatusChanged, q));
            hub.On<int, User, string>("QueueMessageSent", RaiseMessageEvent);
            hubConnection.Headers["User"] = this.user.ToString();
            hubConnection.StateChanged += HubConnectionOnStateChanged;

            TryConnect();
        }

        private async Task TryConnect()
        {
            try
            {
                logger.Info("Attempting to connect");
                await hubConnection.Start();
            }
            catch (Exception e)
            {
                logger.Warn(e, "Cannot connect");
            }
        }

        private async void HubConnectionOnStateChanged(StateChange stateChange)
        {
            logger.Info("hub: " + stateChange.OldState + " -> " + stateChange.NewState);
            OnPropertyChanged("ConnectionState");
            if (hubConnection.State == ConnectionState.Disconnected)
            {
                await Task.Delay(TimeSpan.FromSeconds(10));
                await TryConnect();
            }
        }

        public ConnectionState ConnectionState
        {
            get { return hubConnection.State; }
        }

        public async Task<IEnumerable<Queue>> ListQueues()
        {
            return await hub.Invoke<IEnumerable<Queue>>("ListQueues");
        }

        public async Task CreateQueue(string queueName, string restrictToGroup)
        {
            await hub.Invoke("StartQueue", queueName, restrictToGroup ?? "");
            
        }

        public async Task JoinQueue(int queueId)
        {
            await hub.Invoke("JoinQueue", queueId);
            
        }

        public async Task LeaveQueue(int queueId)
        {
            await hub.Invoke("LeaveQueue", queueId);
        }

        public async Task ActivateQueue(int queueId)
        {
            await hub.Invoke("ActivateQueue", queueId);
        }

        public async Task CloseQueue(int queueId)
        {
            await hub.Invoke("CloseQueue", queueId);
        }

        public async Task MessageQueue(int queueId, string message)
        {
            await hub.Invoke("MessageQueue", queueId, message);
        }


        #region Events triggered by server

        public event EventHandler<QueueActionEventArgs> QueueCreated;
        public event EventHandler<QueueActionEventArgs> QueueMembershipChanged;
        public event EventHandler<QueueActionEventArgs> QueueStatusChanged;
        public event EventHandler<QueueMessageEventArgs> QueueMessageReceived;

        private void RaiseEvent(string name, EventHandler<QueueActionEventArgs> eventHandler, Queue queue)
        {
            logger.Info("hub " + name + " " + queue.Id);
            eventHandler.SafeInvoke(this, new QueueActionEventArgs(queue));
        }

        private void RaiseMessageEvent(int queueId, User sender, string message)
        {
            logger.Info("hub messaage " + queueId + " " + sender + " " + message);
            QueueMessageReceived.SafeInvoke(this, new QueueMessageEventArgs(queueId, sender, message));

        }

        #endregion

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            logger.Info("hub changed " + propertyName);
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }

    }
}
