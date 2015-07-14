using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Q3Client
{
    [DataContract]
    class UserConfig
    {
        [DataMember]
        public bool FirstRun = true;
    }
}
