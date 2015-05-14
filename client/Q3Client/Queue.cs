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
    }
}
