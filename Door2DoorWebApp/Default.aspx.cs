using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Door2DoorWebApp
{
    public partial class Default : System.Web.UI.Page
    {
        private struct PostData
        {
            public string minDepDate;
            public string maxDepDate;
            public int maxDriveKm;
            public bool incPublicTransp;
            public bool allowInter;
            public string outputUrl;
            public string iframeInputUrl;
        }

        /// <summary>
        /// 
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack )
            {
                PopulatePage();

            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private void PopulatePage()
        {
            bool isOk = Request.Form["minDepDate"] != null &&
                        Request.Form["maxDepDate"] != null &&
                        Request.Form["incPublicTransp"] != null &&
                        Request.Form["maxDriveKm"] != null &&
                        Request.Form["allowInter"] != null &&
                        Request.Form["outputUrl"] != null &&
                        Request.Form["iframeInputUrl"] != null;

            
            //incPublicTransp
            if (isOk)
            {
                bool incPublicTransp;
                isOk = bool.TryParse(Request.Form["incPublicTransp"], out incPublicTransp);
            }
            //maxDriveKm
            if (isOk)
            {
                int maxDriveKm;
                isOk = int.TryParse(Request.Form["maxDriveKm"], out maxDriveKm);
            }
            //allowInter
            if (isOk)
            {
                bool allowInter;
                isOk = bool.TryParse(Request.Form["allowInter"], out allowInter);
            }
            //minDepDate
            if (isOk)
            {
                DateTime minDepDate;
                isOk = DateTime.TryParseExact(Request.Form["minDepDate"], "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out minDepDate);
            }
            //maxDepDate
            if (isOk)
            {
                DateTime maxDepDate;
                isOk = DateTime.TryParseExact(Request.Form["maxDepDate"], "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out maxDepDate);
            }
            
            // if everything is ok with the request, 
            // populate json objet with post data so javascript can know what's going on
            if (isOk)
            {
                PostData p = new PostData();
                p.allowInter = bool.Parse(Request.Form["allowInter"]);
                p.incPublicTransp = bool.Parse(Request.Form["incPublicTransp"]);
                p.maxDepDate = Request.Form["maxDepDate"];
                p.maxDriveKm = int.Parse(Request.Form["maxDriveKm"]);
                p.minDepDate = Request.Form["minDepDate"];
                p.outputUrl = Request.Form["outputUrl"];
                p.iframeInputUrl = Request.Form["iframeInputUrl"];
                string json = JsonConvert.SerializeObject(p);
                litJsonRq.Text = json;
            }
        }






    }
}