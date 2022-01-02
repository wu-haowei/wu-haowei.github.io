using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using Hamastar.BusinessObject;
using System.Data;
using Hamastar.Common.Text;
using System.Text;
using Hamastar.Common;
using System.IO;

public partial class Common_Record : BasePage
{
    private string isall = string.Empty;
    private string s = "0";
    private string n = "0";
    protected void Page_Load(object sender, EventArgs e)
    {
        if (Tools.DecryptAES(jSecurity.GetQueryString("isall")) == "Y")
        {
            n = jSecurity.GetQueryString("n");
        }
        else if (Tools.DecryptAES(jSecurity.GetQueryString("isall")) == "N")
        {
            fieldset.Visible = false;
            isall = Tools.DecryptAES(jSecurity.GetQueryString("isall"));
            s = jSecurity.GetQueryString("s");
            n = jSecurity.GetQueryString("n");
        }
        if (!IsPostBack)
        {
            Label lblTitleName = Master.FindControl("lblTitleName") as Label;
            lblTitleName.Text = "操作紀錄";

            #region 下拉式選單
            string DistinctvwRecord = vw_Record.DistinctvwRecord();
            char[] split_array = new char[] { '、' };
            string[] ArrayvwRecord = DistinctvwRecord.Split(split_array, StringSplitOptions.RemoveEmptyEntries);
            foreach (var _ArrayvwRecord in ArrayvwRecord)
            {
                ddlNodeName.Items.Add(new ListItem(_ArrayvwRecord, _ArrayvwRecord));
            }
            #endregion

        }
    }

    protected void gvIndex_RowDataBound(object sender, GridViewRowEventArgs e)
    {

    }

    protected void gvIndex_RowCommand(object sender, GridViewCommandEventArgs e)
    {

    }

    protected void odsIndex_Load(object sender, EventArgs e)
    {
        odsIndex.SelectParameters["isall"] = new Parameter("isall", DbType.String, isall);
        odsIndex.SelectParameters["s"] = new Parameter("s", DbType.Int32, s);
        odsIndex.SelectParameters["n"] = new Parameter("n", DbType.Int32, n);

        odsIndex.SelectParameters["txtID"] = new Parameter("txtID", DbType.String, txtID.Text);  //帳號查詢
        odsIndex.SelectParameters["txtName"] = new Parameter("txtName", DbType.String, txtName.Text);  //姓名查詢
        odsIndex.SelectParameters["ddlNodeName"] = new Parameter("ddlNodeName", DbType.String, ddlNodeName.SelectedValue);  //單元名稱查詢
        odsIndex.SelectParameters["SDate"] = new Parameter("SDate", DbType.String, SDate.Text);  //起始時間查詢
        odsIndex.SelectParameters["EDate"] = new Parameter("EDate", DbType.String, EDate.Text);  //結束時間查詢
        odsIndex.SelectParameters["CaseNo"] = new Parameter("CaseNo", DbType.String, txtCaseNo.Text);  //案號


    }

    #region Page
    protected void gvIndex_DataBound(object sender, EventArgs e)
    {
        if (gvIndex.Rows.Count == 0)
        {
            DataPager1.Visible = false;
            btnGo.Visible = false;
        }
        else
        {
            DataPager1.Visible = true;
            btnGo.Visible = true;
        }
        //儲存目前頁數、頁數大小
        CurrentConditions["pim"] = new PageIndexManager { startrow = DataPager1.StartRowIndex, maxrow = DataPager1.MaximumRows };
    }

    protected void btnGo_Click(object sender, EventArgs e)
    {
        TextBox txtPageSize = this.DataPager1.Controls[2].FindControl("txtPageSize") as TextBox;
        TextBox txtCurrentPage = this.DataPager1.Controls[2].FindControl("txtCurrentPage") as TextBox;
        int PageSize = 10;
        int CurrentPage = 1;

        int.TryParse(txtPageSize.Text, out PageSize);
        int.TryParse(txtCurrentPage.Text, out CurrentPage);

        if (CurrentPage < 1) CurrentPage = 1;
        DataPager1.SetPageProperties((PageSize * (CurrentPage - 1)), PageSize, true);
    }
    #endregion

    protected void btnSearch_Click(object sender, EventArgs e)
    {
        #region 資料檢查
        string chkerror = CheckData();
        if (!string.IsNullOrEmpty(chkerror))
        {
            Pages.AlertByswal(Tools.altertType.注意.ToString(), chkerror, Tools.altertType.注意.ToDescriptionString());
            return;
        }
        #endregion
    }

    #region 資料檢查
    private string CheckData()
    {
        StringBuilder sbError = new StringBuilder();
        if (!string.IsNullOrEmpty(SDate.Text) && !string.IsNullOrEmpty(EDate.Text))
        {
            if (Convert.ToDateTime(SDate.Text) > Convert.ToDateTime(EDate.Text))
                sbError.Append(@"結束時間不可小於起始時間\n");
        }
        return sbError.ToString();
    }
    #endregion


    protected void btnExcel_Click(object sender, EventArgs e)
    {
        DataTable data = vw_Record.GetExcelDataList("", isall, Convert.ToInt32(s), Convert.ToInt32(n), txtID.Text, txtName.Text, ddlNodeName.SelectedValue, SDate.Text, EDate.Text, txtCaseNo.Text);
        if (data.Rows.Count == 0)
        {
            Pages.Alert("查無資料可以匯出");
            return;
        }

        string[] columnname = new string[] { "單位", "帳號", "姓名", "修改日期時間", "上線位置", "操作系統", "操作內容" };
        string sFileName = HttpUtility.UrlEncode("DataOperationRecord", Encoding.UTF8);
        MemoryStream ms = Tools.ExportDataTableToExcel(data, columnname, true) as MemoryStream;
        Response.Clear();
        Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
        Response.AddHeader("Content-Disposition", string.Format("attachment; filename={0}.xls", sFileName));
        Response.BinaryWrite(ms.ToArray());
        ms.Close();
        ms.Dispose();
        Response.Flush();
        Response.End();

    }
}