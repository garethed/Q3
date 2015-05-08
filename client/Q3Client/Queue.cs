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
        private List<string> members;
        public string UserId;

        public override string ToString()
        {
            return "Q" + this.Id + ": " + this.Name + " (" + string.Join(", ", Members) + ")";
        }

        public IEnumerable<string> Members
        {
            get { return members; }
            set
            {
                if (members == null || !value.SequenceEqual(members))
                {
                    members = new List<string>(value);
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
            get { return members.Contains(UserId); }
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
