using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.IO;
using System.Web.SessionState;
using Ionic.Zip;
using Hamastar.Common.Security;
using System.Text;
using System.Configuration;
using System.Web.Configuration;
using System.Net.Configuration;
using System.Net.Mail;
using System.Net;
using Hamastar.BusinessObject;
using System.Drawing.Imaging;
using System.Drawing;
using NPOI.HSSF.UserModel;
using System.Data;
using System.Collections;
using Microsoft.Security.Application;
using System.Reflection;
using System.ComponentModel;
using System.Text.RegularExpressions;
using Hamastar.Common;
using NPOI.SS.UserModel;
using Hamastar.Common.Text;
using System.Web.Services;
using Cht.Component.HiairGw.Sms;
using NPOI.SS.Util;

/// <summary>
/// Tools 的摘要描述
/// </summary>
public class Tools
{
    public static string LogFolderPath = System.Web.HttpContext.Current.Server.MapPath("/logs/");
    public Tools()
    {
        //
        // TODO: 在此加入建構函式的程式碼
        //
    }

    /// <summary>
    ///系統代碼選項類型
    /// </summary>
    public enum KindOption : int
    {
        單選 = 0,
        複選 = 1,
        下拉 = 2
    }

    public enum altertType
    {
        [Description("warning")]
        注意 = 0,
        [Description("error")]
        錯誤 = 1,
        [Description("success")]
        成功 = 2,
        [Description("info")]
        資訊 = 3,
    }

    /// <summary>
    /// 檢查IP是否為白名單
    /// </summary>
    /// <param name="IP"></param>
    /// <returns></returns>
    public static bool IsWhiteIP(string IP)
    {
        if (IP == null)
            return false;


        IPList iplist = new IPList();
        List<string> SN = Comm_Department.GetList(x => x.Status == 1).Select(x => x.SN).ToList();
        List<string> sIPList = Comm_Department_IP.GetList(x => SN.Contains(x.DeptSN)).Select(x => x.IP).ToList();
        sIPList.Add("::1");//localhost
        sIPList.Add("192.168.160.*");//哈瑪星內部ip
        sIPList.Add("61.221.174.157");//哈瑪星外部ip

        //sIPList = new string[] { "192.168.*.*", "194.*.*.*", "193.4.4.*" };
        foreach (var ip in sIPList)
        {
            int maskLevel = 0;
            switch (ip.Split('*').Count() - 1)
            {
                case 1:
                    maskLevel = 24;
                    break;
                case 2:
                    maskLevel = 16;
                    break;
                case 3:
                    maskLevel = 8;
                    break;
                default:
                    iplist.Add(ip);
                    break;
            }
            if (maskLevel > 0)
            {
                iplist.Add(ip.Replace("*", "1"), maskLevel);

            }

        }
        return (IP != null && iplist.CheckNumber(IP.Replace("::ffff:", "")));
    }

    public static string GetIP()
    {
        System.Web.HttpContext context = System.Web.HttpContext.Current;
        string ipAddress = context.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];

        if (!string.IsNullOrEmpty(ipAddress))
        {
            string[] addresses = ipAddress.Split(',');
            if (addresses.Length != 0)
            {
                return addresses[0];
            }
        }

