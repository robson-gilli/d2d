using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Door2DoorCore.Types.Door2DoorResponse;
using Newtonsoft.Json;

namespace Door2DoorWebApp
{
    public partial class OutputPage : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (Request.Form["chosenItin"] != null)
                {
                    try
                    {
                        Itinerary itin = JsonConvert.DeserializeObject<Itinerary>(Request.Form["chosenItin"]);
                        litJsonRq.Text = JsonConvert.SerializeObject(itin);
                    }
                    catch 
                    {
                        litJsonRq.Text = "The posted content is not a valid 'Door2DoorCore.Types.Door2DoorResponse.Itinerary' object.";

                    }
                }
                else
                {
                    litJsonRq.Text = "Nothing was posted or the posted content is not a valid 'Door2DoorCore.Types.Door2DoorResponse.Itinerary' object.";

                }
            }
        }
    }
}