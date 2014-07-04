using Door2DoorCore.Exceptions;
using Door2DoorCore.Types.Door2DoorRequest;
using Door2DoorCore.Types.Door2DoorResponse;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Door2DoorCore
{
    /// <summary>
    /// Base class with common methods and attrubutes. It also forces children classes to implement private methods not exposed on the interface <see cref="Door2DoorCore.Interfaces.IDoor2DoorProvider"/>.
    /// </summary>
    internal abstract class Door2DoorBase
    {
        /// <summary>
        /// Max walking minutes. Controls how much time is acceptable for a walk <see cref="Door2DoorCore.Types.Door2DoorResponse.Segment"/>. If not informed, the default is 10 min.
        /// </summary>
        protected int _maxWalkingMinutes = 10;
        /// <summary>
        /// How many minutes should be considered as antecipation to get to the airport. If not informed, the default is 120 (two hours).
        /// </summary>
        protected int _flightAntecipation = 120;
        /// <summary>
        /// How many minutes should be considered after the Arriving time of a flight. If not informed, the default is 30.
        /// </summary>
        protected int _minutesAfterFlight = 30;
        /// <summary>
        /// A <see cref="Door2DoorCore.Types.Door2DoorRequest.D2DRequest"/>. Holds the request of the itinerary.
        /// </summary>
        protected D2DRequest _req;
        /// <summary>
        /// A <see cref="Door2DoorCore.Types.Door2DoorRequest.D2DRequest"/>. Holds the request of the itinerary.
        /// </summary>
        public D2DRequest Req
        {
            get { return _req; }
            set { _req = value; }
        }
        /// <summary>
        /// A <see cref="Door2DoorCore.Types.Door2DoorResponse.Door2DoorResponse"/>. Complete Door2Door response.
        /// </summary>
        protected Door2DoorResponse _resp;
        /// <summary>
        ///     Routes and schedules for the desired itinerary request. <see cref="Door2DoorCore.Types.Door2DoorResponse.Door2DoorResponse"/>
        /// </summary>
        public Door2DoorResponse Resp
        {
            get { return _resp; }
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="d2dReq">
        /// The request. A <see cref="Door2DoorCore.Types.Door2DoorRequest.D2DRequest"/>
        /// </param>
        /// <param name="maxWalkingMinutes">
        /// Max walking minutes. Controls how much time is acceptable for a walk <see cref="Door2DoorCore.Types.Door2DoorResponse.Segment"/>. If not informed, the default is 10 min.
        /// </param>
        /// <param name="flightAntecipation">
        /// How many minutes should be considered as antecipation to get to the airport. If not informed, the default is 120 (two hours).
        /// </param>
        /// <param name="minutesAfterFlight">
        /// How many minutes should be considered after the Arriving time of a flight. If not informed, the default is 30.
        /// </param>
        public Door2DoorBase(D2DRequest d2dReq, int maxWalkingMinutes, int flightAntecipation, int minutesAfterFlight)
        {
            _maxWalkingMinutes = maxWalkingMinutes;
            _flightAntecipation = flightAntecipation;
            _minutesAfterFlight = minutesAfterFlight;

            VerifyRequest(d2dReq);
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="d2dReq">
        /// The request. A <see cref="Door2DoorCore.Types.Door2DoorRequest.D2DRequest"/>
        /// </param>
        public Door2DoorBase(D2DRequest d2dReq)
        {
            VerifyRequest(d2dReq);
        }

        private void VerifyRequest(D2DRequest d2dReq)
        {
            _req = d2dReq;
            if (!RequestIsOK(_req))
            {
                throw new D2DRequestException("Please check your request.");
            }
        }

        /// <summary>
        ///     Verifies if all necessary data from the request are correctly informed
        /// </summary>
        /// <param name="d2dReq">
        ///     Route request. <see cref="Door2DoorCore.Types.Door2DoorRequest.D2DRequest"/>
        /// </param>
        /// <returns>
        ///     Whether the request is ok or not
        /// </returns>
        protected bool RequestIsOK(D2DRequest d2dReq)
        {
            return d2dReq.flags != null &&
                    d2dReq.desiredOutboundDate != DateTime.MinValue && d2dReq.desiredOutboundDate != DateTime.MaxValue &&
                    d2dReq.oriLocation != null &&
                    d2dReq.destLocation != null;
        }

        /// <summary>
        /// Forces children classes to implement this private method to build the <see cref="Door2DoorCore.Types.Door2DoorResponse.Door2DoorResponse"/>
        /// </summary>
        protected abstract void BuildCompleteItinerarySchedule();
    }
}
