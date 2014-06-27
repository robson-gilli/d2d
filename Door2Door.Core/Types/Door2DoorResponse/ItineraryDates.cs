using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Door2DoorCore.Types.Door2DoorResponse
{
    /// <summary>
    ///     Aux to the itinerary calculation.
    /// </summary>
    public class ItineraryDates
    {
        /// <summary>
        /// Itinerary Arrival
        /// </summary>
        public DateTime? arrivalDateTime;
        /// <summary>
        /// Itinerary Departure
        /// </summary>
        public DateTime? departureDateTime;
        /// <summary>
        /// Constructor
        /// </summary>
        public ItineraryDates() { }

    }
}
