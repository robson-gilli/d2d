using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Door2DoorCore.Types.Door2DoorRequest
{
    /// <summary>
    ///     For now only Rome2rio, r2r will be accepted
    /// </summary>
    public enum D2dRequestTripDateKind
    {
        departureAt = 0,

        arriveAt = 1
    }
}
