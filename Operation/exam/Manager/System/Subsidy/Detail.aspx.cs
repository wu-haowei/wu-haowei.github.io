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
using System.Text.RegularExpressions;

public partial class System_Subsidy_Detail : BasePage
{
    string state = string.Empty;
    int id = 0;


    protected void Page_Load(object sender, EventArgs e)
    {

        if (string.IsNullOrEmpty(CurrentConditions["ID"].ToString()))
            state = "insert";
        else
        {
            if (CurrentConditions.ContainsKey("Action"))
                state = ("Edit".Equals((CurrentConditions["Action"]).ToString())) ? "update" : "read";
            else Response.Redirect(GoQuery());
        }

        bool success = Int32.TryParse(CurrentConditions["ID"].ToString(), out id);
        if (!IsPostBack)
        {
            //設定上一頁的網址
            this.btnGoBack.PostBackUrl = GoQuery();
            this.btnCancel.PostBackUrl = this.btnGoBack.PostBackUrl;
            this.btnSave.CommandArgument = this.btnGoBack.PostBackUrl;
            Literal litNav = Page.Master.FindControl("litNav") as Literal;

            switch (state)
            {
                case "insert":
                    TrUnit.Visible = Catregory.SelectedValue == "1" ? false : true;
                    litNav.Text += " / 新增資料";
                    break;
                case "update":
                case "read":
                    litNav.Text += ("update".Equals(state)) ? " / 修改資料" : " / 檢視資料";
                    Comm_Subsidy update = Comm_Subsidy.GetSubsidy(id);
                    if (update != null)
                    {
                        #region 顯示修改資料
                        Catregory.SelectedValue = update.Catregory.ToString();
                        txtName.Text = update.Name; //項目名稱
                        txtUnit.Text = update.Unit;  //維修項目單位
                        txtAmount.Text = update.Amount.ToString(); //補助金額
                        rdbEnabel.SelectedValue = update.Status.ToString();  //啟用狀態
                        Deactivateillustrate.Text = update.StatusDesc;  //停用說明
                        #endregion
                    }
                    break;
            }
            if ("read".Equals(state))
            {
                //檢視狀態時鎖入輸入框
                Catregory.Enabled = false;
                txtName.Enabled = false;
                txtUnit.Enabled = false;
                txtAmount.Enabled = false;
                rdbEnabel.Enabled = false;
                Deactivateillustrate.Enabled = false;
                btnSave.Visible = false;//保存按鈕
                btnCancel.Visible = false;//取消按鈕
            }
            Catregory_SelectedIndexChanged(this, null);

        }
    }
    /// <summary>
    /// 補助類型 切換
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void Catregory_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (Catregory.SelectedValue == "1")
        {
            TrUnit.Visible = false;
        }
        else
        {
            TrUnit.Visible = true;
        }
    }

    private string GoQuery()
    {
        return "Query.aspx?n=" + jSecurity.GetQueryString("n");
    }

    protected void btnSave_Click(object sender, EventArgs e)
    {
        #region 資料檢查
        string chkerror = CheckData();
        if (!string.IsNullOrEmpty(chkerror))
        {
            Pages.AlertByswal(Tools.altertType.錯誤.ToString(), chkerror, Tools.altertType.錯誤.ToDescriptionString());
            return;
        }
        #endregion

        Comm_Subsidy data = new Comm_Subsidy();
        if (state == "update")
            data = Comm_Subsidy.GetSingle(x => x.SN == id);

        data.Catregory = Convert.ToInt32(Catregory.SelectedValue); //補助類型
        data.Name = jSecurity.XSS(txtName.Text); //項目名稱
        if (data.Catregory == 1)
            data.Unit = string.Empty;
        else
            data.Unit = txtUnit.Text;  //維修項目單位
        data.Amount = Convert.ToInt32(txtAmount.Text); //補助金額
        data.Status = Convert.ToInt32(rdbEnabel.SelectedValue);  //啟用狀態
        data.StatusDesc = Deactivateillustrate.Text;  //停用說明



        #region  資料新增
        if (state == "insert")
        {
            data.CreateDate = DateTime.Now;
            data.CreateID = SessionCenter.AccUser.ID;
            Comm_Subsidy.Insert(data);
        }
        #endregion
        #region  資料更新
        else if (state == "update")
        {
            data.ModifyDate = DateTime.Now;
            data.ModifyID = SessionCenter.AccUser.ID;
            Comm_Subsidy.Update(data);

        }
        #endregion

        #region 維護記錄
        Comm_Record rec = new Comm_Record();
        rec.NodeID = iNodeID;
        rec.IP = Request.UserHostAddress;
        rec.ModifyDate = DateTime.Now;
        rec.ModifyAccountID = SessionCenter.AccUser.ID;
        string fun = (state == "insert") ? "新增" : "修改";
        if (id == 0)
            rec.Record = fun + id + "帳號資料";
        Comm_Record.Insert(rec);
        #endregion

        //導回上一頁
        if (state == "update")
            Pages.AlertByswal(Tools.altertType.成功.ToString(), "修改成功", Tools.altertType.成功.ToDescriptionString(), GoQuery());
        else
            Response.Redirect("Query.aspx?n=" + jSecurity.GetQueryString("n"));

    }

    #region 資料檢查
    private string CheckData()
    {
        StringBuilder sbError = new StringBuilder();
        if (txtName.Text == string.Empty)
            sbError.Append(@"請輸入項目名稱\n");
        else if (txtName.Text.Length > 100)
            sbError.Append(@"項目名稱長度不可超過100字\n");

        if (!Int32.TryParse(txtAmount.Text, out int intAmount))
            sbError.Append(@"金額錯誤\n");

        if (rdbEnabel.Text == "2" && Deactivateillustrate.Text == string.Empty)
            sbError.Append(@"請輸入停用說明\n");
        else if (Deactivateillustrate.Text.Length > 255)
            sbError.Append(@"停用說明長度不可超過255字\n");

        if (Catregory.SelectedIndex == 1 && txtUnit.Text == string.Empty)
            sbError.Append(@"請輸入項目單位\n");
        else if (txtUnit.Text.Length > 50)
            sbError.Append(@"項目單位長度不可超過50字\n");

        return sbError.ToString();
    }
    #endregion

}