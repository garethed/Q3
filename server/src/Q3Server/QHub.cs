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

        public QHub(IQManager queueManager)
        {
            this.queueManager = queueManager;
        }

        public void StartQueue(string queueName)
        {
            Trace.WriteLine("-> StartQueue: " + queueName);
            queueManager.CreateQueue(queueName);
        }

        public void JoinQueue(int id)
        {
            Trace.WriteLine("-> JoinQueue: " + id);
            queueManager.GetQueue(id).AddUser(Context.User.Identity.Name);
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