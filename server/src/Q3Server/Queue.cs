using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace Q3Server
{
    public class Queue
    {
        private List<User> members = new List<User>();
        private object lockable = new object();
        private List<Message> messages = new List<Message>();

        public Queue (int id, string name, User creatingUser, string restrictToGroup)
        {
            this.Id = id;
            this.Name = name;
            this.Status = QueueStatus.Waiting;
            this.members.Add(creatingUser);
            this.RestrictToGroup = restrictToGroup;
        }

        public int Id { get; private set; }
        public string Name { get; private set; }
        public QueueStatus Status { get; private set; }

        public string RestrictToGroup { get; private set; }

        public IEnumerable<User> Members
        {
            get { return members; }
        }

        public IEnumerable<Message> Messages
        {
            get { return messages; }
        }


        public void AddUser(User name)
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

        public void RemoveUser(User user)
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

        internal void AddMessage(User sender, string message)
        {
            lock (lockable)
            {
                var msg = new Message() { Sender = sender, Content = message };
                messages.Add(msg);

                if (QueueMessageSent != null)
                {
                    QueueMessageSent(this, new QueueMessageEventArgs(this, msg));
                }
            }
        }

        public event EventHandler<QueueEventArgs> QueueMembershipChanged;
        public event EventHandler<QueueEventArgs> QueueStatusChanged;
        public event EventHandler<QueueMessageEventArgs> QueueMessageSent;

        internal void Activate()
        {
            UpdateStatus(QueueStatus.Activated);
        }

        internal void Deactivate()
        {
            UpdateStatus(QueueStatus.Waiting);
        }

        public override string ToString()
        {
            return "Q" + this.Id + ": " + this.Name;
        }

        public string Describe()
        {
            return ToString()
                + "; Members: [" + string.Join(", ", Members.Select(u => u.UserName)) + "]; Messages: ["
                + string.Join(", ", Messages.Select(m => m.Sender.UserName + ": " + m.Content.Replace("\n", "").Substring(0,32))) + "]";
        }

        internal void Close()
        {
            UpdateStatus(QueueStatus.Closed);
        }

        private void UpdateStatus(QueueStatus newStatus)
        {
            lock (lockable)
            {
                if (Status != newStatus)
                {
                    Status = newStatus;

                    if (QueueStatusChanged != null)
                    {
                        QueueStatusChanged(this, new QueueEventArgs(this));
                    }
                }
            }
        }

        public class Message
        {
            public User Sender;
            public string Content;
        }
    }
}