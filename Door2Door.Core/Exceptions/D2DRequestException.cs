using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Door2DoorCore.Exceptions
{
    /// <summary>
    ///     Exception thrown when there is wrong or missing information on the <see cref="Door2DoorCore.Types.Door2DoorRequest.D2DRequest"/>.
    /// </summary>
    public class D2DRequestException : Exception
    {
        /// <summary>
        ///     Exception thrown when there is wrong or missing information on the <see cref="Door2DoorCore.Types.Door2DoorRequest.D2DRequest"/>.
        /// </summary>
        public D2DRequestException()
            :base()
        {
        }

        /// <summary>
        /// Exception thrown when there is wrong or missing information on the <see cref="Door2DoorCore.Types.Door2DoorRequest.D2DRequest"/>.
        /// </summary>
        /// <param name="message">Message to be thrown</param>
        public D2DRequestException(string message)
            : base(message)
        {
        }
    }
}
