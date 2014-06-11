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
        }

        /// <summary>
        /// 
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack && IsPostOk())
            {
                //populate json objet with post data so javascript can know what's going on
                PostData p = new PostData();
                p.allowInter = bool.Parse(Request.Form["allowInter"]);
                p.incPublicTransp = bool.Parse(Request.Form["incPublicTransp"]);
                p.maxDepDate = Request.Form["maxDepDate"];
                p.maxDriveKm = int.Parse(Request.Form["maxDriveKm"]);
                p.minDepDate = Request.Form["minDepDate"];
                string json = JsonConvert.SerializeObject(p);
                litJsonRq.Text = json;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private bool IsPostOk()
        {
            bool r = false;

            if( Request.Form["minDepDate"]      != null &&
                Request.Form["maxDepDate"]      != null &&
                Request.Form["incPublicTransp"] != null &&
                Request.Form["maxDriveKm"]      != null &&
                Request.Form["allowInter"]      != null)
            {
                bool incPublicTransp;
                if (bool.TryParse(Request.Form["incPublicTransp"], out incPublicTransp))
                {
                    int maxDriveKm;
                    if (int.TryParse(Request.Form["maxDriveKm"], out maxDriveKm))
                    {
                        bool allowInter;
                        if (bool.TryParse(Request.Form["allowInter"], out allowInter))
                        {
                            DateTime minDepDate;
                            if (DateTime.TryParseExact(Request.Form["minDepDate"], "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out minDepDate))
                            {
                                DateTime maxDepDate;
                                if (DateTime.TryParseExact(Request.Form["maxDepDate"], "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out maxDepDate))
                                {
                                    r = true;
                                }
                            }
                        }
                    }
                }
            }
            return r;
        }






    }
}