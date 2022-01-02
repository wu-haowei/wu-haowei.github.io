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

public partial class System_Examiner_Detail : BasePage
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
                    litNav.Text += " / 新增資料";
                    break;
                case "update":
                case "read":
                    litNav.Text += ("update".Equals(state)) ? " / 修改資料" : " / 檢視資料";
                    Comm_Examiner update = Comm_Examiner.GetExaminer(id);
                    if (update != null)
                    {
                        #region 顯示修改資料
                        txtName.Text = update.Name; //委員姓名
                        identityNumber.Attributes.Add("value", Tools.DecryptAES(update.PID));//身分證號碼
                        txtaddress.Text = update.Addr; //地址
                        rdbEnabel.SelectedValue = update.Status.ToString();  //狀態
                        Deactivateillustrate.Text = update.StatusDesc;  //停用說明
                        #endregion
                    }
                    break;
            }
            if ("read".Equals(state))
            {
                //檢視狀態時鎖入輸入框
                txtName.Enabled = false;
                identityNumber.Enabled = false;
                txtaddress.Enabled = false;
                rdbEnabel.Enabled = false;
                Deactivateillustrate.Enabled = false;
                btnSave.Visible = false;//保存按鈕
                btnCancel.Visible = false;//取消按鈕
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

        Comm_Examiner data = new Comm_Examiner();
        if (state == "update")
            data = Comm_Examiner.GetSingle(x => x.SN == id);

        data.Name = jSecurity.XSS(txtName.Text); //委員姓名
        data.PID = Tools.EncryptAES(identityNumber.Text.ToUpper()); //身分證號碼
        data.Addr = jSecurity.XSS(txtaddress.Text); //地址
        data.Status = Convert.ToInt32(rdbEnabel.SelectedValue);  //狀態
        data.StatusDesc = Deactivateillustrate.Text; //停用說明

        #region  資料新增
        if (state == "insert")
        {
            data.CreateDate = DateTime.Now;//建立日期
            Comm_Examiner.Insert(data);
        }
        #endregion
        #region  資料更新
        else if (state == "update")
        {
            if (data != null)
            {
                data.ModifyDate = DateTime.Now;//修改日期
                Comm_Examiner.Update(data);
            }
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
            sbError.Append(@"請輸入委員姓名\n");
        else if (txtName.Text.Length > 50)
            sbError.Append(@"委員姓名長度不可超過50字\n");

        if (txtaddress.Text == string.Empty)
            sbError.Append(@"請輸入地址\n");
        else if (txtaddress.Text.Length > 255)
            sbError.Append(@"地址長度不可超過255字\n");

        if (rdbEnabel.Text == "2" && Deactivateillustrate.Text == string.Empty)
            sbError.Append(@"請輸入停用說明\n");
        else if (Deactivateillustrate.Text.Length > 255)
            sbError.Append(@"停用說明長度不可超過255字\n");

        if (identityNumber.Text == string.Empty)
            sbError.Append(@"請輸入身分證字號\n");
        if (!identityNumber.Text.isIdentificationId() && !identityNumber.Text.CheckForeignIdNumber())
            sbError.Append(@"請輸入正確的身分證字號\n");
        if (state == "insert")
        {
            if (Comm_Examiner.GetSingle(x => x.PID == identityNumber.Text) != null)
                sbError.Append(@"此身分證字號重複\n");
        }
        return sbError.ToString();
    }
    #endregion



}