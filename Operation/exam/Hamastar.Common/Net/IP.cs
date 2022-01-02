using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace Hamastar.Common.Net
{
    public class IP
    {
        /// <summary>
        /// 驗證ip是否符合白名單，Ex: 192.*.*.*;192.168.0.*;192.168.1.100-150;192.168.10.100
        /// </summary>
        /// <param name="IP"></param>
        /// <param name="IPCollection">Ex: 192.168.0.*;192.168.1.100-150;192.168.10.100</param>
        /// <returns></returns>
        public static bool IPValidation(string IP, string IPCollection)
        {
            string[] targetIP = IP.Split('.');
            string[] IPSet = IPCollection.Split(';');

            bool legal = false;

            if (targetIP.Count() != 4)
            {
                return false;
            }
            foreach (string IPRange in IPSet)
            {
                string[] IPRange2 = IPRange.Split('.');

                if (IPRange2.Count() != 4)
                {
                    continue;
                }

                int IP_1, IP_2, IP_3, IP_4;
                int IP2_1, IP2_2, IP2_3;
                int.TryParse(targetIP[0], out IP_1);
                int.TryParse(targetIP[1], out IP_2);
                int.TryParse(targetIP[2], out IP_3);
                int.TryParse(targetIP[3], out IP_4);

                int.TryParse(IPRange2[0], out IP2_1);
                int.TryParse(IPRange2[1], out IP2_2);
                int.TryParse(IPRange2[2], out IP2_3);

                if ((IP_1 == IP2_1 || IPRange2[0] == "*") && (IP_2 == IP2_2 || IPRange2[1] == "*") && (IP_3 == IP2_3 || IPRange2[2] == "*"))
                {
                    if (IPRange2[3] == "*")
                    {
                        legal = true;
                    }
                    else if (IPRange2[3].Contains("-"))
                    {
                        string[] range = IPRange2[3].Split('-');
                        int small, large;
                        int.TryParse(range[0], out small);
                        int.TryParse(range[1], out large);

                        if (IP_4 >= small && IP_4 <= large)
                            legal = true;
                    }
                    else
                    {
                        int IP2_4;
                        int.TryParse(IPRange2[3], out IP2_4);
                        if (IP_4 == IP2_4)
                            legal = true;
                    }
                }
            }

            return legal;
        }

        /// <summary>
        /// 取得使用者IP位址
        /// </summary>
        /// <returns></returns>
        public static string GetIpAddress()
        {
            string strIpAddr = string.Empty;
            if (HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"] == null || HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"].IndexOf("unknown") > 0)
            {
                strIpAddr = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
            }
            else if (HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"].IndexOf(",") > 0)
            {
                strIpAddr = HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"].Substring(1, HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"].IndexOf(",") - 1);
            }
            else if (HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"].IndexOf(";") > 0)
            {
                strIpAddr = HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"].Substring(1, HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"].IndexOf(";") - 1);
            }
            else
            {
                strIpAddr = HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
            }
            return strIpAddr;
        }
    }
}
