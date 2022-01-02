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

namespace Hamastar.Common.Text
{
    public class Pages
    {

        //#region 警告訊息

        ///// <summary>
        ///// 在網頁上彈出警告訊息。
        ///// </summary>
        ///// <param name="message">訊息內容</param>
        //public static void Alert(string message)
        //{
        //    string script = string.Format("alert('{0}');", HttpUtility.HtmlEncode(message.Replace("\n", "\\n")));
        //    JavaScrpit("window_aler", script);
        //}

        ///// <summary>
        /////  在網頁上彈出警告訊息後，轉頁至新頁面
        ///// </summary>
        ///// <param name="message">訊息內容</param>
        ///// <param name="url">新頁面</param>
        //public static void Alert(string message, string url)
        //{
        //    string script = string.Format("alert('{0}');window.location.href='{1}';", HttpUtility.HtmlEncode(message.Replace("\n", "\\n")), url);
        //    JavaScrpit("window_aler_redir", script);
        //}


        ///// <summary>
        ///// 在網頁上彈出警告訊息，更新視窗或關閉視窗。
        ///// </summary>
        ///// <param name="message">訊息內容</param>
        ///// <param name="reload">是否更新，否則關閉</param>
        //public static void Alert(string message, bool reload)
        //{
        //    System.Web.UI.Page currentPage = (System.Web.UI.Page)HttpContext.Current.Handler;

        //    string script = string.Format("alert('{0}');", HttpUtility.HtmlEncode(message.Replace("\n", "\\n")));
        //    if (reload)
        //    {
        //        script += "if(window.opener!=null)window.opener.open(window.opener.location,'_self');";
        //    }
        //    script += "window.close();";
        //    JavaScrpit("window_aler_reload", script);
        //}

        ///// <summary>
        ///// 彈出此視窗的母網頁重導至url。
        ///// </summary>
        ///// <param name=url>重導的網址</param>
        //public static void OpenrLoad(string url)
        //{
        //    System.Web.UI.Page currentPage = (System.Web.UI.Page)HttpContext.Current.Handler;

        //    string script = string.Format("nw = window.opener.location.href='{0}';", url);
        //    JavaScrpit("window_openr_load", script);
        //}





        ///// <summary>
        ///// 重整母親窗並更新至最新狀態。
        ///// </summary>
        ///// <param name=url>重導的網址</param>
        //public static void OpenrReLoad()
        //{
        //    System.Web.UI.Page currentPage = (System.Web.UI.Page)HttpContext.Current.Handler;

        //    string script = string.Format("nw = window.opener.location.reload();");
        //    JavaScrpit("window_openr_load", script);
        //}


        //private static void JavaScrpit(string name, string script)
        //{
        //    System.Web.UI.Page currentPage = (System.Web.UI.Page)HttpContext.Current.Handler;
        //    //ASP.Net 2.0請用下面Code
        //    System.Web.UI.ClientScriptManager cs = currentPage.ClientScript;
        //    cs.RegisterClientScriptBlock(currentPage.GetType(), name, script, true);

        //    //ASP.Net 1.X請用下面Code
        //    //currentPage.RegisterStartupScript(name, "<script language='javascript'>" + script + "</script>");
        //}

        ///// <summary>
        ///// 彈出此視窗的母網頁重整並更新至最新狀態
        ///// </summary>
        //public static string OpenrWindowOpen(string target, int width, int height)
        //{
        //    System.Web.UI.Page currentPage = (System.Web.UI.Page)HttpContext.Current.Handler;

        //    string jsStr = "";
        //    jsStr += "var scriptName=document.forms[0].action;";
        //    jsStr += string.Format("window.document.forms[0].target='{0}';", target);
        //    jsStr += string.Format("window.document.forms[0].onsubmit=window.open('', '{0}','height={1}, width={2},resizable=1, toolbar=0, directories=0, location=0, menubar=0, personalbar=0, status=0,scrollbars=1, titlebar=0 ');", target, height, width);
        //    jsStr += "setTimeout(function(){window.document.forms[0].target='';";
        //    jsStr += "document.forms[0].action=scriptName;}, 500);";

        //    //string script = string.Format(jsStr);
        //    //JavaScrpit("window_openr_load", script);

