using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Door2DoorCore.Types;
using Door2DoorCore.Types.Rome2RioResponse;
using Door2DoorCore.Types.Door2DoorRequest;
using Door2DoorCore.Exceptions;
using System.Net;
using Newtonsoft.Json;

namespace Door2DoorCore
{
    internal class Rome2RioComm : IDisposable
    {
        private D2DRequest _req;

        private Rome2RioResponse _resp;
        public Rome2RioResponse Resp
        {
            get { return _resp; }
        }

        public delegate void MessageReceivedEventHandler(Rome2RioResponse resp);
        public event MessageReceivedEventHandler OnMessageReceived;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="req"></param>
        public Rome2RioComm(D2DRequest req)
        {
            _req = req;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public Rome2RioResponse Download()
        {
            using (WebClient client = new WebClient())
            {
                try
                {
                    client.QueryString = BuildQueryString();
                    string url = _req.url;
                    string r = client.DownloadString(url);
                    _resp = JsonConvert.DeserializeObject<Rome2RioResponse>(r);
                    return _resp;
                }
                catch (Exception e)
                {
                    throw new D2DResponseException(e);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public void DownloadAsync()
        {
            using (WebClient client = new WebClient())
            {
                client.DownloadStringCompleted += new DownloadStringCompletedEventHandler(OnDownloadStringCompleted);
                client.QueryString = BuildQueryString();
                string url = _req.url;
                client.DownloadStringAsync(new Uri(url));
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void OnDownloadStringCompleted(object sender, DownloadStringCompletedEventArgs e)
        {
            if (e.Error == null)
            {
                _resp = JsonConvert.DeserializeObject<Rome2RioResponse>(e.Result);
                OnMessageReceived(_resp);
            }
            else
            {
                throw new D2DResponseException(e.Error);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private System.Collections.Specialized.NameValueCollection BuildQueryString()
        {
            System.Collections.Specialized.NameValueCollection collection = new System.Collections.Specialized.NameValueCollection();

            collection.Add("key", _req.key);
            collection.Add("currency", _req.currency);
            collection.Add("flags", BuildSearchRequestFlags().ToString());
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
            return collection;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private int BuildSearchRequestFlags()
        {
            //0x01000000 => Exclude path information (saves bandwidth)
            int flagsIncludeAll = Convert.ToInt32("0x01000000", 16);
            if (!_req.flags.includePublicTransp)
            {
                flagsIncludeAll +=
                    //+ parseInt("0x00000010", 16) // train
                    +Convert.ToInt32("0x00000100", 16) // bus
                    //+ parseInt("0x00001000", 16) // ferry
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
