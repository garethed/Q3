using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Infrastructure;
using System;
using System.Diagnostics;

namespace Q3Server
{
    public interface IQEventsListener
    {
        void OnQueueCreated(object sender, QueueEventArgs e);
    }

    public class QEventsListener : IQEventsListener
    {
        private IHubContext hubContext;

        public QEventsListener(IConnectionManager connectionManager)
        {
            hubContext = connectionManager.GetHubContext<QHub>();
        }

        public void OnQueueCreated(object sender, QueueEventArgs e)
        {
            Trace.WriteLine("<- NewQueue " + e.Queue);
            hubContext.Clients.All.NewQueue(e.Queue);

            e.Queue.QueueMembershipChanged += QueueMembershipChanged;
            e.Queue.QueueStatusChanged += QueueStatusChanged;
            e.Queue.QueueMessageSent += QueueMessageSent;
        }

        private void QueueMessageSent(object sender, QueueMessageEventArgs e)
        {
            hubContext.Clients.All.QueueMessageSent(e.Queue.Id, e.Message.Sender, e.Message.Content, e.Message.Timestamp);
        }

        private void QueueStatusChanged(object sender, QueueEventArgs e)
        {
            Trace.WriteLine("<- QueueStatusChanged " + e.Queue);
            hubContext.Clients.All.QueueStatusChanged(e.Queue);
        }

        private void QueueMembershipChanged(object sender, QueueEventArgs e)
        {
            Trace.WriteLine("<- QueueMembershipChanged " + e.Queue);
            //TODO: only send this to the group of clients who are already on this queue
            hubContext.Clients.All.QueueMembershipChanged(e.Queue);
        }
    }
}