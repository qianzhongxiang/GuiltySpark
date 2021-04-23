using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GuiltySpark
{
    public class ConsoleLogger : MarshalByRefObject
    {
        public TextWriter TextWriter;
        public virtual void WriteLine(string message)
        {
            TextWriter?.WriteLine(message);
        }
    }
}
