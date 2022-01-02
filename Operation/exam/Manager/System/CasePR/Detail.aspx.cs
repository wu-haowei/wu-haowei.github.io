using Hamastar.Common.Text;
using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Hamastar.BusinessObject;
using Hamastar.Common;
public partial class System_CasePR_Detail : BasePage
{
    string state = string.Empty;
    string CaseNo = string.Empty;

    List<Comm_Subsidy> listComm_Subsidy = new List<Comm_Subsidy>();

    protected void Page_Load(object sender, EventArgs e)
    {
        if (CurrentConditions.ContainsKey("CaseNo"))
        {
            if (string.IsNullOrEmpty(CurrentConditions["CaseNo"].ToString()))
            {
                state = "insert";
            }
            else
            {
                if (CurrentConditions.ContainsKey("Action"))
                    state = ("Edit".Equals((CurrentConditions["Action"]).ToString())) ? "update" : "read";
                else Response.Redirect(GoQuery());
            }
        }
        else
            Response.Redirect(GoQuery());
        CaseNo = CurrentConditions["CaseNo"].ToString();

        if (!IsPostBack)
        {
            //設定上一頁的網址
            this.btnGoBack.PostBackUrl = GoQuery();
            this.btnCancel.PostBackUrl = this.btnGoBack.PostBackUrl;
            //this.btnSave.CommandArgument = this.btnGoBack.PostBackUrl;
            Literal litNav = Page.Master.FindControl("litNav") as Literal;
            CurrentConditions["listComm_Subsidy"] = listComm_Subsidy;
            //if(state.Equals("insert"))
            //CurrentConditions["TempUploadNo"] = Guid.NewGuid().ToString();
            //RebindDetail();

            #region 繫結資料
            //新增個案
            BindDeptSN();   //院所
            BindSubsidy();  //診治紀錄
            //行政審查
            BindAdminRejectSN();
            InitailAdminStatus();
            //專業審查
            BindProfRejectSN(); //退件原因
            BindProfUser();     //委員
            //特殊個案結案 或 核銷專業審查
            IniWriteOffType();
            BindWriteOffUser(); //委員
            BindWriteOffRejectSN();
            #endregion

            switch (state)
            {
                case "insert":
                    litNav.Text += " / 新增資料";
                    CurrentConditions["CaseStatus"] = "";
                    break;
                case "update":
                case "read":
                    litNav.Text += ("update".Equals(state)) ? " / 修改資料" : " / 檢視資料";
                    Comm_Case GetUpdateData = Comm_Case.GetData(CaseNo.ToString());
                    if (GetUpdateData != null)
                    {
                        #region 顯示修改資料
                        //單頭
                        lblCaseNo.Text = GetUpdateData.CaseNo;
                        dtBeforDate.Text = string.Format("{0:yyy-MM-dd}", GetUpdateData.BeforDate);
                        ddlDeptSN.SelectedValue = GetUpdateData.DeptSN;
                        hdDeptSN.Value = GetUpdateData.DeptSN;
                        SetDepartment(GetUpdateData.DeptSN);
                        BindDoctorSN(GetUpdateData.DeptSN);
                        ddlDoctorSN.SelectedValue = string.Format("{0}", GetUpdateData.DoctorSN);
                        hdDoctorSN.Value = string.Format("{0}", GetUpdateData.DoctorSN);
                        txtPID.Text = Tools.DecryptAES(GetUpdateData.PID);
                        rdbIsLocalUser.SelectedValue = ((bool)GetUpdateData.isLocalUser ? "1" : "2");
                        txtName.Text = GetUpdateData.Name;
                        rdbUserType.SelectedValue = GetUpdateData.UserType;
                        dtBirthday.Text = string.Format("{0:yyy-MM-dd}", GetUpdateData.Birthday);
                        txtTel.Text = GetUpdateData.Tel;
                        rdbGender.SelectedValue = GetUpdateData.Gender;
                        txtAddr.Text = GetUpdateData.Addr;
                        lbApplyAmount.Text = string.Format("{0:#,##0}元", GetUpdateData.ApplyAmount);
                        hdApplyAmount.Value = string.Format("{0}", GetUpdateData.ApplyAmount);
                        dtFinishDate.Text = string.Format("{0:yyy-MM-dd}", GetUpdateData.FinishDate);

                        #region 附件資料
                        //檔案位置
                        string savePath = Server.MapPath($"/FilePool/{GetUpdateData.CaseNo}");  //真實檔案路徑
                        if (Directory.Exists(savePath))
                        {
                            List<Comm_RelFile> listCf = Comm_RelFile.GetAllData(GetUpdateData.CaseNo);
                            StringBuilder sbNames = new StringBuilder();
                            if (listCf.Count > 0)
                            {
                                foreach (var data in listCf)
                                {
                                    sbNames.Append($"<a href='{$"/Common/DownFile.ashx?s={data.SN}"}'>{data.SrcFileName}</a><br>");
                                }
                            }
                            lblFileUpload.Text = sbNames.ToString();
                        }
                        #endregion

                        //案件狀態
                        CurrentConditions["CaseStatus"] = GetUpdateData.Status;

                        //裝置前行政檢查:不顯示>新增個案
                        //if (GetUpdateData.Status.Equals("行政審查通過") || GetUpdateData.Status.Equals("行政審查退件")
                        //    || GetUpdateData.Status.Equals("專業審查通過") || GetUpdateData.Status.Equals("專業審查退件"))
                        if (!string.IsNullOrEmpty(GetUpdateData.Status))
                        {
                            dtAdminDate.Text = string.Format("{0:yyyy-MM-dd}", GetUpdateData.AdminDate);
                            rdbAdminStatus.SelectedValue = GetUpdateData.AdminStatus;
                            if (GetUpdateData.AdminStatus != null && GetUpdateData.AdminStatus.Equals("退件"))
                            {
                                ddlAdminRejectSN.SelectedValue = GetUpdateData.AdminRejectSN.ToString();
                                hdAdminRejectSN.Value = Comm_RejectDesc.GetData(GetUpdateData.AdminRejectSN).RejectDesc;
                            }
                        }

                        //裝置前專業審查:不顯示>新增個案、行政審查退件
                        //if (GetUpdateData.Status.Equals("行政審查通過")
                        //    || GetUpdateData.Status.Equals("專業審查通過") || GetUpdateData.Status.Equals("專業審查退件"))
                        if (!string.IsNullOrEmpty(GetUpdateData.Status) && !GetUpdateData.Status.Equals("行政審查退件"))
                        {
                            dtProfDate.Text = string.Format("{0:yyyy-MM-dd}", GetUpdateData.ProfDate);
                            rdbProfStatus.SelectedValue = GetUpdateData.ProfStatus;
                            InitailProfStatus();
                            if (GetUpdateData.ProfStatus != null && GetUpdateData.ProfStatus.Equals("退件"))
                            {
                                ddlProfRejectSN.SelectedValue = GetUpdateData.ProfRejectSN.ToString();
                                hdProfRejectSN.Value = Comm_RejectDesc.GetData(GetUpdateData.ProfRejectSN).RejectDesc;
                            }
                            if (GetUpdateData.ProfUser != null)
                                ddlProfUser.SelectedValue = GetUpdateData.ProfUser;
                        }

                        //請選擇 特殊個案結案 或 核銷專業審查:顯示>專業審查通過、核銷審查通過、核銷審查退件、特殊個案結案、特殊部分補助
                        if (GetUpdateData.Status.Equals("專業審查通過") || GetUpdateData.Status.Equals("核銷審查通過") || GetUpdateData.Status.Equals("核銷審查退件")
                            || GetUpdateData.Status.Equals("特殊個案結案") || GetUpdateData.Status.Equals("特殊部分補助"))
                        {
                            rdbWriteOffType.SelectedValue = GetUpdateData.WriteOffType;
                            IniWriteOffType();
                            dtWriteOffBefDate.Text = string.Format("{0:yyyy-MM-dd}", GetUpdateData.WriteOffBefDate);
                            dtWriteOffAftDate.Text = string.Format("{0:yyyy-MM-dd}", GetUpdateData.WriteOffAftDate);
                            txtWriteOffAmout.Text = GetUpdateData.WriteOffAmout.ToString();
                            rdbWriteOffFee.SelectedValue = GetUpdateData.WriteOffFee;
                            rdbWriteOffStatus.SelectedValue = GetUpdateData.WriteOffStatus;
                            InitailWriteOffStatusAudit();
                            rdbWriteOffTransfer.SelectedValue = GetUpdateData.WriteOffTransfer;
                            if (rdbWriteOffType.SelectedValue.Equals("核銷專業審查"))
                            {
                                dtWriteOffProDate.Text = string.Format("{0:yyyy-MM-dd}", GetUpdateData.WriteOffProDate);
                                ddlWriteOffUser.SelectedValue = GetUpdateData.WriteOffUser;
                                hdWriteOffUser.Value = GetUpdateData.WriteOffUser;
                                if (rdbWriteOffStatus.SelectedValue.Equals("退件"))
                                {
                                    ddlWriteOffRejectSN.SelectedValue = GetUpdateData.WriteOffRejectSN.ToString();
                                    hdWriteOffRejectSN.Value = GetUpdateData.WriteOffRejectSN.ToString();
                                }
                            }
                        }
                        //detail
                        var Query = Comm_CaseItem.GetData(CaseNo.ToString());
                        var Catregory = Query.itemType;

                        //補助類型
                        CurrentConditions["Subsidy_ItemType"] = Catregory;

                        if (Catregory != -1)
                        {
                            List<Comm_CaseItem> listCaseItem = new List<Comm_CaseItem>();
                            listCaseItem = Query.listItem;

                            //存入有勾選的項目SN及數量
                            Dictionary<int, int> dicSelectItem = new Dictionary<int, int>();
                            foreach (var item in listCaseItem)
                            {
                                int itemCount = 1;
                                if (item.ItemCount != null && item.ItemCount > 1)
                                    itemCount = (int)item.ItemCount;
                                dicSelectItem.Add((int)item.ItemSN, itemCount);
                            }

                            if (Catregory == Convert.ToInt32(Subsidy_Catregory.診治項目))
                            {
                                //診治項目
                                for (int i = 0; i < ItemType1.Items.Count; i++)
                                {
                                    if (dicSelectItem.ContainsKey(int.Parse(ItemType1.Items[i].Value)))
                                        ItemType1.Items[i].Selected = true;
                                    else
                                        ItemType1.Items[i].Selected = false;
                                }
                                ItemType2.Enabled = false;
                            }
                            else
                            {
                                //維修費項目
                                for (int i = 0; i < ItemType1.Items.Count; i++)
                                {
                                    foreach (DataListItem listItem in ItemType2.Items)
                                    {
                                        HiddenField hdItem = listItem.FindControl("hdItem") as HiddenField;
                                        int itemSN = int.Parse(hdItem.Value);   //補助項目SN
                                        if (dicSelectItem.ContainsKey(itemSN))
                                        {
                                            CheckBox cbxItem = listItem.FindControl("cbxItem") as CheckBox;
                                            TextBox txtItem = listItem.FindControl("txtItem") as TextBox;
                                            cbxItem.Checked = true;
                                            txtItem.Text = dicSelectItem[itemSN].ToString();
                                        }
                                    }
                                }
                                ItemType1.Enabled = false;
                            }
                        }
                        #endregion
                    }
                    break;
            }

            string CaseStatus = string.Empty;
            if (CurrentConditions.ContainsKey("CaseStatus"))
                CaseStatus = CurrentConditions["CaseStatus"].ToString();


            if (state.Equals("read") || !string.IsNullOrEmpty(CaseStatus))
            {
                //唯讀或已送出個案
                plAddCase.Enabled = false;
                plSubsidy.Enabled = false;
                fuFileUpload.Enabled = false;
                btnFileUpload.Enabled = false;
            }


            switch (CaseStatus)
            {
                case "待審":
                    plSubsidy.Enabled = true;
                    plAdmin.Visible = true;
                    plProf.Visible = false;     //裝置前專業審查
                    plWriteOff.Visible = false; //特殊個案結案或核銷專業審查
                    break;
                case "行政審查退件":
                    plSubsidy.Enabled = true;
                    plAdmin.Visible = true;
                    plAdmin.Enabled = false;
                    plProf.Visible = false;     //裝置前專業審查
                    plWriteOff.Visible = false; //特殊個案結案或核銷專業審查
                    break;
                case "行政審查通過":
                case "專業審查退件":
                    plSubsidy.Enabled = true;
                    plAdmin.Visible = true;
                    plAdmin.Enabled = false;
                    plProf.Visible = true;
                    plProf.Enabled = true;
                    plWriteOff.Visible = false; //特殊個案結案或核銷專業審查
                    break;
                case "專業審查通過":
                case "核銷審查退件":
                    plSubsidy.Enabled = true;
                    plAdmin.Visible = true;
                    plAdmin.Enabled = false;
                    plProf.Visible = true;     //裝置前專業審查
                    plProf.Enabled = false;
                    plWriteOff.Visible = true; //特殊個案結案或核銷專業審查
                    break;
                case "":    //新增個案
                    plSubsidy.Enabled = true;
                    plAdmin.Visible = false;
                    fuFileUpload.Enabled = true;
                    plProf.Visible = false;     //裝置前專業審查
                    plWriteOff.Visible = false;
                    break;
            }

        }
    }


