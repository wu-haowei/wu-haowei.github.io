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

public partial class System_Department_Detail : BasePage
{
    string state = string.Empty;
    string id = string.Empty;
    List<Comm_Department_IP> listIP = new List<Comm_Department_IP>();

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

            CurrentConditions["Detail"] = listIP;
            RebindDetail();
            BindAreaSN();   //繫結鄉鎮巿資料

            switch (state)
            {
                case "insert":
                    litNav.Text += " / 新增資料";
                    break;
                case "update":
                case "read":
                    litNav.Text += ("update".Equals(state)) ? " / 修改資料" : " / 檢視資料";
                    Comm_Department GetUpdateDate = Comm_Department.GetDepartment(id.ToString());
                    if (GetUpdateDate != null)
                    {
                        #region 顯示修改資料
                        //單頭
                        lbSN.Text = GetUpdateDate.SN;                               //院所編號
                        tbxDeptName.Text = GetUpdateDate.DeptName;                  //院所名稱
                        tbxTel.Text = GetUpdateDate.Tel;                            //院所電話
                        txtFax.Text = GetUpdateDate.Fax;                            //院所傳真
                        ddlAreaSN.SelectedValue = GetUpdateDate.AreaID;             //鄉巿鎮
                        hdAreaID.Value = GetUpdateDate.AreaID;
                        lbPostId.Text = GetUpdateDate.AreaID;
                        tbxAddr.Text = GetUpdateDate.Addr;                          //地址
                        tbxDID.Text = GetUpdateDate.DID;                            //院所統編
                        rdbStatus.SelectedValue = GetUpdateDate.Status.ToString();  //院所狀態
                        if (2 == GetUpdateDate.Status) tbxStatusDesc.Text = GetUpdateDate.StatusDesc;  //停用說明
                        tbxBankName.Text = GetUpdateDate.BankName;                  //入帳銀行
                        tbxBankAccID.Text = GetUpdateDate.BankAccID;                //入帳戶名
                        tbxBankID.Text = GetUpdateDate.BankID;                      //存款帳號

                        listIP = Comm_Department_IP.GetList(x => x.DeptSN == GetUpdateDate.SN).ToList();
                        CurrentConditions["Detail"] = listIP;
                        RebindDetail();
                        #endregion
                    }
                    break;
            }
            if ("read".Equals(state))
            {
                //檢視狀態時鎖入輸入框
                tbxDeptName.Enabled = false;
                tbxTel.Enabled = false;
                txtFax.Enabled = false;
                ddlAreaSN.Enabled = false;
                tbxAddr.Enabled = false;
                tbxDID.Enabled = false;
                rdbStatus.Enabled = false;
                tbxStatusDesc.Enabled = false;
                tbxBankName.Enabled = false;
                tbxBankAccID.Enabled = false;
                tbxBankID.Enabled = false;
                tbxIP.Enabled = false;
                btnAdd.Visible = false;
                btnCancel.Visible = false;

                #region IP列表(唯讀)
                Comm_Department GetUpdateDate = Comm_Department.GetDepartment(id.ToString());
                listIP = Comm_Department_IP.GetList(x => x.DeptSN == GetUpdateDate.SN).ToList();
                #region 產生IP列表資料
                StringBuilder sb = new StringBuilder();
                sb.Append("<table class='Detail-table' width='100%'>");
                foreach (var ip in listIP)
                {
                    sb.AppendFormat("<tr><td>{0}</td></tr>", ip.IP);
                }
                sb.Append("</table>");
                #endregion
                panelIP.Visible = false;
                li.Visible = true;
                li.Text = sb.ToString();
                #endregion
            }
        }
    }

    private void RebindDetail()
    {
        listIP = CurrentConditions["Detail"] as List<Comm_Department_IP>;
        gvIndex.DataSource = listIP;
        tbxIP.Text = "";
        gvIndex.DataBind();
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
        if ("insert".Equals(state))
        {
            Comm_Department insert = new Comm_Department();
            insert.DeptName = jSecurity.XSS(tbxDeptName.Text);                  //院所名稱
            insert.Tel = jSecurity.XSS(tbxTel.Text);                            //院所電話
            insert.Fax = jSecurity.XSS(txtFax.Text);                            //院所傳真
            insert.AreaID = jSecurity.XSS(hdAreaID.Value);                      //鄉鎮巿代號
            insert.Addr = jSecurity.XSS(tbxAddr.Text);                          //地址
            insert.DID = jSecurity.XSS(tbxDID.Text);                            //統編
            insert.Status = Convert.ToInt32(rdbStatus.SelectedValue);           //狀態:1啟用、2停用
            insert.StatusDesc = jSecurity.XSS(tbxStatusDesc.Text);              //停用說明
            insert.BankName = jSecurity.XSS(tbxBankName.Text);                  //入帳銀行名稱
            insert.BankAccID = jSecurity.XSS(tbxBankAccID.Text);                //入帳戶名
            insert.BankID = jSecurity.XSS(tbxBankID.Text);                      //存款帳號
            if ("1".Equals(rdbStatus.SelectedValue)) insert.StatusDesc = "";
            else insert.StatusDesc = jSecurity.XSS(tbxStatusDesc.Text);         //停用說明

            listIP = (List<Comm_Department_IP>)CurrentConditions["Detail"];
            Comm_Department.InsertData(insert, listIP);
        }
        else if ("update".Equals(state))
        {
            Comm_Department update = new Comm_Department();
            update.SN = lbSN.Text;
            update.DeptName = jSecurity.XSS(tbxDeptName.Text);                  //院所名稱
            update.Tel = jSecurity.XSS(tbxTel.Text);                            //院所電話
            update.Fax = jSecurity.XSS(txtFax.Text);                            //院所傳真
            update.AreaID = jSecurity.XSS(hdAreaID.Value);                      //鄉鎮巿代號
            update.Addr = jSecurity.XSS(tbxAddr.Text);                          //地址
            update.DID = jSecurity.XSS(tbxDID.Text);                            //統編
            update.Status = Convert.ToInt32(rdbStatus.SelectedValue);           //狀態:1啟用、2停用
            update.StatusDesc = jSecurity.XSS(tbxStatusDesc.Text);              //停用說明
            update.BankName = jSecurity.XSS(tbxBankName.Text);                  //入帳銀行名稱
            update.BankAccID = jSecurity.XSS(tbxBankAccID.Text);                //入帳戶名
            update.BankID = jSecurity.XSS(tbxBankID.Text);                      //存款帳號
            if ("1".Equals(rdbStatus.SelectedValue)) update.StatusDesc = "";
            else update.StatusDesc = jSecurity.XSS(tbxStatusDesc.Text);         //停用說明

            listIP = (List<Comm_Department_IP>)CurrentConditions["Detail"];
            Comm_Department.UpdateData(update, listIP, lbSN.Text);
        }

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
        StringBuilder sbError = new StringBuilder();
        if (string.Empty.Equals(tbxDeptName.Text))
            sbError.Append(@"請輸入院所名稱\n");
        if (string.Empty.Equals(tbxTel.Text))
            sbError.Append(@"請輸入院所電話\n");
        if (string.Empty.Equals(ddlAreaSN.SelectedValue))
            sbError.Append(@"請選擇鄉巿鎮\n");
        if (state == "insert")
        {
            if (Comm_Department.GetSingle(x => x.DeptName.Equals(tbxDeptName.Text)) != null)
                sbError.Append(@"此院所資料已被新增過\n");
        }
        if (string.Empty.Equals(tbxAddr.Text))
            sbError.Append(@"請輸入地址\n");
        if ("2".Equals(rdbStatus.SelectedValue) && string.Empty.Equals(tbxStatusDesc.Text))
            sbError.Append(@"請輸入停用說明\n");


        #region 檢查資料長度

        if (tbxDeptName.Text.Length > 100) sbError.Append(@"院所名稱長度不可超過100字\n");
        if (tbxTel.Text.Length > 50) sbError.Append(@"院所電話長度不可超過50字\n");
        if (txtFax.Text.Length > 50) sbError.Append(@"院所傳真長度不可超過50字\n");
        if (tbxAddr.Text.Length > 255) sbError.Append(@"地址長度不可超過255字\n");
        if (tbxDID.Text.Length > 100) sbError.Append(@"院所統編長度不可超過10字\n");
        if ("2".Equals(rdbStatus.SelectedValue) && tbxStatusDesc.Text.Length > 100) sbError.Append(@"停用說明長度不可超過255字\n");

        if (tbxBankName.Text.Length > 100) sbError.Append(@"入帳銀行長度不可超過50字\n");
        if (tbxBankAccID.Text.Length > 100) sbError.Append(@"入帳戶名長度不可超過50字\n");
        if (tbxBankID.Text.Length > 100) sbError.Append(@"存款帳號長度不可超過50字\n");

        //if (tbxDeptName.Text.Length > 100) sbError.Append(@"IP長度不可超過50字\n");

        #endregion

        return sbError.ToString();
    }

    #endregion

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
            lbPostId.Text = "";
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
    }

    #endregion

    #region GridView事件
    protected void gvIndex_DataBound(object sender, EventArgs e)
    {
        //if (gvIndex.Rows.Count == 0)
        //{
        //    DataPager1.Visible = false;
        //    btnGo.Visible = false;
        //}
        //else
        //{
        //    DataPager1.Visible = true;
        //    btnGo.Visible = true;
        //}
        //儲存目前頁數、頁數大小
        //   CurrentConditions["pim"] = new PageIndexManager { startrow = DataPager1.StartRowIndex, maxrow = DataPager1.MaximumRows };
    }

    protected void gvIndex_RowCommand(object sender, GridViewCommandEventArgs e)
    {
        string ip = e.CommandArgument.ToString();
        switch (e.CommandName)
        {
            case "btnDel":
                listIP = (List<Comm_Department_IP>)CurrentConditions["Detail"];
                listIP = listIP.Where(x => x.IP != ip).ToList();
                CurrentConditions["Detail"] = listIP;
                RebindDetail();
                break;
        }
    }
    #endregion

    protected void gvIndex_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow) //判斷當前行是否是數據行
        {
            //刪除紐功能
            Comm_Department_IP Item = e.Row.DataItem as Comm_Department_IP;
            Button btnDel = e.Row.FindControl("btnDel") as Button;
            btnDel.CommandArgument = Item.IP.ToString();
        }
    }

    protected void BtnAdd_Click(object sender, EventArgs e)
    {
        //先驗證是否符合IPV4和IPV6
        string ip = jSecurity.XSS(tbxIP.Text);
        //var isIP = Comm_Department.IsIP(ip);
        //if (!isIP.bl)
        //{
        //    Pages.Alert("此IP非合IP");
        //    return;
        //}
        //else { ip = isIP.str; }

        listIP = (List<Comm_Department_IP>)CurrentConditions["Detail"];
        if (listIP.Where(x => x.IP == ip.Trim()).Any())
        {
            Pages.Alert("此IP已在名單中");
            tbxIP.Text = "";
            return;
        }
        listIP.Add(new Comm_Department_IP { DeptSN = jSecurity.XSS(lbSN.Text), IP = ip });
        CurrentConditions["Detail"] = listIP;
        RebindDetail();
    }

}