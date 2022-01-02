using Hamastar.Common.Text;
using System;
using System.Collections.Generic;
using System.Linq;

using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Hamastar.BusinessObject;
using Hamastar.Common;
public partial class System_Doctor_Detail : BasePage
{
    string state = string.Empty;
    string id = string.Empty;

    protected void Page_Load(object sender, EventArgs e)
    {

        if (CurrentConditions.ContainsKey("SN"))
        {
            if (string.IsNullOrEmpty(CurrentConditions["SN"].ToString()))
                state = "insert";
            else
            {
                if (CurrentConditions.ContainsKey("Action"))
                    state = ("Edit".Equals((CurrentConditions["Action"]).ToString())) ? "update" : "read";
                else Response.Redirect(GoQuery());
            }
        }
        else
            Response.Redirect(GoQuery());
        id = CurrentConditions["SN"].ToString();

        if (!IsPostBack)
        {
            //設定上一頁的網址
            this.btnGoBack.PostBackUrl = GoQuery();
            this.btnCancel.PostBackUrl = this.btnGoBack.PostBackUrl;
            this.btnSave.CommandArgument = this.btnGoBack.PostBackUrl;
            Literal litNav = Page.Master.FindControl("litNav") as Literal;

            BindDept();   //服務院所選單

            switch (state)
            {
                case "insert":
                    litNav.Text += " / 新增資料";
                    break;
                case "update":
                case "read":
                    litNav.Text += ("update".Equals(state)) ? " / 修改資料" : " / 檢視資料";
                    int SN = 0;
                    bool hasSN = int.TryParse(CurrentConditions["SN"].ToString(), out SN);
                    if (hasSN)
                    {
                        Comm_Doctor GetUpdateDate = Comm_Doctor.GetData(SN);
                        if (GetUpdateDate != null)
                        {
                            #region 顯示修改資料
                            //單頭
                            lbSN.Text = GetUpdateDate.SN.ToString();
                            txtName.Text = GetUpdateDate.Name;
                            ddlDeptSN.SelectedValue = GetUpdateDate.DeptSN;
                            rdbStatus.SelectedValue = GetUpdateDate.Status.ToString();
                            txtMemo.Text = GetUpdateDate.Memo;
                            txtStatusDesc.Text = GetUpdateDate.StatusDesc;
                            #endregion
                        }
                    }
                    break;
            }
            if ("read".Equals(state))
            {
                //檢視狀態時鎖入輸入框
                lbSN.Enabled = false;
                txtName.Enabled = false;
                ddlDeptSN.Enabled = false;
                rdbStatus.Enabled = false;
                txtMemo.Enabled = false;
                txtStatusDesc.Enabled = false;
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
            Comm_Doctor insert = new Comm_Doctor();
            insert.DeptSN = jSecurity.XSS(ddlDeptSN.SelectedValue);                 //院所代謘
            insert.Name = jSecurity.XSS(txtName.Text);                              //醫師姓名
            insert.Status = Convert.ToInt32(jSecurity.XSS(rdbStatus.SelectedValue));//狀態:1:啟用、2:停用
            if ("2".Equals(rdbStatus.SelectedValue))
                insert.StatusDesc = jSecurity.XSS(txtStatusDesc.Text);              //停用說明
            insert.CreateDate = DateTime.Now;                                       //建立日期
            Comm_Doctor.Insert(insert);
        }
        else if (state == "update")
        {
            Comm_Doctor update = new Comm_Doctor();
            int SN = 0;
            bool hasSN = int.TryParse(lbSN.Text, out SN);
            if (hasSN)
            {
                update = Comm_Doctor.GetData(SN);
                update.DeptSN = jSecurity.XSS(ddlDeptSN.SelectedValue);                 //院所代碼
                update.Name = jSecurity.XSS(txtName.Text);                              //醫師姓名
                update.Status = Convert.ToInt32(jSecurity.XSS(rdbStatus.SelectedValue));//狀態:1:啟用、2:停用
                if ("1".Equals(rdbStatus.SelectedValue)) update.StatusDesc = "";
                else update.StatusDesc = jSecurity.XSS(txtStatusDesc.Text);             //停用說明
                update.ModifyDate = DateTime.Now;
                Comm_Doctor.Update(update);
            }
        }

        //導回上一頁
        if (state == "update")
            Pages.AlertByswal(Tools.altertType.成功.ToString(), "修改成功", Tools.altertType.成功.ToDescriptionString(), GoQuery());
        else
            Response.Redirect(GoQuery());
        #endregion
    }

    #region 資料檢查
    private string CheckData()
    {
        StringBuilder sbError = new StringBuilder();
        //服務院所
        if (string.Empty.Equals(ddlDeptSN.SelectedValue))
            sbError.Append(@"請輸入服務院所\n");
        //醫師姓名
        if (string.Empty.Equals(txtName.Text))
            sbError.Append(@"請輸入醫師姓名\n");
        else if ("insert".Equals(state) && Comm_Doctor.DoctorIsRepeat(txtName.Text))
            sbError.Append(@"醫師姓名重複輸入\n");
        else if (txtName.Text.Length > 100)
            sbError.Append(@"院所電話長度不可超過100字\n");
        //停用說明
        if ("2".Equals(rdbStatus.SelectedValue))
        {
            if (string.Empty.Equals(txtStatusDesc.Text))
                sbError.Append(@"請輸入停用說明\n");
            else if (txtStatusDesc.Text.Length > 50)
                sbError.Append(@"院所電話長度不可超過50字\n");
        }

        return sbError.ToString();
    }
    #endregion

    #region 服務院所
    protected void ddlDeptSN_SelectedIndexChanged(object sender, EventArgs e)
    {
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

    #endregion

}