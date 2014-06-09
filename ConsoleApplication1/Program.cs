using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Door2DoorCore;
using Door2DoorCore.Types;
using Door2DoorCore.Types.Door2DoorRequest;
using Door2DoorCore.Types.Rome2RioResponse;
using Door2DoorCore.Exceptions;


namespace ConsoleApplication1
{
    class Program
    {
        static void Main(string[] args)
        {

            D2DRequest req = new D2DRequest();
            req.desiredArrivalDate = new DateTime(2014, 06, 20, 1, 0, 0);

            D2DRequestLocation oriLoc = new D2DRequestLocation();
            oriLoc.lat="-23.5732882";
            oriLoc.lng= "-46.6416702";
            oriLoc.type="street_address";
            req.oriLocation = oriLoc;

            D2DRequestLocation destLoc = new D2DRequestLocation();
            destLoc.lat = "51.4788527";
            destLoc.lng = "-0.08349459999999453";
            destLoc.type = "street_address";
            req.destLocation = destLoc;

            D2DRequestFlags flags= new D2DRequestFlags();
            flags.includePublicTransp = false;
            req.flags = flags;

            Door2Door d2d = new Door2Door(req);
            Rome2RioResponse resp = d2d.GetResponse();

        }
    }
}
