using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Door2DoorCore.Types;
using Door2DoorCore.Types.Door2DoorResponse;
using Door2DoorCore.Types.Door2DoorRequest;
using Door2DoorCore.Exceptions;
using System.Net;
using Newtonsoft.Json;

namespace Door2DoorCore
{
    /// <summary>
    ///     Responsible for communicating with Rome2rio REST API.
    /// </summary>
    internal class Rome2RioComm : IDisposable
    {
        /// <summary>
        /// A <see cref="Door2DoorCore.Types.Door2DoorRequest.D2DRequest"/>. Holds the request of the itinerary.
        /// </summary>
        private D2DRequest _req;
//        public delegate void MessageReceivedEventHandler(Door2DoorResponse resp);
//        public event MessageReceivedEventHandler OnMessageReceived;

        /// <summary>
        ///     Outbound Response.
        /// </summary>
        private Door2DoorLegResponse  _respIda;
        /// <summary>
        ///     Inbound Response.
        /// </summary>
        private Door2DoorLegResponse _respVolta;
        /// <summary>
        /// Raw <see cref="Door2DoorCore.Types.Door2DoorResponse.Door2DoorResponse"/>, before schedules and totals addition.
        /// </summary>
        private Door2DoorResponse _resp;
        /// <summary>
        ///     Raw <see cref="Door2DoorCore.Types.Door2DoorResponse.Door2DoorResponse"/>, before schedules and totals addition.
        /// </summary>
        public Door2DoorResponse Resp
        {
            get { return _resp; }
        }

        /// <summary>
        ///     Handles the communication between client and Rome2rio api.
        /// </summary>
        /// <param name="req">
        ///     Parameters for the request include coordinates of origin and destination,
        ///     arrival and return dates, external flight options and filters.
        /// </param>
        public Rome2RioComm(D2DRequest req)
        {
            _req = req;
            _resp = new Door2DoorResponse();
        }

        /// <summary>
        ///     Downloads the response from the API, according to the request informed.
        /// </summary>
        /// <returns>
        ///     Raw response, before schedules and totals addition.
        /// </returns>
        public Door2DoorResponse Download()
        {
            List<Task> taskList = new List<Task>();
            string url = _req.url;
            //ida
            using (WebClient client = new WebClient())
            {
                try
                {
                    client.QueryString = BuildQueryString(false);
                    taskList.Add(Task.Factory.StartNew(() =>
                    {
                        string r = client.DownloadString(url);
                        _respIda = JsonConvert.DeserializeObject<Door2DoorLegResponse>(r);
                    }));
                }
                catch (Exception e)
                {
                    throw new D2DResponseException(e);
                }
            }
            
            // volta, se tiver
            if (_req.desiredInboundDate.HasValue) 
            {
                using (WebClient clientVolta = new WebClient())
                {
                    try
                    {
                        taskList.Add(Task.Factory.StartNew(() =>
                        {
                            clientVolta.QueryString = BuildQueryString(true);
                            string r = clientVolta.DownloadString(url);
                            _respVolta = JsonConvert.DeserializeObject<Door2DoorLegResponse>(r);
                        }));
                    }
                    catch (Exception e)
                    {
                        throw new D2DResponseException(e);
                    }
                }
            }

            Task.WaitAll(taskList.ToArray());

            // tem que existir uma resposta para ida e outra para volta pois a ordem em que elas estao na lista '_resp' importa
            // tem que adicionar primeiro a resposta da ida, depois a da volta, se tiver
            _resp.LegResponse = new List<Door2DoorLegResponse>();
            _resp.LegResponse.Add(_respIda);
            if (_respVolta != null)
            {
                _resp.LegResponse.Add(_respVolta);
            }
            return _resp;
        }

        ///// <summary>
        ///// 
        ///// </summary>
        //public void DownloadAsync()
        //{
        //    using (WebClient client = new WebClient())
        //    {
        //        client.DownloadStringCompleted += new DownloadStringCompletedEventHandler(OnDownloadStringCompleted);
        //        client.QueryString = BuildQueryString();
        //        string url = _req.url;
        //        client.DownloadStringAsync(new Uri(url));
        //    }
        //}

        ///// <summary>
        ///// 
        ///// </summary>
        ///// <param name="sender"></param>
        ///// <param name="e"></param>
        //public void OnDownloadStringCompleted(object sender, DownloadStringCompletedEventArgs e)
        //{
        //    if (e.Error == null)
        //    {
        //        _resp = JsonConvert.DeserializeObject<Door2DoorResponse>(e.Result);
        //        OnMessageReceived(_resp);
        //    }
        //    else
        //    {
        //        throw new D2DResponseException(e.Error);
        //    }
        //}

