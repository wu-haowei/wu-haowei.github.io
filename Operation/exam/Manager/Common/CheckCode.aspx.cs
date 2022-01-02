using Hamastar.Common;
using System;
using System.Drawing;

public partial class Common_CheckCode : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        string checkCode = "";
        if (Request.QueryString.Get("t") != null)
        {
            checkCode = Tools.DecryptAES(jSecurity.GetQueryString("t").ToString());
        }
        chkcode.CreateImage(checkCode, this);
    }
}