<%@ WebHandler Language="C#" Class="DownFile" %>

using System;
using System.Web;

using Hamastar.BusinessObject;
public class DownFile : IHttpHandler
{

    public void ProcessRequest(HttpContext context)
    {
        try
        {
            string user_ip = Tools.GetIP();
            if (!Tools.IsWhiteIP(user_ip))
            {
                context.Response.Write("你的IP不是合法IP");
                return;
            }
            int SN = 0;
            int.TryParse(context.Request.QueryString["s"] as string, out SN); //Common/DownFile.ashx?s=xx
            if (SN == 0)
            {
                context.Response.Write("錯誤的檔案代號");
                return;
            }
            Comm_RelFile cf = Comm_RelFile.GetSingle(x => x.SN == SN);
            string FileName = cf.FileName;
          //  FileName = FileName.Replace($"{cf.ParentSN}.h", cf.SrcFileName);
            string readl_filepath = System.Web.HttpContext.Current.Server.MapPath(FileName);
            Byte[] File = System.IO.File.ReadAllBytes(readl_filepath);
            context.Response.Buffer = true;
            context.Response.Clear();
            context.Response.ContentType = "application/download";
            context.Response.AppendHeader("Content-Length", File.Length.ToString());
            context.Response.AddHeader("Content-Disposition", "attachment;   filename=" + HttpUtility.UrlEncode(cf.SrcFileName, System.Text.Encoding.UTF8) + ";");
            context.Response.BinaryWrite(File);
            context.Response.Flush();
            context.Response.End();
        }
        catch (Exception ex)
        {
        }
    }

    public bool IsReusable
    {
        get
        {
            return false;
        }
    }

}