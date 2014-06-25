using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Door2DoorCore.Types.Door2DoorResponse
{
    public class TripTotals
    {
        public TripTotals()
        {
            TotalTimeAtDestination = new TimeSpan(0, 0, 0, 0, 0);
            TotalTripTime = new TimeSpan(0, 0, 0, 0, 0);
            TotalPrice = 0M;
        }


        [JsonProperty("totalTimeAtDestination")]
        public TimeSpan TotalTimeAtDestination { get; set; }

        [JsonProperty("totalTripTime")]
        public TimeSpan TotalTripTime { get; set; }

        [JsonProperty("totalPrice")]
        public decimal TotalPrice{ get; set; }
    }
}
