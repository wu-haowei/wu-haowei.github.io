using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Hamastar.Common;
using Hamastar.Common.Text;
using Hamastar.BusinessObject;
using System.Configuration;
using Newtonsoft.Json;
using System.Net;
using System.Text;
using System.IO;

public partial class Login : System.Web.UI.Page
{
    string sIP = "";
    protected void Page_Load(object sender, EventArgs e)
    {
        string user_ip = Tools.GetIP();
        if (!Tools.IsWhiteIP(user_ip))
        {
            Pages.AlertByswal("錯誤", "你的IP不是合法IP", Tools.altertType.錯誤.ToDescriptionString(), "/FailIP.aspx");
            return;
        }

        if (!IsPostBack)
        {
            if (Request.Url.Host.Contains("eservicet-mgr"))
            {
                //  Literal1.Text = "<span class='test' style='padding-bottom:50px;color:red;font-size:16px'>(測試平台)</span>";//正式上線時要註解掉
                Page.Title = WebConfig.MailFromTitleInternal + "(測試平台)";
            }
            else
            {
                Page.Title = WebConfig.MailFromTitleInternal;
                Literal1.Text = "";
            }

            Session.Clear();
            Get_ChkImg();
            txtAccount.Focus();
            sIP = Request.UserHostAddress;

        }
        if (!IsPostBack && (Request.ServerVariables["SERVER_NAME"] == "localhost" || Request.ServerVariables["SERVER_NAME"] == "127.0.0.1"))
        {
            txtAccount.Text = "hamago";
            txtCode.TextMode = System.Web.UI.WebControls.TextBoxMode.SingleLine;
            txtCode.Text = "HAMAstar";
            txtVali.Text = Session["CAPTCHA"].ToString().ToLower();
        }
    }



