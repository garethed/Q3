using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace Q3Server
{
    public class Queue
    {
        private ConcurrentQueue<string> members = new ConcurrentQueue<string>();
        private object lockable = new object();

        public Queue (int id, string name, string creatingUser)
        {
            this.Id = id;
            this.Name = name;
            this.Status = QueueStatus.Waiting;
            this.members.Enqueue(creatingUser);
        }

        public int Id { get; private set; }
        public string Name { get; private set; }
        public QueueStatus Status { get; private set; }

        public IEnumerable<string> Members
        {
            get { return members; }
        }

        internal void AddUser(string name)
        {
            members.Enqueue(name);
            MembershipChanged();
        }

        private void MembershipChanged()
        {
            if (QueueMembershipChanged != null)
            {
                QueueMembershipChanged(this, new QueueEventArgs(this));
            }
        }

        public event EventHandler<QueueEventArgs> QueueMembershipChanged;
        public event EventHandler<QueueEventArgs> QueueStatusChanged;

        internal void Activate()
        {
            lock (lockable)
            {
                if (Status == QueueStatus.Waiting)
                {
                    Status = QueueStatus.Activated;      
                    
                    if (QueueStatusChanged != null)
                    {
                        QueueStatusChanged(this, new QueueEventArgs(this));
                    }              
                }

            }
        }

        public override string ToString()
        {
            return "Q" + this.Id + ": " + this.Name;
        }
    }
}