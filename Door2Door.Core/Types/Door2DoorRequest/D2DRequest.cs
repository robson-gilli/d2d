using System;
using System.Configuration;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Door2DoorCore.Types.Door2DoorRequest.OuterFlightOption;

namespace Door2DoorCore.Types.Door2DoorRequest
{
    /// <summary>
    /// Information about the desired itinerary.
    /// </summary>
    public class D2DRequest
    {
        /// <summary>
        /// Information about the desired itinerary.
        /// </summary>
        public D2DRequest() { }
        /// <summary>
        ///     For now only accepting Rome2rio (r2r).
        /// </summary>
        public D2DRequestType requestType = D2DRequestType.r2r;
        /// <summary>
        ///     API endpoint. (Will change when in production)
        /// </summary>
        public string url = "http://free.rome2rio.com/api/1.2/json/Search";
        /// <summary>
        ///     API access key. (Will change when in production)
        /// </summary>
        public string key = "5Hhhi538";
        /// <summary>
        ///     Desired currency
        /// </summary>
        public string currency = "BRL";
        /// <summary>
        ///     Return type. ('json' / 'xml')
        /// </summary>
        public string dataTye = "json";
        /// <summary>
        ///     Max distance for driving (not used for now).
        /// </summary>
        public int maxDriveKm { get; set; }
        /// <summary>
        ///     DateTime indicating when the user wants to be at the destination
        /// </summary>
        public DateTime desiredOutboundDate { get; set; }

        public D2dRequestTripDateKind outboundDateKind { get; set; }

        /// <summary>
        ///     Nullable - If RoundTrip, indicates when the user wants to be back at the origin
        /// </summary>
        public DateTime? desiredInboundDate { get; set; }

        public D2dRequestTripDateKind inboundDateKind { get; set; }


        /// <summary>
        ///     SearchRequestFlags iindicating to include or exclude forms of transportation
        /// </summary>
        public D2DRequestFlags flags { get; set; }
        /// <summary>
        ///     Latitude and Longitude of the origin
        /// </summary>
        public D2DRequestLocation oriLocation { get; set; }
        /// <summary>
        /// Latitude and Longitude of the destination
        /// </summary>
        public D2DRequestLocation destLocation { get; set; }
        /// <summary>
        ///     It's possible to inform a FlightOption from an external source.
        ///     When informed, the calculation of schedules and costs will take these values into account,
        ///     instead of the Rome2rio flight results.
        /// </summary>
        public OuterFlightOption.OuterFlightOption[] chosenRoute { get; set; }
    }
}
