using System;
using System.Collections.Generic;
using System.Text;
using System.Net.Mail;
using System.Runtime.Remoting.Messaging;
using System.Net;
using System.Net.Mime;
using System.Web;

namespace Hamastar.Common.Net
{
    /// <summary>
    /// 包裝後的寄信類別
    /// </summary>
    public class Email
    {
        private string subject;
        /// <summary>
        /// 信件標題
        /// </summary>
        public string Subject
        {
            get
            {
                return subject;
            }
            set
            {
                subject = value;
            }
        }

        private string content;
        /// <summary>
        /// 信件內容
        /// </summary>
        public string Content
        {
            get
            {
                return content;
            }

            set
            {
                content = value;
            }
        }

        private string smtpServer;
        /// <summary>
        /// SMTP 伺服器
        /// </summary>
        public string SmtpServer
        {
            get
            {
                return smtpServer;
            }
            set
            {
                smtpServer = value;
            }
        }

        /// <summary>
        /// Relay 可設定 false;
        /// </summary>
        private bool _UseDefaultCredentials = true;
        public bool UseDefaultCredentials
        {
            get { return _UseDefaultCredentials; }
            set { _UseDefaultCredentials = value; }
        }

        private string smtpAccount;
        public string SmtpAccount
        {
            get { return smtpAccount; }
            set { smtpAccount = value; }
        }

        private string smtpPassword;
        public string SmtpPassword
        {
            get { return smtpPassword; }
            set { smtpPassword = value; }
        }

        private string fromName;
        public string FromName
        {
            get { return fromName; }
            set { fromName = value; }
        }

        private string fromEmail;
        /// <summary>
        /// 寄件者信箱
        /// </summary>
        public string FromEmail
        {
            get
            {
                return fromEmail;
            }

            set
            {
                fromEmail = value;
            }
        }

        private string toName;
        public string ToName
        {
            get { return toName; }
            set { toName = value; }
        }

        private string toEmail;
        public string ToEmail
        {
            get { return toEmail; }
            set { toEmail = value; }
        }

        private bool isBodyHtml = true;
        /// <summary>
        /// 是否使用 Html 格式
        /// </summary>
        public bool IsBodyHtml
        {
            get
            {
                return isBodyHtml;
            }

            set
            {
                isBodyHtml = value;
            }
        }

        private bool customHead = false;
        /// <summary>
        /// 自訂 Head
        /// </summary>
        public bool CustomHead
        {
            get
            {
                return customHead;
            }

            set
            {
                customHead = value;
            }
        }

        private string head;
        public string Head
        {
            get
            {
                return head;
            }

            set
            {
                head = value;
            }
        }

        private string _PlainText;
        public string PlainText
        {
            get { return _PlainText; }
            set { _PlainText = value; }
        }
        

        public string SendMail_TxtType(string p_Subject, string p_Content)
        {
            IsBodyHtml = false;

            return SendMail(p_Subject, p_Content);
        }

        public string SendMail(string p_Subject, string p_Content)
        {
            if (ToEmail.Equals(string.Empty) || FromEmail.Equals(string.Empty))
                return "不可為空字串";

            try
            {
                MailAddress from;
                if (FromName.Length > 0)
                {
                    from = new MailAddress(FromEmail, FromName);
                }
                else
                {
                    from = new MailAddress(FromEmail, FromEmail);
                }

                MailAddress to;
                if (ToName.Length > 0)
                {
                    to = new MailAddress(ToEmail, ToName);
                }
                else
                {
                    to = new MailAddress(ToEmail, ToEmail);
                }

                MailMessage mail = new MailMessage(from, to);

                if (customHead)
                {
                    p_Content = "<html><head>" + head + "<meta http-equiv=\"content-type\" content=\"text/html; charset=UTF-8\"></head><body>" + p_Content + "</body></html>"; 
                }
                else if (isBodyHtml)
                {
                    p_Content = "<html><head><meta http-equiv=\"content-type\" content=\"text/html; charset=UTF-8\"></head><body>" + p_Content + "</body></html>";
                }

                AlternateView htmlView = AlternateView.CreateAlternateViewFromString(p_Content, null, "text/html");
                mail.AlternateViews.Add(htmlView);

                if (!string.IsNullOrEmpty(PlainText))
                {
                    AlternateView plainView = AlternateView.CreateAlternateViewFromString(PlainText, null, "text/plain");
                    htmlView.TransferEncoding = TransferEncoding.QuotedPrintable;
                    mail.AlternateViews.Add(plainView);
                }

                mail.Subject = p_Subject;
                
                mail.Body = p_Content;
                mail.IsBodyHtml = isBodyHtml; //是否為 Html 格式
                //mail.SubjectEncoding = System.Text.Encoding.UTF8; //標題編碼
                //mail.BodyEncoding = System.Text.Encoding.UTF8; //內容編碼
                //mail.Headers.Add("Content-Transfer-Encoding", "base64");
                //SmtpClient client = new SmtpClient(smtpServer == null ? Library.Settings.SmtpServer : smtpServer);
                SmtpClient client = new SmtpClient(smtpServer);
                client.Credentials = new NetworkCredential(smtpAccount, smtpPassword); //驗證寄件者的認證
                //client.DeliveryMethod = SmtpDeliveryMethod.Network;

                client.UseDefaultCredentials = UseDefaultCredentials;
                client.Send(mail);
            }
            catch (Exception ex)
            {
                //throw new Exception("Send error：" + p_MailTo);
                throw new Exception("Send error：" + ex.Message);
            }

            return ToEmail;
        }


