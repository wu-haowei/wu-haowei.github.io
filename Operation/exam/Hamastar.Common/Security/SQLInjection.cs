using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Hamastar.Common.Security
{
    //版本：.NET Framework 4.5
    //建議使用微軟AntiXSS(下載位置:http://wpl.codeplex.com/) 
    //全網頁加密 http://msdn.microsoft.com/zh-tw/library/system.web.security.antixss.antixssencoder.aspx

    public static class SQLInjection
    {
        /// <summary>
        /// 檢測SQL InJection
        /// </summary>
        public static string CheckSQLInjection(string InputString)
        {
            if (string.IsNullOrEmpty(InputString)) return string.Empty;
            // Update By kycheng 2013.07.15

            string[] DangerKeySigns = { "+", "-", "*", "/", "=", "&", "|", "<", ">", "=", "{", "}", ";", "\\", "'", "#", "chr(", "char(" };
            string[] DangerKeyWords = { "create", "drop", "alter", "select", "update", "delete", "truncate", "from", "exec", "import", "script", "embed", "and", "or" };
            // CheckString = 沒有空格, 換行的InputString
            string CheckStringSigns = InputString.Replace(" ", "").Replace("\n", "").ToLower();
            // Signs的先把空格刪掉在檢查
            foreach (string KeyWord in DangerKeySigns)
            {
                if (CheckStringSigns.IndexOf(KeyWord) != -1)
                    return string.Empty;
            }

            // 
            string CheckStringWords = " " + InputString + " ";
            // Words的不刪空格檢查 (因為都是英文單字 如果前後兩字中間有空格就不會有危險)
            // (不然查個 abort 就說裡面有or不給查 也太難用了點)
            foreach (string KeyWord in DangerKeyWords)
            {
                int pos = CheckStringWords.IndexOf(KeyWord);
                if (pos > 0)
                {
                    // 出現過 檢查前後是否有字母
                    if (!char.IsLetterOrDigit(CheckStringWords[pos - 1]) && !char.IsLetterOrDigit(CheckStringWords[pos + KeyWord.Length]))
                        return string.Empty;
                }
            }

            return InputString;
        }
    }
}
