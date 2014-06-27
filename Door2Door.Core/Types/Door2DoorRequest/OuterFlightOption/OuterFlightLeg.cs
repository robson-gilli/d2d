using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Door2DoorCore.Types.Door2DoorRequest.OuterFlightOption
{
    /// <summary>
    ///     A leg (single flight) from an external source
    /// </summary>
    public class OuterFlightLeg
    {
        /// <summary>
        /// IATA of Origin
        /// </summary>
        public string origin { get; set; }
        /// <summary>
        /// IATA of Destination
        /// </summary>
        public string destination { get; set; }
        /// <summary>
        /// Flight number
        /// </summary>
        public string number { get; set; }
        /// <summary>
        /// Marketing Airline
        /// </summary>
        public string marketingAirline { get; set; }
        /// <summary>
        /// Operatin Airline
        /// </summary>
        public string operatingAirline { get; set; }
        /// <summary>
        /// Departure Datetime fo the flight
        /// </summary>
        public DateTime departureDate { get; set; }
        /// <summary>
        /// Arrival Datetime fo the flight
        /// </summary>
        public DateTime arrivalDate { get; set; }
        /// <summary>
        /// Class of service
        /// </summary>
        public char fareClass { get; set; }
        /// <summary>
        /// Fare Basis of the class
        /// </summary>
        public string fareBasis { get; set; }
        /// <summary>
        ///     Duration in minutes
        /// </summary>
        public int duration { get; set; }
        /// <summary>
        ///     Distance in kilometers
        /// </summary>
        public int distance { get; set; }
    }
}
