using Door2DoorCore;
using Door2DoorCore.Types.Door2DoorRequest;
using Door2DoorCore.Types.Rome2RioResponse;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Door2DoorWebApp.Controllers
{
    public class D2DController : ApiController
    {

        /// <summary>
        /// 
        /// </summary>
        /// <param name="option"></param>
        /// <returns></returns>
        //[Route("api/getd2d/{option}")]
        //[HttpGet]
        //public Rome2RioResponse Getd2d(string option)
        //{
        //    D2DRequest req = new D2DRequest();
        //    req.desiredArrivalDate = new DateTime(2014, 06, 20, 2, 0, 0);

        //    D2DRequestLocation oriLoc = new D2DRequestLocation();
        //    oriLoc.lat = "-23.5732882";
        //    oriLoc.lng = "-46.6416702";
        //    oriLoc.type = "street_address";
        //    req.oriLocation = oriLoc;

        //    D2DRequestLocation destLoc = new D2DRequestLocation();
        //    destLoc.lat = "51.4788527";
        //    destLoc.lng = "-0.08349459999999453";
        //    destLoc.type = "street_address";
        //    req.destLocation = destLoc;

        //    D2DRequestFlags flags = new D2DRequestFlags();
        //    flags.includePublicTransp = false;
        //    req.flags = flags;

        //    Door2Door d2d = new Door2Door(req);
        //    Rome2RioResponse resp = d2d.GetResponse();


        //    return resp;

        //}

        /// <summary>
        /// 
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        [Route("api/getd2d")]
        [HttpPost]
        public Rome2RioResponse Getd2d(D2DRequest req)
        {
            Rome2RioResponse resp;
            //string json = JsonConvert.SerializeObject(req2);

            using (Door2Door d2d = new Door2Door(req))
            {
                resp = d2d.GetResponse();
            }
            return resp;
        }

    }
}