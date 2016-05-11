using Microsoft.AspNet.SignalR;
using System.Collections.Generic;
using System.Security.Claims;
using Microsoft.Owin.Logging;
using Q3Server.Interfaces;

namespace Q3Server
{
    [ExternalAuthorize]
    public class QHub : Hub
    {
        private IQManager queueManager;
        private ILogger logger;
        private IObjectGetter<User> userGetter;
        private IObjectGetter<List<string>> groupGetter;

        public QHub(IQManager manager,
                    ILogger logger,
                    IObjectGetter<User> userGetter,
                    IObjectGetter<List<string>> groupGetter)
        {
            this.queueManager = manager;
            this.logger = logger;
            this.userGetter = userGetter;
            this.groupGetter = groupGetter;
        }

        public void StartQueue(string queueName, string restrictToGroup)
        {
            logger.WriteInformation(User.FullName + " -> StartQueue: " + queueName + " restricted to " + restrictToGroup);
            var q = queueManager.CreateQueue(queueName, restrictToGroup, User);
            TraceQueue(q.Id);
        }

        public void JoinQueue(int id)
        {
            logger.WriteInformation(User.FullName + " -> JoinQueue: " + id);
            queueManager.GetQueue(id).AddUser(User);
            TraceQueue(id);
        }

        public void LeaveQueue(int id)
        {
            logger.WriteInformation(User.FullName + " -> LeaveQueue: " + id);
            queueManager.GetQueue(id).RemoveUser(User);
            TraceQueue(id);

        }

        public void ActivateQueue(int id)
        {
            logger.WriteInformation(User.FullName + " -> ActivateQueue: " + id);
            queueManager.GetQueue(id).Activate();
            TraceQueue(id);
        }

        public void DeactivateQueue(int id)
        {
            logger.WriteInformation(User.FullName + " -> DeactivateQueue: " + id);
            queueManager.GetQueue(id).Deactivate();
            TraceQueue(id);
        }

        public void NagQueue(int id)
        {
            logger.WriteInformation(User.FullName + " -> NagQueue: " + id);
            Clients.All.NagQueue(id);
            TraceQueue(id);
        }

        public void MessageQueue(int id, string message)
        {
            logger.WriteInformation(User.FullName + " -> MessageQueue: " + id + " - " + message);
            queueManager.GetQueue(id).AddMessage(User, message);
            TraceQueue(id);
        }

        public void CloseQueue(int id)
        {
            logger.WriteInformation(User.FullName + " -> CloseQueue: " + id);
            queueManager.CloseQueue(id);
        }

        public IEnumerable<Queue> ListQueues()
        {
            return queueManager.ListQueues();
        }

        public IEnumerable<string> ListGroups()
        {
            return groupGetter.Get(User.DistinguishedName);
        }

        private User User => userGetter.Get(ClaimsPrincipal.Current.FindFirst(ClaimTypes.Name).Value);

        private void TraceQueue(int id)
        {
            logger.WriteInformation(queueManager.GetQueue(id).Describe());
        }
    }
}
