using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Hamastar.Common;
using Hamastar.Common.Text;
using Hamastar.BusinessObject;
using System.Configuration;
using Newtonsoft.Json;
using System.Net;
using System.Text;
using System.IO;

public partial class CaseLog : BasePage
{
    string CaseNo = string.Empty;
    protected void Page_Load(object sender, EventArgs e)
    {
        var CaseNo = Page.Request.QueryString["c"];
        int NodeID = 0;
        int.TryParse(Page.Request.QueryString["n"], out NodeID);
        StringBuilder sbLog = new StringBuilder();
        List<vw_Record> listRecord = new List<vw_Record>();
        listRecord = vw_Record.GetvwRecordForCase(NodeID, CaseNo);
        foreach (var record in listRecord)
        {
            sbLog.AppendFormat("<tr><td style='text-align:center'>{0}</td><td style='text-align:center'>{1}</td><td>{2}</td><td style='text-align:center'>{3:yyyy-MM-dd HH:mm:ss}</td></tr>"
                                 , record.Name, record.Action, record.Record, record.ModifyDate);
        }

        litLog.Text = sbLog.ToString();
    }

}