        //    return jsStr;
        //}



        //#endregion

        ///// <summary>
        ///// 彈跳警告視窗
        ///// 用於非同步
        ///// </summary>
        ///// <param name="objPage"></param>
        ///// <param name="AlertMessage"></param>
        //public static void Server_ShowMsg(Page objPage, string AlertMessage)
        //{
        //    ScriptManager.RegisterClientScriptBlock(objPage, objPage.GetType(), "msgAlert", "alert(\"" + AlertMessage + "\");", true);
        //}

        ///// <summary>
        ///// 彈跳警告視窗
        ///// </summary>
        ///// <param name="ObjPage"></param>
        ///// <param name="AlertMessage"></param>
        //public static void ShowMsg(Page ObjPage, string AlertMessage)
        //{
        //    Literal txtMsg = new Literal();
        //    txtMsg.Text = "<script>alert(\"" + AlertMessage + "\");</script>" + "\n";
        //    ObjPage.Controls.Add(txtMsg);
        //}

        ///// <summary>
        ///// 更新母視窗
        ///// </summary>
        ///// <param name="ObjPage"></param>
        //public static void RefreshWindow(Page ObjPage)
        //{
        //    StringBuilder sbScript = new StringBuilder();
        //    sbScript.Append("if(opener != null) { \n");
        //    sbScript.Append("   if(opener.document.getElementById('btnSearch') != null) { \n");
        //    sbScript.Append("       var btn = opener.document.getElementById('btnSearch'); \n");
        //    sbScript.Append("       btn.click(); \n");
        //    sbScript.Append("   } \n");
        //    sbScript.Append("   else \n");
        //    sbScript.Append("   { \n");
        //    sbScript.Append("       opener.location.href = opener.location.href; \n");
        //    sbScript.Append("   } \n");
        //    sbScript.Append("} \n");
        //    ScriptManager.RegisterClientScriptBlock(ObjPage, ObjPage.GetType(), "RefreshWindow", sbScript.ToString(), true);
        //}

        ///// <summary>
        ///// 關閉視窗
        ///// </summary>
        ///// <param name="ObjPage"></param>
        //public static void CloseWinodw(Page ObjPage)
        //{
        //    string js = "JavaScript:window.close();";
        //    ScriptManager.RegisterClientScriptBlock(ObjPage, ObjPage.GetType(), "CloseWinodw", js, true);
        //}

        #region 警告訊息

        /// <summary>
        /// 在網頁上彈出警告訊息。
        /// </summary>
        /// <param name="message">訊息內容</param>
        public static void Alert(string message)
        {
            //string script = string.Format("alert('{0}');", message.Replace("\n", "\\n"));
            //JavaScrpit("window_aler", script);
            string script = string.Format("swal('{0}');", message.Replace("\n", "\\n"));
            JavaScrpit("window_alertByswal", script);
        }

        /// <summary>
        /// 在網頁上彈出警告訊息。
        /// </summary>
        /// <param name="title">標題</param>
        /// <param name="message">簡述</param>
        /// <param name="status">注意:"warning"、錯誤:"error"、成功:"success"、資訊:"info" </param>
        public static void AlertByswal(string title, string message, string status)
        {
            string script = string.Format("swal('{0}','{1}','{2}');", title, message.Replace("\n", "\\n"), status);
            JavaScrpit("window_alertByswal", script);
        }

        /// <summary>
        /// 在網頁上彈出警告訊息並導頁。
        /// </summary>
        /// <param name="title">標題</param>
        /// <param name="message">簡述</param>
        /// <param name="status">注意:"warning"、錯誤:"error"、成功:"success"、資訊:"info" </param>
        public static void AlertByswalUrl(string title, string message, string status, string url)
        {
            string script = "swal({title:\"" + title + "\",text:\"" + message + "\",type:\"" + status + "\"},function(){window.location=\"" + url + "\";});";
            JavaScrpit("window_alertByswal", script);
        }

        /// <summary>
        /// 在網頁上彈出警告訊息並重新整理父頁。
        /// </summary>
        /// <param name="title">標題</param>
        /// <param name="message">簡述</param>
        /// <param name="status">注意:"warning"、錯誤:"error"、成功:"success"、資訊:"info" </param>
        public static void AlertByswalRed(string title, string message, string status)
        {
            string script = "swal({title:\"" + title + "\",text:\"" + message + "\",type:\"" + status + "\"},function(){parent.location.reload();});";
            JavaScrpit("window_alertByswal", script);
        }



