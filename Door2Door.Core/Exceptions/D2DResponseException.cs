using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Door2DoorCore.Exceptions
{
    public class D2DResponseException : Exception
    {
        private Exception exception;

        public D2DResponseException()
            :base()
        {
        }

        public D2DResponseException(string message)
            : base(message)
        {
        }

        public D2DResponseException(Exception exception)
            :base("Response Error", exception)
        {
        }
    }
}
