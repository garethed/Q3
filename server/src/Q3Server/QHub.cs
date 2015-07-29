using Microsoft.AspNet.SignalR;
using System.Collections.Generic;
using System.Diagnostics;

namespace Q3Server
{
    [Authorize]
    public class QHub : Hub
    {

        private static IQManager queueManager;

        static QHub()
        {
            queueManager = new QManager(new QEventsListener(GlobalHost.ConnectionManager));
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