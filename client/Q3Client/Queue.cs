using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Q3Client
{
    public class Queue : IEquatable<Queue>
    {
        public int Id;
        public string Name;
        public QueueStatus Status;
        public IEnumerable<string> Members;

        public override string ToString()
        {
            return "Q" + this.Id + ": " + this.Name + " (" + string.Join(", ", Members) + ")";
        }



        public bool Equals(Queue other)
        {
            return other.Id == this.Id;
        }
    }
}
