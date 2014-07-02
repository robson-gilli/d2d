using Door2DoorCore.Exceptions;
using Door2DoorCore.Types.Door2DoorRequest;
using Door2DoorCore.Types.Door2DoorResponse;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Door2DoorCore
{
    abstract class Door2DoorBase
    {
        /// <summary>
        /// A <see cref="Door2DoorCore.Types.Door2DoorRequest.D2DRequest"/>. Holds the request of the itinerary.
        /// </summary>
        protected D2DRequest _req;
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

        public Door2DoorBase(D2DRequest d2dReq)
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

        protected abstract void BuildCompleteItinerarySchedule();
    }
}
