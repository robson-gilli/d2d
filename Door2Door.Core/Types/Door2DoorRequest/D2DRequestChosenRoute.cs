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
    public class D2DRequestChosenRoute
    {
        public int? itineraryIndex { get; set; }
        public int? segmentIndex { get; set; }
        public int? routeIndex { get; set; }
    }
}
