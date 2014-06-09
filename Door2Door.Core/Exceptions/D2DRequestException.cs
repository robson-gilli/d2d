using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Door2DoorCore.Exceptions
{
    public class D2DRequestException : Exception
    {
        public D2DRequestException()
            :base()
        {
        }

        public D2DRequestException(string message)
            : base(message)
        {
        }
    }
}
