using Door2DoorCore;
using Door2DoorCore.Types.Door2DoorRequest;
using Door2DoorCore.Types.Door2DoorResponse;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

namespace Door2DoorWebApp.Controllers
{
    public class Door2DoorController : ApiController
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        [Route("api/getd2d")]
        [HttpPost]
        public Door2DoorResponse Getd2d(D2DRequest req)
        {
            Door2DoorResponse resp;

            Door2Door d2d = new Door2Door(req);
            resp = d2d.GetResponse();
            string json = JsonConvert.SerializeObject(resp);

            return resp;
        }
    }
}