﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using Hamastar.BusinessObject;
using System.Data;
using Microsoft.Security.Application;
using NPOI.HSSF.UserModel;
using Hamastar.Common.Text;
public partial class System_AccUser_Query : BasePage
{

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            //新增紐連結頁
            CurrentConditions["ID"] = "";
            btnAdd.PostBackUrl = string.Format("/System/AccUser/Detail.aspx{0}", jSecurity.XSS(Request.Url.Query));

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
                DataPager1.SetPageProperties(pim.startrow, pim.maxrow, true);
            }
            catch
            {

            }
        }

        if (CurrentConditions.ContainsKey("qtxtKeyWord"))
            txtKeyWord.Text = CurrentConditions["qtxtKeyWord"].ToString();
    }

    #region 按鈕事件
    protected void btnInsert_Click(object sender, EventArgs e)
    {
        SetQparm();
    }

    protected void btnSearch_Click(object sender, EventArgs e)
    {
        SetQparm();
    }

    private void SetQparm()
    {
        CurrentConditions["qtxtKeyWord"] = txtKeyWord.Text;

    }

    protected void btnRecord_Click(object sender, EventArgs e)
    {
        //操作紀錄
        Pages.WindowOpen(string.Format("/Common/Record.aspx{0}", jSecurity.XSS(Request.Url.Query)), "_DealData", 800, 600);

    }
    #endregion

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
        string ID = e.CommandArgument.ToString();
        switch (e.CommandName)
        {
            case "btnEdit":
                SetQparm();
                CurrentConditions["ID"] = ID;
                Response.Redirect(string.Format("/System/AccUser/Detail.aspx{0}", Request.Url.Query));
                break;
        }
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
            vw_AccUser Item = e.Row.DataItem as vw_AccUser;
            Button btnEdit = e.Row.FindControl("btnEdit") as Button;
            btnEdit.CommandArgument = Item.ID.ToString();

            /*//刪除紐功能
            Button btnStatus = e.Row.FindControl("btnStatus") as Button;
            btnStatus.CommandArgument = Item.ID.ToString();

            //如果帳號是網站管理員時，不准關閉帳號資料
            Comm_AccUser GetUpdateDate = Comm_AccUser.GetAccUser(e.Row.Cells[1].Text);
            if (GetUpdateDate != null && GetUpdateDate.IsManager == true)
                btnStatus.Visible = false;*/

            //如果帳號是鎖定的狀態，文字顏色變成紅色
            if ("停用".Equals(e.Row.Cells[3].Text))
            {
                e.Row.Cells[3].ForeColor = System.Drawing.Color.Red;
            }

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

    protected void odsIndex_Load(object sender, EventArgs e)
    {
        odsIndex.SelectParameters["KeyWord"] = new Parameter("KeyWord", DbType.String, this.txtKeyWord.Text);
    }


}