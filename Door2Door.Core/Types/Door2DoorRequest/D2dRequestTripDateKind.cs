using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Door2DoorCore.Types.Door2DoorRequest
{
    /// <summary>
    ///     How to interpret dates in the request.
    /// </summary>
    public enum D2dRequestTripDateKind
    {
        /// <summary>
        /// The date informed is the departure date.
        /// </summary>
        departureAt = 0,
        /// <summary>
        /// The date informed is the arrival date.
        /// </summary>
        arriveAt = 1
    }
}
