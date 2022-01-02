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

public partial class System_Satisfaction_Detail : BasePage
{
    string state = string.Empty;
    string id = string.Empty;


    protected void Page_Load(object sender, EventArgs e)
    {
        if (CurrentConditions.ContainsKey("Action"))
            state = ("Edit".Equals((CurrentConditions["Action"]).ToString())) ? "update" : "read";
        else Response.Redirect(GoQuery(default));

        id = CurrentConditions["ID"].ToString();
        Panel1.Enabled = false;
        if (!IsPostBack)
        {
            //設定上一頁的網址
            this.btnGoBack.PostBackUrl = GoQuery(default);
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
                    Comm_Case date = vw_Installationcomplete.GetCase(id);
                    Satisfaction QuestionAnsdate = vw_Installationcomplete.GetSatisfaction(id);
                    vw_Installationcomplete vw_installationcomplete = vw_Installationcomplete.GetInstallationcomplete(id);
                    if (date != null)
                    {
                        #region 顯示修改資料
                        txtBeforDate.Text = string.Format("{0:yyyy-MM-dd}", date.BeforDate);//裝置前收件日期
                        txtCaseNo.Text = date.CaseNo;//案件編號
                        txtName.Text = date.Name;//個案姓名
                        txtGender.Text = date.Gender;//性別
                        txtBirthday.Text = string.Format("{0:yyy-MM-dd}", date.Birthday);//出生日期
                        txtPID.Text = date.PID;//身分證字號
                        txtTel.Text = date.Tel;//電話
                        txtUserType.Text = date.UserType;//身分別
                        txtAddr.Text = date.Addr;//地址
                        txtDeptSN.Text = vw_installationcomplete.DeptName;//合約院所
                        txtDoctorSN.Text = vw_installationcomplete.DoctorName;//負責醫生
                        txtCatregory.Text = vw_installationcomplete.申請項目;//裝置項目
                        txtApplyAmount.Text = date.WriteOffAmout.ToString();//申請費用
                        txtProfDate.Text = string.Format("{0:yyy-MM-dd}", date.ProfDate);//裝置前審查委員會日期
                        txtWriteOffProDate.Text = string.Format("{0:yyy-MM-dd}", date.WriteOffProDate);//裝置後審查委員會日期
                        if (QuestionAnsdate != null)
                        {
                            string[] CheckBoxList_Split = QuestionAnsdate.QuestionAns1.Split(',');//取出資料並以，切割
                            foreach (var item in CheckBoxList_Split)
                            {
                                ListItem listItem = cblQuestionAns1.Items.FindByValue(item);//1.請問您從何處知道本項補助計畫?
                                if (listItem != null)
                                    listItem.Selected = true;
                            }
                            txbQuestionAns1Other.Text = QuestionAnsdate.QuestionAns1Other;
                            rdlQuestionAns2.SelectedValue = QuestionAnsdate.QuestionAns2;//2.請問目前您假牙裝置後有無使用？
                            rdlQuestionAns3.SelectedValue = QuestionAnsdate.QuestionAns3;//3.請問您對假牙裝置後是否舒適？
                            if (rdlQuestionAns3.SelectedValue.Contains("不舒適"))
                            {
                                tr_QuestionAns4.Visible = true;
                                idRadioButtonList2.Visible = true;
                                tr_QuestionAns5.Visible = true;
                                idRadioButtonList3.Visible = true;
                            }
                            rblQuestionAns4.SelectedValue = QuestionAnsdate.QuestionAns4;//3-1.承上題，目前您裝置後不舒適回診狀況？
                            if (QuestionAnsdate.QuestionAns5 == "疾病因素無法前往")
                                ddlQuestionAns5.SelectedValue = QuestionAnsdate.QuestionAns5Otherfactor;//疾病因素無法前往原因(下拉選單)
                            rblQuestionAns5.SelectedValue = QuestionAnsdate.QuestionAns5;//3-2.承上題，目前您裝置後不舒適回診狀況？
                            rblQuestionAns6.SelectedValue = QuestionAnsdate.QuestionAns6;//4.您對縣長重要施政「65歲以上長者及55歲以上原住民裝置假牙補助計畫」滿意嗎？
                            if (rblQuestionAns6.SelectedValue.Contains("不滿意"))
                            {
                                tr_QuestionAns7.Visible = true;
                                idRadioButtonList5.Visible = true;
                            }
                            rblQuestionAns7.SelectedValue = QuestionAnsdate.QuestionAns7;//4-1.承上題，請問您不滿意的原因是？
                            txtMemo.Text = QuestionAnsdate.Memo;//5.其他建議事項？
                            txtRemark.Text = QuestionAnsdate.Remark;//6.備註
                        }
                        #endregion
                    }
                    break;
            }
        }
    }


    private string GoQuery(string Turnto)
    {
        switch (Turnto)
        {
            case "不舒適":
            case "非常不舒適":
                return "/System/SatisfactionDis/Query.aspx?n=17";
            case "舒適":
            case "疾病因素無法前往":
                return "/System/SatisfactionHis/Query.aspx?n=19";
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
        //請問您從何處知道本項補助計畫(多選) 分號寫入
        var result = string.Join(",", this.cblQuestionAns1.Items.Cast<ListItem>().Where(x => x.Selected).Select(x => x.Value).ToArray<string>());
        QuestionAnsdate.QuestionAns1 = result;//1.請問您從何處知道本項補助計畫
        if (result.Contains("其他"))
            QuestionAnsdate.QuestionAns1Other = txbQuestionAns1Other.Text;
        else
            QuestionAnsdate.QuestionAns1Other = string.Empty;

        QuestionAnsdate.QuestionAns2 = rdlQuestionAns2.SelectedValue;//2.請問目前您假牙裝置後有無使用？
        QuestionAnsdate.QuestionAns3 = rdlQuestionAns3.SelectedValue;//3.請問您對假牙裝置後是否舒適？

        if (rdlQuestionAns3.SelectedValue.Contains("不舒適"))
        {
            QuestionAnsdate.QuestionAns4 = rblQuestionAns4.SelectedValue;//承上題，請問您裝置後不舒適的原因是？
            QuestionAnsdate.QuestionAns5 = rblQuestionAns5.SelectedValue;//承上題，目前您裝置後不舒適回診狀況？
            QuestionAnsdate.Status = "不滿意追蹤";
            if (rblQuestionAns5.SelectedValue == "疾病因素無法前往")
            {
                QuestionAnsdate.QuestionAns5Otherfactor = ddlQuestionAns5.SelectedValue;//疾病因素無法前往
                QuestionAnsdate.Status = "結案";
            }
        }
        else
        {
            QuestionAnsdate.QuestionAns4 = string.Empty;//承上題，請問您裝置後不舒適的原因是？
            QuestionAnsdate.QuestionAns5 = string.Empty;//承上題，目前您裝置後不舒適回診狀況？
            QuestionAnsdate.QuestionAns5Otherfactor = string.Empty;//疾病因素無法前往
            QuestionAnsdate.Status = "結案";
        }
        QuestionAnsdate.QuestionAns6 = rblQuestionAns6.SelectedValue;//4.您對縣長重要施政「65歲以上長者及55歲以上原住民裝置假牙補助計畫」滿意嗎？
        if (rblQuestionAns6.SelectedValue.Contains("不滿意"))
        {
            QuestionAnsdate.QuestionAns7 = rblQuestionAns7.SelectedValue;//承上題，請問您不滿意的原因是？
        }
        else
        {
            QuestionAnsdate.QuestionAns7 = string.Empty;//承上題，請問您不滿意的原因是？
        }
        QuestionAnsdate.Memo = txtMemo.Text;//請問您對於假牙裝置補助是否有其他建議事項？
        QuestionAnsdate.Remark = txtRemark.Text;//備註：

        #region  資料更新
        if (state == "update")
        {
            if (Num != null)//查有無有相同編號的調查
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
        if (rblQuestionAns5.SelectedValue == "疾病因素無法前往")//直接結案
            Pages.AlertByswal(Tools.altertType.成功.ToString(), "修改成功", Tools.altertType.成功.ToDescriptionString(), GoQuery(rblQuestionAns5.SelectedValue));
        else if (rdlQuestionAns3.SelectedValue != null)
            Pages.AlertByswal(Tools.altertType.成功.ToString(), "修改成功", Tools.altertType.成功.ToDescriptionString(), GoQuery(rdlQuestionAns3.SelectedValue));
        else
            Response.Redirect("Query.aspx?n=" + jSecurity.GetQueryString("n"));
    }

    #region 資料檢查
    private string CheckData()
    {
        StringBuilder sbError = new StringBuilder();
        if (cblQuestionAns1.SelectedItem == null)
        {
            sbError.Append(@"您從何處知道本項補助計畫選項錯誤\n");
        }
        if (rdlQuestionAns2.SelectedItem == null)
        {
            sbError.Append(@"假牙裝置後有無使用錯誤\n");
        }
        if (rblQuestionAns5.SelectedValue == "疾病因素無法前往")
        {
            if (ddlQuestionAns5.SelectedIndex == 0)
            {
                sbError.Append(@"疾病因素無法前往選項錯誤\n");
            }
        }
        if (rblQuestionAns5.SelectedValue != "疾病因素無法前往" && ddlQuestionAns5.SelectedIndex != 0)
        {
            sbError.Append(@"疾病因素無法前往選項未勾選\n");
        }
        if (rblQuestionAns6.SelectedItem == null)
        {
            sbError.Append(@"對縣長重要施政「65歲以上長者及55歲以上原住民裝置假牙補助計畫」滿意嗎?選項錯誤\n");
        }
        if (txbQuestionAns1Other.Text.Length > 100)
        {
            sbError.Append(@"字數超過設定\n");
        }

        return sbError.ToString();
    }
    #endregion

    protected void rdlQuestionAns3_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (rdlQuestionAns3.SelectedValue.Contains("不舒適"))
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
    protected void rblQuestionAns6_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (rblQuestionAns6.SelectedValue.Contains("不滿意"))
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


}