        return context.Request.ServerVariables["REMOTE_ADDR"];
    }

    //取得經base64編碼後的字串
    public static string GetBase64FileToStr(string FilePath, bool isLocalfile)
    {
        string file = "";
        if (isLocalfile)
        {
            Byte[] bytes = File.ReadAllBytes(FilePath);
            file = Convert.ToBase64String(bytes);
        }
        else//weburl
        {
            byte[] data;
            using (var webClient = new WebClient())
                data = webClient.DownloadData(FilePath);
            file = Convert.ToBase64String(data);
        }
        return file;
    }

    //讀取經base64編碼後轉回成檔案
    public static void GetBase64StrToFile(string b64Str, string FilePath)
    {
        Byte[] bytes = Convert.FromBase64String(b64Str);
        File.WriteAllBytes(FilePath, bytes);
    }
    private static string CleanFileName(string fileName)
    {
        return Path.GetInvalidFileNameChars().Aggregate(fileName, (current, c) => current.Replace(c.ToString(), string.Empty));
    }



    #region datatable 轉excel
    public static void ExportDataTableToExcel(Page oPage, DataTable pDataTable, string FileName)  //將DataTable匯出至Excel
    {

        int tRowCount = pDataTable.Rows.Count;
        int tColumnCount = pDataTable.Columns.Count;

        oPage.Response.Expires = 0;
        oPage.Response.Clear();
        oPage.Response.Buffer = true;
        oPage.Response.Charset = "utf-8";
        oPage.Response.ContentEncoding = System.Text.Encoding.UTF8;
        oPage.Response.ContentType = "application/vnd.ms-excel";
        FileName = FileName.Replace("/", "-");
        //產生Excel物件、工作表
        HSSFWorkbook workbook = new HSSFWorkbook();
        HSSFSheet u_sheet = (HSSFSheet)workbook.CreateSheet(CleanFileName(FileName));

        // Title列
        NPOI.SS.UserModel.IRow titleRow = u_sheet.CreateRow(0);

        NPOI.SS.UserModel.ICellStyle boldStyle = workbook.CreateCellStyle();

        boldStyle.BorderTop = NPOI.SS.UserModel.BorderStyle.Medium;
        boldStyle.BorderRight = NPOI.SS.UserModel.BorderStyle.Medium;
        boldStyle.BorderLeft = NPOI.SS.UserModel.BorderStyle.Medium;
        boldStyle.BorderBottom = NPOI.SS.UserModel.BorderStyle.Medium;
        for (int i = 0; i < tColumnCount; i++)
        {
            NPOI.SS.UserModel.ICell oICell = titleRow.CreateCell(i);
            oICell.SetCellValue(pDataTable.Columns[i].ColumnName);
            oICell.CellStyle = boldStyle;
        }

        // 資料列
        for (int j = 0; j < tRowCount; j++)
        {
            NPOI.SS.UserModel.IRow dataRow = u_sheet.CreateRow(j + 1);
            for (int k = 0; k < tColumnCount; k++)
            {
                string Value = pDataTable.Rows[j][k].ToString();
                ICell oCell = dataRow.CreateCell(k);
                oCell.SetCellValue(Value);
                oCell.CellStyle.VerticalAlignment = VerticalAlignment.Center;
                if (Value.Split(System.Environment.NewLine.ToArray()).Length > 0)
                {
                    oCell.CellStyle.WrapText = true;
                    // dataRow.HeightInPoints = Value.Split(System.Environment.NewLine.ToArray()).Length * dataRow.HeightInPoints / 20;
                }
            }
        }

        //輸入內容
        MemoryStream ms = new MemoryStream();
        workbook.Write(ms);
        string FullFileName = HttpContext.Current.Server.UrlPathEncode(FileName + DateTime.Now.ToShortDateString() + ".xls");
        HttpUtility.UrlEncode(FullFileName, System.Text.Encoding.UTF8);
        oPage.Response.AppendHeader("Content-Length", ms.ToArray().Length.ToString());
        oPage.Response.AddHeader("Content-Disposition", "attachment; filename=" + FullFileName);
        oPage.Response.BinaryWrite(ms.ToArray());
        //釋放資源
        workbook = null;
        ms.Close();
        ms.Dispose();
        oPage.Response.Flush();
        oPage.Response.End();
    }
    #endregion

    #region npoi datatable 轉excel
    /// <summary>
    /// npoi datatable 轉excel
    /// </summary>
    /// <param name="來源資料"></param>
    /// <param name="欄位名稱"></param>
    /// <param name="是否要自動欄寬"></param>
    /// <returns></returns>
    public static Stream ExportDataTableToExcel(DataTable SourceTable, string[] columnname, bool IsAutoSize = true)
    {
        HSSFWorkbook workbook = new HSSFWorkbook();
        MemoryStream ms = new MemoryStream();
        HSSFSheet sheet = (HSSFSheet)workbook.CreateSheet();
        HSSFRow headerRow = (HSSFRow)sheet.CreateRow(0);
        //styling
        IFont boldFont = workbook.CreateFont();
        boldFont.Boldweight = (short)FontBoldWeight.Bold;
        ICellStyle boldStyle = workbook.CreateCellStyle();
        boldStyle.SetFont(boldFont);
        //標頭樣式
        boldStyle.BorderBottom = NPOI.SS.UserModel.BorderStyle.Medium;
        boldStyle.FillForegroundColor = NPOI.HSSF.Util.HSSFColor.Grey25Percent.Index;
        boldStyle.FillPattern = FillPattern.SolidForeground;
        // handling header.
        int i = 0;
        foreach (string column in columnname)
        {
            ICell cell = headerRow.CreateCell(i);
            cell.SetCellValue(column);
            // cell.CellStyle = boldStyle;
            i = i + 1;
        }
        // handling value.
        int rowIndex = 1;

        foreach (DataRow row in SourceTable.Rows)
        {
            HSSFRow dataRow = (HSSFRow)sheet.CreateRow(rowIndex);
            foreach (DataColumn column in SourceTable.Columns)
            {
                dataRow.CreateCell(column.Ordinal).SetCellValue(row[column].ToString());
                if (row[column].ToString().Contains("\n"))
                {
                    //將目前欄位的CellStyle設定為自動換行
                    ICellStyle cs = workbook.CreateCellStyle();
                    cs.WrapText = true;
                    dataRow.GetCell(column.Ordinal).CellStyle = cs;

                    //因為換行所以預設幫他Row的高度變成兩倍
                    dataRow.HeightInPoints = 2 * sheet.DefaultRowHeight / 20;
                }
            }
            rowIndex++;
        }
        //自動欄寬
        if (IsAutoSize)
        {
            for (int x = 0; x < SourceTable.Columns.Count; x++)
                sheet.AutoSizeColumn(x);
        }
        //凍結窗格-標頭列
        //sheet.CreateFreezePane(0, 1, 0, 1);

        workbook.Write(ms);
        ms.Flush();
        ms.Position = 0;
        sheet = null;
        headerRow = null;
        workbook = null;
        return ms;
    }
    #endregion

    #region AntiXss https://www.evernote.com/shard/s288/sh/e90908c1-6e2c-42d4-bf1e-086bc5906bb4/091870253c9e7ef9a6c62e8d4eb04925
    //請先將 AntiXSSLibrary.dll,HtmlSanitizationLibrary.dll 加入參考,放在BusinessObject\DLL
    public static string AntiXss_URL(string strURL)
    {
        //檢查 Resource Injection, LDAP Injection, Open Redirect, SQL Injection, Tag Injection, Cross-Site Scripting
        string AntiString = string.Empty;
        //檢查是否傳入網址
        if (strURL.IndexOf('?') != -1)
        {
            AntiString = Microsoft.Security.Application.Encoder.HtmlAttributeEncode(strURL.Substring(0, strURL.IndexOf('?')));
            strURL = strURL.Substring(strURL.IndexOf('?'), strURL.Length - AntiString.Length);
        }
        //排除&符號
        if (strURL.IndexOf('&') != -1)
        {
            string[] AllQuery = strURL.Split('&');
            foreach (string str in AllQuery)
            {
                if (str.IndexOf('?') == 0)
                    AntiString += Microsoft.Security.Application.Sanitizer.GetSafeHtmlFragment(str);
                else
                    AntiString += "&" + Microsoft.Security.Application.Sanitizer.GetSafeHtmlFragment(str);
            }
        }
        else if (strURL.IndexOf('?') == 0) // 是網址, 但沒有&符號 (單參數的網址)
        {
            AntiString += Microsoft.Security.Application.Sanitizer.GetSafeHtmlFragment(strURL);
        }
        if (string.IsNullOrEmpty(AntiString))
            AntiString = Microsoft.Security.Application.Sanitizer.GetSafeHtmlFragment(strURL);
        return AntiString;
    }
    #endregion



    /// <summary>
    /// 檔案ContentType轉檔名 
    /// </summary>
    /// <param name="ContentType"></param>
    /// <returns></returns>
    public static string ContentTypeToExtention(string ContentType)
    {
        switch (ContentType)
        {
            case "text/plain": return ".txt";
            case "application/vnd.openxmlformats-officedocument.wordprocessingml.document": return ".docx";
            case "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet": return ".xlsx";
            case "application/vnd.ms-excel": return ".xls";
            case "application/msword": return ".doc";
            case "application/vnd.ms-powerpoint": return ".ppt";
            case "application/x-msaccess": return ".mdb";
            case "application/x-zip-compressed": return ".zip";
            case "image/jpeg": return ".jpg";
            case "image/pjpeg": return ".jpg";
            case "image/tiff": return ".tiff";
            case "image/bmp": return ".bmp";
            case "image/gif": return ".gif";
            case "image/x-png": return ".png";
            case "video/x-msvideo": return ".avi";
            case "video/mpeg": return ".mpeg";
            case "application/x-shockwave-flash": return ".swf";
            case "image/png": return ".png";
            case "video/mp4": return ".mp4";
            case "application/octet-stream": return ".flv";
            case "application/vnd.openxmlformats-officedocument.presentationml.presentation": return ".pptx";
            case "application/pdf": return ".pdf";
            case "video/x-ms-wmv": return ".wmv";
            case "video/avi": return ".avi";

            default: return (null);
        }
    }

    /// <summary>
    /// 檢查副檔名
    /// </summary>
    /// <param name="ContentType"></param>
    /// <param name="AllowExtentions"></param>
    /// <returns></returns>
    public static bool CheckExtention(string ContentType, string[] AllowExtentions)
    {
        foreach (string Extention in AllowExtentions)
        {
            if (Extention.Equals(ContentType))
            {
                return true;
            }
        }

        return false;
    }

    #region 取得目前網址中指定的站台路徑名稱
    public static string get_ContentPath(Page ObjPage)
    {
        return get_ContentPath(ObjPage.Request);
    }
    public static string get_ContentPath(HttpContext context)
    {
        return get_ContentPath(context.Request);
    }
    public static string get_ContentPath(HttpRequest Request)
    {
        string strContentPath = "";
        string src = Request.PhysicalPath.Replace("\\", "@");
        string input = "";// WebConfig.SubSitePath.Replace("\\", "@");
        string[] findPath = Regex.Replace(src, input, "", RegexOptions.IgnoreCase).Split(Convert.ToChar("@"));
        strContentPath = findPath[0];
        return Sanitizer.GetSafeHtmlFragment(strContentPath);
    }
    #endregion

    #region 利用系統Mail發通知信
    public static bool Send_Mail(Page objPage, bool pIsHTML, string pToMail, string pToName, string pSubject, string pBody, string[] AttachmentPathList)
    {
        Hamastar.Common.Net.EasyEmail mail = new Hamastar.Common.Net.EasyEmail();
        Configuration config = WebConfigurationManager.OpenWebConfiguration(objPage.Request.ApplicationPath);
        //取得Web.config中Mail設定區段
        MailSettingsSectionGroup mailSettings = (MailSettingsSectionGroup)config.GetSectionGroup("system.net/mailSettings");
        mail.SmtpServer = mailSettings.Smtp.Network.Host;
        mail.SmtpAccount = mailSettings.Smtp.Network.UserName;
        mail.SmtpPassword = mailSettings.Smtp.Network.Password;

        mail.Subject = pSubject;
        if (pIsHTML)
            mail.HtmlBody = pBody.Replace("\n\r", "<br/>").Replace("\r\n", "<br/>");
        else
            mail.TextBody = pBody;

        mail.FromEmail = WebConfig.EmailAccountExternal;
        mail.FromName = WebConfig.MailFromTitleExternal;

        mail.ToEmail = pToMail;
        mail.ToName = pToName;

        if (AttachmentPathList != null)
        {
            mail.Attachments = AttachmentPathList.ToList();
        }
        try
        {
            mail.SendMail();
            return true;
        }
        catch
        {
            return false;
        }
    }
    public bool m_MailTo(Page objPage, bool pIsHTML, string pToMail, string pToName, string pSubject, string pBody, string pCC, string pBCC, string pAttach)
    {
        return m_MailTo(objPage, pIsHTML, "", "", pToMail, pToName, pSubject, pBody, pAttach, pCC, pBCC);
    }
    public bool m_MailTo(Page objPage, bool pIsHTML, string pFromName, string pFromEmail, string pToMail, string pToName, string pSubject, string pBody, string pAttach, string pCC, string pBCC)
    {
        if (pToMail.Trim() == "") return false;
        bool result = false;
        //使用方法 aspx 第一行加入 Async="true"
        //開啓Request所在路徑網站的Web.config檔
        Configuration config = WebConfigurationManager.OpenWebConfiguration(objPage.Request.ApplicationPath);
        //取得Web.config中Mail設定區段
        MailSettingsSectionGroup mailSettings = (MailSettingsSectionGroup)config.GetSectionGroup("system.net/mailSettings");
        string host = "";
        string password = "";
        string account = "";
        int port = 0;
        if (mailSettings != null)
        {
            port = mailSettings.Smtp.Network.Port;
            host = mailSettings.Smtp.Network.Host;
            password = mailSettings.Smtp.Network.Password;
            account = mailSettings.Smtp.Network.UserName;

            if (pFromName == "") pFromName = WebConfig.MailFromTitleExternal;
            if (pFromEmail == "") pFromEmail = WebConfig.EmailAccountExternal;

            foreach (string toMail in pToMail.Split(','))
            {
                if (toMail.Trim() != "")
                {
                    result = m_SendMail(pFromName, pFromEmail, host, account, password, toMail, pToName, pSubject, pBody, pAttach, objPage, pCC, pBCC, pIsHTML);
                }
            }
        }

        return result;
    }
    public bool m_SendMail(string MailFromTitle, string pFromMail, string MailSmtp, string MailName, string MailPassword, string pToMail, string pToName, string pSubject, string pBody, string pAttach, Page objPage, string pCC, string pBCC, bool pIsHTML)
    {
        Encoding strCode = Encoding.GetEncoding("UTF-8");
        //mail
        if (string.IsNullOrEmpty(MailFromTitle)) MailFromTitle = WebConfigurationManager.AppSettings["MailFromTitle"].ToString();
        MailAddress mFrom = new MailAddress(pFromMail, MailFromTitle, strCode);
        MailAddress mTo = new MailAddress(pToMail, pToName, strCode);

        MailMessage mail = new MailMessage(mFrom, mTo);

        mail.Subject = pSubject.Replace("\r", "").Replace("\n", "");
        mail.SubjectEncoding = strCode;
        if (pIsHTML) pBody = pBody.Replace("\n\r", "<br/>").Replace("\r\n", "<br/>");

        mail.Body = pBody;
        mail.BodyEncoding = strCode;
        mail.IsBodyHtml = pIsHTML;
        //mail.DeliveryNotificationOptions = true;

        #region real path
        string file = pAttach;
        if (!string.IsNullOrEmpty(file))
        {
            // Create  the file attachment for this e-mail message.
            Attachment data = new Attachment(file);
            // Add the file attachment to this e-mail message.
            mail.Attachments.Add(data);
        }
        #endregion

        #region cc
        if (!string.IsNullOrEmpty(pCC))
        {
            mail.CC.Add(pCC);
        }
        if (!string.IsNullOrEmpty(pBCC))
        {
            mail.Bcc.Add(pBCC);
        }
        #endregion

        //smtp
        SmtpClient smtp = new SmtpClient(MailSmtp);
        //smtp.Credentials = new NetworkCredential("nikola", "610814hm");
        smtp.Credentials = new NetworkCredential(MailName, MailPassword);
        //send completed event
        smtp.SendCompleted += new SendCompletedEventHandler(smtp_SendCompleted);

        //send
        bool vChk = true;
        try
        {
            smtp.SendAsync(mail, null);
        }
        catch (Exception ex)
        {
            vChk = false;
            throw new Exception(ex.InnerException.Message);
        }
        return vChk;
    }
    void smtp_SendCompleted(object sender, System.ComponentModel.AsyncCompletedEventArgs e)
    {
        if (e.Error != null)
            throw new Exception(e.Error.InnerException.Message);
    }
    #endregion

    #region Enter轉換斷行
    public static string strReplace(string prVal)
    {
        if (!string.IsNullOrEmpty(prVal))
            return prVal.Replace("\n", "<br>").Replace(" ", "&nbsp;&nbsp;");
        else
            return prVal;
    }
    #endregion

    #region 檢查字串取代惡意字元
    /// <summary>
    /// 用来檢测使用者输入是否带有惡意字元
    /// </summary>
    /// <param name="text">使用者输入的文字</param>
    /// <param name="maxlength">最大的長度</param>
    /// <returns>返回過濾後的文字</returns>
    public static string InputText(string text, int maxlength)
    {
        text = text.ToLower().Trim();
        if (string.IsNullOrEmpty(text))
            return string.Empty;
        if (text.Length > maxlength)
            text = text.Substring(0, maxlength);

        text = Regex.Replace(text, "[\\s]{2,{", " ");
        text = Regex.Replace(text, "(<[b|B][r|R]/*>)+|(<[p|P](.|\\n)*?>)", "\n"); //<br>
        text = Regex.Replace(text, "(\\s*&[n|N][b|B][s|S][p|P];\\s*)+", " "); //&nbsp;
        text = Regex.Replace(text, "<(.|\\n)*?>", string.Empty); //any other tags
        text = Regex.Replace(text, "=", "");
        text = Regex.Replace(text, "%", "");
        text = Regex.Replace(text, "'", "");
        text = Regex.Replace(text, "select", "");
        text = Regex.Replace(text, "insert", "");
        text = Regex.Replace(text, "delete", "");
        text = Regex.Replace(text, "or", "");
        text = Regex.Replace(text, "exec", "");
        text = Regex.Replace(text, "--", "");
        text = Regex.Replace(text, "and", "");
        text = Regex.Replace(text, "where", "");
        text = Regex.Replace(text, "update", "");
        text = Regex.Replace(text, "script", "");
        text = Regex.Replace(text, "iframe", "");
        text = Regex.Replace(text, "master", "");
        text = Regex.Replace(text, "exec", "");
        text = Regex.Replace(text, "<", "");
        text = Regex.Replace(text, ">", "");
        text = Regex.Replace(text, "\r\n", "");

        return text;
    }
    #endregion

    #region 圖片等比例縮放
    public static string DisplayPic(string PictureFilePath, bool fix, string WidthLim, string HeightLim)
    {
        Bitmap imgGif = null;
        if (PictureFilePath.ToLower().IndexOf("http:") > -1)
        {
            System.Net.WebRequest request =
            System.Net.WebRequest.Create(PictureFilePath);
            System.Net.WebResponse response = request.GetResponse();
            System.IO.Stream responseStream =
                response.GetResponseStream();
            Bitmap bitmap2 = new Bitmap(responseStream);
            imgGif = bitmap2;
        }
        else
        {
            PictureFilePath = HttpContext.Current.Server.MapPath(PictureFilePath);
            imgGif = new Bitmap(PictureFilePath);
        }

        if (fix)
        {
            return WidthLim + ";" + HeightLim;
        }
        else
        {
            int NewWidth = 0;
            int NewHeight = 0;
            if (WidthLim != "")
            {
                Int32.TryParse(WidthLim, out NewWidth);
            }
            if (HeightLim != "")
            {
                Int32.TryParse(HeightLim, out NewHeight);
            }

            try
            {
                using (Bitmap bmp = imgGif)
                {
                    int PicWidth = bmp.Width;
                    int PicHeight = bmp.Height;

                    if (NewWidth <= PicWidth || NewHeight <= PicHeight)
                    {
                        if (NewWidth != 0 && NewHeight == 0)
                        {
                            NewHeight = Convert.ToInt32((PicHeight * NewWidth) / PicWidth);
                        }
                        else if (NewWidth == 0 && NewHeight != 0)
                        {
                            NewWidth = Convert.ToInt32((PicWidth * NewHeight) / PicHeight);
                        }
                        else if (NewWidth != 0 && NewHeight != 0)
                        {
                            if ((PicWidth / PicHeight) >= (NewWidth / NewHeight))
                            {
                                if (Convert.ToInt32((PicHeight * NewWidth) / PicWidth) > NewHeight)
                                {
                                    NewWidth = Convert.ToInt32((PicWidth * NewHeight) / PicHeight);
                                    //NewHeight = Convert.ToInt32((PicHeight * NewWidth) / PicWidth);
                                }
                                else
                                {
                                    NewHeight = Convert.ToInt32((PicHeight * NewWidth) / PicWidth);
                                    //NewWidth = Convert.ToInt32((PicWidth * NewHeight) / PicHeight);
                                }
                            }
                            else
                            {
                                if (Convert.ToInt32((PicWidth * NewHeight) / PicHeight) > NewWidth)
                                {
                                    NewHeight = Convert.ToInt32((PicHeight * NewWidth) / PicWidth);
                                    //NewWidth = Convert.ToInt32((PicWidth * NewHeight) / PicHeight);
                                }
                                else
                                {
                                    NewWidth = Convert.ToInt32((PicWidth * NewHeight) / PicHeight);
                                    //NewHeight = Convert.ToInt32((PicHeight * NewWidth) / PicWidth);
                                }
                            }
                        }
                        else if (NewWidth == 0 && NewHeight == 0)
                        {
                            NewWidth = PicWidth;
                            NewHeight = PicHeight;
                        }
                    }
                    else
                    {
                        NewWidth = PicWidth;
                        NewHeight = PicHeight;
                    }

                    return NewWidth + ";" + NewHeight;
                }
            }
            catch (InvalidOperationException ex)
            {
                string strError = ex.Message;
                return WidthLim + ";" + HeightLim;
            }
            catch (Exception ex)
            {
                string strError = ex.Message;
                return WidthLim + ";" + HeightLim;
            }
        }
    }
    #endregion


    public static string String2Base64(string p)
    {
        byte[] bytes = System.Text.Encoding.GetEncoding("utf-8").GetBytes(p);
        return HttpContext.Current.Server.UrlEncode(Convert.ToBase64String(bytes));
    }

    public static List<string> GetServerIP()
    {
        StringBuilder sResult = new StringBuilder();
        IPHostEntry ipHostInfo = Dns.GetHostEntry(Dns.GetHostName());
        return ipHostInfo.AddressList.Select(o => o.ToString()).ToList<string>();
    }

    /// <summary>
    /// AES加密
    /// </summary>
    /// <param name="Text">加密字符</param>
    /// <returns></returns>
    public static string EncryptAES(string Text)
    {
        string key = WebConfig.AesKey;
        return Aes.EncryptCustom(key, Text);
    }

    /// <summary>
    /// AES解密
    /// </summary>
    /// <param name="Text">解密字符</param>
    /// <returns></returns>
    public static string DecryptAES(string Text)
    {
        try
        {
            string key = WebConfig.AesKey;

            return Aes.DecryptCustom(key, Text);
        }
        catch
        {
            return Text;
        }
    }



    public static void closeWinodw(Page objPage)
    {
        Hamastar.Common.Text.Pages.WindowClose();
    }
    public static void closeFancybox()
    {
        Hamastar.Common.Text.Pages.closeFancybox();
    }



    //複製檔案
    public static void CopyFile(string srcFile, string dstFile)
    {
        System.IO.FileInfo fi = new FileInfo(dstFile);
        string dstFolder = fi.Directory.ToString();
        //檢查目錄是否存
        if (!System.IO.Directory.Exists(dstFolder))
        {
            System.IO.Directory.CreateDirectory(dstFolder);//建立目錄
        }
        try
        {
            if (System.IO.File.Exists(dstFile)) System.IO.File.Delete(dstFile);
            System.Threading.Thread.Sleep(1000); // 1 sec
            System.IO.File.Copy(srcFile, dstFile);
        }
        catch { }
    }
    //刪除目錄下所有檔案
    public static void DeleteFile(string dstFolder)
    {
        if (!System.IO.Directory.Exists(dstFolder)) return;

        System.IO.DirectoryInfo dstDirectory = new System.IO.DirectoryInfo(dstFolder);
        foreach (System.IO.FileInfo fi in dstDirectory.GetFiles())
        {
            try
            {
                //先將所有的檔案屬性設定成一般 免得有的是唯讀     
                fi.Attributes = System.IO.FileAttributes.Normal;
                fi.Delete();
            }
            catch { }
        }
        //遞迴向下探索刪除
        foreach (System.IO.DirectoryInfo dr in dstDirectory.GetDirectories())
        {
            try
            {
                DeleteFile(dr.FullName);
                dr.Delete();
            }
            catch { }
        }

    }

    #region 將temp資料夾中日期為今天以前的刪除
    public static void DeleteYesterdayTemp(string delpath)
    {
        string tempPath = HttpContext.Current.Server.MapPath("/" + delpath);

        //刪除檔案
        string[] files = Directory.GetFiles(tempPath);

        foreach (string file in files)
        {
            FileInfo fi = new FileInfo(file);
            //先將檔案的屬性設定成一般 免得有的是唯讀     
            fi.Attributes = System.IO.FileAttributes.Normal;
            if (fi.LastAccessTime < DateTime.Now.AddDays(-1))
                fi.Delete();
        }
        //刪除資料夾
        string[] di = Directory.GetDirectories(tempPath);

        foreach (string file in di)
        {
            DirectoryInfo fi = new DirectoryInfo(file);
            //先將資料夾的屬性設定成一般 免得有的是唯讀     
            fi.Attributes = System.IO.FileAttributes.Normal;
            if (fi.LastAccessTime < DateTime.Now.AddDays(-1))
                fi.Delete(true);
        }
    }













    #endregion

    #region 將temp資料夾中日期為今天以前的刪除
    public static void DelFile(string delpath)
    {
        string tempPath = HttpContext.Current.Server.MapPath("/" + delpath);

        //刪除檔案
        FileInfo fi = new FileInfo(tempPath);
        //先將檔案的屬性設定成一般 免得有的是唯讀     
        fi.Attributes = System.IO.FileAttributes.Normal;
        fi.Delete();
    }

    #endregion

    /// <summary>
    /// 網址本身要排除的參數
    /// </summary>
    /// <param name="Parameter"></param>
    /// <returns></returns>
    public static string GetURL(string Parameter)
    {
        System.Web.UI.Page currentPage = (System.Web.UI.Page)HttpContext.Current.Handler;
        string Path = currentPage.Request.Url.AbsolutePath;
        string[] AllQuery = currentPage.Request.Url.Query.Split('&');
        foreach (string str in AllQuery)
        {
            if (!str.StartsWith(Parameter) || Parameter.Equals(string.Empty))
            {
                if (str.IndexOf('?') == 0)
                {
                    Path += str;
                }
                else
                {
                    Path += "&" + str;
                }
            }
        }
        return Sanitizer.GetSafeHtmlFragment(Path).Replace("&amp;", "&");
    }

    /// <summary>
    /// 連結傳入URL並過濾排除的參數
    /// </summary>
    /// <param name="Url"></param>
    /// <param name="Parameter"></param>
    /// <returns></returns>
    public static string GetURL(string Url, string Parameter)
    {
        System.Web.UI.Page currentPage = (System.Web.UI.Page)HttpContext.Current.Handler;
        string Path = Url;
        string[] AllQuery = currentPage.Request.Url.Query.Split('&');
        foreach (string str in AllQuery)
        {
            if (!str.StartsWith(Parameter) || Parameter.Equals(string.Empty))
            {
                if (str.IndexOf('?') == 0)
                {
                    Path += str;
                }
                else
                {
                    Path += "&" + str;
                }
            }
        }
        return Sanitizer.GetSafeHtmlFragment(Path).Replace("&amp;", "&");
    }

    #region 寄送Mail(非同步)
    private System.Net.Configuration.MailSettingsSectionGroup netSmtpMailSection;
    private string gm_ManagerMail = string.Empty;
    public void Async_SendMail(Page objPage, string ManagerMail, string WorkTitle, string[] pToMail, string[] pSubject, string[] pStrBody, string[] pAttach, bool pIsHTML)
    {
        System.Configuration.Configuration config = System.Web.Configuration.WebConfigurationManager.OpenWebConfiguration(objPage.Request.ApplicationPath);
        netSmtpMailSection = (MailSettingsSectionGroup)config.GetSectionGroup("system.net/mailSettings");
        gm_ManagerMail = ManagerMail;
        AsyncSendMail delt = new AsyncSendMail(SendMailByAsync);
        AsyncCallback acb = new AsyncCallback(Alert_Manager);
        IAsyncResult isr = delt.BeginInvoke(WorkTitle, pToMail, pSubject, pStrBody, pAttach, pIsHTML, acb, delt);
    }
    public delegate string AsyncSendMail(string WorkTitle, string[] pToMail, string[] pSubject, string[] pBody, string[] pAttach, bool pIsHTML);
    public string SendMailByAsync(string WorkTitle, string[] pToMail, string[] pSubject, string[] pBody, string[] pAttach, bool pIsHTML)
    {
        StringBuilder sb = new StringBuilder();
        System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
        sb.AppendFormat("<p>排程名稱：" + WorkTitle + "</p>");
        sb.AppendFormat("<p>起始時間 - {0} - </p>", DateTime.Now);
        sb.AppendFormat("<p>總共{0}封郵件<p>", pToMail.Length - 1);
        int Success = 0;
        int Failure = 0;
        //取得Web.config中Mail設定區段
        SmtpClient emailClient = new SmtpClient(netSmtpMailSection.Smtp.Network.Host);
        emailClient.Credentials = new NetworkCredential(netSmtpMailSection.Smtp.Network.UserName, netSmtpMailSection.Smtp.Network.Password);
        sw.Reset();
        sw.Start();

        for (int i = 0; i < pToMail.Length; i++)
        {
            if (string.IsNullOrEmpty(pToMail[i]))
                continue;
            string StrBody = pBody[i];
            if (pIsHTML) StrBody = StrBody.Replace("\n\r", "<br/>").Replace("\r\n", "<br/>");
            try
            {
                MailMessage message = new MailMessage()
                {
                    Body = StrBody,
                    BodyEncoding = Encoding.UTF8,
                    From = new MailAddress(WebConfig.EmailAccountExternal, WebConfig.MailFromTitleExternal),
                    IsBodyHtml = pIsHTML,
                    Subject = pSubject[i],
                    SubjectEncoding = Encoding.UTF8
                };
                string file = pAttach[i];
                if (!string.IsNullOrEmpty(file))
                {
                    //Create  the file attachment for this e-mail message.
                    Attachment data = new Attachment(file);
                    //Add the file attachment to this e-mail message.
                    message.Attachments.Add(data);
                }
                if (pToMail[i].ToString() != "")
                {
                    message.To.Add(pToMail[i]);
                    //message.To.Add("kl@mail.hamastar.com.tw");
                    emailClient.Send(message);
                    Success++;
                }
                message.Dispose();
            }
            catch (Exception ex)
            {
                sb.AppendFormat("<p>寄送失敗的郵件 - {0} - {1}</p>", pToMail[i], ex.Message);
                Failure++;
                continue;
            }
        }

        sw.Stop();
        emailClient = null;
        sb.AppendFormat("<p>成功郵件數量：{0}</p><p>失敗郵件數量：{1}<p>", Success, Failure);
        sb.AppendFormat("<p>結束時間 - {0} - </p>", DateTime.Now);
        sb.AppendFormat("<p>總共時數 - {0}小時{1}分{2}秒{3}毫秒</p>", sw.Elapsed.Hours, sw.Elapsed.Minutes, sw.Elapsed.Seconds, sw.Elapsed.Milliseconds);
        return sb.ToString();
    }
    public void Alert_Manager(IAsyncResult isr)
    {
        if (string.IsNullOrEmpty(gm_ManagerMail))
            return;
        AsyncSendMail asm = (AsyncSendMail)isr.AsyncState;
        string result = asm.EndInvoke(isr);

        SmtpClient emailClient = new SmtpClient(netSmtpMailSection.Smtp.Network.Host);
        emailClient.Credentials = new NetworkCredential(netSmtpMailSection.Smtp.Network.UserName, netSmtpMailSection.Smtp.Network.Password);
        //MailMessage ResultMail = new MailMessage(WebConfig.EmailAccountExternal, gm_ManagerMail, "排程通知信寄送完成通知.", result);
        MailMessage ResultMail = new MailMessage()
        {
            BodyEncoding = Encoding.UTF8,
            Body = result,
            From = new MailAddress(WebConfig.EmailAccountExternal, WebConfig.MailFromTitleInternal),
            SubjectEncoding = Encoding.UTF8,
            Subject = "排程通知信寄送完成通知"
        };
        ResultMail.To.Add(gm_ManagerMail);
        //ResultMail.Bcc.Add("kl@mail.hamastar.com.tw");

        ResultMail.IsBodyHtml = true;
        emailClient.Send(ResultMail);
        emailClient = null;
        ResultMail.Dispose();
    }
    #endregion


    /// <summary>
    /// 依日期取得星期幾
    /// </summary>
    public static string GetWeek(DateTime EventDate)
    {
        string strWeek = "";
        switch (EventDate.DayOfWeek)
        {
            case DayOfWeek.Sunday:
                strWeek = "日";
                break;
            case DayOfWeek.Monday:
                strWeek = "一";
                break;
            case DayOfWeek.Tuesday:
                strWeek = "二";
                break;
            case DayOfWeek.Wednesday:
                strWeek = "三";
                break;
            case DayOfWeek.Thursday:
                strWeek = "四";
                break;
            case DayOfWeek.Friday:
                strWeek = "五";
                break;
            case DayOfWeek.Saturday:
                strWeek = "六";
                break;
        }
        return strWeek;
    }
    #region 產生縮圖
    public static void m_ProduceSmallPic(string pPath, string FileName, int ImgSize)
    {
        #region
        System.Drawing.Image image = System.Drawing.Image.FromFile(pPath + "\\" + FileName);
        //必須使用絕對路徑
        ImageFormat thisFormat = image.RawFormat;
        //取得影像的格式
        int fixWidth = 0;
        int fixHeight = 0;
        //第一種縮圖用
        int maxPx = 150;
        if (ImgSize != 0)
            maxPx = ImgSize;
        //宣告一個最大值，demo是把該值寫到web.config裡
        if (image.Width > maxPx || image.Height > maxPx)
        //如果圖片的寬大於最大值或高大於最大值就往下執行
        {
            if (image.Width >= image.Height)
            //圖片的寬大於圖片的高
            {
                fixWidth = maxPx;
                //設定修改後的圖寬
                fixHeight = Convert.ToInt32((Convert.ToDouble(fixWidth) / Convert.ToDouble(image.Width)) * Convert.ToDouble(image.Height));
                //設定修改後的圖高
            }
            else
            {
                fixHeight = maxPx;
                //設定修改後的圖高
                fixWidth = Convert.ToInt32((Convert.ToDouble(fixHeight) / Convert.ToDouble(image.Height)) * Convert.ToDouble(image.Width));
                //設定修改後的圖寬
            }

        }
        else
        //圖片沒有超過設定值，不執行縮圖
        {
            fixHeight = image.Height;
            fixWidth = image.Width;
        }
        Bitmap imageOutput = new Bitmap(image, fixWidth, fixHeight);

        //輸出一個新圖(就是修改過的圖)
        string fixSaveName = "s_" + FileName;
        //副檔名不應該這樣給，但因為此範例沒有讀取檔案的部份所以demo就直接給啦

        System.IO.FileInfo FI = new System.IO.FileInfo(pPath + "\\" + fixSaveName); //原始檔名
        if (!FI.Exists)
        {
            imageOutput.Save(string.Concat(pPath + "\\", fixSaveName), thisFormat);


        }
        //將修改過的圖存於設定的位子
        imageOutput.Dispose();
        //釋放記憶體
        image.Dispose();
        #endregion
    }
    #endregion


    #region 檢查上傳檔案
    public enum UploadType
    {
        Pic,
        Media
    }
    public static string CheckUploadFile(FileUpload fu, UploadType eUploadType)
    {
        if (!fu.HasFile) return "";
        long Size = 0;
        string AllowExtention = "";
        string sType = "";
        if (eUploadType == UploadType.Pic)
        {
            Size = WebConfig.UploadPicSize;
            AllowExtention = WebConfig.UploadPicType.Replace("*", "");
            sType = "圖檔";
        }
        if (eUploadType == UploadType.Media)
        {
            Size = WebConfig.UploadMediaSize;
            AllowExtention = WebConfig.UploadMediaType.Replace("*", "");
            sType = "影音";
        }
        if (fu.PostedFile.ContentLength > 1024 * 1024 * Size)
        {
            return sType + "超過允許上傳限制 " + Size + "MB";
        }

        if (!Tools.CheckExtention(Path.GetExtension(fu.PostedFile.FileName).ToLower(), AllowExtention.ToLower().Split(';')))
        {
            return "只允許上傳" + sType + "  " + AllowExtention;
        }
        return "";
    }
    #endregion
    /// <summary>
    /// 寫Log
    /// </summary>
    /// <param name="Path"></param>
    /// <param name="Message"></param>
    public static void WriteLog(string Path, string Message)
    {
        System.IO.Directory.CreateDirectory(Path);
        //using (StreamWriter w = File.AppendText(Path + "\\log.txt"))
        //{
        //    w.Write("\r\nLog Entry : ");
        //    w.WriteLine("{0} {1}", DateTime.Now.ToLongTimeString(),
        //        DateTime.Now.ToLongDateString());
        //    w.WriteLine("{0}", Message);
        //    w.WriteLine("------------------------------------------");
        //    // Update the underlying file.
        //    w.Flush();
        //    // Close the writer and underlying file.
        //    w.Close();
        //}
    }

    /// <summary>
    /// 寫Log
    /// </summary>
    /// <param name="Path"></param>
    /// <param name="Message"></param>
    public static void WriteAutoLog(string Path, string Message)
    {
        string FolderPath = System.Web.HttpContext.Current.Server.MapPath(WebConfig.ContentPath + "/logs/") + Path;
        System.IO.Directory.CreateDirectory(FolderPath);
        using (StreamWriter w = File.AppendText(FolderPath + "/log.txt"))
        {
            w.Write("\r\nLog Entry : ");
            w.WriteLine("{0} {1}", DateTime.Now.ToLongTimeString(),
                DateTime.Now.ToLongDateString());
            w.WriteLine("{0}", Message);
            w.WriteLine("------------------------------------------");
            // Update the underlying file.
            w.Flush();
            // Close the writer and underlying file.
            w.Close();
        }
    }
    /// <summary>
    /// 執行windows Cmd
    /// </summary>
    /// <param name="command"></param>
    /// <returns></returns>
    public static string ExecuteCommandSync(string command)
    {
        try
        {
            // create the ProcessStartInfo using "cmd" as the program to be run,
            // and "/c " as the parameters.
            // Incidentally, /c tells cmd that we want it to execute the command that follows,
            // and then exit.
            System.Diagnostics.ProcessStartInfo procStartInfo =
                new System.Diagnostics.ProcessStartInfo("cmd", "/c " + command);

            // The following commands are needed to redirect the standard output.
            // This means that it will be redirected to the Process.StandardOutput StreamReader.
            procStartInfo.RedirectStandardOutput = true;
            procStartInfo.UseShellExecute = false;
            // Do not create the black window.
            procStartInfo.CreateNoWindow = true;
            // Now we create a process, assign its ProcessStartInfo and start it
            System.Diagnostics.Process proc = new System.Diagnostics.Process();
            proc.StartInfo = procStartInfo;
            proc.Start();
            // Get the output into a string
            string result = proc.StandardOutput.ReadToEnd();
            // Display the command output.
            return result;
        }
        catch (Exception objException)
        {
            return objException.Message;
        }
    }


    #region 下載檔案
    /// <summary>
    /// 下載檔案 如果失敗回傳 Null
    /// </summary>
    /// <param name="Url"></param>
    /// <returns></returns>
    public static MemoryStream DownLoadFile(string Url)
    {
        try
        {
            //System.Net.WebClient WC = new System.Net.WebClient();
            //System.IO.MemoryStream Ms = new System.IO.MemoryStream(WC.DownloadData(Url));
            //return Ms;

            HttpWebRequest Request = (HttpWebRequest)WebRequest.Create(Url);
            Request.Method = "GET";
            Request.Timeout = 1200000;//(單位毫秒:20分)
            WebResponse Response = Request.GetResponse();
            Stream ResponseStream = Response.GetResponseStream();
            MemoryStream memoryStream = new MemoryStream();
            Response.GetResponseStream().CopyTo(memoryStream);
            ResponseStream.Close();
            Response.Close();
            memoryStream.Position = 0;

            return memoryStream;
        }
        catch
        {
            return null;
        }
    }
    #endregion

    #region npoi datatable 轉excel
    /// <summary>
    /// npoi datatable 轉excel
    /// </summary>
    /// <param name="來源資料"></param>
    /// <param name="欄位名稱"></param>
    /// <param name="是否要自動欄寬"></param>
    /// <returns></returns>
    public static Stream GenExcel(DataTable SourceTable, string[] columnname, bool IsAutoSize = true, string Caption = "")
    {
        HSSFWorkbook workbook = new HSSFWorkbook();
        MemoryStream ms = new MemoryStream();
        HSSFSheet sheet = (HSSFSheet)workbook.CreateSheet("報表");
        //指定直式或橫式 true=橫式 false=直式
        sheet.PrintSetup.Landscape = true;
        //9=A4
        sheet.PrintSetup.PaperSize = 9;
        // sheet.FitToPage = false;

        HSSFRow headerRow = null;
        HSSFRow captionrow = null;
        //styling
        IFont boldFont = workbook.CreateFont();
        boldFont.Boldweight = (short)FontBoldWeight.Bold;
        ICellStyle boldStyle = workbook.CreateCellStyle();
        boldStyle.SetFont(boldFont);
        //標頭樣式
        boldStyle.BorderBottom = NPOI.SS.UserModel.BorderStyle.Thin;
        boldStyle.FillForegroundColor = NPOI.HSSF.Util.HSSFColor.Grey25Percent.Index;
        boldStyle.FillPattern = FillPattern.SolidForeground;
        int rowIndex = 1;
        //主題
        if (!string.IsNullOrEmpty(Caption))
        {
            captionrow = (HSSFRow)sheet.CreateRow(0);
            headerRow = (HSSFRow)sheet.CreateRow(1);
            ICell cell = headerRow.CreateCell(0);
            int colct = SourceTable.Columns.Count - 1;

            HSSFCellStyle cs = (HSSFCellStyle)workbook.CreateCellStyle();
            //框線樣式及顏色
            cs.BorderBottom = NPOI.SS.UserModel.BorderStyle.Thin;
            cs.BorderLeft = NPOI.SS.UserModel.BorderStyle.Thin;
            cs.BorderRight = NPOI.SS.UserModel.BorderStyle.Thin;
            cs.BorderTop = NPOI.SS.UserModel.BorderStyle.Thin;
            cs.BottomBorderColor = NPOI.HSSF.Util.HSSFColor.Black.Index;
            cs.LeftBorderColor = NPOI.HSSF.Util.HSSFColor.Black.Index;
            cs.RightBorderColor = NPOI.HSSF.Util.HSSFColor.Black.Index;
            cs.TopBorderColor = NPOI.HSSF.Util.HSSFColor.Black.Index;
            cs.VerticalAlignment = NPOI.SS.UserModel.VerticalAlignment.Center;
            cs.Alignment = NPOI.SS.UserModel.HorizontalAlignment.Center;

            HSSFFont font1 = (HSSFFont)workbook.CreateFont();
            font1.FontHeightInPoints = 12;
            cs.SetFont(font1);
            sheet.CreateRow(0).CreateCell(0).SetCellValue(string.Format("{0}", Caption));
            CellRangeAddress region0 = new CellRangeAddress(0, 0, 0, colct);//跨欄
            sheet.AddMergedRegion(region0);
            sheet.GetRow(0).GetCell(0).CellStyle = cs;

            ((HSSFSheet)sheet).SetEnclosedBorderOfRegion(region0, NPOI.SS.UserModel.BorderStyle.Thin, NPOI.HSSF.Util.HSSFColor.Black.Index);
            sheet.GetRow(0).Height = 500;// ((short)700);
            rowIndex = 2;
        }
        else
            headerRow = (HSSFRow)sheet.CreateRow(0);
        //標題
        int i = 0;

        foreach (string column in columnname)
        {
            ICell cell = headerRow.CreateCell(i);
            cell.SetCellValue(column);
            cell.CellStyle = boldStyle;
            cell.CellStyle.WrapText = true;

            i = i + 1;
        }
        // handling value.


        foreach (DataRow row in SourceTable.Rows)
        {
            HSSFRow dataRow = (HSSFRow)sheet.CreateRow(rowIndex);
            foreach (DataColumn column in SourceTable.Columns)
            {
                dataRow.CreateCell(column.Ordinal).SetCellValue(row[column].ToString());
            }
            rowIndex++;
        }
        //自動欄寬
        if (IsAutoSize)
        {
            for (int x = 0; x < SourceTable.Columns.Count; x++)
                sheet.AutoSizeColumn(x);
        }
        //凍結窗格-標頭列
        sheet.CreateFreezePane(0, 1, 0, 1);

        workbook.Write(ms);
        ms.Flush();
        ms.Position = 0;
        sheet = null;
        headerRow = null;
        workbook = null;
        return ms;
    }
    #endregion

    #region npoi取excel
    /// <summary>
    /// npoi取excel
    /// </summary>
    /// <param name="ExcelFileStream">物件的stream，例如fileupload1.FileContent</param>
    /// <param name="SheetIndex">要讀第幾個sheet，從 0開始</param>
    /// <param name="HeaderRowIndex">要從第幾列開始讀</param>
    /// <returns></returns>
    public static DataTable ExcelToDataTable(Stream ExcelFileStream, int SheetIndex, int HeaderRowIndex)
    {
        HSSFWorkbook workbook = new HSSFWorkbook(ExcelFileStream);
        HSSFSheet sheet = (HSSFSheet)workbook.GetSheetAt(SheetIndex);

        DataTable table = new DataTable();

        HSSFRow headerRow = (HSSFRow)sheet.GetRow(HeaderRowIndex);
        int cellCount = headerRow.LastCellNum;

        for (int i = headerRow.FirstCellNum; i < cellCount; i++)
        {
            DataColumn column = new DataColumn();
            if (headerRow.GetCell(i) != null)
                column.ColumnName = headerRow.GetCell(i).StringCellValue;

            table.Columns.Add(column);
        }

        int rowCount = sheet.LastRowNum;

        for (int i = (sheet.FirstRowNum + 1); i <= sheet.LastRowNum; i++)
        {
            HSSFRow row = (HSSFRow)sheet.GetRow(i);
            DataRow dataRow = table.NewRow();

            for (int j = row.FirstCellNum; j < cellCount; j++)
            {
                if (row.GetCell(j) != null)
                    dataRow[j] = row.GetCell(j).ToString();
            }

            table.Rows.Add(dataRow);
        }
        ExcelFileStream.Close();
        workbook = null;
        sheet = null;
        return table;
    }
    #endregion

    #region post File to HttpUploadHandler
    public static string PostFileToHttpUploadHandler(FileUpload fu, string UploadType, int SystemModuleListSN, int ParentSN)
    {
        return PostFileToHttpUploadHandler(new System.IO.MemoryStream(fu.FileBytes), jSecurity.Path_Traversal(fu.FileName), UploadType, SystemModuleListSN, ParentSN);
    }
    /// <summary>
    /// jason Upload File to CCMS HttpUploadHandler
    /// </summary>
    /// <param name="sFileStream"></param>
    /// <param name="FileName">檔案名稱</param>
    /// <param name="UploadType">RelPic , RelFile</param>
    /// <param name="SystemModuleListSN"></param>
    /// <param name="ParentSN"></param>
    /// <returns></returns>
    public static string PostFileToHttpUploadHandler(Stream sFileStream, string FileName, string UploadType, int SystemModuleListSN, int ParentSN)
    {
        string Url = "http://" + HttpContext.Current.Request.Url.Authority + WebConfig.ContentPath + "/HttpUploadHandler.ashx?param=" + SessionCenter.SelectedSitesSN + ";" + UploadType + ";" + SystemModuleListSN + ";" + ParentSN + "&file=" + FileName;
        string boundary = "----------------------------" + DateTime.Now.Ticks.ToString("x");
        HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(Url);
        httpWebRequest.ContentType = "multipart/form-data; boundary=" + boundary;
        httpWebRequest.Method = "POST";
        httpWebRequest.KeepAlive = true;
        httpWebRequest.Credentials = System.Net.CredentialCache.DefaultCredentials;
        httpWebRequest.Timeout = 120 * 1000;
        Stream memStream = new System.IO.MemoryStream();
        byte[] boundarybytes = System.Text.Encoding.ASCII.GetBytes("\r\n--" + boundary + "\r\n");
        string formdataTemplate = "\r\n--" + boundary + "\r\nContent-Disposition:  form-data; name=\"{0}\";\r\n\r\n{1}";
        string headerTemplate = "Content-Disposition: form-data; name=\"{0}\"; filename=\"FileName\"\r\n Content-Type: application/octet-stream\r\n\r\n";
        memStream.Write(boundarybytes, 0, boundarybytes.Length);

        string header = string.Format(headerTemplate, "file0");
        //string header = string.Format(headerTemplate, "uplTheFile", files[i]);
        byte[] headerbytes = System.Text.Encoding.UTF8.GetBytes(header);
        memStream.Write(headerbytes, 0, headerbytes.Length);
        byte[] buffer = new byte[1024];
        int bytesRead = 0;
        while ((bytesRead = sFileStream.Read(buffer, 0, buffer.Length)) != 0)
        {
            memStream.Write(buffer, 0, bytesRead);
        }
        memStream.Write(boundarybytes, 0, boundarybytes.Length);
        sFileStream.Close();

        httpWebRequest.ContentLength = memStream.Length;
        Stream requestStream = httpWebRequest.GetRequestStream();
        memStream.Position = 0;
        byte[] tempBuffer = new byte[memStream.Length];
        memStream.Read(tempBuffer, 0, tempBuffer.Length);
        memStream.Close();
        requestStream.Write(tempBuffer, 0, tempBuffer.Length);
        requestStream.Close();
        string responseContent = string.Empty;
        try
        {
            WebResponse webResponse = httpWebRequest.GetResponse();
            Stream stream = webResponse.GetResponseStream();
            StreamReader reader = new StreamReader(stream);
            responseContent = reader.ReadToEnd();

        }
        catch (Exception ex)
        {
            responseContent = ex.Message;
        }
        httpWebRequest = null;
        return responseContent;
    }
    #endregion

    #region 數字轉中文
    /// <summary>
    /// 數字轉中文
    /// </summary>
    /// <param name="number"></param>
    /// <returns></returns>
    public static string GetChineseNumber(int number)
    {
        string[] chineseNumber = { "零", "一", "二", "三", "四", "五", "六", "七", "八", "九" };
        string[] unit = { "", "十", "百", "千", "萬", "十萬", "百萬", "千萬", "億", "十億", "百億", "千億", "兆", "十兆", "百兆", "千兆" };
        StringBuilder ret = new StringBuilder();
        string inputNumber = number.ToString();
        int idx = inputNumber.Length;
        bool needAppendZero = false;
        foreach (char c in inputNumber)
        {
            idx--;
            if (c > '0')
            {
                if (needAppendZero)
                {
                    ret.Append(chineseNumber[0]);
                    needAppendZero = false;
                }
                ret.Append(chineseNumber[(int)(c - '0')] + unit[idx]);
            }
            else
                needAppendZero = true;
        }
        return ret.Length == 0 ? chineseNumber[0] : ret.ToString();
    }
    #endregion

    public static void CopyStream(Stream input, Stream output)
    {
        byte[] buffer = new byte[16 * 1024];
        int read;
        while ((read = input.Read(buffer, 0, buffer.Length)) > 0)
        {
            output.Write(buffer, 0, read);
        }
        output.Position = 0;
    }


    #region 解析CSV
    /// <summary>
    /// 解析CSV
    /// </summary>
    /// <param name="FilePath"></param>
    /// <param name="lstCsv"></param>
    public static List<List<string>> GetDataFromCsv(string FilePath, FileEncoding FileEnc = FileEncoding.Big5)
    {
        List<List<string>> lstCsv = new List<List<string>>();
        // 換行字元: \r\n
        Encoding Enc = Encoding.GetEncoding("BIG5");
        #region 決定Csv開檔編碼
        switch (FileEnc)
        {
            case FileEncoding.Big5:
                Enc = Encoding.GetEncoding("BIG5");
                break;
            case FileEncoding.Utf8:
                Enc = Encoding.UTF8;
                break;
        }
        #endregion

        using (StreamReader sr = new StreamReader(FilePath, Enc))
        {
            int x = 0, y = 0;
            char ch;
            bool LastWordInLine = false;
            lstCsv.Add(new List<string>());
            while (sr.Peek() >= 0)
            {
                ch = (char)sr.Read();
                if (ch == '\r' && (char)sr.Peek() == '\n')
                {
                    // '\r\n' 換行
                    sr.Read();
                    lstCsv[x].Add("");
                    ++x;
                    y = 0;
                    lstCsv.Add(new List<string>());
                }
                else if (ch == ',')
                {
                    // ',' 換格
                    lstCsv[x].Add("");
                    ++y;
                }
                else
                {
                    // 同一格的東西
                    string token = ch.ToString();
                    if (ch == '\"')
                    {
                        // 具特殊字元的格子
                        int Cnt = 1;
                        while (sr.Peek() >= 0)
                        {
                            ch = (char)sr.Read();
                            if (ch == ',' && Cnt % 2 == 0)
                                break;
                            if (ch == '\r' && (char)sr.Peek() == '\n')
                            {
                                sr.Read();
                                LastWordInLine = true;
                                break;
                            }
                            token += ch;
                            if (ch == '\"')
                                ++Cnt;
                        }
                    }
                    else
                    {
                        // 不具特殊字元的格子, 一路讀到 ,
                        while (sr.Peek() >= 0)
                        {
                            ch = (char)sr.Read();
                            if (ch == ',')
                                break;
                            else if (ch == '\r' && (char)sr.Peek() == '\n')
                            {
                                sr.Read();
                                LastWordInLine = true;
                                break;
                            }
                            token += ch;
                        }
                    }
                    //token = token.Replace("\n", "<br />");
                    if (token.StartsWith("\""))
                        lstCsv[x].Add(token.Substring(1, token.Length - 2).Replace("\"\"", "\""));
                    else
                        lstCsv[x].Add(token);
                    ++y;
                    if (LastWordInLine)
                    {
                        LastWordInLine = false;
                        ++x;
                        y = 0;
                        lstCsv.Add(new List<string>());
                    }
                }
            }
        }

        return lstCsv;
    }

    public enum FileEncoding
    {
        Big5 = 1,
        Utf8 = 2
    }
    #endregion





}
