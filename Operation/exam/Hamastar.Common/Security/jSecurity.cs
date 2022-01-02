using Ganss.XSS;

using Microsoft.Security.Application;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

using static Ganss.XSS.HtmlSanitizer;
public static class jSecurity
{


    public static string XSS_Questionnaire(string s)
    {
        return HttpUtility.HtmlDecode(HttpUtility.HtmlEncode(s));
    }

    public static string GetQueryString(string s)
    {
        string QueryString = HttpContext.Current.Request.QueryString.Get(s);
        if (QueryString == null) return null;
        else
            return HttpUtility.HtmlDecode(HttpUtility.HtmlEncode(QueryString));
        // return HttpUtility.HtmlDecode(System.Web.Security.AntiXss.AntiXssEncoder.HtmlEncode(QueryString,true));
    }

    public static string XssAndSqlFilter(string s)
    {

        if (s == null) return null;
        else
            return jSecurity.SQLInjection(s);
    }

    /// <summary>
    /// --Data_Filter_Injection 
    /// </summary>
    /// <param name="s"></param>
    /// <returns></returns>
    public static string SQLInjection(string s)
    {
        return XSS(s.Replace("'", "").Replace("'--", ""));
    }

    /// <summary>
    /// --Path_Traversal
    /// </summary>
    /// <param name="s"></param>
    /// <returns></returns>
    public static string Path_Traversal(string s)
    {
        return s.Replace("\\", "").Replace("..", "");
    }

    /// <summary>
    /// --Cross_Site_History_Manipulation
    /// </summary>
    /// <param name="Response"></param>
    /// <param name="Url"></param>
    public static void Redirect(HttpResponse Response, string Url)
    {
        Response.Redirect(Url + (new Random()).Next());
    }

    /// <summary>
    /// 針對後端ck做處理
    /// </summary>
    /// <param name="s"></param>
    /// <returns></returns>
    public static string XSS(string s)
    {
        var sanitizer = new HtmlSanitizer();
        sanitizer.AllowedAttributes.Add("class");
        sanitizer.AllowedAttributes.Add("id");
        sanitizer.AllowedAttributes.Add("style");
        sanitizer.AllowedAttributes.Add("tag");
        sanitizer.AllowedAttributes.Add("href");

        sanitizer.AllowedTags.Add("style");
        s = sanitizer.Sanitize(s);
        return s;
    }


    class Random
    {
        public string Next()
        {
            return "";
        }
    }

}
