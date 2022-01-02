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
public partial class System_CasePR_Query : BasePage
{

    protected void Page_Load(object sender, EventArgs e)
    {
        (this.Master.FindControl("litNav_Title") as Literal).Text = "核銷案件";
        if (this.IsPostBack) return;
        CurrentConditions["CaseNo"] = "";
        if (!IsPostBack)
            BindDept(); //繫結院所資料

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
                Pages.WindowOpen($"/System/CaseLog.aspx?c={CaseNo}&n=14", "_DealData", 800, 300);
                break;

        }
    }

    protected void odsIndex_Load(object sender, EventArgs e)
    {
        SetQparm();

    }

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

    #endregion

    #region 醫療院所
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

    #region 繫結下拉選單
    //服務院所
    protected void ddlDeptSN_SelectedIndexChanged(object sender, EventArgs e)
    {
        hdDeptSN.Value = ddlDeptSN.SelectedValue;
    }
    private void BindDept()
    {
        //取得啟用之院所
        List<Comm_Department> listDept = Comm_Department.GetAllDepartmentNotStop();
        ddlDeptSN.Items.Add(new ListItem("請選擇院所", ""));
        foreach (var d in listDept)
        {
            ddlDeptSN.Items.Add(new ListItem(d.DeptName, d.SN.ToString()));
        }
    }
    //案件狀態
    protected void ddlStatus_SelectedIndexChanged(object sender, EventArgs e)
    {
        hdStatus.Value = ddlStatus.SelectedValue;
    }
    #endregion

    #endregion

    protected void btnSearch_Click(object sender, EventArgs e)
    {
        SetQparm();
    }

    protected void btnClear_Click(object sender, EventArgs e)
    {
        //CurrentConditions["qtbxKeyDeptSN"] = "";
        //CurrentConditions["qtbxKeyName"] = "";
        //CurrentConditions["qtbxKeyStatus"] = "";
        txtBeforDateS.Text = "";
        txtBeforDateE.Text = "";
        ddlStatus.SelectedValue = "";
        hdStatus.Value = "";
        ddlDeptSN.SelectedValue = "";
        hdDeptSN.Value = "";
        txtPID.Text = "";

        //odsIndex.SelectParameters["KeyWordBeforDateS"] = new Parameter("KeyWordBeforDateS", DbType.String, txtBeforDateS.Text);     //收件日期-起
        //odsIndex.SelectParameters["KeyWordBeforDateE"] = new Parameter("KeyWordBeforDateE", DbType.String, txtBeforDateE.Text);     //收件日期-迄
        //odsIndex.SelectParameters["KeyWordStatus"] = new Parameter("KeyWordStatus", DbType.String, ddlStatus.SelectedValue);        //案件狀態
        //odsIndex.SelectParameters["KeyWordDeptSN"] = new Parameter("KeyWordDeptSN", DbType.String, hdDeptSN.Value);                 //醫療院所名稱
        //odsIndex.SelectParameters["KeyWordPID"] = new Parameter("KeyWordPID", DbType.String, txtPID.Text);                          //個案身分證字號

        SetQparm();
        gvIndex.DataBind();
    }

    private void SetQparm()
    {

        odsIndex.SelectParameters["KeyWordWriteOffTransfer"] = new Parameter("KeyWordWriteOffTransfer", DbType.String, string.Empty);
        odsIndex.SelectParameters["KeyWordName"] = new Parameter("KeyWordName", DbType.String, string.Empty);   //個案姓名



        odsIndex.SelectParameters["KeyWordBeforDateS"] = new Parameter("KeyWordBeforDateS", DbType.String, txtBeforDateS.Text);     //收件日期-起
        odsIndex.SelectParameters["KeyWordBeforDateE"] = new Parameter("KeyWordBeforDateE", DbType.String, txtBeforDateE.Text);     //收件日期-迄
        odsIndex.SelectParameters["KeyWordStatus"] = new Parameter("KeyWordStatus", DbType.String, hdStatus.Value);                 //案件狀態
        odsIndex.SelectParameters["KeyWordDeptSN"] = new Parameter("KeyWordDeptSN", DbType.String, hdDeptSN.Value);                 //醫療院所名稱
        odsIndex.SelectParameters["KeyWordPID"] = new Parameter("KeyWordPID", DbType.String, txtPID.Text);                          //個案身分證字號
        odsIndex.SelectParameters["CaseType"] = new Parameter("CaseType", DbType.String, "CaseRE");                                 //案件管理類型

    }

}