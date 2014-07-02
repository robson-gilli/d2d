using Door2DoorCore.Types.Door2DoorRequest;
using Door2DoorCore.Types.Door2DoorResponse;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Door2DoorCore.Interfaces
{
    interface IDoor2DoorProvider
    {
        Door2DoorResponse GetResponse();
    }
}