    //private void RebindDetail()
    //{
    //    listIP = CurrentConditions["Detail"] as List<Comm_Department_IP>;
    //    //gvIndex.DataSource = listIP;
    //    //tbxIP.Text = "";
    //    //gvIndex.DataBind();
    //}


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

        bool WriteToDBIsSuccess = false; //增修是否成功


        #region  資料更新/新增
        if ("insert".Equals(state))
        {
            Comm_Case insert = new Comm_Case();
            insert.BeforDate = Convert.ToDateTime(dtBeforDate.Text);                        //裝置前收件日期
            insert.DeptSN = ddlDeptSN.SelectedValue;                                        //醫療院所名稱(院所代號)
            insert.DoctorSN = int.Parse(ddlDoctorSN.SelectedValue);                         //負責醫師姓名(醫師代號)                
            insert.PID = Tools.EncryptAES(jSecurity.XSS(txtPID.Text));                      //身分證字號
            insert.isLocalUser = rdbIsLocalUser.SelectedValue.Equals("1") ? true : false;   //本縣設籍滿一年
            insert.Name = jSecurity.XSS(txtName.Text);                                      //姓名
            insert.UserType = jSecurity.XSS(rdbUserType.SelectedValue);                     //身分別
            insert.Birthday = Convert.ToDateTime(dtBirthday.Text);                          //生日
            insert.Tel = jSecurity.XSS(txtTel.Text);                                        //電話
            insert.Gender = jSecurity.XSS(rdbGender.SelectedValue);                         //性別
            insert.Addr = jSecurity.XSS(txtAddr.Text);                                      //地址
            insert.ApplyAmount = int.Parse(hdApplyAmount.Value);                            //申請補助金額
            insert.FinishDate = Convert.ToDateTime(dtFinishDate.Text);                      //預定完成日期
            insert.CreateID = SessionCenter.AccUser.ID;
            insert.ModifyID = SessionCenter.AccUser.ID;
            insert.CreateDate = DateTime.Now;
            insert.ModifyDate = DateTime.Now;

            #region 診治紀錄項目(detail)
            Comm_CaseItem insertDetail = new Comm_CaseItem();
            List<Comm_CaseItem> listDetail = new List<Comm_CaseItem>(); //所有的項目list
            if (CurrentConditions.ContainsKey("Subsidy_ItemType"))
            {
                int Catregory = 0;
                if (CurrentConditions.ContainsKey("Subsidy_ItemType"))
                {
                    //判斷補助項目並寫入Comm_CaseItem(Detail) 
                    Catregory = (CurrentConditions["Subsidy_ItemType"].ToString().Equals("診治項目") ? 1 : 2);
                    int SN = Comm_CaseItem.GetItemMaxSN();  //當下最大的SN

                    if (Catregory == Convert.ToInt32(Subsidy_Catregory.診治項目))
                    {
                        //診治項目
                        for (int i = 0; i < ItemType1.Items.Count; i++)
                        {
                            if (ItemType1.Items[i].Selected)
                            {
                                //insertDetail.ItemType = Catregory;                                  //項目(1:診治項目、2:維修費項目)
                                //insertDetail.ItemSN = Convert.ToInt32(ItemType1.Items[i].Value);    //項目的SN
                                //listDetail.Add(insertDetail);
                                listDetail.Add(new Comm_CaseItem { SN = ++SN, ItemType = Catregory, ItemSN = Convert.ToInt32(ItemType1.Items[i].Value) });

                            }
                        }
                    }
                    else
                    {
                        //維修費項目
                        foreach (DataListItem listItem in ItemType2.Items)
                        {
                            CheckBox cbxItem = listItem.FindControl("cbxItem") as CheckBox;
                            if (cbxItem.Checked)
                            {
                                HiddenField hdItem = listItem.FindControl("hdItem") as HiddenField; //該項目的SN
                                TextBox txtItem = listItem.FindControl("txtItem") as TextBox;       //該項目的數量輸入框
                                int intItemCounts = int.Parse(txtItem.Text);
                                //insertDetail.ItemType = Catregory;                                  //項目(1:診治項目、2:維修費項目)
                                //insertDetail.ItemSN = Convert.ToInt32(hdItem.Value);                //項目的SN
                                //insertDetail.ItemCount = intItemCounts;                             //維修費項目數量
                                listDetail.Add(new Comm_CaseItem { SN = ++SN, ItemType = Catregory, ItemSN = Convert.ToInt32(hdItem.Value), ItemCount = intItemCounts });
                            }
                        }
                    }
                }
            }
            #endregion
            Comm_Case insertCase = Comm_Case.InsertData(insert, listDetail, SessionCenter.AccUser.ID);


            if (!string.IsNullOrEmpty(insertCase.CaseNo))
            {
                //判斷新增成功
                WriteToDBIsSuccess = true;

                //處理附件上傳
                if (CurrentConditions.ContainsKey("TempUploadNo"))
                {
                    string tempNo = CurrentConditions["TempUploadNo"].ToString();
                    string rtnSetFileCaseNoFromTemp = Comm_RelFile.SetFileCaseNoFromTemp(tempNo, insertCase.CaseNo);
                    if (string.IsNullOrEmpty(rtnSetFileCaseNoFromTemp))
                        SaveFile(tempNo, insertCase.CaseNo);  //將暫存的資料夾名稱(GUID)變更為案號(CaseNo)
                }
            }
        }
        else if ("update".Equals(state))
        {
            string CaseStatus = string.Empty;
            if (CurrentConditions.ContainsKey("CaseStatus"))
                CaseStatus = CurrentConditions["CaseStatus"].ToString();
            Comm_Case update = Comm_Case.GetData(lblCaseNo.Text);
            switch (CaseStatus)
            {
                case "":
                    // update.CaseNo = lblCaseNo.Text;
                    update.BeforDate = Convert.ToDateTime(dtBeforDate.Text);                        //裝置前收件日期
                    update.DeptSN = ddlDeptSN.SelectedValue;                                        //醫療院所名稱(院所代號)
                    update.DoctorSN = int.Parse(ddlDoctorSN.SelectedValue);                         //負責醫師姓名(醫師代號)                
                    update.PID = Tools.EncryptAES(jSecurity.XSS(txtPID.Text));                      //身分證字號
                    update.isLocalUser = rdbIsLocalUser.SelectedValue.Equals("1") ? true : false;   //本縣設籍滿一年
                    update.Name = jSecurity.XSS(txtName.Text);                                      //姓名
                    update.UserType = jSecurity.XSS(rdbUserType.SelectedValue);                     //身分別
                    update.Birthday = Convert.ToDateTime(dtBirthday.Text);                          //生日
                    update.Tel = jSecurity.XSS(txtTel.Text);                                        //電話
                    update.Gender = jSecurity.XSS(rdbGender.SelectedValue);                         //性別
                    update.Addr = jSecurity.XSS(txtAddr.Text);                                      //地址
                    update.ApplyAmount = int.Parse(hdApplyAmount.Value);                            //申請補助金額
                    update.FinishDate = Convert.ToDateTime(dtFinishDate.Text);                      //預定完成日期
                                                                                                    // update.CreateID = SessionCenter.AccUser.ID;
                    update.ModifyID = SessionCenter.AccUser.ID;
                    // update.CreateDate = DateTime.Now;
                    update.ModifyDate = DateTime.Now;

                    #region 診治紀錄項目(detail)
                    Comm_CaseItem updateDetail = new Comm_CaseItem();
                    List<Comm_CaseItem> listDetail = new List<Comm_CaseItem>(); //所有的項目list
                    int Catregory = 0;

                    if (CurrentConditions.ContainsKey("Subsidy_ItemType"))
                    {
                        //判斷補助項目並寫入Comm_CaseItem(Detail) 
                        Catregory = (CurrentConditions["Subsidy_ItemType"].ToString().Equals("診治項目") ? 1 : 2);
                        int SN = Comm_CaseItem.GetItemMaxSN();  //當下最大的SN

                        if (Catregory == Convert.ToInt32(Subsidy_Catregory.診治項目))
                        {
                            //診治項目
                            for (int i = 0; i < ItemType1.Items.Count; i++)
                            {
                                if (ItemType1.Items[i].Selected)
                                {
                                    //updateDetail.ItemType = Catregory;                                  //項目(1:診治項目、2:維修費項目)
                                    //updateDetail.ItemSN = Convert.ToInt32(ItemType1.Items[i].Value);    //項目的SN
                                    listDetail.Add(new Comm_CaseItem { SN = ++SN, ItemType = Catregory, ItemSN = Convert.ToInt32(ItemType1.Items[i].Value) });
                                }
                            }
                        }
                        else
                        {
                            //維修費項目
                            foreach (DataListItem listItem in ItemType2.Items)
                            {
                                CheckBox cbxItem = listItem.FindControl("cbxItem") as CheckBox;
                                if (cbxItem.Checked)
                                {
                                    HiddenField hdItem = listItem.FindControl("hdItem") as HiddenField; //該項目的SN
                                    TextBox txtItem = listItem.FindControl("txtItem") as TextBox;       //該項目的數量輸入框
                                    int intItemCounts = int.Parse(txtItem.Text);
                                    //updateDetail.ItemType = Catregory;                                  //項目(1:診治項目、2:維修費項目)
                                    //updateDetail.ItemSN = Convert.ToInt32(hdItem.Value);                //項目的SN
                                    //updateDetail.ItemCount = intItemCounts;                             //維修費項目數量
                                    listDetail.Add(new Comm_CaseItem { SN = ++SN, ItemType = Catregory, ItemSN = Convert.ToInt32(hdItem.Value), ItemCount = intItemCounts });
                                }
                            }
                        }
                    }
                    #endregion
                    Comm_Case.updateData(update, listDetail, "待審", SessionCenter.AccUser.ID);
                    break;
                case "待審":
                    //update.CaseNo = lblCaseNo.Text;
                    update.AdminDate = DateTime.Parse(dtAdminDate.Text);
                    if (rdbAdminStatus.SelectedValue.Equals("通過"))
                    {
                        update.Status = "行政審查通過";
                        update.AdminStatus = "通過";

                        #region 記錄log
                        Comm_Record record = new Comm_Record();
                        record.NodeID = 13;
                        record.CaseNo = CaseNo;
                        record.ModifyAccountID = SessionCenter.AccUser.ID;
                        record.ModifyDate = DateTime.Now;
                        record.Action = "編輯";
                        record.Record = $"行政審查日期：{dtAdminDate.Text}<br>行政審查結果：通過";
                        Comm_Record.Insert(record);
                        #endregion
                    }
                    else
                    {
                        update.Status = "行政審查退件";
                        update.AdminStatus = "退件";
                        update.AdminRejectSN = int.Parse(hdAdminRejectSN.Value);

                        #region 記錄log
                        Comm_Record record = new Comm_Record();
                        record.NodeID = 13;
                        record.CaseNo = CaseNo;
                        record.ModifyAccountID = SessionCenter.AccUser.ID;
                        record.ModifyDate = DateTime.Now;
                        record.Action = "編輯";
                        record.Record = $"行政審查日期：{dtAdminDate.Text}<br>行政審查結果：退件";
                        Comm_Record.Insert(record);
                        #endregion
                    }
                    update.ModifyID = SessionCenter.AccUser.ID;
                    update.ModifyDate = DateTime.Now;
                    Comm_Case.Update(update);
                    break;
                case "行政審查通過":
                case "專業審查退件":
                    //update.CaseNo = lblCaseNo.Text;
                    update.ProfDate = DateTime.Parse(dtProfDate.Text);
                    update.ProfUser = ddlProfUser.SelectedValue;
                    if (rdbProfStatus.SelectedValue.Equals("通過"))
                    {
                        update.Status = "專業審查通過";
                        update.ProfStatus = "通過";

                        #region 記錄log
                        Comm_Record record = new Comm_Record();
                        record.NodeID = 13;
                        record.CaseNo = CaseNo;
                        record.ModifyAccountID = SessionCenter.AccUser.ID;
                        record.ModifyDate = DateTime.Now;
                        record.Action = "編輯";
                        record.Record = "專業審查結果：通過";
                        Comm_Record.Insert(record);
                        #endregion
                    }
                    else
                    {
                        update.Status = "專業審查退件";
                        update.ProfStatus = "退件";
                        update.ProfRejectSN = int.Parse(ddlProfRejectSN.SelectedValue);

                        #region 記錄log
                        Comm_Record record = new Comm_Record();
                        record.NodeID = 13;
                        record.CaseNo = CaseNo;
                        record.ModifyAccountID = SessionCenter.AccUser.ID;
                        record.ModifyDate = DateTime.Now;
                        record.Action = "編輯";
                        record.Record = "專業審查結果：退件";
                        Comm_Record.Insert(record);
                        #endregion
                    }
                    update.ModifyID = SessionCenter.AccUser.ID;
                    // update.CreateDate = DateTime.Now;
                    update.ModifyDate = DateTime.Now;
                    Comm_Case.Update(update);

                    break;
                case "專業審查通過":
                case "核銷審查退件":
                    update.WriteOffType = rdbWriteOffType.SelectedValue;                    //特殊個案結案 或 核銷專業審查
                    update.WriteOffBefDate = DateTime.Parse(dtWriteOffBefDate.Text);        //裝置前審查通過發文日期
                    update.WriteOffAftDate = DateTime.Parse(dtWriteOffAftDate.Text);        //裝置後收件日期
                    update.WriteOffAmout = int.Parse(jSecurity.XSS(txtWriteOffAmout.Text)); //撥付費用
                    update.WriteOffFee = rdbWriteOffFee.SelectedValue;                      //篩檢費
                    update.WriteOffMemo = jSecurity.XSS(txtWriteOffMemo.Text);              //備註
                    update.WriteOffTransfer = rdbWriteOffTransfer.SelectedValue;            //是否移交社會局
                    if (rdbWriteOffType.SelectedValue.Equals("核銷專業審查"))
                    {
                        update.WriteOffProDate = DateTime.Parse(dtWriteOffProDate.Text);    //專業審查日期
                        update.WriteOffUser = ddlWriteOffUser.SelectedValue;                //專業審查委員
                        //專業審查結果
                        if (rdbWriteOffStatus.SelectedValue.Equals("通過"))
                        {
                            update.Status = "核銷審查通過";
                            update.WriteOffStatus = "通過";

                            #region 記錄log
                            Comm_Record record = new Comm_Record();
                            record.NodeID = 14;
                            record.CaseNo = CaseNo;
                            record.ModifyAccountID = SessionCenter.AccUser.ID;
                            record.ModifyDate = DateTime.Now;
                            record.Action = "編輯";
                            record.Record = $"裝置前審查通過日期：{dtWriteOffBefDate.Text}<br>" +
                                            $"裝置後收件日期：{dtWriteOffAftDate.Text}<br>" +
                                            $"撥付費用：{txtWriteOffAmout.Text}<br>" +
                                            $"篩檢費：{rdbWriteOffFee.SelectedValue}<br>" +
                                            $"專業審查結果：通過<br>" +
                                            $"是否移交社會局：{rdbWriteOffTransfer.SelectedValue}";
                            Comm_Record.Insert(record);
                            #endregion
                        }
                        else
                        {
                            update.Status = "核銷審查退件";
                            update.WriteOffStatus = "退件";
                            update.WriteOffRejectSN = int.Parse(ddlWriteOffRejectSN.SelectedValue);

                            #region 記錄log
                            Comm_Record record = new Comm_Record();
                            record.NodeID = 14;
                            record.CaseNo = CaseNo;
                            record.ModifyAccountID = SessionCenter.AccUser.ID;
                            record.ModifyDate = DateTime.Now;
                            record.Action = "編輯";
                            record.Record = "核銷審查結果：退件";
                            Comm_Record.Insert(record);
                            #endregion
                        }
                    }
                    else
                    {
                        //特殊個案結案
                        var OffStatus = rdbWriteOffStatus.SelectedValue.Equals("結案");
                        if (rdbWriteOffStatus.SelectedValue.Equals("結案"))
                        {
                            update.Status = "特殊個案結案";
                            update.WriteOffStatus = "結案";

                            #region 記錄log
                            Comm_Record record = new Comm_Record();
                            record.NodeID = 14;
                            record.CaseNo = CaseNo;
                            record.ModifyAccountID = SessionCenter.AccUser.ID;
                            record.ModifyDate = DateTime.Now;
                            record.Action = "編輯";
                            record.Record = "特殊個案結案：結案";
                            Comm_Record.Insert(record);
                            #endregion
                        }
                        else
                        {
                            update.Status = "特殊部分補助";
                            update.WriteOffStatus = "特殊部分補助";

                            #region 記錄log
                            Comm_Record record = new Comm_Record();
                            record.NodeID = 14;
                            record.CaseNo = CaseNo;
                            record.ModifyAccountID = SessionCenter.AccUser.ID;
                            record.ModifyDate = DateTime.Now;
                            record.Action = "編輯";
                            record.Record = "特殊個案結案：特殊部分補助";
                            Comm_Record.Insert(record);
                            #endregion
                        }
                    }
                    update.ModifyID = SessionCenter.AccUser.ID;
                    update.ModifyDate = DateTime.Now;
                    Comm_Case.Update(update);
                    break;
            }

        }

