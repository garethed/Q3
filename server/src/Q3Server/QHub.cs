using Microsoft.AspNet.SignalR;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.Owin.Logging;

namespace Q3Server
{
    [Authorize]
    public class QHub : Hub
    {
        private IQManager queueManager;
        private ILogger logger;

        public QHub(IQManager manager, ILogger logger)
        {
            this.queueManager = manager;
            this.logger = logger;
        }

        public void StartQueue(string queueName, string restrictToGroup)
        {
            Trace.WriteLine("-> StartQueue: " + queueName + " restricted to " + restrictToGroup);
            queueManager.CreateQueue(queueName, restrictToGroup, User);
        }

        public void JoinQueue(int id)
        {
            Trace.WriteLine("-> JoinQueue: " + id);
            queueManager.GetQueue(id).AddUser(User);
        }

        public void LeaveQueue(int id)
        {
            Trace.WriteLine("-> LeaveQueue: " + id);
            queueManager.GetQueue(id).RemoveUser(User);
        }

        public void ActivateQueue(int id)
        {
            Trace.WriteLine("-> ActivateQueue: " + id);
            queueManager.GetQueue(id).Activate();
        }

        public void NagQueue(int id)
        {
            Trace.WriteLine("-> NagQueue: " + id);
            Clients.All.NagQueue(id);
        }

        public void MessageQueue(int id, string message)
        {
            Trace.WriteLine("-> MessageQueue: " + id + " - " + message);
            queueManager.GetQueue(id).AddMessage(User, message);
        }

        public void CloseQueue(int id)
        {
            Trace.WriteLine("-> CloseQueue: " + id);
            queueManager.CloseQueue(id);
        }

        public IEnumerable<Queue> ListQueues()
        {
            return queueManager.ListQueues();
        }

        private User User
        {
            get { return new User(Context.User.Identity.Name); }
        }
    }
}