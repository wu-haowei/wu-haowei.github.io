using System;
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
public partial class System_Department_Query : BasePage
{

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)

        {
            //新增紐連結頁
            CurrentConditions["SN"] = "";
            CurrentConditions["Action"] = "";
            btnAdd.PostBackUrl = string.Format("/System/Department/Detail.aspx{0}", jSecurity.XSS(Request.Url.Query));

            this.BindUI();

            BindAreaSN();   //繫結鄉鎮巿資料
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

        if (CurrentConditions.ContainsKey("qtbxKeyWordName")) tbxKeyWordName.Text = CurrentConditions["qtbxKeyWordName"].ToString();
        if (CurrentConditions.ContainsKey("qtbxKeyWordSN")) tbxKeyWordSN.Text = CurrentConditions["qtbxKeyWordSN"].ToString();
        if (CurrentConditions.ContainsKey("qtbxKeyWordArea")) hdAreaID.Value = CurrentConditions["qtbxKeyWordArea"].ToString();
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

    protected void btnClear_Click(object sender, EventArgs e)
    {
        CurrentConditions["qtbxKeyWordName"] = "";
        CurrentConditions["qtbxKeyWordSN"] = "";
        CurrentConditions["qtbxKeyWordArea"] = "";
        tbxKeyWordName.Text = "";
        tbxKeyWordSN.Text = "";
        ddlAreaSN.SelectedValue = "";
        hdAreaID.Value = "";

        odsIndex.SelectParameters["KeyWorddName"] = new Parameter("KeyWorddName", DbType.String, this.tbxKeyWordName.Text);
        odsIndex.SelectParameters["KeyWordSN"] = new Parameter("KeyWordSN", DbType.String, this.tbxKeyWordSN.Text);
        odsIndex.SelectParameters["KeyWordAreaSN"] = new Parameter("KeyWordAreaSN", DbType.String, this.hdAreaID.Value);

        SetQparm();
        gvIndex.DataBind();
    }

    private void SetQparm()
    {
        CurrentConditions["qtbxKeyWordName"] = tbxKeyWordName.Text;
        CurrentConditions["qtbxKeyWordSN"] = tbxKeyWordSN.Text;
        CurrentConditions["qtbxKeyWordArea"] = hdAreaID.Value;
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
        string SN = e.CommandArgument.ToString();
        switch (e.CommandName)
        {
            case "btnEdit":
            case "btnRead":
                SetQparm();
                CurrentConditions["SN"] = SN;
                CurrentConditions["Action"] = e.CommandName.Substring(3);
                Response.Redirect(string.Format("/System/Department/Detail.aspx{0}", Request.Url.Query));
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
            vw_Department Item = e.Row.DataItem as vw_Department;
            Button btnEdit = e.Row.FindControl("btnEdit") as Button;
            btnEdit.CommandArgument = Item.SN.ToString();
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
        odsIndex.SelectParameters["KeyWorddName"] = new Parameter("KeyWorddName", DbType.String, this.tbxKeyWordName.Text);
        odsIndex.SelectParameters["KeyWordSN"] = new Parameter("KeyWordSN", DbType.String, this.tbxKeyWordSN.Text);
        odsIndex.SelectParameters["KeyWordAreaSN"] = new Parameter("KeyWordAreaSN", DbType.String, this.hdAreaID.Value);
    }

    #region 鄉鎮巿
    protected void ddlArea_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (!string.IsNullOrEmpty(ddlAreaSN.SelectedValue))
        {
            using (dbEntities db = new dbEntities())
            {
                //var Query = Comm_Area.GetAreaPostID(ddlAreaSN.SelectedValue);
                //lbPostId.Text = Query;
                //hdAreaID.Value = ddlAreaSN.SelectedValue;
            }
        }
        else
        {
            //lbPostId.Text = "";
            hdAreaID.Value = "";
        }
    }

    private void BindAreaSN()
    {
        List<Comm_Area> dArea = Comm_Area.GetArea("C16");
        ddlAreaSN.Items.Add(new ListItem("請選擇", ""));
        foreach (var d in dArea)
        {
            ddlAreaSN.Items.Add(new ListItem(d.Name, d.Id.ToString()));
        }
        hdAreaID.Value = ddlAreaSN.SelectedValue;
        //ddlAreaSN.DataSource = dArea;
        //ddlAreaSN.DataTextField = "Name";
        //ddlAreaSN.DataValueField = "Serno";
        //ddlAreaSN.DataBind();
        //ddlAreaSN.Items.Insert(0, new ListItem("請選擇", "-1"));
    }

    #endregion//

}