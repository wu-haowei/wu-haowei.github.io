using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Hamastar.BusinessObject;
using Hamastar.Common.Text;
using System.Text;
using System.Configuration;
using System.Web.Configuration;
using System.Net.Configuration;
using Hamastar.Common;
public partial class ForgotPasswd : System.Web.UI.Page
{
    bool IsRandomPW = true;//忘記密碼是否隨機產生密碼(發送Email時)
    bool IsKeyName = false;//忘記密碼是否需要輸入姓名
    protected void Page_Load(object sender, EventArgs e)
    {
        //IsRandomPW = JtoolConfig.GetFunctionDisable(28);
        //IsKeyName = JtoolConfig.GetFunctionDisable(29);
        if (!IsPostBack)
        {
            lblIP.Text = Request.UserHostAddress;

            Label lblTitleName = Master.FindControl("lblTitleName") as Label;
            lblTitleName.Text = "忘記密碼";

            if (IsKeyName)
            {
                trName.Visible = true;
            }
            else
            {
                trName.Visible = false;
            }
        }
    }

    protected void btnOK_Click(object sender, EventArgs e)
    {
        Comm_AccUser accuser = Comm_AccUser.GetSingle(x => x.ID == txtAccount.Text);
        if (accuser != null)
        {
            if (accuser.Email != txtEmail.Text)
            {
                Pages.AlertByswal("錯誤", "帳號或電子信箱錯誤", Tools.altertType.錯誤.ToDescriptionString());
                return;
            }
            if (IsKeyName && accuser.Name != txtName.Text)
            {
                Pages.AlertByswal("錯誤", "姓名錯誤", Tools.altertType.錯誤.ToDescriptionString());
                return;
            }
            if (IsRandomPW)
            {
                accuser.P_W = Tools.EncryptAES(chkcode.CreateRandomCode(6));
                int iChangePWDate = 0;//修改密碼週期
                //int.TryParse(JtoolConfig.GetFunctionValue(21), out iChangePWDate);
                accuser.LastUpdateP_WDate = DateTime.Now.AddDays(-(iChangePWDate + 1));
                Comm_AccUser.Update(accuser);
            }

            string P_W = accuser.P_W; // Tools.GenerateRandomText(8);



            StringBuilder sb = new StringBuilder();
            sb.AppendLine("您好：");
            sb.AppendLine("您的密碼為 " + Tools.DecryptAES(P_W));
            sb.AppendLine("敬祝　愉快");
            sb.AppendLine("◎此為系統發出的電子郵件，請勿直接回復。");

            try
            {
                Tools tool = new Tools();
                tool.m_MailTo(Page, true, accuser.Email, accuser.Name, "密碼通知信", sb.ToString(), "", "", "");
                Pages.AlertByswal("提醒你", "已寄發密碼通知信至您的電子信箱", Tools.altertType.資訊.ToDescriptionString());
            }
            catch (Exception ex)
            {
                Pages.Alert(ex.Message);
                return;
            }
        }
        else
        {
            Pages.AlertByswal("錯誤", "帳號或電子信箱錯誤", Tools.altertType.錯誤.ToDescriptionString());
            return;
        }
    }
}