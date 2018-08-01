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
using Microsoft.AspNet.SignalR.Client.Transports;
using System.Windows.Threading;

namespace Q3Client
{
    public class Hub : INotifyPropertyChanged
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();

        private readonly User user;

        private HubConnection hubConnection;
        private IHubProxy hub;
        private Dispatcher dispatcher;


        public Hub(User user, Dispatcher dispatcher)
        {
            this.user = user;
            this.dispatcher = dispatcher;

#if DEBUG && !LIVE
            hubConnection = new HubConnection("http://localhost:51443/");
#else
            hubConnection = new HubConnection("http://poolq3.zoo.lan/"); 
#endif

            hubConnection.TraceLevel = TraceLevels.All;
            hubConnection.TraceWriter = new NLogTextWriter("signalr");

            hub = hubConnection.CreateHubProxy("QHub");
            hub.On<Queue>("NewQueue", q => RaiseEvent("created", QueueCreated, q));
            hub.On<Queue>("QueueMembershipChanged", q => RaiseEvent("membershipchanged", QueueMembershipChanged, q));
            hub.On<Queue>("QueueStatusChanged", q => RaiseEvent("statuschanged", QueueStatusChanged, q));
            hub.On<int, User, string, DateTimeOffset>("QueueMessageSent", RaiseMessageEvent);
            hub.On<int>("NagQueue", id => RaiseEvent("nag", QueueNagged, id));
            hubConnection.Headers["User"] = this.user.ToString();
            hubConnection.StateChanged += HubConnectionOnStateChanged;
            hubConnection.Error += e => logger.Error(e, "hub error");

            TryConnect();
        }

        private async Task TryConnect()
        {
            try
            {
                logger.Info("Attempting to connect");
                await hubConnection.Start().ConfigureAwait(false);
                logger.Info("Finished connection attempt");
            }
            catch (Exception e)
            {
                logger.Warn(e, "Cannot connect");
            }
        }

        private async void HubConnectionOnStateChanged(StateChange stateChange)
        {
            logger.Info("hub connection state changed: " + stateChange.OldState + " -> " + stateChange.NewState);
            OnPropertyChanged("ConnectionState");
            if (hubConnection.State == ConnectionState.Disconnected)
            {
                logger.Info("Queuing new connection attempt in 10s");
                await Task.Delay(TimeSpan.FromSeconds(10)).ConfigureAwait(false);
                await TryConnect().ConfigureAwait(false);
            }
        }

        public ConnectionState ConnectionState
        {
            get { return hubConnection.State; }
        }

        public async Task<IEnumerable<Queue>> ListQueues()
        {
            logger.Debug("listqueues");
            return await hub.Invoke<IEnumerable<Queue>>("ListQueues");
        }

        public async Task CreateQueue(string queueName, string restrictToGroup)
        {
            logger.Debug("createqueue");
            await hub.Invoke("StartQueue", queueName, restrictToGroup ?? "");
            
        }

        public async Task JoinQueue(int queueId)
        {
            logger.Debug("joinqueue " + queueId);
            await hub.Invoke("JoinQueue", queueId);            
        }

        public async Task LeaveQueue(int queueId)
        {
            logger.Debug("leavequeue " + queueId);
            await hub.Invoke("LeaveQueue", queueId);
        }

        public async Task ActivateQueue(int queueId)
        {
            logger.Debug("activatequeue " + queueId);
            await hub.Invoke("ActivateQueue", queueId);
        }

        public async Task DeactivateQueue(int queueId)
        {
            logger.Debug("deactivatequeue " + queueId);
            await hub.Invoke("DeactivateQueue", queueId);
        }

        public async Task NagQueue(int queueId)
        {
            logger.Debug("nagqueue " + queueId);
            await hub.Invoke("NagQueue", queueId);
        }

        public async Task CloseQueue(int queueId)
        {
            logger.Debug("closequeue " + queueId);
            await hub.Invoke("CloseQueue", queueId);
        }

        public async Task MessageQueue(int queueId, string message)
        {
            logger.Debug("messagequeue " + queueId + " - " + message);
            await hub.Invoke("MessageQueue", queueId, message);
        }

        #region Events triggered by server

        public event EventHandler<QueueActionEventArgs> QueueCreated;
        public event EventHandler<QueueActionEventArgs> QueueMembershipChanged;
        public event EventHandler<QueueActionEventArgs> QueueStatusChanged;
        public event EventHandler<QueueMessageEventArgs> QueueMessageReceived;
        public event EventHandler<QueueIdEventArgs> QueueNagged;

        private void RaiseEvent(string name, EventHandler<QueueActionEventArgs> eventHandler, Queue queue)
        {
            logger.Info("hub " + name + " " + queue.Id + " " + queue.Status);
            eventHandler.SafeInvoke(this, new QueueActionEventArgs(queue));
        }

        private void RaiseEvent(string name, EventHandler<QueueIdEventArgs> eventHandler, int id)
        {
            logger.Info("hub " + name + " " + id);
            eventHandler.SafeInvoke(this, new QueueIdEventArgs(id));
        }

        private void RaiseMessageEvent(int queueId, User sender, string message, DateTimeOffset timestamp)
        {
            logger.Info("hub message " + queueId + " " + sender + " " + message);
            QueueMessageReceived.SafeInvoke(this, new QueueMessageEventArgs(queueId, sender, message, timestamp));
        }

        #endregion

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            dispatcher.Invoke(() =>
            {
                logger.Info("hub changed " + propertyName);
                PropertyChangedEventHandler handler = PropertyChanged;
                if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
            });
        }
    }
}