        ///// <summary>
        ///// 在網頁上彈出警告訊息。
        ///// </summary>
        ///// <param name="message">訊息內容</param>
        //public static void AlertByswal(string message)
        //{
        //    string script = string.Format("swal('{0}');", message.Replace("\n", "\\n"));
        //    JavaScrpit("window_alertByswal", script);
        //}

        /// <summary>
        ///  在網頁上彈出警告訊息後，轉頁至新頁面
        /// </summary>
        /// <param name="message">訊息內容</param>
        /// <param name="url">新頁面</param>
        public static void Alert(string message, string url)
        {
            string script = "var fUrlCallBack =function(){ window.location.href='" + url + "';};";
            string MSG = HttpUtility.HtmlEncode(message.Replace("\n", "\\n"));
            script += "swal('" + MSG + "');fUrlCallBack();";
            JavaScrpit("window_aler_redir", script);
        }

        public static void AlertByswal(string title, string message, string status, string url)
        {
            //string script = string.Format("swal('{0}','{1}','{2}',function(){window.location=\"{3}\";});", title, message.Replace("\n", "\\n"), status, url);
            string script = "swal({title:\""+ title + "\",text:\""+ message + "\",type:\""+ status + "\"},function(){window.location=\""+ url + "\";});";
            JavaScrpit("window_alertByswal", script);
        }

        public static void AlertByswalWithHtml(string title, string message, string status, string url)
        {
            //string script = string.Format("swal('{0}','{1}','{2}',function(){window.location=\"{3}\";});", title, message.Replace("\n", "\\n"), status, url);
            string script = "swal({title:\"" + title + "\",text:'" + message + "',type:\"" + status + "\",html:true},function(){window.location=\"" + url + "\";});";
            JavaScrpit("window_alertByswal", script);
        }

        /// <summary>
        /// 在網頁上彈出警告訊息，更新視窗或關閉視窗。
        /// </summary>
        /// <param name="message">訊息內容</param>
        /// <param name="reload">是否更新，否則關閉</param>
        public static void Alert(string message, bool reload)
        {
            string script = string.Format("alert('{0}',0);", HttpUtility.HtmlEncode(message.Replace("\n", "\\n")));
            JavaScrpit("window_aler_reload", script);
        }

        /// <summary>
        /// 彈出此視窗的母網頁重導至url。
        /// </summary>
        /// <param name=url>重導的網址</param>
        public static void OpenrLoad(string url)
        {
            System.Web.UI.Page currentPage = (System.Web.UI.Page)HttpContext.Current.Handler;

            string script = string.Format("nw = window.opener.location.href='{0}';", url);
            JavaScrpit("window_openr_load", script);
        }

        /// <summary>
        /// 重整母親窗並更新至最新狀態。
        /// </summary>
        /// <param name=url>重導的網址</param>
        public static void OpenrReLoad(string OpenrSearchButtonID)
        {
            System.Web.UI.Page currentPage = (System.Web.UI.Page)HttpContext.Current.Handler;

            //string script = string.Format("nw = window.opener.location.reload();");
            StringBuilder sbScript = new StringBuilder();
            if (OpenrSearchButtonID.Length > 0)
            {
                sbScript.AppendLine("if(opener != null) { ");
                sbScript.AppendLine("   if(opener.document.getElementById('" + OpenrSearchButtonID + "') != null) { ");
                sbScript.AppendLine("       var btn = opener.document.getElementById('" + OpenrSearchButtonID + "'); ");
                sbScript.AppendLine("       btn.click(); ");
                sbScript.AppendLine("   } ");
                sbScript.AppendLine("   else ");
                sbScript.AppendLine("   { ");
                sbScript.AppendLine("       opener.location.href = opener.location.href; ");
                sbScript.AppendLine("   } ");
                sbScript.AppendLine("} ");
                sbScript.AppendLine("if(window.parent  != null) { ");
                sbScript.AppendLine("   if(window.parent.document.getElementById('" + OpenrSearchButtonID + "') != null) { ");
                sbScript.AppendLine("       var btn = window.parent.document.getElementById('" + OpenrSearchButtonID + "'); ");
                sbScript.AppendLine("       btn.click(); ");
                sbScript.AppendLine("   } ");
                sbScript.AppendLine("   else ");
                sbScript.AppendLine("   { ");
                sbScript.AppendLine("       window.parent.location.href = window.parent.location.href; ");
                sbScript.AppendLine("   } ");
                sbScript.AppendLine("} ");
            }
            else
            {
                sbScript.AppendLine("if(opener != null) { ");
                sbScript.AppendLine("   opener.location.href = opener.location.href; ");
                sbScript.AppendLine("} ");
                sbScript.AppendLine("if(window.parent != null) { ");
                sbScript.AppendLine("   window.parent.location.href = window.parent.location.href; ");
                sbScript.AppendLine("} ");
            }
            
            
            sbScript.AppendLine(" $(function(){");
            sbScript.AppendLine("   if(window.WWWEditRefresh) WWWEditRefresh(); ");
            sbScript.AppendLine("})");
            JavaScrpit("window_openr_load", sbScript.ToString());
        }


