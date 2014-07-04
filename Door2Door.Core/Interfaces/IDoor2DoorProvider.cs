using Door2DoorCore.Types.Door2DoorRequest;
using Door2DoorCore.Types.Door2DoorResponse;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Door2DoorCore.Interfaces
{
    /// <summary>
    /// Sets a pattern for future providers implementation.
    /// </summary>
    internal interface IDoor2DoorProvider
    {
        /// <summary>
        /// <para>If it's a new instance, it you communicate to the providers an calculate the routes. </para>
        /// <para>If it's the second call to an existing istance, it will only recalculate, but not communicate to the providers </para>
        /// </summary>
        /// <returns>
        /// A <see cref="Door2DoorCore.Types.Door2DoorResponse.Door2DoorResponse"/>.
        /// </returns>
        Door2DoorResponse GetResponse();
        /// <summary>
        /// <para>If 'getNewResponse' is true, it you communicate to the providers even if it is an existing instance.</para>
        /// </summary>
        /// <param name="getNewResponse">
        /// Bool. It's possible to force the communication to the providers.
        /// </param>
        /// <returns>
        /// A <see cref="Door2DoorCore.Types.Door2DoorResponse.Door2DoorResponse"/>.
        /// </returns>
        Door2DoorResponse GetResponse(bool getNewResponse);
    }
}
