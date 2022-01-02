using Hamastar.BusinessObject;
using Hamastar.Common;
using Hamastar.Common.Text;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Microsoft.Reporting.WebForms;

public partial class System_Report_Query : BasePage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
        }
    }

    protected void btnExport_Click(object sender, EventArgs e)
    {
        #region 資料檢查
        if (ddlReportType.SelectedValue == "")
        {
            Pages.AlertByswal(Tools.altertType.錯誤.ToString(), "請選擇要匯出的報表類型", Tools.altertType.錯誤.ToDescriptionString());
            return;
        }
        string check = CheckData();
        if (!string.IsNullOrEmpty(check))
        {
            Pages.AlertByswal(Tools.altertType.錯誤.ToString(), check, Tools.altertType.錯誤.ToDescriptionString());
            return;
        }
        #endregion


        byte[] result = null;

        #region 組要傳的參數
        ReportModel parm = new ReportModel();
        switch (ddlReportType.SelectedValue)
        {
            case "審查案件一覽表":
                if (!string.IsNullOrEmpty(txtSDate.Text))
                    parm.Sdate = Convert.ToDateTime(txtSDate.Text);
                if (!string.IsNullOrEmpty(txtEDate.Text))
                    parm.Edate = Convert.ToDateTime(txtEDate.Text);
                parm.MeetType = ddlMeetingType.SelectedValue;
                parm.DataType = ddlDataType.SelectedValue;
                break;
            case "裝置完成統計表":
                break;
            case "核銷統計表(名冊)":
                break;
            case "合約院所執行表":
                break;
            case "滿意度調查表":
                break;
        }
        #endregion
        ReportService rs = new ReportService();
        result = rs.ReportProcess(ddlReportType.SelectedValue, parm);
        string filename = HttpUtility.UrlEncode(ddlReportType.SelectedValue);
        HttpContext.Current.Response.AddHeader("Content-Disposition", string.Format("attachment; filename={0}", filename + ".xls"));
        Response.BinaryWrite(result);


    }

    #region 資料檢查
    private string CheckData()
    {
        StringBuilder sbError = new StringBuilder();
        //if (string.IsNullOrWhiteSpace(SDate.Text) || string.IsNullOrWhiteSpace(EDate.Text))
        //    sbError.Append(@"請輸入日期區間\n");

        return sbError.ToString();
    }
    #endregion





    protected void ddlReportType_SelectedIndexChanged(object sender, EventArgs e)
    {
        trReport1_1.Visible = false;
        trReport1_2.Visible = false;
        switch (ddlReportType.SelectedValue)
        {
            case "審查案件一覽表":
                trReport1_1.Visible = true;
                trReport1_2.Visible = true;
                break;
            case "裝置完成統計表":
                break;
            case "核銷統計表(名冊)":
                break;
            case "合約院所執行表":
                break;
            case "滿意度調查表":
                break;
        }

    }
}