        /// <summary>
        /// 彈出此視窗的母網頁重整並更新至最新狀態
        /// </summary>
        public static string OpenrWindowOpen(string target, int width, int height)
        {
            System.Web.UI.Page currentPage = (System.Web.UI.Page)HttpContext.Current.Handler;

            string jsStr = "";
            jsStr += "var scriptName=document.forms[0].action;";
            jsStr += string.Format("window.document.forms[0].target='{0}';", target);
            jsStr += string.Format("window.document.forms[0].onsubmit=window.open('', '{0}','height={1}, width={2},resizable=1, toolbar=0, directories=0, location=0, menubar=0, personalbar=0, status=0,scrollbars=1, titlebar=0 ');", target, height, width);
            jsStr += "setTimeout(function(){window.document.forms[0].target='';";
            jsStr += "document.forms[0].action=scriptName;}, 500);";

            //string script = string.Format(jsStr);
            //JavaScrpit("window_openr_load", script);

            return jsStr;
        }



        #endregion

        #region 另開視窗

        /// <summary>
        /// 在網頁上開啟新視窗，會依照大小自動置中。
        /// </summary>
        /// <param name="url">開啟的網址</param>
        /// <param name="target">目標</param>
        /// <param name="width">寬度</param>
        /// <param name="height">高度</param>
        public static void WindowOpen(string url, string target, int width, int height)
        {
            //string script = string.Format("nw = window.open('{0}', '{1}','height={2}, width={3},resizable=1, toolbar=0, directories=0, location=0, menubar=0, personalbar=0, status=0,scrollbars=1, titlebar=0 ');", url, target, height, width);
            //script += string.Format("window.nw.moveTo(((screen.availWidth-{0}-10)*.5),((screen.availHeight-{1}-10)*.5));", width, height);
            //script += string.Format("window.nw.focus();");
            //JavaScrpit("window_open" + target, script);
            WindowOpenPopup(url, target, width, height);
        }

        /// <summary>
        /// 在網頁上開啟新視窗，會依照大小自動置中。
        /// </summary>
        /// <param name="url">開啟的網址</param>
        /// <param name="target">目標</param>
        /// <param name="width">寬度</param>
        /// <param name="height">高度</param>
        private static void WindowOpenPopup(string url, string target, int width, int height)
        {
            if (width != 0) { 
                width = width + 80;
            }
            string iFrameName = "_iframe" + target + "_ID";
            StringBuilder sb = new StringBuilder();
            sb.AppendLine(string.Format(@"$.fn.CCMS_OpenDialog('{0}','{1}',{2},{3})", url, target, width,height));            
            JavaScrpit("window_open" + target, "setTimeout(function() { " +  sb.ToString() + " }, 100);");
        }
       
        
    


                    
            
        
      
