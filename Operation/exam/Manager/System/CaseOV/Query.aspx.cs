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
public partial class System_CaseOV_Query : BasePage
{

    protected void Page_Load(object sender, EventArgs e)
    {
        (this.Master.FindControl("litNav_Title") as Literal).Text = "案件總覽";
        if (this.IsPostBack) return;
        CurrentConditions["CaseNo"] = "";

        // btnAdd.PostBackUrl = string.Format("/System/CasePR/Detail.aspx{0}", jSecurity.XSS(Request.Url.Query));


    }


    #region GridView事件
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

    protected void gvIndex_RowCommand(object sender, GridViewCommandEventArgs e)
    {
        string CaseNo = e.CommandArgument.ToString();
        switch (e.CommandName)
        {
            case "btnEdit":
            case "btnRead":
                //SetQparm();
                CurrentConditions["CaseNo"] = CaseNo;
                CurrentConditions["Action"] = e.CommandName.Substring(3);
                Response.Redirect(string.Format("/System/CasePR/Detail.aspx{0}", Request.Url.Query));
                break;
            case "btnLog":
                Pages.WindowOpen("/CaseLog.aspx", "_DealData", 600, 300);
                break;
        }
    }



    protected void odsIndex_Load(object sender, EventArgs e)
    {
        SetQparm();

    }

    #endregion

    #region Page
    //資料重整
    protected void BtnRefresh_Click(object sender, EventArgs e)
    {
        gvIndex.DataBind();
    }
    protected void gvIndex_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow) //判斷當前行是否是數據行
        {
            //編輯紐功能
            //Comm_Case Item = e.Row.DataItem as Comm_Case;
            //Button btnEdit = e.Row.FindControl("btnEdit") as Button;
            //btnEdit.CommandArgument = Item.CaseNo.ToString();
        }
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
        SetQparm();
    }

    protected void btnClear_Click(object sender, EventArgs e)
    {
        txtPID.Text = "";
        txtName.Text = "";

        SetQparm();
        gvIndex.DataBind();
    }

    private void SetQparm()
    {
        odsIndex.SelectParameters["KeyWordBeforDateS"] = new Parameter("KeyWordBeforDateS", DbType.String, string.Empty);     //收件日期-起
        odsIndex.SelectParameters["KeyWordBeforDateE"] = new Parameter("KeyWordBeforDateE", DbType.String, string.Empty);     //收件日期-迄
        odsIndex.SelectParameters["KeyWordStatus"] = new Parameter("KeyWordStatus", DbType.String, string.Empty);                 //案件狀態
        odsIndex.SelectParameters["KeyWordDeptSN"] = new Parameter("KeyWordDeptSN", DbType.String, string.Empty);                 //醫療院所名稱
        odsIndex.SelectParameters["KeyWordWriteOffTransfer"] = new Parameter("KeyWordWriteOffTransfer", DbType.String, string.Empty);    //


        odsIndex.SelectParameters["KeyWordPID"] = new Parameter("KeyWordPID", DbType.String, txtPID.Text);      //個案身分證字號
        odsIndex.SelectParameters["KeyWordName"] = new Parameter("KeyWordName", DbType.String, txtName.Text);   //個案姓名
        odsIndex.SelectParameters["CaseType"] = new Parameter("CaseType", DbType.String, "CaseOV");             //案件管理類型
    }

    /// <summary>
    /// 取得院所名稱
    /// </summary>
    /// <param name="DeptSN"></param>
    /// <returns></returns>
    protected string GetDept(string DeptSN)
    {
        string rtn = "";
        Comm_Department d = Comm_Department.GetDepartment(DeptSN);
        if (null != d)
            rtn = d.DeptName;
        return rtn;
    }
}