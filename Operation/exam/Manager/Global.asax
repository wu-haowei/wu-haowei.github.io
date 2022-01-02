<%@ Application Language="C#" %>
<%@ Import Namespace="System.Web.Configuration" %>
<%@ Import Namespace="Hamastar.Common.Text" %>
<%@ Import Namespace="NLog" %>
<%@ Import Namespace="Hamastar.BusinessObject" %>


<script RunAt="server">



    void Application_Start(object sender, EventArgs e)
    {
        // 應用程式啟動時執行的程式碼
        //# region 刪除錯誤紀錄
        //try
        //{
        //    int agodays = WebConfig.ErrorLogSaveDays;
        //    string dirpath = WebConfig.ContentPath + "/" + WebConfig.WriteErrorLogDirectory + "/";

        //    System.IO.DirectoryInfo dirinfo = new System.IO.DirectoryInfo(System.Web.HttpContext.Current.Server.MapPath(dirpath));
        //    if (dirinfo.Exists)
        //    {
        //        DateTime SDate = DateTime.Now.AddDays(-agodays);
        //        foreach (System.IO.FileInfo fileinfo in dirinfo.GetFiles())
        //        {
        //            if (fileinfo.LastWriteTime.CompareTo(SDate) == -1)
        //            {
        //                fileinfo.Delete();
        //            }
        //        }
        //    }
        //}
        //catch (Exception ex)
        //{ 
        ////    string err = ex.Message.ToString() + "\n\r";
        ////    err += ex.Source + "\n\r";
        ////    err += ex.StackTrace + "\n\r";

        ////    // Log the error
        ////    ErrHandler.WriteError(err);
        //}
        //# endregion
        ScriptManager.ScriptResourceMapping.AddDefinition("jquery",
     new ScriptResourceDefinition
     {
         Path = "~/scripts/jquery-3.6.0.min.js",
         DebugPath = "~/scripts/jquery-3.6.0.min.js",
     });

    }



    void Application_End(object sender, EventArgs e)
    {

    }

    void Application_Error(object sender, EventArgs e)
    {
        if (Request.ServerVariables["SERVER_NAME"] != "localhost")
        {
            Exception objErr = Server.GetLastError().GetBaseException();
            string err = objErr.ToString() + "\n\r";
            err += objErr.Source + "\n\r";
            err += objErr.StackTrace + "\n\r";

            //Logger logger = LogManager.GetCurrentClassLogger();
            //logger.Error(err);
            Server.Transfer("Error.aspx");
        }
    }

    void Session_Start(object sender, EventArgs e)
    {
        // 啟動新工作階段時執行的程式碼

    }

    void Session_End(object sender, EventArgs e)
    {
 
    }

    protected void Application_BeginRequest()
    {
    }
    protected void Application_EndRequest()
    {
    }

</script>
