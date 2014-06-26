using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Door2DoorCore.Types.Door2DoorResponse
{
    /// <summary>
    ///     Summary of one route. Including total durations and costs
    /// </summary>
    public class RouteTotals
    {
        public RouteTotals()
        {
            TotalDistance = 0.0;

            TotalTimeOnTrain = new TimeSpan(0, 0, 0, 0, 0);
            TotalTimeOnCar = new TimeSpan(0, 0, 0, 0, 0);
            TotalTimeOnFlight = new TimeSpan(0, 0, 0, 0, 0);
            TotalTimeOnBus = new TimeSpan(0, 0, 0, 0, 0);
            TotalTimeOnWalk = new TimeSpan(0, 0, 0, 0, 0);
            TotalTimeWaiting = new TimeSpan(0, 0, 0, 0, 0);

            TotalPriceOfTrain = 0M;
            TotalPriceOfCar = 0M;
            TotalPriceOfBus = 0M;
            TotalPriceOfFlight = 0M;
        }

        [JsonProperty("totalDistance")]
        public double TotalDistance { get; set; }

        [JsonProperty("totalTimeOnTrain")]
        public TimeSpan TotalTimeOnTrain { get; set; }

        [JsonProperty("totalTimeOnCar")]
        public TimeSpan TotalTimeOnCar { get; set; }

        [JsonProperty("totalTimeOnFlight")]
        public TimeSpan TotalTimeOnFlight { get; set; }

        [JsonProperty("totalTimeOnBus")]
        public TimeSpan TotalTimeOnBus { get; set; }

        [JsonProperty("totalTimeOnWalk")]
        public TimeSpan TotalTimeOnWalk { get; set; }

        [JsonProperty("totalTimeWaiting")]
        public TimeSpan TotalTimeWaiting { get; set; }

        [JsonProperty("totalPriceOfTrain")]
        public decimal TotalPriceOfTrain { get; set; }

        [JsonProperty("totalPriceOfCar")]
        public decimal TotalPriceOfCar { get; set; }

        [JsonProperty("totalPriceOfBus")]
        public decimal TotalPriceOfBus { get; set; }

        [JsonProperty("totalPriceOfFlight")]
        public decimal TotalPriceOfFlight { get; set; }
    }
}
