using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Door2DoorCore.Types.Door2DoorRequest.OuterFlightOption
{
    /// <summary>
    ///     A Complete FlightOption. Inbound or Outbound
    /// </summary>
    public class OuterFlightOption
    {
        public OuterFlightSegment flightSegment { get; set; }
        /// <summary>
        ///  I which segment should this option be inserted
        /// </summary>
        public int segmentIndex { get; set; }
        /// <summary>
        ///  I which route should this option be inserted
        /// </summary>
        public int routeIndex { get; set; }

        public decimal price { get; set; }

        public string currency { get; set; }
    }
}
