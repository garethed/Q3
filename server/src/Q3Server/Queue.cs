using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace Q3
{
    public class Queue
    {
        private ConcurrentQueue<string> members = new ConcurrentQueue<string>();
        private object lockable = new object();

        public Queue (string Name)
        {
            this.Name = Name;
            this.Status = QueueStatus.Waiting;
        }

        public string Name { get; private set; }
        public QueueStatus Status { get; private set; }

        internal void AddUser(string name)
        {
            members.Enqueue(name);
            MembershipChanged();
        }

        private void MembershipChanged()
        {
            if (QueueMembershipChanged != null)
            {
                QueueMembershipChanged(this, new QueueMembershipChangedEventArgs() { Name = Name,  Members = members.ToList() });
            }
        }

        public event EventHandler<QueueMembershipChangedEventArgs> QueueMembershipChanged;
        public event EventHandler<QueueStatusChangedEventArgs> QueueStatusChanged;

        internal void Activate()
        {
            lock (lockable)
            {
                if (Status == QueueStatus.Waiting)
                {
                    Status = QueueStatus.Activated;      
                    
                    if (QueueStatusChanged != null)
                    {
                        QueueStatusChanged(this, new QueueStatusChangedEventArgs() { Name = Name });
                    }              
                }

            }
        }
    }

    public class QueueMembershipChangedEventArgs : EventArgs
    {
        public string Name;
        public IEnumerable<string> Members;
    }

    public class QueueStatusChangedEventArgs : EventArgs
    {
        public string Name;
    }
}