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
using System.Drawing;

public partial class System_SatisfactionDis_Detail : BasePage
{
    string state = string.Empty;
    string id = string.Empty;


    protected void Page_Load(object sender, EventArgs e)
    {
        if (CurrentConditions.ContainsKey("Action"))
            state = ("Edit".Equals((CurrentConditions["Action"]).ToString())) ? "update" : "read";
        else Response.Redirect(GoQuery(-1));
        id = CurrentConditions["ID"].ToString();
        Panel1.Enabled = false;
        Panel2.Enabled = false;
        Asterisk.Visible = false;

        if (!IsPostBack)
        {
            //設定上一頁的網址
            this.btnGoBack.PostBackUrl = GoQuery(-1);
            this.btnCancel.PostBackUrl = this.btnGoBack.PostBackUrl;
            this.btnSave.CommandArgument = this.btnGoBack.PostBackUrl;
            Literal litNav = Page.Master.FindControl("litNav") as Literal;

            tr_QuestionAns4.Visible = false;//承上題，請問您裝置後不舒適的原因是
            idRadioButtonList2.Visible = false;//RadioButtonList
            tr_QuestionAns5.Visible = false;//承上題，目前您裝置後不舒適回診狀況？
            idRadioButtonList3.Visible = false;//RadioButtonList
            tr_QuestionAns7.Visible = false;//承上題，請問您不滿意的原因是？
            idRadioButtonList5.Visible = false;//RadioButtonList
            switch (state)
            {
                case "insert":
                    litNav.Text += " / 新增資料";
                    break;
                case "update":
                case "read":
                    litNav.Text += ("update".Equals(state)) ? " / 修改資料" : " / 檢視資料";
                    Comm_Case date = vw_Dissatisfied.GetCase(id);
                    Satisfaction QuestionAnsdate = vw_Dissatisfied.GetSatisfaction(id);
                    vw_Dissatisfied vw_dissatisfied = vw_Dissatisfied.GetInstallationcomplete(id);

                    if (date != null)
                    {
                        Track(QuestionAnsdate.QuestionAns5);
                        #region 顯示修改資料
                        txtBeforDate.Text = string.Format("{0:yyy-MM-dd}", date.BeforDate); ;//裝置前收件日期
                        txtCaseNo.Text = date.CaseNo;//案件編號
                        txtName.Text = date.Name;//個案姓名
                        txtGender.Text = date.Gender;//性別
                        txtBirthday.Text = string.Format("{0:yyy-MM-dd}", date.Birthday);//出生日期
                        txtPID.Text = date.PID;//身分證字號
                        txtTel.Text = date.Tel;//電話
                        txtUserType.Text = date.UserType;//身分別
                        txtAddr.Text = date.Addr;//地址
                        txtDeptSN.Text = vw_dissatisfied.DeptName;//合約院所
                        txtDoctorSN.Text = vw_dissatisfied.Expr1;//負責醫生
                        txtCatregory.Text = vw_dissatisfied.申請項目;//裝置項目
                        txtApplyAmount.Text = date.WriteOffAmout.ToString();//申請費用
                        txtProfDate.Text = string.Format("{0:yyy-MM-dd}", date.ProfDate);//裝置前審查委員會日期
                        txtWriteOffProDate.Text = string.Format("{0:yyy-MM-dd}", date.WriteOffProDate);//裝置後審查委員會日期
                        if (QuestionAnsdate != null)
                        {
                            string[] CheckBoxList_Split = QuestionAnsdate.QuestionAns1.Split(',');//取出資料並以，切割
                            foreach (var item in CheckBoxList_Split)
                            {
                                ListItem listItem = cblQuestionAns1.Items.FindByValue(item);//請問您從何處知道本項補助計畫?
                                if (listItem != null)
                                    listItem.Selected = true;
                            }
                            txbQuestionAns1Other.Text = QuestionAnsdate.QuestionAns1Other;
                            rdlQuestionAns2.SelectedValue = QuestionAnsdate.QuestionAns2;//請問目前您假牙裝置後有無使用？
                            rdlQuestionAns3.SelectedValue = QuestionAnsdate.QuestionAns3;//請問您對假牙裝置後是否舒適？
                            if (rdlQuestionAns3.SelectedValue.Contains("不舒適"))
                            {
                                tr_QuestionAns4.Visible = true;
                                idRadioButtonList2.Visible = true;
                                tr_QuestionAns5.Visible = true;
                                idRadioButtonList3.Visible = true;
                            }
                            rdlQuestionAns4.SelectedValue = QuestionAnsdate.QuestionAns4;//承上題，目前您裝置後不舒適回診狀況？
                            if (QuestionAnsdate.QuestionAns5 == "疾病因素無法前往")
                                ddlQuestionAns5.SelectedValue = QuestionAnsdate.QuestionAns5Otherfactor;//疾病因素無法前往原因(下拉選單)

                            rdlQuestionAns5.SelectedValue = QuestionAnsdate.QuestionAns5;//3-2.承上題，目前您裝置後不舒適回診狀況？

                            rdlQuestionAns6.SelectedValue = QuestionAnsdate.QuestionAns6;//您對縣長重要施政「65歲以上長者及55歲以上原住民裝置假牙補助計畫」滿意嗎？
                            if (rdlQuestionAns6.SelectedValue.Contains("不滿意"))
                            {
                                tr_QuestionAns7.Visible = true;
                                idRadioButtonList5.Visible = true;
                            }
                            rdlQuestionAns7.SelectedValue = QuestionAnsdate.QuestionAns7;//承上題，請問您不滿意的原因是？
                            txtMemo.Text = QuestionAnsdate.Memo;//其他建議事項？
                            rblDissatistrackoptions.SelectedValue = QuestionAnsdate.DissatisfiedReason;//不滿意後續追蹤
                            cblHealthcenter.SelectedValue = QuestionAnsdate.HealthCenter;//個案轉衛生所追蹤
                            txtRemark.Text = QuestionAnsdate.Remark;//備註
                        }
                        #endregion
                    }
                    break;
            }
        }
    }


