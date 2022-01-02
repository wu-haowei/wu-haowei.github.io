using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Quiksoft.EasyMail.SMTP;
using System.Collections;
using System.IO;

namespace Hamastar.Common.Net
{
    public class EasyEmail
    {
        private string _Subject;
        public string Subject
        {
            get { return _Subject; }
            set { _Subject = value; }
        }

        private string _HtmlBody;
        public string HtmlBody
        {
            get { return _HtmlBody; }
            set { _HtmlBody = value; }
        }

        private string _TextBody;
        public string TextBody
        {
            get { return _TextBody; }
            set { _TextBody = value; }
        }
    
        private string _SmtpServer;
        public string SmtpServer
        {
            get { return _SmtpServer; }
            set { _SmtpServer = value; }
        }

        private string _SmtpAccount;
        public string SmtpAccount
        {
            get { return _SmtpAccount; }
            set { _SmtpAccount = value; }
        }

        private string _SmtpPassword;
        public string SmtpPassword
        {
            get { return _SmtpPassword; }
            set { _SmtpPassword = value; }
        }

        private int _SmtpPort;
        public int SmtpPort
        {
            get { return _SmtpPort; }
            set { _SmtpPort = value; }
        }

        private string _FromName;
        public string FromName
        {
            get { return _FromName; }
            set { _FromName = value; }
        }

        private string _FromEmail;
        public string FromEmail
        {
            get { return _FromEmail; }
            set { _FromEmail = value; }
        }

        private string _ToName;
        public string ToName
        {
            get { return _ToName; }
            set { _ToName = value; }
        }

        private string _ToEmail;
        public string ToEmail
        {
            get { return _ToEmail; }
            set { _ToEmail = value; }
        }

        private Hashtable _To;
        public Hashtable To
        {
            get { return _To; }
            set { _To = value; }
        }

        private Hashtable _CC;
        public Hashtable CC
        {
          get { return _CC; }
          set { _CC = value; }
        }

        private Hashtable _BCC;
        public Hashtable BCC
        {
            get { return _BCC; }
            set { _BCC = value; }
        }

        private List<string> _Attachments = new List<string>();
        public List<string> Attachments
        {
            get { return _Attachments; }
            set { _Attachments = value; }
        }

        private Hashtable _StreamAttachments;
        public Hashtable StreamAttachments
        {
            get { return _StreamAttachments; }
            set { _StreamAttachments = value; }
        }

        //public Hashtable BCC
        //{
        //    get { return _BCC; }
        //    set { _BCC = value; }
        //}
        //MemoryStream

        public EasyEmail()
	    {
		    //
		    // TODO: 在此加入建構函式的程式碼
		    //
	    }

        public void SendMail()
        {
            //License.Key = "GSS (Single Developer)/3921601F22633001F#69C4#3EEDB1";
            License.Key = "HAMASTAR (Single Developer)/5482541F427136070748FB5C#975D#A88";

            //Create the EmailMessage object
            EmailMessage msgObj = new EmailMessage();

            msgObj.CharsetEncoding = System.Text.Encoding.GetEncoding("UTF-8");            

            //Specify from address and display name
            msgObj.From.Email = FromEmail;
            msgObj.From.Name = FromName;

            //Add a normal recipient
            if (To != null)
            {
                foreach (DictionaryEntry de in To)
                {
                    //msgObj.Recipients.Add(ToEmail, ToName, RecipientType.To);
                    msgObj.Recipients.Add(de.Key.ToString(), de.Value.ToString(), RecipientType.To);
                }
            }
            else
            {
                msgObj.Recipients.Add(ToEmail, ToName, RecipientType.To);
            }

            // 副本
            if (CC != null)
            {
                foreach (DictionaryEntry de in CC)
                {
                    msgObj.Recipients.Add(de.Key.ToString(), de.Value.ToString(), RecipientType.CC);
                }
            }

            // 密件副本
            if (BCC != null)
            {
                foreach (DictionaryEntry de in BCC)
                {
                    msgObj.Recipients.Add(de.Key.ToString(), de.Value.ToString(), RecipientType.BCC);
                }    
            }

            //Specify the subject
            msgObj.Subject = Subject;

            //Add an HTML body part
            if (!string.IsNullOrEmpty(HtmlBody))
            {
                msgObj.BodyParts.Add(HtmlBody, BodyPartFormat.HTML, BodyPartEncoding.QuotedPrintable);    
            }
        
            //Add a text body part to server as alternative text for non HTML mail readers
            if (!string.IsNullOrEmpty(TextBody))
            {
                msgObj.BodyParts.Add(TextBody, BodyPartFormat.Plain, BodyPartEncoding.Base64);
            }
            else
            {
                msgObj.BodyParts.Add(Hamastar.Common.Text.String.RemoveHtmlTag(HtmlBody), BodyPartFormat.Plain, BodyPartEncoding.Base64);
            }

            //Add an attachment
            if (Attachments.Count > 0)
            {
                foreach (string Attachmentpath in Attachments)
                {
                    msgObj.Attachments.Add(Attachmentpath);
                }
            }

            if (StreamAttachments != null)
            {
                foreach (DictionaryEntry de in StreamAttachments)
                {
                    MemoryStream stream = de.Value as MemoryStream;
                    Attachment att = new Attachment(stream, de.Key.ToString());
                    msgObj.Attachments.Add(att);
                }
            }
        
            //Create the SMTP object using the constructor to specify the mail server
            SMTPServer Smtpserver = new SMTPServer(SmtpServer);
            if (!string.IsNullOrEmpty(SmtpAccount) && !string.IsNullOrEmpty(SmtpPassword))
            {
                Smtpserver.AuthMode = SMTPAuthMode.AuthLogin;
                Smtpserver.Account = SmtpAccount;
                Smtpserver.Password = SmtpPassword;
            }
            else
            {
                Smtpserver.AuthMode = SMTPAuthMode.None;
            }

            //if (_IsAuth)
            //{
            //    Smtpserver.AuthMode = SMTPAuthMode.AuthLogin;
            //}
            //else
            //{
            //    Smtpserver.AuthMode = SMTPAuthMode.None;
            //}

            SMTP smtpObj = new SMTP();
            smtpObj.SMTPServers.Add(Smtpserver);

            //Send the message
            smtpObj.Send(msgObj);
        }
    }
}
