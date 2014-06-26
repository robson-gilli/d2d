using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Door2DoorCore.Types.Door2DoorRequest
{
    /// <summary>
    ///     Filters allowed transportation types
    /// </summary>
    public class D2DRequestFlags
    {
        /// <summary>
        ///     Should it include public transportation?
        /// </summary>
        public bool includePublicTransp { get; set; }
    }
}
