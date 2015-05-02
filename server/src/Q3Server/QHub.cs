using Microsoft.AspNet.SignalR;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Q3
{
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

        public void JoinQueue(string queueName)
        {
            Trace.WriteLine("-> JoinQueue: " + queueName);
            queueManager.GetQueue(queueName).AddUser(Context.User.Identity.Name);
        }

        public void ActivateQueue(string queueName)
        {
            Trace.WriteLine("-> ActivateQueue: " + queueName);
            queueManager.GetQueue(queueName).Activate();
        }

        public IEnumerable<string> ListQueues()
        {
            return queueManager.ListQueues();
        }
    }
}