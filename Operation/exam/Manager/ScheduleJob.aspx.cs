using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Hamastar.Common.Text;

using Hamastar.BusinessObject;
using System.Xml.Linq;
using System.Threading.Tasks;
using Hamastar.Common;
using System.Data;

public partial class ScheduleJob : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        string sType = Request.QueryString.Get("Type");
        if (sType == null)
        {
            Response.Write("<font color=\"#ff0000\">AM 1:00 檢查案件過期前N天通知 Type=BefNotice</font><br/>");

        }
        else if (sType.Equals("CheckSMS"))//檢查案件過期前通知
        {

        }
    }



}