    private string GoQuery(int Turnto)
    {
        switch (Turnto)
        {
            case 0://問題已改善
                return "/System/SatisfactionHis/Query.aspx?n=19";
            case 1://多次回診未改善(共有)or沒在使用(拒絕回診)
            case 2://未回診(其他)
            case 3://沒在使用(其他)
            case 4://未接(2次以上)(其他)
                return "/System/SatisfactionDis/Query.aspx?n=17";
            default:
                return "Query.aspx?n=" + jSecurity.GetQueryString("n");
        }
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
        var Num = Satisfaction.GetSingle(x => x.CaseNo == id);

        Satisfaction QuestionAnsdate = new Satisfaction();


        QuestionAnsdate.CaseNo = id;
        //多選
        var CheckBoxList_QuestionAns1 = string.Empty;
        for (int i = 0; i < cblQuestionAns1.Items.Count; i++)
        {
            if (cblQuestionAns1.Items[i].Selected)
            {
                if (CheckBoxList_QuestionAns1 == string.Empty)
                    CheckBoxList_QuestionAns1 = cblQuestionAns1.Items[i].Value;
                else
                    CheckBoxList_QuestionAns1 += "," + cblQuestionAns1.Items[i].Value;

            }
        }

        if (CheckBoxList_QuestionAns1.Contains("其他"))
            QuestionAnsdate.QuestionAns1Other = txbQuestionAns1Other.Text;
        else
            QuestionAnsdate.QuestionAns1Other = string.Empty;

        QuestionAnsdate.QuestionAns1 = CheckBoxList_QuestionAns1;//1.請問您從何處知道本項補助計畫
        QuestionAnsdate.QuestionAns2 = rdlQuestionAns2.SelectedValue;//2.請問目前您假牙裝置後有無使用？
        QuestionAnsdate.QuestionAns3 = rdlQuestionAns3.SelectedValue;//3.請問您對假牙裝置後是否舒適？

        if (rdlQuestionAns3.SelectedValue.Contains("不舒適"))
        {
            QuestionAnsdate.QuestionAns4 = rdlQuestionAns4.SelectedValue;//承上題，請問您裝置後不舒適的原因是？
            QuestionAnsdate.QuestionAns5 = rdlQuestionAns5.SelectedValue;//承上題，目前您裝置後不舒適回診狀況？

            if (rdlQuestionAns5.SelectedValue == "疾病因素無法前往")
            {
                QuestionAnsdate.QuestionAns5Otherfactor = ddlQuestionAns5.SelectedValue;
            }
        }
        else
        {
            QuestionAnsdate.QuestionAns4 = string.Empty;//承上題，請問您裝置後不舒適的原因是？
            QuestionAnsdate.QuestionAns5 = string.Empty;//承上題，目前您裝置後不舒適回診狀況？
            QuestionAnsdate.QuestionAns5Otherfactor = string.Empty;
        }
        QuestionAnsdate.QuestionAns6 = rdlQuestionAns6.SelectedValue;//4.您對縣長重要施政「65歲以上長者及55歲以上原住民裝置假牙補助計畫」滿意嗎？
        if (rdlQuestionAns6.SelectedValue.Contains("不滿意"))
        {
            QuestionAnsdate.QuestionAns7 = rdlQuestionAns7.SelectedValue;//承上題，請問您不滿意的原因是？
        }
        else
        {
            QuestionAnsdate.QuestionAns7 = string.Empty;//承上題，請問您不滿意的原因是？
        }
        QuestionAnsdate.Memo = txtMemo.Text;
        QuestionAnsdate.DissatisfiedReason = rblDissatistrackoptions.SelectedValue;//不滿意後續追蹤
        if (rblDissatistrackoptions.SelectedValue.Contains("問題已改善"))
            QuestionAnsdate.Status = "結案";
        else
            QuestionAnsdate.Status = "不滿意追蹤";

        QuestionAnsdate.HealthCenter = cblHealthcenter.SelectedValue;//個案轉衛生所追蹤
        QuestionAnsdate.Memo = txtMemo.Text;//請問您對於假牙裝置補助是否有其他建議事項？
        QuestionAnsdate.Remark = txtRemark.Text;//備註：


        #region  資料更新
        if (state == "update")
        {
            if (Num != null)
            {
                QuestionAnsdate.ModifyID = SessionCenter.AccUser.ID;
                QuestionAnsdate.ModifyDate = DateTime.Now;
                Satisfaction.Update(QuestionAnsdate);
            }
            else
            {
                QuestionAnsdate.CreateID = SessionCenter.AccUser.ID;
                QuestionAnsdate.CreateDate = DateTime.Now;
                Satisfaction.Insert(QuestionAnsdate);
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
        if (id == string.Empty)
            rec.Record = fun + id + "帳號資料";
        Comm_Record.Insert(rec);
        #endregion

        //導回上一頁
        if (rdlQuestionAns2.SelectedValue != null && cblHealthcenter.SelectedIndex == 0)//直接轉至衛生所追蹤
            Pages.AlertByswal(Tools.altertType.成功.ToString(), "修改成功", Tools.altertType.成功.ToDescriptionString(), "/System/Satisfactionhealth/Query.aspx?n=18");
        else if (rdlQuestionAns2.SelectedValue != null)
            Pages.AlertByswal(Tools.altertType.成功.ToString(), "修改成功", Tools.altertType.成功.ToDescriptionString(), GoQuery(rblDissatistrackoptions.SelectedIndex));
        else
            Response.Redirect("Query.aspx?n=" + jSecurity.GetQueryString("n"));

    }

    #region 資料檢查
    private string CheckData()
    {
        StringBuilder sbError = new StringBuilder();
        if (rblDissatistrackoptions.SelectedItem == null)
        {
            sbError.Append(@"請選擇不舒適回診狀況？\n");
        }
        if (rblDissatistrackoptions.SelectedIndex == 0 && cblHealthcenter.SelectedIndex == 0)
        {
            sbError.Append(@"問題已改善與個案轉衛生所追蹤重複選擇\n");
        }
        return sbError.ToString();
    }
    #endregion

    protected void rdlQuestionAns3_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (rdlQuestionAns2.SelectedValue.Contains("不舒適"))
        {
            tr_QuestionAns4.Visible = true;
            idRadioButtonList2.Visible = true;
            tr_QuestionAns5.Visible = true;
            idRadioButtonList3.Visible = true;
        }
        else
        {
            tr_QuestionAns4.Visible = false;
            idRadioButtonList2.Visible = false;
            tr_QuestionAns5.Visible = false;
            idRadioButtonList3.Visible = false;

        }
    }
    protected void rdlQuestionAns6_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (rdlQuestionAns6.SelectedValue.Contains("不滿意"))
        {
            tr_QuestionAns7.Visible = true;
            idRadioButtonList5.Visible = true;
        }
        else
        {
            tr_QuestionAns7.Visible = false;
            idRadioButtonList5.Visible = false;
        }
    }

