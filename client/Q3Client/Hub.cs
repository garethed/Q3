using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNet.SignalR.Client;

namespace Q3Client
{
    public class Hub
    {
        private readonly User user;

        private HubConnection hubConnection;
        private IHubProxy hub;


        public Hub(User user)
        {
            this.user = user;

            hubConnection = new HubConnection("http://localhost:51442/");
            hub = hubConnection.CreateHubProxy("QHub");
            hub.On<Queue>("NewQueue", q => RaiseEvent("created", QueueCreated, q));
            hub.On<Queue>("QueueMembershipChanged", q => RaiseEvent("membershipchanged", QueueMembershipChanged, q));
            hub.On<Queue>("QueueStatusChanged", q => RaiseEvent("statuschanged", QueueStatusChanged, q));
            hubConnection.Headers["User"] = this.user.ToString();
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

        private void RaiseEvent(string name, EventHandler<QueueActionEventArgs> eventHandler, Queue queue)
        {
            Trace.WriteLine("hub " + name + " " + queue.Id);
            eventHandler.SafeInvoke(this, new QueueActionEventArgs(queue));
        }

        #endregion

    }
}
