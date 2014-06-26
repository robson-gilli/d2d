using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Door2DoorCore.Types.Door2DoorRequest.OuterFlightOption
{
    /// <summary>
    ///      Aggregates flights when onw trip have connections.
    /// </summary>
    public class OuterFlightSegment
    {
        public OuterFlightLeg[] flightLegs { get; set; }
    }
}
