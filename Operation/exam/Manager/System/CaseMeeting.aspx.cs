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

public partial class CaseMeeting : BasePage
{
    string CaseNo = string.Empty;
    protected void Page_Load(object sender, EventArgs e)
    {
        //var CaseNo = Page.Request.QueryString["c"];
        //int NodeID = 0;
        //int.TryParse(Page.Request.QueryString["n"], out NodeID);
        //StringBuilder sbLog = new StringBuilder();
        //List<vw_Record> listRecord = new List<vw_Record>();
        //listRecord = vw_Record.GetvwRecordForCase(NodeID, CaseNo);
        //foreach (var record in listRecord)
        //{
        //    sbLog.AppendFormat("<tr><td>{0}</td><td>{1}</td><td>{2}</td><td>{3:yyyy-MM-dd HH:mm:ss}</td></tr>"
        //                         , record.Name, record.Record, "", DateTime.Now);
        //}

        //litLog.Text = sbLog.ToString();
        if (IsPostBack)
            return;

        Label lblTitleName = Master.FindControl("lblTitleName") as Label;
        lblTitleName.Text = "案件歷程";


        BindProfUser();
    }


    #region 專業審查委員
    protected void ddlProfUser_SelectedIndexChanged(object sender, EventArgs e)
    {
        string strSN = string.Empty;
        if (!string.IsNullOrEmpty(ddlProfUser.SelectedValue))
        {
            strSN = ddlProfUser.SelectedValue;
        }
        hdProfUser.Value = strSN;
    }
    private void BindProfUser()
    {
        var Query = Comm_Examiner.GetAllData();
        ddlProfUser.Items.Add(new ListItem("請選擇委員姓名", ""));
        foreach (var data in Query)
        {
            ddlProfUser.Items.Add(new ListItem(data.Name, data.SN.ToString()));
        }
    }
    #endregion
}