        /// <summary>
        /// 在網頁上開啟新視窗，會依照大小自動置中。
        /// </summary>
        /// <param name="url">開啟的網址</param>
        /// <param name="target">目標</param>
        /// <param name="width">寬度</param>
        /// <param name="height">高度</param>
        public static void WindowOpen(string url, string target, string width, string height)
        {

            string script = string.Format("nw = window.open('{0}', '{1}','height={2}, width={3},resizable=1, toolbar=0, directories=0, location=0, menubar=0, personalbar=0, status=0,scrollbars=1, titlebar=0 ');", url, target, height, width);
            script += string.Format("window.nw.moveTo(((screen.availWidth-{0}-10)*.5),((screen.availHeight-{1}-10)*.5));", width, height);
            script += string.Format("window.nw.focus();");
            JavaScrpit("window_open" + target, script);
        }

        /// <summary>
        /// 在網頁上開啟新視窗，不指定樣式。
        /// </summary>
        /// <param name="url"></param>
        /// <param name="target"></param>
        public static void WindowOpen(string url, string target)
        {
            JavaScrpit("window_open", string.Format("window.open(\"{0}\", \"{1}\");", url, target));
        }

        /// <summary>
        /// 在網頁上開啟新視窗，樣式自訂。
        /// </summary>
        /// <param name="url"></param>
        /// <param name="target"></param>
        public static void WindowOpen(string url, string target, string style)
        {
            JavaScrpit("window_open", string.Format("window.open(\"{0}\", \"{1}\", \"{2}\");", url, target, style));
        }

        /// <summary>
        /// 觸發母網頁事件
        /// </summary>
        /// <param name="clientcontrolid">母網頁觸發目標id</param>
        /// <param name="eventname">事件名稱</param>
        public static void OpenerFireEvent(string clientcontrolid, string eventname)
        {
            JavaScrpit("fireevent", string.Format("window.opener.document.getElementById('{0}').{1};", clientcontrolid, eventname));
        }

        #endregion

        #region 關閉視窗
        /// <summary>
        /// 關閉本視窗。
        /// </summary>
        public static void WindowClose()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("if(window.parent != null) { ");
            sb.AppendLine("   window.parent.$.prompt.close();");
            sb.AppendLine("} ");
             sb.AppendLine("window.opener=null;window.close();");
             JavaScrpit("window_close", sb.ToString());
        }
        /// <summary>
        /// 關閉本視窗。
        /// </summary>
        public static void closeFancybox()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("if( top.$.fancybox != null) { ");
            sb.AppendLine("   top.$.fancybox.close();");
            sb.AppendLine("} ");
            JavaScrpit("window_close", sb.ToString());
        }

        #endregion

        /// <summary>
        /// 改變字體大小
        /// </summary>
        /// <param name="FontSize">字級</param>
        public static void ChangeFontSize(int FontSize)
        {
            StringBuilder sbScript = new StringBuilder();
            sbScript.Append("$(function () {");
            sbScript.Append("   var ss = document.all ? document.styleSheets[0].rules : document.styleSheets[0].cssRules;  //ie : ff  \n");
            sbScript.Append("   ss[0].style.fontSize = '" + FontSize + "px';");
            sbScript.Append("});");

            JavaScrpit("FontSize_" + DateTime.Now.Ticks, sbScript.ToString());
            //if (!Page.ClientScript.IsClientScriptBlockRegistered("FontSize"))
            //{
            //    Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "FontSize", sb.ToString(), true);
            //}
        }

        public static void JavaScrpit(string name, string script)
        {
            System.Web.UI.Page currentPage = (System.Web.UI.Page)HttpContext.Current.Handler;
            //ASP.Net 2.0請用下面Code
            System.Web.UI.ClientScriptManager cs = currentPage.ClientScript;
            StringBuilder sbScript = new StringBuilder();
            sbScript.Append("$(function () {");
            sbScript.AppendFormat("  {0}", script);
            sbScript.Append("});");

            cs.RegisterClientScriptBlock(currentPage.GetType(), name, sbScript.ToString(), true);

            //ASP.Net 1.X請用下面Code
            //currentPage.RegisterStartupScript(name, "<script language='javascript'>" + script + "</script>");
        }

        public static void OpenNewWindow(string v)
        {
            StringBuilder sbScript = new StringBuilder();
            sbScript.Append("$(function () {");
            sbScript.AppendFormat("   window.open('{0}');",v);
            sbScript.Append("});");

            JavaScrpit("OpenNewWindow" + DateTime.Now.Ticks, sbScript.ToString());
        }
    }
}