        /// <summary>
        /// 加上防止主旨為亂碼(院長信箱用)
        /// </summary>
        /// <param name="p_Subject"></param>
        /// <param name="p_Content"></param>
        /// <param name="IsToBase64String"></param>
        /// <returns></returns>
        public string SendMail(string p_Subject, string p_Content, bool IsToBase64String)
        {
            if (ToEmail.Equals(string.Empty) || FromEmail.Equals(string.Empty))
                return "不可為空字串";

            try
            {
                MailAddress from;
                if (FromName.Length > 0)
                {
                    from = new MailAddress(FromEmail, FromName);
                }
                else
                {
                    from = new MailAddress(FromEmail, FromEmail);
                }

                MailAddress to;
                if (ToName.Length > 0)
                {
                    to = new MailAddress(ToEmail, ToName);
                }
                else
                {
                    to = new MailAddress(ToEmail, ToEmail);
                }

                MailMessage mail = new MailMessage(from, to);

                if (customHead)
                {
                    p_Content = "<html><head>" + head + "<meta http-equiv=\"content-type\" content=\"text/html; charset=UTF-8\"></head><body>" + p_Content + "</body></html>";
                }
                else if (isBodyHtml)
                {
                    p_Content = "<html><head><meta http-equiv=\"content-type\" content=\"text/html; charset=UTF-8\"></head><body>" + p_Content + "</body></html>";
                }

                AlternateView htmlView = AlternateView.CreateAlternateViewFromString(p_Content, null, "text/html");

                if (!string.IsNullOrEmpty(PlainText))
                {
                    //mail.Headers.Add("Content-Transfer-Encoding", "quoted-printable");                    
                    AlternateView plainView = AlternateView.CreateAlternateViewFromString(PlainText, null, "text/plain");
                    mail.AlternateViews.Add(plainView);
                }
                mail.AlternateViews.Add(htmlView);

                if (IsToBase64String && p_Subject.Trim().Length > 0)
                    mail.Subject = string.Format(@"=?UTF-8?B?{0}?=", Convert.ToBase64String(Encoding.UTF8.GetBytes(p_Subject)));
                else
                    mail.Subject = p_Subject;
                
                mail.Body = p_Content;
                mail.IsBodyHtml = isBodyHtml; //是否為 Html 格式
                //mail.SubjectEncoding = System.Text.Encoding.UTF8; //標題編碼
                //mail.BodyEncoding = System.Text.Encoding.UTF8; //內容編碼
                //mail.Headers.Add("Content-Transfer-Encoding", "base64");
                //SmtpClient client = new SmtpClient(smtpServer == null ? Library.Settings.SmtpServer : smtpServer);
                SmtpClient client = new SmtpClient(smtpServer);
                client.Credentials = new NetworkCredential(smtpAccount, smtpPassword); //驗證寄件者的認證
                //client.DeliveryMethod = SmtpDeliveryMethod.Network;

                client.UseDefaultCredentials = UseDefaultCredentials;
                client.Send(mail);
            }
            catch (Exception ex)
            {
                //throw new Exception("Send error：" + p_MailTo);
                throw new Exception("Send error：" + ex.Message);
            }

            return ToEmail;
        }