    private void Track(string situation)//不滿意後續追蹤
    {
        switch (situation)
        {
            case "持續回診":
                Asterisk.Visible = true;
                labDissatistracktopic.Text = "目前您裝置後不舒適回診狀況？(持續回診)";
                rblDissatistrackoptions.Items.Add("問題已改善");
                rblDissatistrackoptions.Items.Add("多次回診未改善");
                rblDissatistrackoptions.Items.Add("未回診");
                break;
            case "有空再去":
                Asterisk.Visible = true;
                labDissatistracktopic.Text = "目前您裝置後不舒適回診狀況？(有空再去)";
                rblDissatistrackoptions.Items.Add("問題已改善");
                rblDissatistrackoptions.Items.Add("多次回診未改善");
                rblDissatistrackoptions.Items.Add("未回診");
                break;
            case "拒絕回診":
                Asterisk.Visible = true;
                labDissatistracktopic.Text = "目前您裝置後不舒適回診狀況？(拒絕回診)";
                rblDissatistrackoptions.Items.Add("問題已改善");
                rblDissatistrackoptions.Items.Add("沒在使用");
                rblDissatistrackoptions.Items.Add("未回診");
                break;
            case "其他":
                Asterisk.Visible = true;
                labDissatistracktopic.Text = "目前您裝置後不舒適回診狀況？(其他)";
                rblDissatistrackoptions.Items.Add("問題已改善");
                rblDissatistrackoptions.Items.Add("多次回診未改善");
                rblDissatistrackoptions.Items.Add("未回診");
                rblDissatistrackoptions.Items.Add("沒在使用");
                rblDissatistrackoptions.Items.Add("未接(2次以上)");
                break;
            default:
                Asterisk.Visible = false;
                labDissatistrac.Visible = false;
                tr_Dissatistracktopic.Visible = false;
                tr_Dissatistrackoptions.Visible = false;
                break;
        }
    }


}