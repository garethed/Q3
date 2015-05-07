using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Q3Client
{
    public class Queue : IEquatable<Queue>
    {
        public int Id;
        public string Name;
        private QueueStatus status;
        private List<string> members;

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
                    MembersChanged.SafeInvoke(this);
                }
            }
        }

        public QueueStatus Status
        {
            get { return status; }
            set
            {
                status = value; 
                StatusChanged.SafeInvoke(this);
            }
        }


        public bool Equals(Queue other)
        {
            return other.Id == this.Id;
        }

        public event EventHandler StatusChanged;
        public event EventHandler MembersChanged;
    }
}
