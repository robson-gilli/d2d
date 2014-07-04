using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Door2DoorCore.Types.Door2DoorRequest;

namespace Door2DoorCore.Types.Door2DoorRequest.OuterHotelOption
{
    /// <summary>
    /// <para>It's possible to inform the hotel location for roundtrip jouneys.</para>
    /// <para>The cost of the stay and the local taxi transfers will be included into the <see cref="Door2DoorCore.Types.Door2DoorResponse.RouteTotals"/>.</para>
    /// </summary>
    public class OuterHotelOption
    {
        /// <summary>
        /// Hotel coordinates
        /// </summary>
        public D2DRequestLocation location { get; set; }
        /// <summary>
        /// Total cost of the entire stay
        /// </summary>
        public decimal totalPrice { get; set; }
        /// <summary>
        /// <para>If true, considers the inboundDate and outboundDate as the checkin and checkout dates.</para>
        /// <para>If false, checkin and checkou dates must be informed and must be inside the trip date range.</para>
        /// </summary>
        public bool completeStay { get; set; }
        /// <summary>
        /// If 'completeStay' is false, this date will be considered as the checkin date. 
        /// Obs.: It must be greater than the outboundDate of the trip.
        /// </summary>
        public DateTime checkinDate { get; set; }
        /// <summary>
        /// If 'completeStay' is false, this date will be considered as the checkout date. 
        /// Obs.: It must be smaller than the inboundDate of the trip.
        /// </summary>
        public DateTime checkoutDate { get; set; }
    }
}
