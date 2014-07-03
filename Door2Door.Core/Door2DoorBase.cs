using Door2DoorCore.Exceptions;
using Door2DoorCore.Types.Door2DoorRequest;
using Door2DoorCore.Types.Door2DoorResponse;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Door2DoorCore
{
    internal abstract class Door2DoorBase
    {
        protected int _maxWalkingMinutes = 10;
        protected int _flightAntecipation = 120;
        protected int _minutesAfterFlight = 30;

        //static protected string _path;

        /// <summary>
        /// A <see cref="Door2DoorCore.Types.Door2DoorRequest.D2DRequest"/>. Holds the request of the itinerary.
        /// </summary>
        protected D2DRequest _req;
        public D2DRequest Req
        {
            get { return _req; }
            set { _req = value; }
        }


        /// <summary>
        /// A <see cref="Door2DoorCore.Types.Door2DoorResponse.Door2DoorResponse"/>. Complete Door2Door response.
        /// </summary>
        protected Door2DoorResponse _resp;
        /// <summary>
        ///     Routes and schedules for the desired itinerary request. <see cref="Door2DoorCore.Types.Door2DoorResponse.Door2DoorResponse"/>
        /// </summary>
        public Door2DoorResponse Resp
        {
            get { return _resp; }
        }

        public Door2DoorBase(D2DRequest d2dReq, int maxWalkingMinutes, int flightAntecipation, int minutesAfterFlight)
        {
            _maxWalkingMinutes = maxWalkingMinutes;
            _flightAntecipation = flightAntecipation;
            _minutesAfterFlight = minutesAfterFlight;

            VerifyRequest(d2dReq);
        }


        public Door2DoorBase(D2DRequest d2dReq)
        {
            VerifyRequest(d2dReq);
        }

        private void VerifyRequest(D2DRequest d2dReq)
        {
            _req = d2dReq;
            if (!RequestIsOK(_req))
            {
                throw new D2DRequestException("Please check your request.");
            }
        }

        //private void LoadConfigs()
        //{
        //    if (!_configLoaded)
        //    {
        //        try
        //        {
        //            string path = Path.GetDirectoryName(GetType().Assembly.Location);
        //            string file = (new System.Uri(Assembly.GetExecutingAssembly().CodeBase)).AbsolutePath;
        //            FileInfo fi = new FileInfo(file);
        //            string configPath = Path.Combine(fi.DirectoryName, "Configs", "Config.xml");
        //            _path = configPath;

        //            XmlDocument config = new XmlDocument();
        //            config.Load(configPath);

        //            _maxWalkingMinutes = int.Parse(config.SelectNodes("Door2DoorConfig//MaxWalkingMinutes")[0].InnerText);

        //            //_configLoaded = true;
        //        }
        //        catch (Exception e)
        //        {
        //            _path = e.Message + " - " + e.StackTrace;
        //            //throw new Exception("Invalid Door2Door config file.", e);
        //        }

        //    }

        //}

        /// <summary>
        ///     Verifies if all necessary data from the request are correctly informed
        /// </summary>
        /// <param name="d2dReq">
        ///     Route request. <see cref="Door2DoorCore.Types.Door2DoorRequest.D2DRequest"/>
        /// </param>
        /// <returns>
        ///     Whether the request is ok or not
        /// </returns>
        protected bool RequestIsOK(D2DRequest d2dReq)
        {
            return d2dReq.flags != null &&
                    d2dReq.desiredOutboundDate != DateTime.MinValue && d2dReq.desiredOutboundDate != DateTime.MaxValue &&
                    d2dReq.oriLocation != null &&
                    d2dReq.destLocation != null;
        }

        protected abstract void BuildCompleteItinerarySchedule();
    }
}
