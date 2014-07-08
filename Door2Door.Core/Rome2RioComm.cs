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

        /// <summary>
        /// Delegate for asynchronous use
        /// </summary>
        /// <param name="resp">
        /// <see cref="Door2DoorCore.Types.Door2DoorResponse.Door2DoorResponse"/>
        /// </param>
        internal delegate void MessageReceivedEventHandler(Door2DoorResponse resp);
        
        /// <summary>
        /// Event raised when the response arrives if it was requested asynchronously
        /// </summary>
        internal event MessageReceivedEventHandler OnMessageReceived;

        /// <summary>
        /// Preventing critical region
        /// </summary>
        private static object mutex;

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
        internal Door2DoorResponse Resp
        {
            get { return _resp; }
        }

        /// <summary>
        /// Request flags informed on the request
        /// </summary>
        private int _requestFlags;

        /// <summary>
        ///     Handles the communication between client and Rome2rio api.
        /// </summary>
        /// <param name="req">
        ///     Parameters for the request include coordinates of origin and destination,
        ///     arrival and return dates, external flight options and filters.
        /// </param>
        /// <param name="requestFlags">
        ///     Filters the response segment types.
        /// </param>
        internal Rome2RioComm(D2DRequest req, int requestFlags)
        {
            _requestFlags = requestFlags;
            _req = req;
            _resp = new Door2DoorResponse();
        }

        /// <summary>
        /// Downloads the response from the API, according to the stay informed.
        /// </summary>
        /// <param name="useStayAsDestination">
        /// If it was informed a stay, it calculates de routes between the hotel and the destination
        /// </param>
        /// <returns>Door2Door response <see cref="Door2DoorCore.Types.Door2DoorResponse.Door2DoorResponse"/></returns>
        internal Door2DoorResponse Download(bool useStayAsDestination)
        {
            if (useStayAsDestination)
            {
                _req.oriLocation = _req.destLocation;
                _req.destLocation = _req.chosenStay.location;
            }
            return Download();
        }

        /// <summary>
        ///     Downloads the response from the API, according to the request informed.
        /// </summary>
        /// <returns>
        ///     Raw response, before schedules and totals addition.
        /// </returns>
        internal Door2DoorResponse Download()
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

        /// <summary>
        ///  Performs the operation asynchronously.
        ///  When all reponse come, the event 'OnMessageReceived' will be raised
        /// </summary>
        internal void DownloadAsync()
        {
            List<Task> taskList = new List<Task>();
            string url = _req.url;
            //ida
            using (WebClient client = new WebClient())
            {
                client.DownloadStringCompleted += new DownloadStringCompletedEventHandler(OnOutboundDownloadStringCompleted);
                client.QueryString = BuildQueryString(false);
                client.DownloadStringAsync(new Uri(url));
            }

            // volta, se tiver
            if (_req.desiredInboundDate.HasValue)
            {
                using (WebClient client = new WebClient())
                {
                    client.DownloadStringCompleted += new DownloadStringCompletedEventHandler(OnInboundDownloadStringCompleted);
                    client.QueryString = BuildQueryString(false);
                    client.DownloadStringAsync(new Uri(url));
                }
            }
        }

        /// <summary>
        /// Event raised when the inbound message comes asynchronously
        /// </summary>
        /// <param name="sender">Object sender</param>
        /// <param name="e">DownloadStringCompletedEventArgs e </param>
        private void OnInboundDownloadStringCompleted(object sender, DownloadStringCompletedEventArgs e)
        {
            if (e.Error == null)
            {
                // O lock me garante que :
                // 1 -  Nao vai ter duas tasks escrevendo e/ou instanciando o mesmo objeto
                // 2 - Eu saiba quando chega a ultima mensabem para chamar o evento 'OnMessageReceived'
                lock (mutex)
                {
                    _respVolta = JsonConvert.DeserializeObject<Door2DoorLegResponse>(e.Result);

                    // se é nulo é porque é a primeira mensagem que chega
                    if (_resp.LegResponse == null)
                    {
                        _resp.LegResponse = new List<Door2DoorLegResponse>(2);
                        _resp.LegResponse[1] = _respVolta;
                    }
                    else // senao ja chegou a outra, pode chamar o evento
                    {
                        _resp.LegResponse[1] = _respVolta;
                        OnMessageReceived(_resp);
                    }
                }
            }
            else
            {
                throw new D2DResponseException(e.Error);
            }
        }

        /// <summary>
        /// Event raised when the outbound message comes asynchronously
        /// </summary>
        /// <param name="sender">Object sender</param>
        /// <param name="e">DownloadStringCompletedEventArgs e </param>
        private void OnOutboundDownloadStringCompleted(object sender, DownloadStringCompletedEventArgs e)
        {
            if (e.Error == null)
            {
                // O lock me garante que :
                // 1 -  Nao vai ter duas tasks escrevendo e/ou instanciando o mesmo objeto
                // 2 - Eu saiba quando chega a ultima mensabem para chamar o evento 'OnMessageReceived'
                lock (mutex)
                {
                    _respIda = JsonConvert.DeserializeObject<Door2DoorLegResponse>(e.Result);

                    // se é nulo é porque é a primeira mensagem que chega
                    if (_resp.LegResponse == null)
                    {
                        _resp.LegResponse = new List<Door2DoorLegResponse>(2);
                        _resp.LegResponse[0] = _respIda;
                    }
                    else // senao ja chegou a outra, pode chamar o evento
                    {
                        _resp.LegResponse[0] = _respIda;
                        OnMessageReceived(_resp);
                    }
                }
            }
            else
            {
                throw new D2DResponseException(e.Error);
            }
        }

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
            collection.Add("flags", _requestFlags.ToString());
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
        /// 
        /// </summary>
        public void Dispose()
        {
            _req = null;
            _resp = null;
        }
    }
}
