﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Door2DoorCore.Types.Door2DoorRequest
{
    /// <summary>
    ///     For now only Rome2rio, r2r will be accepted
    /// </summary>
    public enum D2DRequestType
    {
        /// <summary>
        /// Rome2Rio
        /// </summary>
        r2r = 0,
        /// <summary>
        /// Not used
        /// </summary>
        other = 1
    }
}
