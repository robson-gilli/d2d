using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Door2DoorCore.Exceptions
{
    /// <summary>
    ///     Exception thrown when some error occurred during the response processing
    /// </summary>
    public class D2DResponseException : Exception
    {
        /// <summary>
        ///     Exception thrown when some error occurred during the response processing
        /// </summary>
        public D2DResponseException()
            :base()
        {
        }

        /// <summary>
        ///     Exception thrown when some error occurred during the response processing
        /// </summary>
        /// <param name="message">Exception message</param>
        public D2DResponseException(string message)
            : base(message)
        {
        }
        /// <summary>
        ///     Exception thrown when some error occurred during the response processing
        /// </summary>
        /// <param name="exception">Exception to be thrown</param>
        public D2DResponseException(Exception exception)
            :base("Response Error", exception)
        {
        }
    }
}
