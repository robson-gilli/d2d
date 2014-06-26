using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Door2DoorCore.Types.Door2DoorRequest
{

    /// <summary>
    ///     Coordenates: latitude, longitude and place type.
    /// </summary>
    public class D2DRequestLocation
    {
        public string lat { get; set; }
        public string lng { get; set; }
        /// <summary>
        ///     Supported values include Rome2rio Place kinds, Google Geocoding result types and Yahoo WOEID place types.
        /// </summary>
        public string type { get; set; }

    }
}