        /// <summary>
        /// 權責單位用(包含附檔)
        /// </summary>
        /// <param name="p_Subject"></param>
        /// <param name="p_Content"></param>
        /// <param name="AttList"></param>
        /// <returns></returns>
        public string SendMail(string p_Subject, string p_Content, List<Attachment> AttList)
        {
            if (ToEmail.Equals(string.Empty) || FromEmail.Equals(string.Empty))
                return "不可為空字串";

            try
            {
                MailAddress from;
                if (FromName.Length > 0)
                {
                    from = new MailAddress(FromEmail, FromName);
                }
                else
                {
                    from = new MailAddress(FromEmail, FromEmail);
                }

                MailAddress to;
                if (ToName.Length > 0)
                {
                    to = new MailAddress(ToEmail, ToName);
                }
                else
                {
                    to = new MailAddress(ToEmail, ToEmail);
                }

                MailMessage mail = new MailMessage(from, to);

                if (AttList.Count > 0)
                {
                    foreach (Attachment a in AttList)
                    {
                        //  a.NameEncoding = System.Text.Encoding.UTF8;
                        //  a.Name = string.Format(@"=?UTF-8?B?{0}?=", Convert.ToBase64String(Encoding.UTF8.GetBytes(a.Name)));

                        //a.Name = System.Web.HttpUtility.UrlEncode(a.Name, System.Text.Encoding.UTF8);
                        //a.NameEncoding = System.Text.Encoding.UTF8;
                        //mail.Attachments.Add(a);
                        /*Attachment encodeAttach = CreateAttachment(a, a.Name, TransferEncoding.Base64);
                        mail.Attachments.Add(encodeAttach);*/
                        mail.Attachments.Add(a);
                    }
                }

                if (customHead)
                {
                    p_Content = "<html><head>" + head + "<meta http-equiv=\"content-type\" content=\"text/html; charset=UTF-8\"></head><body>" + p_Content + "</body></html>";
                }
                else if (isBodyHtml)
                {
                    p_Content = "<html><head><meta http-equiv=\"content-type\" content=\"text/html; charset=UTF-8\"></head><body>" + p_Content + "</body></html>";
                }
                mail.AlternateViews.Clear();

                if (!string.IsNullOrEmpty(PlainText))
                {
                    //mail.Headers.Add("Content-Transfer-Encoding", "quoted-printable");
                    AlternateView plainView = AlternateView.CreateAlternateViewFromString(PlainText, null, "text/plain");
                    mail.AlternateViews.Add(plainView);
                    //mail.Body = PlainText;
                }

                //ContentType mimeType = new System.Net.Mime.ContentType("text/html");
                AlternateView htmlView = AlternateView.CreateAlternateViewFromString(p_Content, null, "text/html");
                //AlternateView htmlView = AlternateView.CreateAlternateViewFromString(p_Content, mimeType);
                htmlView.TransferEncoding = TransferEncoding.QuotedPrintable;
                mail.AlternateViews.Add(htmlView);

                mail.Subject = p_Subject; //信件標題

                //mail.Subject=  string.Format(@"=?UTF-8?B?{0}?=", Convert.ToBase64String(Encoding.UTF8.GetBytes(p_Subject)));
                //mail.Body = p_Content;
                
                mail.IsBodyHtml = isBodyHtml; //是否為 Html 格式
                mail.SubjectEncoding = System.Text.Encoding.UTF8; //標題編碼
                mail.BodyEncoding = System.Text.Encoding.UTF8; //內容編碼

                //mail.Headers.Add("Content-Transfer-Encoding", "base64");
   
                //SmtpClient client = new SmtpClient(smtpServer == null ? Library.Settings.SmtpServer : smtpServer);
                SmtpClient client = new SmtpClient(smtpServer);
                client.Credentials = new NetworkCredential(smtpAccount, smtpPassword); //驗證寄件者的認證
                client.UseDefaultCredentials = UseDefaultCredentials;
                client.Send(mail);
                mail.Dispose();
            }
            catch (Exception ex)
            {
                //throw new Exception("Send error：" + p_MailTo);
                throw new Exception("Send error：" + ex.Message);
            }

            return ToEmail;
        }

