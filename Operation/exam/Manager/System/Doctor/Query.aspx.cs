using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using System.Data;
using Hamastar.BusinessObject;
using Hamastar.Common.Text;
using Hamastar.Common;

public partial class System_Doctor_Query : BasePage
{
    protected void Page_Load(object sender, EventArgs e)
    {

        if (!IsPostBack)
        {
            BindDept();     //繫結服務院所選單
            CurrentConditions["SN"] = "";
            CurrentConditions["Action"] = "";
            this.BindUI();
        }
    }

    private void BindUI()
    {
        if (CurrentConditions.ContainsKey("pim"))
        {
            try
            {
                PageIndexManager pim = (PageIndexManager)CurrentConditions["pim"];
                //  DataPager1.SetPageProperties(pim.startrow, pim.maxrow, true);
            }
            catch
            {

            }
        }
    }



    protected void gvIndex_RowCommand(object sender, GridViewCommandEventArgs e)
    {
        int SN = 0;
        int.TryParse(e.CommandArgument.ToString(), out SN);
        switch (e.CommandName)
        {
            case "btnEdit":
            case "btnRead":
                SetQparm();
                CurrentConditions["SN"] = SN;
                CurrentConditions["Action"] = e.CommandName.Substring(3);
                Response.Redirect(string.Format("/System/Doctor/Detail.aspx{0}", Request.Url.Query));
                break;
        }
    }

    protected void gvIndex_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow) //判斷當前行是否是數據行
        {
            if ("停用".Equals(e.Row.Cells[3].Text))
            {
                e.Row.Cells[3].ForeColor = System.Drawing.Color.Red;
            }
        }

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
        SetQparm();
    }

    protected void btnAdd_Click(object sender, EventArgs e)
    {
        CurrentConditions["SN"] = "";
        Response.Redirect(string.Format("/System/Doctor/Detail.aspx{0}", Request.Url.Query));
    }

    protected void odsIndex_Load(object sender, EventArgs e)
    {
        odsIndex.SelectParameters["KeyDeptSN"] = new Parameter("KeyDeptSN", DbType.String, this.hdDeptSN.Value);
        odsIndex.SelectParameters["KeyName"] = new Parameter("KeyName", DbType.String, this.txtName.Text);
        odsIndex.SelectParameters["KeyStatus"] = new Parameter("KeyStatus", DbType.String, this.hdStatus.Value);
    }

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

    //狀態
    protected void ddlStatus_SelectedIndexChanged(object sender, EventArgs e)
    {
        hdStatus.Value = ddlStatus.SelectedValue;
    }

    #endregion

    protected void btnClear_Click(object sender, EventArgs e)
    {
        CurrentConditions["qtbxKeyDeptSN"] = "";
        CurrentConditions["qtbxKeyName"] = "";
        CurrentConditions["qtbxKeyStatus"] = "";
        ddlDeptSN.SelectedValue = "";
        hdDeptSN.Value = "";
        ddlStatus.SelectedValue = "";
        hdStatus.Value = "";
        txtName.Text = "";

        odsIndex.SelectParameters["KeyDeptSN"] = new Parameter("KeyDeptSN", DbType.String, this.hdDeptSN.Value);
        odsIndex.SelectParameters["KeyName"] = new Parameter("KeyName", DbType.String, this.txtName.Text);
        odsIndex.SelectParameters["KeyStatus"] = new Parameter("KeyStatus", DbType.String, this.hdStatus.Value);

        SetQparm();
        gvIndex.DataBind();
    }

    private void SetQparm()
    {
        CurrentConditions["qtbxKeyDeptSN"] = hdDeptSN.Value;
        CurrentConditions["qtbxKeyName"] = txtName.Text;
        CurrentConditions["qtbxKeyStatus"] = hdStatus.Value;
    }


}