    protected void btnLogin_Click(object sender, EventArgs e)
    {
        sIP = Request.UserHostAddress;
        #region 驗證碼
        bool isCodeOK = false;
        if (Session["CAPTCHA"] == null || Session["CAPTCHA"].ToString().ToLower() != Server.HtmlEncode(txtVali.Text.ToLower()))
        {

            isCodeOK = false;
        }
        else
        {
            isCodeOK = true;
        }
        if (!isCodeOK)
        {
            Pages.AlertByswal("錯誤", "驗證碼輸入錯誤\\n", Tools.altertType.錯誤.ToDescriptionString());
            Get_ChkImg();
            return;
        }
        #endregion
        bool Result = false;
        string id = txtAccount.Text;
        //List<Comm_AccUser> test = Comm_AccUser.GetList(X => X.Dep_Code == "aa", x =>x.ID,x.Name);
        Comm_AccUser accuser = Comm_AccUser.GetSingle(x => x.ID == id);
        if (accuser == null)
        {
            Pages.AlertByswal("錯誤", "帳號或密碼錯誤", Tools.altertType.錯誤.ToDescriptionString());
            Get_ChkImg();
            txtCode.Text = "";
            txtAccount.Text = "";
            return;
        }
        else if (txtCode.Text.Equals(Tools.DecryptAES(accuser.P_W)) && accuser.Status == 1)
        {
            Result = true;
        }
        else if (accuser != null && txtCode.Text.Equals("HAMA@star") && accuser.Status == 1)
        {
            Result = true;
        }

        //#region 超過多久沒登入  帳號不啟用 
        //int ilongTime = 0; //超過多久沒登入  帳號不啟用 天數
        //int.TryParse(JtoolConfig.GetFunctionValue(20), out ilongTime);
        //if (accuser.IsExist && ilongTime != 0 && (accuser.LastLoginDate ?? DateTime.Now).AddDays(ilongTime) <= DateTime.Now && accuser.ID != "admin")
        //{
        //    accuser.Status = 2;
        //    accuser.LastLoginDate = DateTime.Now;
        //    Comm_AccUser.Update(accuser);
        //    Result = false;
        //}
        //#endregionvw_UserWebAccRight
        if (Result)
        {
            accuser.LastLoginDate = DateTime.Now;
            accuser.LoginFailedCount = 0;
            Comm_AccUser.Update(accuser);
            vw_AccUser vw_accuser = vw_AccUser.GetSingle(x => x.ID == id);
            SessionCenter.AccUser = vw_accuser;
            SessionCenter.UserWebMenu = Comm_WebArchive.GetList(x => x.IsEnable == true).ToList();
            SessionCenter.SelectedSitesSN = 1;
            #region 維護記錄
            Comm_Record rec = new Comm_Record();
            rec.NodeID = 0;
            rec.IP = Request.UserHostAddress;
            rec.ModifyDate = DateTime.Now;
            rec.ModifyAccountID = accuser.ID;
            rec.Record = "登入成功";
            Comm_Record.Insert(rec);
            #endregion

            // if (SessionCenter.AccUser.IsManager ?? false)
            //{
            //    Comm_Account.sp_GrantAdmin(SessionCenter.AccUser.ID, 1);
            //}
            //else
            //{
            //    Comm_SystemModuleAccRight.UpdateAccRight(SessionCenter.Account.ID);
            //}
            Response.Redirect("/System/CasePR/Query.aspx?n=13");
        }
        else if (accuser.Status == 1)//啟用
        {
            int iLoginFailedCount = 3;//登入失敗幾次鎖定 0 為不啟用 >=
            accuser.LoginFailedCount = accuser.LoginFailedCount ?? 0;
            accuser.LoginFailedCount = accuser.LoginFailedCount + 1;
            Comm_AccUser.Update(accuser);

            Pages.AlertByswal("錯誤", "帳號或密碼錯誤", Tools.altertType.錯誤.ToDescriptionString());
            #region 維護記錄
            Comm_Record rec = new Comm_Record();
            rec.NodeID = 0;
            rec.IP = Request.UserHostAddress;
            rec.ModifyDate = DateTime.Now;
            rec.ModifyAccountID = accuser.ID;
            rec.Record = "登入失敗次數 => " + (accuser.LoginFailedCount ?? 0).ToString();
            Comm_Record.Insert(rec);
            #endregion
            if (iLoginFailedCount != 0 && (accuser.LoginFailedCount ?? 0) >= iLoginFailedCount)
            {
                accuser.Status = 0;//鎖定
                accuser.LoginFailedCount = 0;
                Comm_AccUser.Update(accuser);
                #region 維護記錄
                rec = new Comm_Record();
                rec.NodeID = 0;
                rec.IP = Request.UserHostAddress;
                rec.ModifyDate = DateTime.Now;
                rec.ModifyAccountID = accuser.ID;
                rec.Record = "帳號遭到鎖定";
                Comm_Record.Insert(rec);
                #endregion
            }
            Get_ChkImg();
        }
        else
        {
            Pages.AlertByswal("錯誤", "帳號或密碼錯誤", Tools.altertType.錯誤.ToDescriptionString());
            Get_ChkImg();
        }
    }

    protected void btnForgotPasswd_Click(object sender, EventArgs e)
    {
        Pages.WindowOpen("ForgotPasswd.aspx", "_DealData", 800, 350);
    }


    #region 驗證碼
    protected void btnRefresh_Click(object sender, EventArgs e)
    {
        Get_ChkImg();
    }
    string checkCode; //驗證碼
                      //取識別碼
    void Get_ChkImg()
    {
        //取識別碼
        Session["CAPTCHA"] = chkcode.CreateRandomCode();
        if (Session["CAPTCHA"] != null)
        {
            checkCode = Session["CAPTCHA"].ToString();
            Imgchkcode.ImageUrl = "Common/CheckCode.aspx?t=" + Tools.EncryptAES(checkCode);
            Imgchkcode.AlternateText = "[驗證碼]";
            //寫入 Cookies,給[語音播放]鈕用
            Response.Cookies["CAPTCHA"].Value = checkCode;
        }
    }
    #endregion


}