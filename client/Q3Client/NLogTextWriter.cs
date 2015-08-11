using NLog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Q3Client
{
    class NLogTextWriter : TextWriter
    {
        private Logger logger;

        public NLogTextWriter(string name)
        {
            this.logger = LogManager.GetLogger(name);
        }

        public override Encoding Encoding
        {
            get
            {
                return Encoding.UTF8;
            }
        }

        public override void Write(string value)
        {
            logger.Info(value);
        }

        public override void Write(char[] buffer, int index, int count)
        {
            Write(new string(buffer, index, count));
        }

    }
}
