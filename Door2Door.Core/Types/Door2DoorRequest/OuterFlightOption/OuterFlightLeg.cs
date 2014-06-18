using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Door2DoorCore.Types.Door2DoorRequest.OuterFlightOption
{
    public class OuterFlightLeg
    {
        public string origin { get; set; }

        public string destination { get; set; }

        public string number { get; set; }

        public string marketingAirline { get; set; }

        public string operatingAirline { get; set; }

        public DateTime departureDate { get; set; }

        public DateTime arrivalDate { get; set; }

        public char fareClass { get; set; }

        public string fareBasis { get; set; }

        public int duration { get; set; }

        public int distance { get; set; }

    }
}
