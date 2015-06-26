using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Q3Client
{
    public class Queue : IEquatable<Queue>, INotifyPropertyChanged
    {
        public int Id;
        public string Name;
        private QueueStatus status;
        private List<User> members;
        public User User;
        private IList<Message> messages = new List<Message>();
        public string RestrictToGroup { get; set; }

        public override string ToString()
        {
            return "Q" + this.Id + ": " + this.Name + " (" + string.Join(", ", Members.Select(u => u.UserName)) + ")";
        }

        public IEnumerable<User> Members
        {
            get { return members; }
            set
            {
                if (members == null || !value.SequenceEqual(members))
                {
                    members = new List<User>(value);
                    OnPropertyChanged();
                    OnPropertyChanged("UserIsOnQueue");
                }
            }
        }

        public QueueStatus Status
        {
            get { return status; }
            set
            {
                if (status != value)
                {
                    status = value;
                    OnPropertyChanged();
                }
            }
        }

        public bool UserIsOnQueue
        {
            get { return members.Contains(User); }
        }

        public IList<Message> Messages
        {
            get { return messages; }
            set
            {
                if (messages == null || !value.SequenceEqual(messages))
                {
                    messages = value;
                    OnPropertyChanged();
                }
            }
        }


        public bool Equals(Queue other)
        {
            return other.Id == this.Id;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected  void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            Trace.WriteLine("q changed " + propertyName);
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }

        public class Message
        {
            public User Sender;
            public string Content;

            protected bool Equals(Message other)
            {
                return Equals(Sender, other.Sender) && string.Equals(Content, other.Content);
            }

            public override bool Equals(object obj)
            {
                if (ReferenceEquals(null, obj)) return false;
                if (ReferenceEquals(this, obj)) return true;
                if (obj.GetType() != this.GetType()) return false;
                return Equals((Message) obj);
            }

            public override int GetHashCode()
            {
                unchecked
                {
                    return ((Sender != null ? Sender.GetHashCode() : 0)*397) ^ (Content != null ? Content.GetHashCode() : 0);
                }
            }
        }

        public void AddMessage(Message message)
        {
            if (!messages.Contains(message))
            {
                messages.Add(message);
                OnPropertyChanged("Messages");
            }
        }
    }
}
