using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace Q3Server
{
    public class Queue
    {
        private List<string> members = new List<string>();
        private object lockable = new object();

        public Queue (int id, string name, string creatingUser)
        {
            this.Id = id;
            this.Name = name;
            this.Status = QueueStatus.Waiting;
            this.members.Add(creatingUser);
        }

        public int Id { get; private set; }
        public string Name { get; private set; }
        public QueueStatus Status { get; private set; }

        public IEnumerable<string> Members
        {
            get { return members; }
        }

        public void AddUser(string name)
        {
            lock (lockable)
            {
                if (!members.Contains(name))
                {
                    members.Add(name);
                    MembershipChanged();
                }
            }
        }

        private void MembershipChanged()
        {
            if (QueueMembershipChanged != null)
            {
                QueueMembershipChanged(this, new QueueEventArgs(this));
            }
        }

        public void RemoveUser(string user)
        {
            lock (lockable)
            {
                if (members.Contains(user))
                {
                    members.Remove(user);
                    MembershipChanged();
                }
            }
        }

        public event EventHandler<QueueEventArgs> QueueMembershipChanged;
        public event EventHandler<QueueEventArgs> QueueStatusChanged;

        internal void Activate()
        {
            UpdateStatus(QueueStatus.Activated);
        }

        public override string ToString()
        {
            return "Q" + this.Id + ": " + this.Name;
        }

        internal void Close()
        {
            UpdateStatus(QueueStatus.Closed);
        }

        private void UpdateStatus(QueueStatus newStatus)
        {
            lock (lockable)
            {
                if (Status < newStatus)
                {
                    Status = newStatus;

                    if (QueueStatusChanged != null)
                    {
                        QueueStatusChanged(this, new QueueEventArgs(this));
                    }
                }
            }
        }
    }
}