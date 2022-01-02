using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Hamastar.Common.tw.com.hamastar.ss;
namespace Hamastar.Common.SMSCenter
{
    public class SMS
    {
        public string ID = string.Empty;
        public string PW = string.Empty;
        SmsWebService BL;
        public SMS(string ID, string PW, string WebServiceURL)
        {
            this.ID = ID;
            this.PW = PW;
            BL = new SmsWebService();
            BL.Timeout = 600000;
            BL.Url = WebServiceURL;
        }

        #region 呼叫公司WebService發送簡訊 http://ss.hamastar.com.tw/smswebservice.asmx http://ss.hamastar.com.tw/account.asmx http://ss.hamastar.com.tw/mgr/Default.aspx
        //送出簡訊
        public string SendMessage(string Phone, string Content)
        {
            return BL.SendMessage(ID, PW, Phone, Content);        
        }

        //取得簡訊發送結果
        public string GetMessageStatus(string MsgID)
        {       
            return BL.GetMessageStatus(ID , PW, MsgID);
        }
        #endregion
    }
}
