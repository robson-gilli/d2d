using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Door2DoorCore.Types.Door2DoorRequest
{
    /// <summary>
    /// 
    /// </summary>
    public class D2DRequest
    {
        public D2DRequest() { }

        public D2DRequestType requestType = D2DRequestType.r2r;

        public string url = "http://free.rome2rio.com/api/1.2/json/Search";
        public string key = "5Hhhi538";
        public string currency = "BRL";
        public string type = "GET";
        public string dataTye = "json";

        public DateTime desiredArrivalDate { get; set; }
        public D2DRequestFlags flags { get; set; }
        public D2DRequestLocation oriLocation { get; set; }
        public D2DRequestLocation destLocation { get; set; }
    }
}