        /// <summary>
        /// 加上防止主旨為亂碼(院長信箱用)
        /// </summary>
        /// <param name="p_Subject"></param>
        /// <param name="p_Content"></param>
        /// <param name="AttList"></param>
        /// <param name="IsToBase64String"></param>
        /// <returns></returns>
        public string SendMail(string p_Subject, string p_Content, List<Attachment> AttList, bool IsToBase64String)
        {
            if (ToEmail.Equals(string.Empty) || FromEmail.Equals(string.Empty))
                return "不可為空字串";

            try
            {
                MailAddress from;
                if (FromName.Length > 0)
                {
                    from = new MailAddress(FromEmail, FromName);
                }
                else
                {
                    from = new MailAddress(FromEmail, FromEmail);
                }

                MailAddress to;
                if (ToName.Length > 0)
                {
                    to = new MailAddress(ToEmail, ToName);
                }
                else
                {
                    to = new MailAddress(ToEmail, ToEmail);
                }

                MailMessage mail = new MailMessage(from, to);

                if (customHead)
                {
                    p_Content = "<html><head>" + head + "<meta http-equiv=\"content-type\" content=\"text/html; charset=UTF-8\"></head><body>" + p_Content + "</body></html>";
                }
                else if (isBodyHtml)
                {
                    p_Content = "<html><head><meta http-equiv=\"content-type\" content=\"text/html; charset=UTF-8\"></head><body>" + p_Content + "</body></html>";
                }

                AlternateView htmlView = AlternateView.CreateAlternateViewFromString(p_Content, null, "text/html");
                mail.AlternateViews.Add(htmlView);

                if (!string.IsNullOrEmpty(PlainText))
                {
                    AlternateView plainView = AlternateView.CreateAlternateViewFromString(PlainText, null, "text/plain");
                    mail.AlternateViews.Add(plainView);
                }


                if (IsToBase64String && p_Subject.Trim().Length > 0)
                    mail.Subject = string.Format(@"=?UTF-8?B?{0}?=", Convert.ToBase64String(Encoding.UTF8.GetBytes(p_Subject)));
                else
                    mail.Subject = p_Subject; //信件標題

                mail.Body = p_Content;

                mail.IsBodyHtml = isBodyHtml; //是否為 Html 格式
                mail.SubjectEncoding = System.Text.Encoding.UTF8; //標題編碼
                mail.BodyEncoding = System.Text.Encoding.UTF8; //內容編碼

                //mail.Headers.Add("Content-Transfer-Encoding", "base64");

                if (AttList.Count > 0)
                {
                    foreach (Attachment a in AttList)
                    {
                        //  a.NameEncoding = System.Text.Encoding.UTF8;
                        //  a.Name = string.Format(@"=?UTF-8?B?{0}?=", Convert.ToBase64String(Encoding.UTF8.GetBytes(a.Name)));

                        //a.Name = System.Web.HttpUtility.UrlEncode(a.Name, System.Text.Encoding.UTF8);
                        //a.NameEncoding = System.Text.Encoding.UTF8;
                        //mail.Attachments.Add(a);
                        /*Attachment encodeAttach = CreateAttachment(a, a.Name, TransferEncoding.Base64);
                        mail.Attachments.Add(encodeAttach);*/
                        mail.Attachments.Add(a);
                    }
                }

                //SmtpClient client = new SmtpClient(smtpServer == null ? Library.Settings.SmtpServer : smtpServer);
                SmtpClient client = new SmtpClient(smtpServer);
                client.Credentials = new NetworkCredential(smtpAccount, smtpPassword); //驗證寄件者的認證
                client.UseDefaultCredentials = UseDefaultCredentials;
                client.Send(mail);
                mail.Dispose();
            }
            catch (Exception ex)
            {
                //throw new Exception("Send error：" + p_MailTo);
                throw new Exception("Send error：" + ex.Message);
            }

            return ToEmail;
        }

        public delegate string SendEmailDelegate(string p_Subject, string p_Content);

        private void GetResultsOnCallback(IAsyncResult ar)
        {
            SendEmailDelegate del = (SendEmailDelegate)((AsyncResult)ar).AsyncDelegate;
            string result = del.EndInvoke(ar);
        }

