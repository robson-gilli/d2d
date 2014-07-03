using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Door2DoorCore.Types;
using Door2DoorCore.Types.Door2DoorResponse;
using Door2DoorCore.Types.Door2DoorRequest;
using Door2DoorCore.Types.Door2DoorRequest.OuterFlightOption;
using Door2DoorCore.Exceptions;

/// <summary>
///     Provides Door2Door routes
/// </summary>
namespace Door2DoorCore
{
    /// <summary>
    ///     <para>
    ///     The Door2Door Class Library searches for itineraries from and to any location.
    ///     </para>
    ///     <para>
    ///     Just inform the coordinates for the origin and destination, the desired arrival date at the destination and a list of routes will be returned.
    ///     </para>
    ///     <para>
    ///     The routes may include several means of transportation, including walking, flights, taxis, cars, buses, trains, ferrys trams and more.
    ///     </para>
    ///     <para>
    ///     A desired return date can also be specified. In this case two sets of routes will be returned, one for the outbound and another for the inbound segment.
    ///     </para>
    ///     <para>
    ///     You can also filter wheter you want or not include public transportation on the response.
    ///     </para>
    ///     <para>
    ///     NOTE: All schedules and pricing are estimatives, It's impossible to predict with perfect accuracy the correct time walking or the price of the taxi, for instance.
    ///     </para>
    /// </summary>
    public class Door2Door
    {
        private Door2DoorRome2Rio r2r;
        private bool newRequest;

        /// <summary>
        ///     Routes and schedules for the desired itinerary request. <see cref="Door2DoorCore.Types.Door2DoorResponse.Door2DoorResponse"/>
        /// </summary>
        public Door2DoorResponse Resp
        {
            get { return r2r.Resp; }
        }
        public D2DRequest _Req
        {
            get { return r2r.Req; }
            set 
            { 
                r2r.Req = value;
                newRequest = true;
            }
        }


        /// <summary>
        ///     Constructor of the class. <see cref="Door2DoorCore.Door2Door"/>
        /// </summary>
        /// <param name="d2dReq">
        ///     <para><see cref="Door2DoorCore.Types.Door2DoorRequest.D2DRequest"/></para>
        ///     Parameters for the request include coordinates of origin and destination,
        ///     arrival and return dates, external flight options and filters.
        /// </param>
        public Door2Door(D2DRequest d2dReq)
        {
            if (d2dReq.requestType == D2DRequestType.r2r)
            {
                r2r = new Door2DoorRome2Rio(d2dReq);
            }
            else
            {
                throw new D2DRequestException("Only D2DRequestType.r2r is supported so far.");
            }

        }

        public Door2Door(D2DRequest d2dReq, int maxWalkingMinutes, int flightAntecipation, int minutesAfterFlight)
        {
            if (d2dReq.requestType == D2DRequestType.r2r)
            {
                r2r = new Door2DoorRome2Rio(d2dReq, maxWalkingMinutes, flightAntecipation, minutesAfterFlight);
            }
            else
            {
                throw new D2DRequestException("Only D2DRequestType.r2r is supported so far.");
            }
        }

        /// <summary>
        ///     <para>After instantiating the <see cref="Door2DoorCore.Door2Door"/> class, you should invoke this method.</para>
        ///     <para>It calculates all possible routes, itineraries and schedules for the given request.</para>
        /// </summary>
        /// <returns>
        ///     <see cref="Door2DoorCore.Types.Door2DoorResponse.Door2DoorResponse"/>. Complete itinerary with indicative pricing and schedules.
        /// </returns>
        public Door2DoorResponse GetResponse()
        {
            return r2r.GetResponse(newRequest);
        }
    }
}
