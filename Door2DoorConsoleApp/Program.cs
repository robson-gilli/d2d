﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
/************** Door2Door Namespaces *********************/
using Door2DoorCore;
using Door2DoorCore.Exceptions;
using Door2DoorCore.Types;
using Door2DoorCore.Types.Door2DoorRequest;
using Door2DoorCore.Types.Door2DoorRequest.OuterFlightOption;
using Door2DoorCore.Types.Door2DoorRequest.OuterHotelOption;
using Door2DoorCore.Types.Door2DoorResponse;
using Door2DoorCore.Types.Door2DoorResponse;
using System.Globalization;
using Newtonsoft.Json;
/*********************************************************/

namespace Door2DoorConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            // request object
            D2DRequest req = new D2DRequest(); 

            // I want to get to my destination at the datetime bellow
            req.desiredOutboundDate = DateTime.ParseExact("2014-10-15T20:00:00", "yyyy-MM-ddTHH:mm:ss", CultureInfo.InvariantCulture);
            req.outboundDateKind = D2dRequestTripDateKind.arriveAt;

            // I want to leave my destination at the datetime bellow
            req.desiredInboundDate = DateTime.ParseExact("2014-10-17T20:00:00", "yyyy-MM-ddTHH:mm:ss", CultureInfo.InvariantCulture);
            req.outboundDateKind = D2dRequestTripDateKind.departureAt;

            // specifying the origin
            req.oriLocation = new D2DRequestLocation();
            req.oriLocation.lat = "-23.5732853";
            req.oriLocation.lng = "-46.64167550000002";
            req.oriLocation.type = "street_address";

            // specifying the destination
            req.destLocation = new D2DRequestLocation();
            req.destLocation.lat = "52.3661876";
            req.destLocation.lng = "4.899111500000004";
            req.destLocation.type = "route";

            // requesting public transp info as well
            req.flags = new D2DRequestFlags();
            req.flags.includePublicTransp = true;

            // submitting the request
            Door2Door d2d = new Door2Door(req);
            Door2DoorResponse resp = d2d.GetResponse();
    
            Console.WriteLine(JsonConvert.SerializeObject(resp));

        }
    }
}
