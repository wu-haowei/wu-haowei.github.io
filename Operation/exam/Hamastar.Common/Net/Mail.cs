using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Net;
using System.Net.Mail;
using System.Web.Configuration;
using System.Net.Configuration;
using System.Configuration;

namespace Hamastar.Common.Net
{
    public class Mail
    {
        public void MailTo(int SiteID, string pToMail, string pToName, string pSubject, string pBody, string pAttach, Page objPage, string pCC, string pBCC, bool pIsHTML)
        {
            //使用方法 aspx 第一行加入 Async="true"
            //開啓Request所在路徑網站的Web.config檔
            Configuration config = WebConfigurationManager.OpenWebConfiguration(objPage.Request.ApplicationPath);
            //取得Web.config中Mail設定區段
            MailSettingsSectionGroup netSmtpMailSection = (MailSettingsSectionGroup)config.GetSectionGroup("system.net/mailSettings");

            string FromMail = netSmtpMailSection.Smtp.From;
            string MailSmtp = netSmtpMailSection.Smtp.Network.Host;
            string MailName = netSmtpMailSection.Smtp.Network.UserName;
            string MailPassword = netSmtpMailSection.Smtp.Network.Password;
            int Port = netSmtpMailSection.Smtp.Network.Port;

            bool EnableSsl = false;
            bool.TryParse(WebConfigurationManager.AppSettings["MailEnableSSL"].ToString(), out EnableSsl);

            SendMail(FromMail, MailSmtp, Port, EnableSsl, MailName, MailPassword, pToMail, pToName, pSubject, pBody, pAttach, objPage, pCC, pBCC, pIsHTML);
        }
        
        public bool SendMail(string pFromMail, string MailSmtp, int Port, bool EnableSsl, string MailName, string MailPassword, string pToMail, string pToName, string pSubject, string pBody, string pAttach, Page objPage, string pCC, string pBCC, bool pIsHTML)
        {
            //開啓Request所在路徑網站的Web.config檔
            //Configuration config = WebConfigurationManager.OpenWebConfiguration(objPage.Request.ApplicationPath);
            //取得Web.config中Mail設定區段
            //MailSettingsSectionGroup netSmtpMailSection = (MailSettingsSectionGroup)config.GetSectionGroup("system.net/mailSettings");

            Encoding strCode = Encoding.GetEncoding("UTF-8");
            //mail
            MailAddress mFrom = new MailAddress(pFromMail, WebConfigurationManager.AppSettings["MailFromTitle"].ToString(), strCode);
            MailAddress mTo = new MailAddress(pToMail, pToName, strCode);

            MailMessage mail = new MailMessage(mFrom, mTo);

            mail.Subject = pSubject;
            mail.SubjectEncoding = strCode;

            mail.Body = pBody;
            mail.BodyEncoding = strCode;
            mail.IsBodyHtml = pIsHTML;
            //mail.DeliveryNotificationOptions = true;

            #region real path
            string file = pAttach;
            if (!String.IsNullOrEmpty(file))
            {
                // Create  the file attachment for this e-mail message.
                Attachment data = new Attachment(file);
                // Add the file attachment to this e-mail message.
                mail.Attachments.Add(data);
            }
            #endregion

            #region cc
            if (!string.IsNullOrEmpty(pCC))
            {
                mail.CC.Add(pCC);
            }
            if (!string.IsNullOrEmpty(pBCC))
            {
                mail.Bcc.Add(pBCC);
            }
            #endregion

            //smtp
            SmtpClient smtp = new SmtpClient(MailSmtp, Port);
            //smtp.Credentials = new NetworkCredential("nikola", "610814hm");
            smtp.Credentials = new NetworkCredential(MailName, MailPassword);
            smtp.EnableSsl = EnableSsl;
            //send completed event
            smtp.SendCompleted += new SendCompletedEventHandler(smtp_SendCompleted);

            //smtp.a
            //send
            bool vChk = true;
            try
            {
                smtp.SendAsync(mail, null);
            }
            catch (Exception ex)
            {
                vChk = false;
                throw new Exception(ex.InnerException.Message);
            }
            return vChk;
        }

        void smtp_SendCompleted(object sender, System.ComponentModel.AsyncCompletedEventArgs e)
        {
            if (e.Error != null)
                throw new Exception(e.Error.InnerException.Message);
        }
    }
}
