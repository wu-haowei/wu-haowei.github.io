using Hamastar.BusinessObject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Hamastar.Common.Text;
using Hamastar.Common;
using System.Data;
using System.Text;
public partial class Default : BasePage
{

    protected void Page_Load(object sender, EventArgs e)
    {
        Response.Redirect("/System/CasePR/Query.aspx?n=13");


    }
}