        /// <summary>
        ///     Builds parameters for the API request
        /// </summary>
        /// <param name="isReturnRoute">
        ///     Whether this is a inbound leg
        /// </param>
        /// <returns>
        ///     NameValueCollection with the querystring
        /// </returns>
        private System.Collections.Specialized.NameValueCollection BuildQueryString(bool isReturnRoute)
        {
            System.Collections.Specialized.NameValueCollection collection = new System.Collections.Specialized.NameValueCollection();

            collection.Add("key", _req.key);
            collection.Add("currency", _req.currency);
            collection.Add("flags", BuildSearchRequestFlags().ToString());
            if (isReturnRoute)
            {
                collection.Add("dPos", Uri.EscapeDataString(_req.oriLocation.lat + "," + _req.oriLocation.lng));
                collection.Add("oPos", Uri.EscapeDataString(_req.destLocation.lat + "," + _req.destLocation.lng));
                if (!_req.oriLocation.type.Equals(string.Empty))
                {
                    collection.Add("dKind", Uri.EscapeDataString(_req.oriLocation.type));
                }
                if (!_req.destLocation.type.Equals(string.Empty))
                {
                    collection.Add("oKind", Uri.EscapeDataString(_req.destLocation.type));
                }
            }
            else
            {
                collection.Add("oPos", Uri.EscapeDataString(_req.oriLocation.lat + "," + _req.oriLocation.lng));
                collection.Add("dPos", Uri.EscapeDataString(_req.destLocation.lat + "," + _req.destLocation.lng));
                if (!_req.oriLocation.type.Equals(string.Empty))
                {
                    collection.Add("oKind", Uri.EscapeDataString(_req.oriLocation.type));
                }
                if (!_req.destLocation.type.Equals(string.Empty))
                {
                    collection.Add("dKind", Uri.EscapeDataString(_req.destLocation.type));
                }
            }
            return collection;
        }

        /// <summary>
        ///     <list type="bullet">
        ///         <item><description>0x00000000	Include all kinds of segments (Default)</description></item>
        ///         <item><description>0x000FFFFF	Exclude all kinds of segments (See example below)</description></item>
        ///         <item><description>0x00000001	Exclude flight segments</description></item>
        ///         <item><description>0x00000002	Exclude flight itineraries</description></item>
        ///         <item><description>0x00000010	Exclude train segments</description></item>
        ///         <item><description>0x00000020	Exclude train itineraries</description></item>
        ///         <item><description>0x00000100	Exclude bus segments</description></item>
        ///         <item><description>0x00000200	Exclude bus itineraries</description></item>
        ///         <item><description>0x00001000	Exclude ferry segments</description></item>
        ///         <item><description>0x00002000	Exclude ferry itineraries</description></item>
        ///         <item><description>0x00010000	Exclude car segments</description></item>
        ///         <item><description>0x00100000	Exclude commuter hops (commuter = local bus, train, trams, subways, etc.)</description></item>
        ///         <item><description>0x00200000	Exclude special hops (special = funiculars, steam trains, tours, etc.)</description></item>
        ///         <item><description>0x00400000	Exclude minor start segments</description></item>
        ///         <item><description>0x00800000	Exclude minor end segments</description></item>
        ///         <item><description>0x01000000	Exclude paths (saves bandwidth)</description></item>
        ///         <item><description>0x04000000	Exclude indicative prices (saves bandwidth)</description></item>
        ///         <item><description>0x10000000	Disable scoring and pruning (debug only)</description></item>
        ///         <item><description>Flights only: 0x000FFFF0 (0x000FFFFF - 0x0000000F)</description></item>
        ///         <item><description>Not via road: 0x00010100 (0x00000100 + 0x00010000)</description></item>
        ///     </list>
        ///     <para>NOTE: You can pass these flags either as a hexadecimal value (&flags=0x00010100) or simply as a decimal (&flags=65792).</para> 
        /// </summary>
        /// <returns>
        ///     Integer flag
        /// </returns>
        private int BuildSearchRequestFlags()
        {
            //0x01000000 => Exclude path information (saves bandwidth)
            int flagsIncludeAll = Convert.ToInt32("0x01000000", 16);
            if (!_req.flags.includePublicTransp)
            {
                flagsIncludeAll +=
                    +Convert.ToInt32("0x00000010", 16) // train
                    +Convert.ToInt32("0x00000100", 16) // bus
                    + Convert.ToInt32("0x00001000", 16) // ferry
                    + Convert.ToInt32("0x00100000", 16); // commutes
            }
            return flagsIncludeAll;
        }

        /// <summary>
        /// 
        /// </summary>
        public void Dispose()
        {
            _req = null;
            _resp = null;
        }
    }
}
