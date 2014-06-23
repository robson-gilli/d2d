using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Door2DoorCore.Types.Door2DoorRequest.OuterFlightOption
{
    public class OuterFlightOption
    {
        public OuterFlightSegment flightSegment { get; set; }

        public int segmentIndex { get; set; }

        public int routeIndex { get; set; }

        public decimal price { get; set; }

        public string currency { get; set; }

    }
}