        #endregion

        #region 維護記錄
        //Comm_Record rec = new Comm_Record();
        //rec.NodeID = iNodeID;
        //rec.IP = Request.UserHostAddress;
        //rec.ModifyDate = DateTime.Now;
        //rec.ModifyAccountID = SessionCenter.LoginAccUser.ID;
        //string fun = (state == "insert") ? "新增" : "修改";
        //if (id == string.Empty)
        //    rec.Record = fun + txtAccount.Text + "帳號資料";
        //Comm_Record.Insert(rec);
        #endregion

        //導回上一頁
        if (state.Equals("update"))
            Pages.AlertByswal(Tools.altertType.成功.ToString(), "修改成功", Tools.altertType.成功.ToDescriptionString(), GoQuery());
        else if (string.IsNullOrEmpty(state) && WriteToDBIsSuccess)
            Pages.AlertByswal(Tools.altertType.成功.ToString(), "新增成功", Tools.altertType.成功.ToDescriptionString(), GoQuery());
        else
            Response.Redirect("Query.aspx?n=" + jSecurity.GetQueryString("n"));
        //    #endregion
    }

    private void SaveFile(string TempNo, string CaseNo)
    {
        if (!string.IsNullOrEmpty(TempNo) && !string.IsNullOrEmpty(CaseNo))
        {
            //將暫存的資料夾名稱(GUID)變更為案號(CaseNo)
            string savaPath = Server.MapPath($"/FilePool/");
            if (Directory.Exists(savaPath + TempNo))
            {
                string tempFolder = TempNo;
                string NewFolder = CaseNo;
                Directory.Move(savaPath + TempNo, savaPath + CaseNo);
                //同步變更路徑(GUID)變更為案號(CaseNo)
                List<Comm_RelFile> listCf = Comm_RelFile.GetAllData(CaseNo);
                string strPath = string.Empty;
                foreach (var data in listCf)
                {
                    strPath = data.FileName;
                    strPath = strPath.Replace(TempNo, CaseNo);
                    data.FileName = strPath;
                    Comm_RelFile.Update(data);
                }
            }
        }
    }
    #region 資料檢查
    private string CheckData()
    {
        string CaseStatus = string.Empty;
        if (CurrentConditions.ContainsKey("CaseStatus"))
            CaseStatus = CurrentConditions["CaseStatus"].ToString();
        StringBuilder sbError = new StringBuilder();
        switch (CaseStatus)
        {
            case "":
                if (string.IsNullOrWhiteSpace(dtBeforDate.Text))
                    sbError.Append(@"請選擇裝置前收件日期\n");
                else if (!DateTime.TryParse(dtBeforDate.Text, out DateTime tryParseDateTime))
                    sbError.Append(@"裝置前收件日期格式不正確\n");

                if (string.IsNullOrWhiteSpace(hdDeptSN.Value))
                    sbError.Append(@"請選擇醫療院所名稱\n");

                if (string.IsNullOrWhiteSpace(hdDoctorSN.Value))
                    sbError.Append(@"請選擇負責醫師姓名\n");

                if (string.IsNullOrWhiteSpace(txtPID.Text))
                    sbError.Append(@"請輸入身分證字號\n");
                else if (!txtPID.Text.isIdentificationId() && !txtPID.Text.CheckForeignIdNumber())
                    sbError.Append(@"請輸入正確的身分證字號\n");
                else
                {
                    //判斷是否申請過，若有則另外判斷條件
                    string auditPID = AuditPID(Tools.EncryptAES(jSecurity.XSS(txtPID.Text)));
                    if (!string.IsNullOrEmpty(auditPID))
                        sbError.Append($"{auditPID}<br>");
                }

                if (string.IsNullOrWhiteSpace(txtName.Text))
                    sbError.Append(@"請輸入姓名\n");
                else if (txtName.Text.Length > 50)
                    sbError.Append(@"姓名不可超過50字\n");

                if (string.IsNullOrWhiteSpace(dtBirthday.Text))
                    sbError.Append(@"請選擇生日\n");
                else if (!DateTime.TryParse(dtBirthday.Text, out DateTime tryParseDateTime))
                    sbError.Append(@"生日日期格式不正確\n");

                if (string.IsNullOrWhiteSpace(txtTel.Text))
                    sbError.Append(@"請輸入電話\n");
                else if (txtTel.Text.Length > 50)
                    sbError.Append(@"電話不可超過50字\n");

                if (string.IsNullOrWhiteSpace(txtAddr.Text))
                    sbError.Append(@"請輸入地址\n");
                else if (txtAddr.Text.Length > 255)
                    sbError.Append(@"地址不可超過255字\n");

                if (string.IsNullOrWhiteSpace(dtFinishDate.Text))
                    sbError.Append(@"請選擇預定完成日期\n");
                else if (!DateTime.TryParse(dtFinishDate.Text, out DateTime tryParseDateTime))
                    sbError.Append(@"預定完成日期格式不正確\n");

                //補助項目
                if (!string.IsNullOrEmpty(CheckErrorItem()))
                    sbError.Append(CheckErrorItem());
                break;
            case "待審":
                //補助項目
                if (!string.IsNullOrEmpty(CheckErrorItem()))
                    sbError.Append(CheckErrorItem());
                //行政審查
                if (string.IsNullOrWhiteSpace(dtAdminDate.Text))
                    sbError.Append(@"請選擇行政審查日期\n");
                else if (!DateTime.TryParse(dtAdminDate.Text, out DateTime tryParseDateTime))
                    sbError.Append(@"行政審查日期格式不正確\n");

                if (rdbAdminStatus.SelectedValue.Equals("退件") && string.IsNullOrEmpty(hdAdminRejectSN.Value))
                    sbError.Append(@"請選擇退件原因\n");
                break;
            case "行政審查通過":
                //補助項目
                if (!string.IsNullOrEmpty(CheckErrorItem()))
                    sbError.Append(CheckErrorItem());
                //裝置前審查
                if (string.IsNullOrWhiteSpace(dtProfDate.Text))
                    sbError.Append(@"請選擇專業審查日期\n");
                else if (!DateTime.TryParse(dtAdminDate.Text, out DateTime tryParseDateTime))
                    sbError.Append(@"專業審查日期格式不正確\n");

                if (rdbProfStatus.SelectedValue.Equals("退件") && string.IsNullOrEmpty(hdProfRejectSN.Value))
                    sbError.Append(@"請選擇退件原因\n");

                if (string.IsNullOrEmpty(ddlProfUser.SelectedValue))
                    sbError.Append(@"請選擇專業審查委員\n");
                break;
            case "專業審查通過":
            case "核銷審查退件":
                //補助項目
                if (!string.IsNullOrEmpty(CheckErrorItem()))
                    sbError.Append(CheckErrorItem());
                //特殊個案結案 或 核銷專業審查
                if (string.IsNullOrWhiteSpace(dtWriteOffBefDate.Text))
                    sbError.Append(@"請選擇裝置前審查通過發文日期\n");
                else if (!DateTime.TryParse(dtWriteOffBefDate.Text, out DateTime tryParseDateTime))
                    sbError.Append(@"裝置前審查通過發文日期格式不正確\n");

                if (string.IsNullOrWhiteSpace(dtWriteOffAftDate.Text))
                    sbError.Append(@"請選擇裝置後收件日期\n");
                else if (!DateTime.TryParse(dtWriteOffAftDate.Text, out DateTime tryParseDateTime))
                    sbError.Append(@"裝置後收件日期格式不正確\n");

                else if (!string.IsNullOrWhiteSpace(dtWriteOffBefDate.Text) && !string.IsNullOrWhiteSpace(dtWriteOffAftDate.Text)
                        && DateTime.TryParse(dtWriteOffBefDate.Text, out DateTime tryParseDateTimeS) && DateTime.TryParse(dtWriteOffAftDate.Text, out DateTime tryParseDateTimeE)
                        && DateTime.Parse(dtWriteOffBefDate.Text) > DateTime.Parse(dtWriteOffAftDate.Text)
                        )
                    sbError.Append(@"裝置後收件日期不可早於裝置前審查日期\n");

                if (string.IsNullOrWhiteSpace(txtWriteOffAmout.Text))
                    sbError.Append(@"請輸入撥付費用\n");
                else if (!int.TryParse(jSecurity.XSS(txtWriteOffAmout.Text), out int tyrParseInt))
                    sbError.Append(@"撥付費用格式不正確\n");

                if (rdbWriteOffType.SelectedValue.Equals("核銷專業審查") && string.IsNullOrWhiteSpace(dtWriteOffAftDate.Text))
                    sbError.Append(@"請選擇專業審查日期\n");
                else if (rdbWriteOffType.SelectedValue.Equals("核銷專業審查") && !DateTime.TryParse(dtWriteOffAftDate.Text, out DateTime tryParseDateTime))
                    sbError.Append(@"專業審查日期格式不正確\n");

                if (rdbWriteOffType.SelectedValue.Equals("核銷專業審查") && string.IsNullOrWhiteSpace(ddlWriteOffUser.SelectedValue))
                    sbError.Append(@"請選擇專業審查委員\n");

                if (rdbWriteOffStatus.SelectedValue.Equals("退件") && string.IsNullOrEmpty(hdWriteOffRejectSN.Value))
                    sbError.Append(@"請選擇退件原因\n");
                break;
        }


        //else if()

        //if (state == "insert")
        //{
        //    if (Comm_Department.GetSingle(x => x.DeptName.Equals(tbxDeptName.Text)) != null)
        //        sbError.Append(@"此院所資料已被新增過\n");
        //}
        return sbError.ToString();
    }

    //共用確認項目
    protected string CheckErrorItem()
    {
        StringBuilder sbErrorItem = new StringBuilder();
        //補助項目
        if (!CurrentConditions.ContainsKey("Subsidy_ItemType") || (CurrentConditions.ContainsKey("Subsidy_ItemType") && string.IsNullOrEmpty(CurrentConditions["Subsidy_ItemType"].ToString())))
            sbErrorItem.Append(@"請勾選申請補助之項目\n");

        if (CurrentConditions.ContainsKey("IsSubsidyItemCountSpace"))
        {
            if (Convert.ToBoolean(CurrentConditions["IsSubsidyItemCountSpace"]))
                sbErrorItem.Append(@"請輸入維修費項目中已勾選的數量\n");
            else if (int.Parse(hdApplyAmount.Value) > 6000)
                sbErrorItem.Append(@"申請補助金額不可超過 6,000 元\n");
        }
        return sbErrorItem.ToString();
    }


    #endregion

    #region 繫結資料

    #region 醫療院所
    protected void ddlDeptSN_SelectedIndexChanged(object sender, EventArgs e)
    {
        string strDeptSN = "";
        if (!string.IsNullOrEmpty(ddlDeptSN.SelectedValue))
        {
            strDeptSN = ddlDeptSN.SelectedValue;
        }

        hdDeptSN.Value = strDeptSN;
        SetDepartment(strDeptSN);   //連動院所資料
        BindDoctorSN(strDeptSN);    //繫結醫師姓名
    }

    private void BindDeptSN()
    {
        var Query = Comm_Department.GetAllDepartmentNotStop();
        ddlDeptSN.Items.Add(new ListItem("請選擇院所名稱", ""));
        foreach (var data in Query)
        {
            ddlDeptSN.Items.Add(new ListItem(data.DeptName, data.SN));
        }

        //initail Doctor
        ddlDoctorSN.Items.Clear();
        ddlDoctorSN.Items.Add(new ListItem("請選擇負責醫師姓名", ""));
    }

    //將院所相關資料代入
    private void SetDepartment(string strDeptSN)
    {
        if (string.IsNullOrEmpty(strDeptSN))
        {
            lbDID.Text = string.Empty;
            lbTel.Text = string.Empty;
            lbFax.Text = string.Empty;
            lbAddr.Text = string.Empty;
        }
        else
        {
            var data = Comm_Department.GetDepartment(strDeptSN);
            lbDID.Text = data.DID;
            lbTel.Text = data.Tel;
            lbFax.Text = data.Fax;
            lbAddr.Text = data.Addr;
        }
    }
    #endregion

    #region 醫師
    protected void ddlDoctorSN_SelectedIndexChanged(object sender, EventArgs e)
    {
        string SN = string.Empty;
        if (!string.IsNullOrEmpty(ddlDoctorSN.SelectedValue))
            SN = ddlDoctorSN.SelectedValue;
        hdDoctorSN.Value = SN;
    }

    private void BindDoctorSN(string DeptSN)
    {
        ddlDoctorSN.Items.Clear();  //initail
        if (!string.IsNullOrEmpty(DeptSN))
            ddlDoctorSN.Enabled = true;
        else
            ddlDoctorSN.Enabled = false;

        var Query = Comm_Doctor.GetAllData(DeptSN);
        ddlDoctorSN.Items.Add(new ListItem("請選擇負責醫師姓名", ""));
        foreach (var daTA in Query)
        {
            ddlDoctorSN.Items.Add(new ListItem(daTA.Name, daTA.SN.ToString()));
        }
    }
    #endregion

    #region 診治紀錄
    protected void ItemType1_SelectedIndexChanged(object sender, EventArgs e)
    {
        //診治項目
        if (!string.IsNullOrEmpty(ItemType1.SelectedValue))
        {
            ItemType2.Enabled = false;
            CurrentConditions["Subsidy_ItemType"] = Subsidy_Catregory.診治項目;   //申請補助的項目
            CurrentConditions.Remove("IsSubsidyItemCountSpace");                 //移除修費項目判斷
        }
        else
        {
            ItemType2.Enabled = true;
            CurrentConditions["Subsidy_ItemType"] = "";
        }

        //計算金額
        SumApplyAmount(Convert.ToInt32(Subsidy_Catregory.診治項目));
    }
    protected void ItemType2CheckBox_changed(object sender, EventArgs e)
    {
        //維修費項目-checkbox
        ItemType1.Enabled = true;   //先預設診治項目為true
        CurrentConditions["Subsidy_ItemType"] = "";
        foreach (DataListItem listItem in ItemType2.Items)
        {
            CheckBox cbxItem = listItem.FindControl("cbxItem") as CheckBox;
            if (cbxItem.Checked)
            {
                //一旦維修費項目有勾選，便將診治項目設為false
                ItemType1.Enabled = false;
                CurrentConditions["Subsidy_ItemType"] = Subsidy_Catregory.維修費項目;   //申請補助的項目
                break;
            }
        }

        //取消勾選同步清空輸入之數量
        CheckBox thisCheckBox = (CheckBox)sender;
        if (!thisCheckBox.Checked)
        {
            TextBox txtItem = thisCheckBox.Parent.FindControl("txtItem") as TextBox;
            txtItem.Text = string.Empty;
        }

        //計算金額
        SumApplyAmount(Convert.ToInt32(Subsidy_Catregory.維修費項目));
    }
    protected void ItemType2TextBox_changed(object sender, EventArgs e)
    {
        //維修費項目-textbox
        string strItemCount = string.Empty;
        TextBox thisTextBox = (TextBox)sender;
        strItemCount = thisTextBox.Text;    //輸入的項目數
        int intIemCount = 0;
        if (!int.TryParse(strItemCount, out intIemCount))
            Pages.AlertByswal(Tools.altertType.錯誤.ToString(), "維修費項目數量須為數字", Tools.altertType.錯誤.ToDescriptionString());
        else if (!string.IsNullOrWhiteSpace(strItemCount))
        {
            //當輸入框有值時，判斷核取框有勾選則計算補助金額
            CheckBox cbxItem = thisTextBox.Parent.FindControl("cbxItem") as CheckBox;
            if (cbxItem.Checked)
                SumApplyAmount(Convert.ToInt32(Subsidy_Catregory.維修費項目));
        }
    }

    private void BindSubsidy()
    {
        //診治紀錄所需資料
        var Query = Comm_Subsidy.GetAllSubsidy();
        foreach (var data in Query)
        {
            listComm_Subsidy.Add(data);
            if (data.Catregory == 1)
                ItemType1.Items.Add(new ListItem(value: data.SN.ToString(), text: data.Name));
        }
        CurrentConditions["listComm_Subsidy"] = listComm_Subsidy;

        ItemType2.DataSource = (from d in listComm_Subsidy where d.Catregory == 2 select d).ToList();
        ItemType2.DataBind();

    }

    #endregion

    #region 補助金額計算
    /// <summary>
    /// 加總申請補助金額
    /// </summary>
    /// <param name="Catregory">1:診治項目、2:維修費項目</param>
    private void SumApplyAmount(int Catregory)
    {
        //取得有選擇的項目及數量(診治數量預設1)
        Dictionary<int, int> dicSelectItem = new Dictionary<int, int>();
        if (Catregory == Convert.ToInt32(Subsidy_Catregory.診治項目))
        {
            //診治項目
            for (int i = 0; i < ItemType1.Items.Count; i++)
            {
                if (ItemType1.Items[i].Selected)
                    dicSelectItem.Add(Convert.ToInt32(ItemType1.Items[i].Value), 1);
            }
        }
        else
        {
            //維修費項目
            bool IsSubsidyItemCountSpace = false;                   //是否有未填寫的項目數量(預設無)
            if (CurrentConditions.ContainsKey("IsSubsidyItemCountSpace"))
                CurrentConditions.Remove("IsSubsidyItemCountSpace");    //預設沒有判斷項目的Session

            foreach (DataListItem listItem in ItemType2.Items)
            {
                CheckBox cbxItem = listItem.FindControl("cbxItem") as CheckBox;
                if (cbxItem.Checked)
                {
                    HiddenField hdItem = listItem.FindControl("hdItem") as HiddenField;
                    TextBox txtItem = listItem.FindControl("txtItem") as TextBox;
                    int intItemCounts = 0;  //該項目的數量
                    bool hasItemCount = int.TryParse(txtItem.Text, out intItemCounts);
                    if (string.IsNullOrWhiteSpace(txtItem.Text))
                        IsSubsidyItemCountSpace = true;
                    else if (hasItemCount)
                        dicSelectItem.Add(Convert.ToInt32(hdItem.Value), intItemCounts);
                }
            }


            //是否有勾選但未填寫數量，若有則寫入Session
            if (IsSubsidyItemCountSpace)
                CurrentConditions["IsSubsidyItemCountSpace"] = true;
        }

        //計算加總
        if (CurrentConditions.ContainsKey("listComm_Subsidy"))
            listComm_Subsidy = CurrentConditions["listComm_Subsidy"] as List<Comm_Subsidy>;

        int intSum = Comm_Subsidy.SumAmountIsSelected(listComm_Subsidy, dicSelectItem, Catregory);   //總金額
        lbApplyAmount.Text = string.Format("{0:#,##0}元", intSum);
        hdApplyAmount.Value = intSum.ToString();
        if (Catregory == Convert.ToInt32(Subsidy_Catregory.維修費項目) && intSum > 6000)
        {
            Pages.AlertByswal(Tools.altertType.注意.ToString(), "申請補助金額不可超過 6,000元", Tools.altertType.注意.ToDescriptionString());
            lbApplyAmount.Style.Add("Color", "red");
        }
    }
    #endregion

    #region 裝置前行政檢查
    protected void rdbAdminStatus_SelectedIndexChanged(object sender, EventArgs e)
    {
        InitailAdminStatus();
    }

    private void InitailAdminStatus()
    {
        if (rdbAdminStatus.SelectedValue.Equals("退件"))
            ddlAdminRejectSN.Enabled = true;
        else
            ddlAdminRejectSN.Enabled = false;
    }
    protected void ddlAdminRejectSN_SelectedIndexChanged(object sender, EventArgs e)
    {
        string strSN = string.Empty;
        if (!string.IsNullOrEmpty(ddlAdminRejectSN.SelectedValue))
        {
            hdAdminRejectSN.Value = ddlAdminRejectSN.SelectedValue;
        }
        else
            hdAdminRejectSN.Value = strSN;
    }

    private void BindAdminRejectSN()
    {
        var Query = Comm_RejectDesc.GetRejectDescForCase(1);
        ddlAdminRejectSN.Items.Add(new ListItem("請選擇退件原因", ""));
        foreach (var data in Query)
        {
            ddlAdminRejectSN.Items.Add(new ListItem(data.RejectDesc, data.SN.ToString()));
        }
        InitailAdminStatus();
    }
    #endregion

    #region 裝置前專業檢查

    #region 專業審查結果
    protected void rdbProfStatus_SelectedIndexChanged(object sender, EventArgs e)
    {
        InitailProfStatus();
    }

    private void InitailProfStatus()
    {
        if (rdbProfStatus.SelectedValue.Equals("退件"))
            ddlProfRejectSN.Enabled = true;
        else
            ddlProfRejectSN.Enabled = false;
    }

    protected void ddlProfRejectSN_SelectedIndexChanged(object sender, EventArgs e)
    {
        string strSN = string.Empty;
        if (!string.IsNullOrEmpty(ddlProfRejectSN.SelectedValue))
        {
            strSN = ddlProfRejectSN.SelectedValue;
        }
        hdProfRejectSN.Value = strSN;
    }

    private void BindProfRejectSN()
    {
        var Query = Comm_RejectDesc.GetRejectDescForCase(2);
        ddlProfRejectSN.Items.Add(new ListItem("請選擇退件原因", ""));
        foreach (var data in Query)
        {
            ddlProfRejectSN.Items.Add(new ListItem(data.RejectDesc, data.SN.ToString()));
        }
        InitailProfStatus();
    }
    #endregion

    #region 專業審查委員
    protected void ddlProfUser_SelectedIndexChanged(object sender, EventArgs e)
    {
        string strSN = string.Empty;
        if (!string.IsNullOrEmpty(ddlProfUser.SelectedValue))
        {
            strSN = ddlProfUser.SelectedValue;
        }
        hdProfUser.Value = strSN;
    }

    private void BindProfUser()
    {
        var Query = Comm_Examiner.GetAllData();
        ddlProfUser.Items.Add(new ListItem("請選擇委員姓名", ""));
        foreach (var data in Query)
        {
            ddlProfUser.Items.Add(new ListItem(data.Name, data.SN.ToString()));
        }
    }
    #endregion

    #endregion

    #region 特殊個案結案 或 核銷專業審查

    #region 選擇類型
    protected void rdbWriteOffType_SelectedIndexChanged(object sender, EventArgs e)
    {
        IniWriteOffType();
    }

    protected void IniWriteOffType()
    {
        if (rdbWriteOffType.SelectedValue.Equals("特殊個案結案"))
        {
            lblWriteTitle.Text = "特殊個案結案";
            plAudit.Visible = false;
            lblWriteOffStatus.Text = "特殊結案結果";
            rdbWriteOffStatus.Items.Clear();
            rdbWriteOffStatus.Items.Add(new ListItem { Text = "結案", Value = "結案" });
            rdbWriteOffStatus.Items.Add(new ListItem { Text = "特殊部分補助", Value = "特殊部分補助" });
            rdbWriteOffStatus.SelectedValue = "特殊部分補助";
            ddlWriteOffRejectSN.Visible = false;
            hdWriteOffRejectSN.Visible = false;
            //plWriteOffAudit.Visible = false;
            //plWriteOffFinish.Visible = true;
        }
        else
        {
            lblWriteTitle.Text = "核銷專業審查";
            plAudit.Visible = true;
            lblWriteOffStatus.Text = "專業審查結果";
            rdbWriteOffStatus.Items.Clear();
            rdbWriteOffStatus.Items.Add(new ListItem { Text = "通過", Value = "通過" });
            rdbWriteOffStatus.Items.Add(new ListItem { Text = "退件", Value = "退件" });
            rdbWriteOffStatus.SelectedValue = "通過";
            ddlWriteOffRejectSN.Visible = true;
            hdWriteOffRejectSN.Visible = true;
            //plWriteOffAudit.Visible = true;
            //plWriteOffFinish.Visible = false;
        }
    }
    #endregion

    #region 專業審查委員
    protected void ddlWriteOffUser_SelectedIndexChanged(object sender, EventArgs e)
    {
        string strSN = string.Empty;
        if (!string.IsNullOrEmpty(ddlWriteOffUser.SelectedValue))
        {
            strSN = ddlWriteOffUser.SelectedValue;
        }
        hdWriteOffUser.Value = strSN;
    }

    private void BindWriteOffUser()
    {
        var Query = Comm_Examiner.GetAllData();
        ddlWriteOffUser.Items.Add(new ListItem("請選擇委員姓名", ""));
        foreach (var data in Query)
        {
            ddlWriteOffUser.Items.Add(new ListItem(data.Name, data.SN.ToString()));
        }
    }
    #endregion

    #region 特殊結案結果 or 專業審查結果
    protected void rdbWriteOffStatus_SelectedIndexChanged(object sender, EventArgs e)
    {
        InitailWriteOffStatusAudit();
    }

    private void InitailWriteOffStatusAudit()
    {
        if (rdbWriteOffStatus.SelectedValue.Equals("退件"))
            ddlWriteOffRejectSN.Enabled = true;
        else
            ddlWriteOffRejectSN.Enabled = false;
    }

    protected void ddlWriteOffRejectSN_SelectedIndexChanged(object sender, EventArgs e)
    {
        string strSN = string.Empty;
        if (!string.IsNullOrEmpty(ddlWriteOffRejectSN.SelectedValue))
        {
            strSN = ddlWriteOffRejectSN.SelectedValue;
        }
        hdWriteOffRejectSN.Value = strSN;
    }

    private void BindWriteOffRejectSN()
    {
        var Query = Comm_RejectDesc.GetRejectDescForCase(3);
        ddlWriteOffRejectSN.Items.Add(new ListItem("請選擇退件原因", ""));
        foreach (var data in Query)
        {
            ddlWriteOffRejectSN.Items.Add(new ListItem(data.RejectDesc, data.SN.ToString()));
        }
        InitailProfStatus();
    }
    #endregion

    #endregion

    #endregion

    #region ENUM

    public enum Subsidy_Catregory  //補助類型
    {
        診治項目 = 1,
        維修費項目 = 2
    }

    #endregion


    protected void btnFileUpload_Click(object sender, EventArgs e)
    {
        string strError = "";

        #region 確認資料夾
        string tempNo = "";//GUID為暫存資料夾名
        if (CurrentConditions.ContainsKey("TempUploadNo"))
            tempNo = CurrentConditions["TempUploadNo"].ToString();
        else
        {
            tempNo = Guid.NewGuid().ToString();
            CurrentConditions["TempUploadNo"] = tempNo;
        }
        //檔案位置
        string savePath = Server.MapPath($"/FilePool/{tempNo}");
        if (!Directory.Exists(savePath))
            Directory.CreateDirectory(savePath);
        #endregion

        //如果user有選擇檔案
        if (fuFileUpload.HasFile)
        {
            //儲存檔案
            string fileName = "";
            string filePath = "";
            string fileGuid = "";
            foreach (HttpPostedFile file in fuFileUpload.PostedFiles)
            {
                //遍巡所有上傳檔案
                fileName = file.FileName;
                fileGuid = Guid.NewGuid().ToString();
                filePath = $"{savePath}/{fileGuid}.h";
                if (!File.Exists(filePath))
                {
                    //資料夾及資料庫皆無此檔名才新增
                    fuFileUpload.SaveAs(filePath);
                    #region 儲存到db.RelFile
                    Comm_RelFile insert = new Comm_RelFile();
                    insert.NodeID = 13;
                    insert.ParentSN = tempNo;
                    insert.FileName = $"/FilePool/{tempNo}/{fileGuid}.h";
                    insert.SrcFileName = fileName;
                    insert.ModifyDate = DateTime.Now;
                    insert.CaseNo = tempNo;   //暫存時先將CaseNo設為Guid，新增後再更新(方便getdata顯示資料用)
                    Comm_RelFile.Insert(insert);
                    #endregion
                }
                else
                    strError += $"{fileName} ";
            }

            //判斷是否有誤並顯示訊息
            if (!string.IsNullOrEmpty(strError))
            {
                Pages.AlertByswal(Tools.altertType.錯誤.ToString(), $"檔案新增失敗，檔名重複：\n{strError}", Tools.altertType.錯誤.ToDescriptionString());
            }
            else
                Pages.AlertByswal(Tools.altertType.成功.ToString(), "上傳成功", Tools.altertType.成功.ToDescriptionString());
        }
        else Pages.AlertByswal(Tools.altertType.錯誤.ToString(), "請選擇檔案", Tools.altertType.錯誤.ToDescriptionString());


        #region 附件資料
        if (!string.IsNullOrEmpty(savePath))
        {
            //檔案位置
            if (Directory.Exists(savePath))
            {
                List<Comm_RelFile> listCf = Comm_RelFile.GetAllData(tempNo);
                StringBuilder sbNames = new StringBuilder();
                if (listCf.Count > 0)
                {
                    foreach (var data in listCf)
                    {
                        sbNames.Append($"<a href='{$"/Common/DownFile.ashx?s={data.SN}"}'>{data.SrcFileName}</a><br>");
                    }
                }
                lblFileUpload.Text = sbNames.ToString();
            }
            
        }
        #endregion

        //DownloadFile();
    }

    private void DownloadFile()
    {
        // HttpContext context= this.Context;
        //string Extension=Path.GetExtension("");//取得副檔名
        string name = "todo.txt";
        HttpContext.Current.Response.Buffer = true;
        HttpContext.Current.Response.Clear();
        HttpContext.Current.Response.ContentType = "application/download";
        HttpContext.Current.Response.AddHeader("Content-Disposition", "attachment;   filename=" + HttpUtility.UrlEncode(name, System.Text.Encoding.UTF8) + ";");
        HttpContext.Current.Response.BinaryWrite(File.ReadAllBytes(HttpContext.Current.Server.MapPath(string.Format(@"..\..\FilePool\110-10-002-002\{0}", name))));
        HttpContext.Current.Response.Flush();
        HttpContext.Current.Response.End();
    }

    protected void btnAudit_Click(object sender, EventArgs e)
    {
        var PID = txtPID.Text;
        if (!PID.isIdentificationId() && !PID.CheckForeignIdNumber())
            Pages.AlertByswal(Tools.altertType.錯誤.ToString(), "請輸入正確的身分證字號", Tools.altertType.錯誤.ToDescriptionString());
        else if (!string.IsNullOrEmpty(AuditPID(Tools.EncryptAES(jSecurity.XSS(PID)))))
            Pages.AlertByswal(Tools.altertType.注意.ToString(), (AuditPID(Tools.EncryptAES(jSecurity.XSS(PID)))), Tools.altertType.注意.ToDescriptionString());
        else
            Pages.AlertByswal(Tools.altertType.資訊.ToString(), "身分證字號檢核通過", Tools.altertType.資訊.ToDescriptionString());
    }

    protected string AuditPID(string PID)
    {
        //身分證字號檢核：判斷是否申請過，若有則另外判斷條件
        string rtn = string.Empty;
        Comm_Case data = Comm_Case.GetSingle(x => x.PID.Equals(PID));
        if (data != null)
        {
            string auditPID = Comm_Case.AuditPID(PID);
            if (!string.IsNullOrEmpty(auditPID))
                rtn = auditPID;
        }
        return rtn;
    }



}