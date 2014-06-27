using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Door2DoorCore.Types.Door2DoorRequest.OuterFlightOption
{
    /// <summary>
    ///      Aggregates flights when a trip have connections.
    /// </summary>
    public class OuterFlightSegment
    {
        /// <summary>
        ///     List of <see cref="Door2DoorCore.Types.Door2DoorRequest.OuterFlightOption.OuterFlightLeg"/>, each one indicating one flight connection.
        /// </summary>
        public OuterFlightLeg[] flightLegs { get; set; }
    }
}