        public bool SendEmailAsync(string p_Subject, string p_Content)
        {
            try
            {
                SendEmailDelegate dc = new SendEmailDelegate(this.SendMail);
                AsyncCallback cb = new AsyncCallback(this.GetResultsOnCallback);
                IAsyncResult ar = dc.BeginInvoke(p_Subject, p_Content, cb, null);

                //其實寫這樣就可以了
                //SendEmailDelegate dc = new SendEmailDelegate(this.SendMail);
                //dc.BeginInvoke(p_MailTo, p_MailFrom, p_Subject, p_Content, null, null);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool SendEmailAsync(string p_MailTo)
        {
            return SendEmailAsync(subject, content);
        }

        #region 修正.Net的Bug，若附件檔名bytes超過41會重覆Encode
        public static Attachment CreateAttachment(string attachmentFile, string displayName, TransferEncoding transferEncoding)
        {
            Attachment attachment = new Attachment(attachmentFile);
            attachment.TransferEncoding = transferEncoding;

            string tranferEncodingMarker = string.Empty;
            string encodingMarker = string.Empty;
            int maxChunkLength = 0;

            switch (transferEncoding)
            {
                case TransferEncoding.Base64:
                    tranferEncodingMarker = "B";
                    encodingMarker = "UTF-8";
                    maxChunkLength = 30;
                    break;
                case TransferEncoding.QuotedPrintable:
                    tranferEncodingMarker = "Q";
                    encodingMarker = "ISO-8859-1";
                    maxChunkLength = 76;
                    break;
                default:
                    throw (new ArgumentException(string.Format("The specified TransferEncoding is not supported: {0}", transferEncoding, "transferEncoding")));
            }

            attachment.NameEncoding = Encoding.GetEncoding(encodingMarker);

            string encodingtoken = string.Format("=?{0}?{1}?", encodingMarker, tranferEncodingMarker);
            string softbreak = "?=";
            string encodedAttachmentName = encodingtoken;

            if (attachment.TransferEncoding == TransferEncoding.QuotedPrintable)
                encodedAttachmentName = HttpUtility.UrlEncode(displayName, Encoding.Default).Replace("+", " ").Replace("%", "=");
            else
                encodedAttachmentName = Convert.ToBase64String(Encoding.UTF8.GetBytes(displayName));

            encodedAttachmentName = SplitEncodedAttachmentName(encodingtoken, softbreak, maxChunkLength, encodedAttachmentName);
            attachment.Name = encodedAttachmentName;

            return attachment;
        }
        public static Attachment CreateAttachment(Attachment attachment, string displayName, TransferEncoding transferEncoding)
        {
            //Attachment attachment = new Attachment(attachmentFile);
            attachment.TransferEncoding = transferEncoding;

            string tranferEncodingMarker = string.Empty;
            string encodingMarker = string.Empty;
            int maxChunkLength = 0;

            switch (transferEncoding)
            {
                case TransferEncoding.Base64:
                    tranferEncodingMarker = "B";
                    encodingMarker = "UTF-8";
                    maxChunkLength = 30;
                    break;
                case TransferEncoding.QuotedPrintable:
                    tranferEncodingMarker = "Q";
                    encodingMarker = "ISO-8859-1";
                    maxChunkLength = 76;
                    break;
                default:
                    throw (new ArgumentException(string.Format("The specified TransferEncoding is not supported: {0}", transferEncoding, "transferEncoding")));
            }

            attachment.NameEncoding = Encoding.GetEncoding(encodingMarker);

            string encodingtoken = string.Format("=?{0}?{1}?", encodingMarker, tranferEncodingMarker);
            string softbreak = "?=";
            string encodedAttachmentName = encodingtoken;

            if (attachment.TransferEncoding == TransferEncoding.QuotedPrintable)
                encodedAttachmentName = HttpUtility.UrlEncode(displayName, Encoding.Default).Replace("+", " ").Replace("%", "=");
            else
                encodedAttachmentName = Convert.ToBase64String(Encoding.UTF8.GetBytes(displayName));

            encodedAttachmentName = SplitEncodedAttachmentName(encodingtoken, softbreak, maxChunkLength, encodedAttachmentName);
            attachment.Name = encodedAttachmentName;

            return attachment;
        }

        private static string SplitEncodedAttachmentName(string encodingtoken, string softbreak, int maxChunkLength, string encoded)
        {
            int splitLength = maxChunkLength - encodingtoken.Length - (softbreak.Length * 2);
            var parts = SplitByLength(encoded, splitLength);

            string encodedAttachmentName = encodingtoken;

            foreach (var part in parts)
                encodedAttachmentName += part + softbreak + encodingtoken;

            encodedAttachmentName = encodedAttachmentName.Remove(encodedAttachmentName.Length - encodingtoken.Length, encodingtoken.Length);
            return encodedAttachmentName;
        }

        private static IEnumerable<string> SplitByLength(string stringToSplit, int length)
        {
            while (stringToSplit.Length > length)
            {
                yield return stringToSplit.Substring(0, length);
                stringToSplit = stringToSplit.Substring(length);
            }

            if (stringToSplit.Length > 0) yield return stringToSplit;
        }
        #endregion
    }
}
