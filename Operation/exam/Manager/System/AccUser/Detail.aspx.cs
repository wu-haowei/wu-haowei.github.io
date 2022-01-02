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

public partial class System_AccUser_Detail : BasePage
{
    string state = string.Empty;
    string id = string.Empty;

    protected void Page_Load(object sender, EventArgs e)
    {
        if (CurrentConditions.ContainsKey("ID"))
            state = CurrentConditions["ID"].ToString() == "" ? "insert" : "update";
        else
            Response.Redirect(GoQuery());
        id = CurrentConditions["ID"].ToString();

        if (!IsPostBack)
        {
            //設定上一頁的網址
            this.btnGoBack.PostBackUrl = GoQuery();
            this.btnCancel.PostBackUrl = this.btnGoBack.PostBackUrl;
            this.btnSave.CommandArgument = this.btnGoBack.PostBackUrl;
            Literal litNav = Page.Master.FindControl("litNav") as Literal;


            if (state == "insert")
            {
                litNav.Text += " / 新增資料";
            }
            else if (state == "update")
            {
                litNav.Text += " / 修改資料";
                Comm_AccUser GetUpdateDate = Comm_AccUser.GetAccUser(id.ToString());
                if (GetUpdateDate != null)
                {
                    #region 顯示修改資料
                    txtAccount.Text = GetUpdateDate.ID;  //帳號
                    txtAccount.Enabled = false;  //帳號不能修改
                    txtPassword.Attributes.Add("value", Tools.DecryptAES(GetUpdateDate.P_W)); //密碼
                    txtName.Text = GetUpdateDate.Name; //姓名
                    rdbEnabel.SelectedValue = GetUpdateDate.Status.ToString();  //狀態
                    #endregion
                }
            }

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

        #region  資料更新/新增
        if (state == "insert")
        {
            Comm_AccUser insert = new Comm_AccUser();
            insert.ID = jSecurity.SQLInjection(txtAccount.Text);  //帳號
            insert.P_W = Tools.EncryptAES(txtPassword.Text);  //密碼
            insert.Name = jSecurity.XSS(txtName.Text);  //姓名
           
            insert.Status = Convert.ToInt32(rdbEnabel.SelectedValue);  //狀態
            if (!string.IsNullOrEmpty(tbxStatusDesc.Text)) insert.StatusDesc = tbxStatusDesc.Text;  //停用說明
            Comm_AccUser.Insert(insert);

        }
        else if (state == "update")
        {
            Comm_AccUser update = new Comm_AccUser();
            update = Comm_AccUser.GetSingle(x => x.ID == id);
            //帳號無法修改
            update.P_W = Tools.EncryptAES(txtPassword.Text);  //密碼
            update.LastUpdateP_WDate = DateTime.Now;  //修改密碼日期
            update.Name = jSecurity.SQLInjection(txtName.Text);  //姓名
           
            update.Status = Convert.ToInt32(rdbEnabel.SelectedValue);  //狀態
            if ("1".Equals(rdbEnabel.SelectedValue))
                update.StatusDesc = "";                 //啟用清空說明
            else
                update.StatusDesc = tbxStatusDesc.Text; //停用說明
            Comm_AccUser.Update(update);

        }
        #region 維護記錄
        Comm_Record rec = new Comm_Record();
        rec.NodeID = iNodeID;
        rec.IP = Request.UserHostAddress;
        rec.ModifyDate = DateTime.Now;
        rec.ModifyAccountID = SessionCenter.AccUser.ID;
        string fun = (state == "insert") ? "新增" : "修改";
        if (id == string.Empty)
            rec.Record = fun + txtAccount.Text + "帳號資料";
        Comm_Record.Insert(rec);
        #endregion

        //導回上一頁
        if (state == "update")
            Pages.AlertByswal(Tools.altertType.成功.ToString(), "修改成功", Tools.altertType.成功.ToDescriptionString(), GoQuery());
        else
            Response.Redirect("Query.aspx?n=" + jSecurity.GetQueryString("n"));
        #endregion
    }

    #region 資料檢查
    private string CheckData()
    {
        //檢查必填及長度
        StringBuilder sbError = new StringBuilder();

        if (string.Empty.Equals(txtAccount.Text))
            sbError.Append(@"請輸入帳號\n");
        else if (txtAccount.Text.Length > 50)
            sbError.Append(@"帳號長度不可超過50字\n");

        if (string.Empty.Equals(txtPassword.Text))
            sbError.Append(@"請輸入密碼\n");
        else if (txtPassword.Text.Length > 50)
            sbError.Append(@"密碼長度不可超過50字\n");

        if (string.Empty.Equals(txtName.Text))
            sbError.Append(@"請輸入姓名\n");
        else if (txtName.Text.Length > 100)
            sbError.Append(@"姓名長度不可超過100字\n");

        if (state == "insert")
        {
            if (Comm_AccUser.GetSingle(x => x.ID == txtAccount.Text) != null)
                sbError.Append(@"此帳號已被申請過\n");
        }
        if ("2".Equals(rdbEnabel.SelectedValue))
        {
            if (string.Empty.Equals(tbxStatusDesc.Text))
                sbError.Append(@"請輸入停用說明\n");
            else if (tbxStatusDesc.Text.Length > 50)
                sbError.Append(@"停用說明長度不可超過50字\n");
        }

        return sbError.ToString();
    }
    #endregion
}