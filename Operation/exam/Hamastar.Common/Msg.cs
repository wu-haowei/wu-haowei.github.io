using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hamastar.Common
{
    public static class Msg
    {
         /// <summary>
        /// 產生訊息內容
        /// </summary>
        /// <param name="ParentSN"></param>
        /// <param name="MsgType"></param>
        /// <param name="DocNo">公文文號</param>
        ///<param name="Title">申請項目</param>
        /// <param name="DeptName">承辦單位</param>
        /// <param name="CreateDate">申請日期</param>
        /// <param name="CreateName">申請者姓名</param>
        /// <param name="CorrectionDate">補正期限</param>
        /// <param name="PayLimitDate">繳費期限</param>
        /// <param name="PayAmt">繳費金額</param>
        /// <param name="Reason">駁回原因</param>
        /// <returns></returns>
        public static string GenMsg(string msg, string DocNo = "", string Title = "", string DeptName = ""
            , DateTime? CreateDate = null, string CreateName = "", DateTime? CorrectionDate = null, DateTime? PayLimitDate = null
            , int PayAmt = 0, string Reason = "")
        {

             if (!string.IsNullOrWhiteSpace(DocNo))
                msg = msg.Replace("@公文文號@", DocNo);
            if (!string.IsNullOrWhiteSpace(Title))
                msg = msg.Replace("@申請項目@", Title).Replace("@申辦項目@", Title);
            if (!string.IsNullOrWhiteSpace(DeptName))
                msg = msg.Replace("@承辦單位@", DeptName);
            if (CreateDate != null)
                msg = msg.Replace("@申請日期@", CreateDate.Value.ToYmdDateString());
            if (!string.IsNullOrWhiteSpace(CreateName))
                msg = msg.Replace("@申請者姓名@", CreateName);
            if (CorrectionDate != null)
                msg = msg.Replace("@補正期限@", CorrectionDate.Value.ToYmdDateString());
            if (PayLimitDate != null)
                msg = msg.Replace("@繳費期限@", PayLimitDate.Value.ToYmdDateString());
            if (PayAmt != 0)
                msg = msg.Replace("@繳費金額@", PayAmt.ToString());
            if (!string.IsNullOrWhiteSpace(Reason))
                msg = msg.Replace("@駁回原因@", Reason);
            return msg;
        }
    }
}
