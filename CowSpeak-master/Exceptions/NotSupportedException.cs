using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CowSpeak.Exceptions
{
    public class NotSupportedException : BaseException
    {
        public NotSupportedException(string message) : base(message)
        {

        }
    }
}
