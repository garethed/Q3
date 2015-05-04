using Microsoft.AspNet.Hosting;
using Microsoft.AspNet.SignalR;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Q3Server
{
    [Authorize]
    public class QHub : Hub
    {
        private IQManager queueManager;
        private IUserAccessor userAccessor;

        public QHub(IQManager queueManager, IUserAccessor userAccessor)
        {
            this.queueManager = queueManager;
            this.userAccessor = userAccessor;
        }

        public void StartQueue(string queueName)
        {
            Trace.WriteLine("-> StartQueue: " + queueName);
            queueManager.CreateQueue(queueName, userAccessor.User);

        }

        public void JoinQueue(int id)
        {
            Trace.WriteLine("-> JoinQueue: " + id);
            queueManager.GetQueue(id).AddUser(userAccessor.User);
        }

        public void ActivateQueue(int id)
        {
            Trace.WriteLine("-> ActivateQueue: " + id);
            queueManager.GetQueue(id).Activate();
        }

        public IEnumerable<Queue> ListQueues()
        {
            return queueManager.ListQueues();
        }
    }
}