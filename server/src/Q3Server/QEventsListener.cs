using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Infrastructure;
using System;
using System.Diagnostics;

namespace Q3
{
    public class QEventsListener
    {
        private IHubContext hubContext;

        public QEventsListener(IConnectionManager connectionManager)
        {
            hubContext = connectionManager.GetHubContext<QHub>();
        }

        public void OnQueueCreated(object sender, QueueCreatedEventArgs e)
        {
            Trace.WriteLine("<- NewQueue " + e.Queue.Name);
            hubContext.Clients.All.NewQueue(e.Queue.Name);

            e.Queue.QueueMembershipChanged += QueueMembershipChanged;
            e.Queue.QueueStatusChanged += QueueStatusChanged;
        }

        private void QueueStatusChanged(object sender, QueueStatusChangedEventArgs e)
        {
            Trace.WriteLine("<- QueueStatusChanged " + e.Name);
            hubContext.Clients.All.QueueStatusChanged(e.Name);
        }

        private void QueueMembershipChanged(object sender, QueueMembershipChangedEventArgs e)
        {
            Trace.WriteLine("<- QueueMembershipChanged " + e.Name);
            //TODO: only send this to the group of clients who are already on this queue
            hubContext.Clients.All.QueueMembershipChanged(e.Name, e.Members);
        }
    }
}