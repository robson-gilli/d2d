using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Door2DoorCore.Types.Door2DoorRequest.OuterFlightOption
{
    public class OuterFlightSegment
    {
        public OuterFlightLeg[] flightLegs { get; set; }
    }
}
