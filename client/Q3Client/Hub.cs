using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNet.SignalR.Client;

namespace Q3Client
{
    public class Hub
    {
        private readonly string userId;

        private HubConnection hubConnection;
        private IHubProxy hub;


        public Hub(string userId)
        {
            this.userId = userId;

            hubConnection = new HubConnection("http://localhost:51442/");
            hub = hubConnection.CreateHubProxy("QHub");
            hub.On<Queue>("NewQueue", q => RaiseEvent(QueueCreated, q));
            hub.On<Queue>("QueueMembershipChanged", q => RaiseEvent(QueueMembershipChanged, q));
            hub.On<Queue>("QueueStatusChanged", q => RaiseEvent(QueueStatusChanged, q));
            hubConnection.Headers["User"] = userId;
            hubConnection.Start().Wait();
        }


        public async Task<IEnumerable<Queue>> ListQueues()
        {
            return await hub.Invoke<IEnumerable<Queue>>("ListQueues");
        }

        public async Task CreateQueue(string queueName)
        {
            await hub.Invoke("StartQueue", queueName);
            
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


        #region Events triggered by server

        public event EventHandler<QueueActionEventArgs> QueueCreated;
        public event EventHandler<QueueActionEventArgs> QueueMembershipChanged;
        public event EventHandler<QueueActionEventArgs> QueueStatusChanged;

        private void RaiseEvent(EventHandler<QueueActionEventArgs> eventHandler, Queue queue)
        {
            eventHandler.SafeInvoke(this, new QueueActionEventArgs(queue));
        }

        #endregion

    }
}
