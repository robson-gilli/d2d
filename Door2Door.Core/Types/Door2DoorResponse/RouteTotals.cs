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
        /// <summary>
        /// Constructor
        /// </summary>
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
        /// <summary>
        /// Total Distance of the route from origin to destination
        /// </summary>
        [JsonProperty("totalDistance")]
        public double TotalDistance { get; set; }
        /// <summary>
        /// Total time on train segments
        /// </summary>
        [JsonProperty("totalTimeOnTrain")]
        public TimeSpan TotalTimeOnTrain { get; set; }
        /// <summary>
        /// Total time on car segments
        /// </summary>
        [JsonProperty("totalTimeOnCar")]
        public TimeSpan TotalTimeOnCar { get; set; }
        /// <summary>
        /// Total time on flight segments
        /// </summary>
        [JsonProperty("totalTimeOnFlight")]
        public TimeSpan TotalTimeOnFlight { get; set; }
        /// <summary>
        /// Total time on bus segments
        /// </summary>
        [JsonProperty("totalTimeOnBus")]
        public TimeSpan TotalTimeOnBus { get; set; }
        /// <summary>
        /// Total time walking
        /// </summary>
        [JsonProperty("totalTimeOnWalk")]
        public TimeSpan TotalTimeOnWalk { get; set; }
        /// <summary>
        /// Total time waiting between connections
        /// </summary>
        [JsonProperty("totalTimeWaiting")]
        public TimeSpan TotalTimeWaiting { get; set; }
        /// <summary>
        /// Total cost of trains inside this route
        /// </summary>
        [JsonProperty("totalPriceOfTrain")]
        public decimal TotalPriceOfTrain { get; set; }
        /// <summary>
        /// Total cost of cars inside this route
        /// </summary>
        [JsonProperty("totalPriceOfCar")]
        public decimal TotalPriceOfCar { get; set; }
        /// <summary>
        /// Total cost of buses inside this route
        /// </summary>
        [JsonProperty("totalPriceOfBus")]
        public decimal TotalPriceOfBus { get; set; }
        /// <summary>
        /// Total cost of flights inside this route
        /// </summary>
        [JsonProperty("totalPriceOfFlight")]
        public decimal TotalPriceOfFlight { get; set; }
    }
}
