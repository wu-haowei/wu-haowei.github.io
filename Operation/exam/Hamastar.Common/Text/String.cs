using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

using Microsoft.VisualBasic;

namespace Hamastar.Common.Text
{
    public class String
    {
        /// <summary>
        /// 繁體轉換簡体
        /// </summary>
        /// <param name="val"></param>
        /// <returns></returns>
        public static string ConvertToSimplifiedChinese(string Value)
        {
            return Strings.StrConv(Value, VbStrConv.SimplifiedChinese, 2052);
        }

        /// <summary>
        /// 字串切割
        /// </summary>
        /// <param name="Value"></param>
        /// <param name="Length"></param>
        /// <param name="EndStr"></param>
        /// <returns></returns>
        public static string StringCut(string Value, int Length, string EndStr)
        {
            if (!string.IsNullOrEmpty(Value) && Value.Length > Length)
                return Value.Substring(0, Length) + EndStr;
            else
                return Value;
        }

        /// <summary>
        /// 移除HTML所有Tag
        /// </summary>
        /// <param name="Value"></param>
        /// <returns></returns>
        public static string RemoveHtmlTag(string Value)
        {
            return Regex.Replace(Value, "(?is)<.+?>", "");
        }
         
        /// <summary> 
        /// 秒數轉HH:mm:ss
        /// </summary>
        /// <param name="Second"></param>
        /// <returns></returns>
        public static string TotalSecondToTimeFormat(int Second)
        {
            double hour, min, sec;
            string shour, smin, ssec;
            string Format = string.Empty;
            if (Second > 0)
            {
                hour = Math.Floor(Convert.ToDouble(Second) / 3600) % 60;
                min = Math.Floor(Convert.ToDouble(Second) / 60) % 60;
                sec = Math.Floor(Convert.ToDouble(Second)) % 60;

                shour = hour.ToString("00");
                smin = min.ToString("00");
                ssec = sec.ToString("00");

                Format = shour + ":" + smin + ":" + ssec;
            }
            else
            {
                Format = "00:00:00";
            }
            return Format; 
        }
        public static string strReplace(string prVal)
        {
            if (!string.IsNullOrEmpty(prVal))
                return prVal.Replace("\n", "<br>").Replace(" ", "&nbsp;&nbsp;");
            else
                return prVal;
        }
    }
}
