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
        /// <summary>
        /// <see cref="Door2DoorCore.Types.Door2DoorRequest.OuterFlightOption.OuterFlightSegment"/>. External flight segment.
        /// </summary>
        public OuterFlightSegment flightSegment { get; set; }
        /// <summary>
        ///     In which segment should this option be inserted
        /// </summary>
        public int segmentIndex { get; set; }
        /// <summary>
        ///     In which route should this option be inserted
        /// </summary>
        public int routeIndex { get; set; }
        /// <summary>
        /// Price for the option
        /// </summary>
        public decimal price { get; set; }
        /// <summary>
        /// Cuurency for the option
        /// </summary>
        public string currency { get; set; }
    }
}
