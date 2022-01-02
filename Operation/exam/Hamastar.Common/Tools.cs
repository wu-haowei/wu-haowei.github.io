using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.IO;
using System.Web.SessionState;
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
public   class Tools
{
    public static string LogFolderPath = System.Web.HttpContext.Current.Server.MapPath( "/logs/");
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

    //取得經base64編碼後的字串
    public static  string GetBase64FileToStr(string  FilePath,bool isLocalfile)
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
                data = webClient.DownloadData(FilePath );
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

    public static void SendMsg(int State, int iSN, string DataType)
    {
        //檢查是否要發email或簡訊
        if ((State == 0  && DataType == "AcceptCase")|| State == 1 || State == 3 || State == 4 || State == 6 || State == 97 || State == 98)
        {
            int sumamt = 0;
            vw_ResAmt_SumAmt sum = vw_ResAmt_SumAmt.GetSingle(x => x.ResAppSN == iSN);
            if (sum != null)
                sumamt = sum.Amt.Value;
            string mailmsg = "";
            string smsmsg = "";
            string subject = "";
            string msgtype = "", msgtypename = "";
            Comm_MsgSet cms = new Comm_MsgSet();
            if (DataType == "ResApp")
            {
                vw_ResApp cr = vw_ResApp.GetSingle(x => x.SN == iSN);
                Comm_ServiceStep csi = Comm_ServiceStep.GetSingle(x => x.ServiceInfoSN == cr.ServiceInfoSN);
                switch (State)
                {
                    case 1://收件通知
                        if (csi.RecMsg == 2)
                            cms = Comm_MsgSet.GetSingle(x => x.ParentSN == cr.ServiceInfoSN && x.MsgType == "RecMsg" && x.IsDelete == false);
                        smsmsg = Msg.GenMsg(cms.Content, cr.DocNo, cr.Title, cr.TransAccName, cr.CreateDate, cr.Name, cr.CorrectionDate, cr.PayLimitDate, sumamt, cr.Reason);
                        if (csi.RecEMail == 2)
                        {
                            cms = Comm_MsgSet.GetSingle(x => x.ParentSN == cr.ServiceInfoSN && x.MsgType == "RecEMail" && x.IsDelete == false);
                            mailmsg = Msg.GenMsg(cms.Content, cr.DocNo, cr.Title, cr.TransAccName, cr.CreateDate, cr.Name, cr.CorrectionDate, cr.PayLimitDate, sumamt, cr.Reason);
                            subject = Msg.GenMsg(cms.Title, cr.DocNo, cr.Title, cr.TransAccName, cr.CreateDate, cr.Name, cr.CorrectionDate, cr.PayLimitDate, sumamt, cr.Reason);
                        }
                        msgtype = "RecMsg";
                        msgtypename = "收件";
                        break;
                    case 3://補正通知
                        if (csi.CorMsg == 2 || (csi.CorMsg == 1 && cr.IsSendMsg.Value == true))
                        {
                            cms = Comm_MsgSet.GetSingle(x => x.ParentSN == cr.ServiceInfoSN && x.MsgType == "CorMsg" && x.IsDelete == false);
                            smsmsg = Msg.GenMsg(cms.Content, cr.DocNo, cr.Title, cr.TransAccName, cr.CreateDate, cr.Name, cr.CorrectionDate, cr.PayLimitDate, sumamt, cr.Reason);
                        }
                        if (csi.CorEMail == 2)
                        {
                            cms = Comm_MsgSet.GetSingle(x => x.ParentSN == cr.ServiceInfoSN && x.MsgType == "CorEMail" && x.IsDelete == false);
                            mailmsg = Msg.GenMsg(cms.Content, cr.DocNo, cr.Title, cr.TransAccName, cr.CreateDate, cr.Name, cr.CorrectionDate, cr.PayLimitDate, sumamt, cr.Reason);
                            subject = Msg.GenMsg(cms.Title, cr.DocNo, cr.Title, cr.TransAccName, cr.CreateDate, cr.Name, cr.CorrectionDate, cr.PayLimitDate, sumamt, cr.Reason);
                        }
                        msgtype = "CorMsg";
                        msgtypename = "補正";
                        break;
                    case 4://繳費通知
                    case 6://繳費通知
                        if (csi.PayMsg == 2 || (csi.CorMsg == 1 && cr.IsSendMsg.Value == true))
                        {
                            cms = Comm_MsgSet.GetSingle(x => x.ParentSN == cr.ServiceInfoSN && x.MsgType == "PayMsg" && x.IsDelete == false);
                            smsmsg = Msg.GenMsg(cms.Content, cr.DocNo, cr.Title, cr.TransAccName, cr.CreateDate, cr.Name, cr.CorrectionDate, cr.PayLimitDate, sumamt, cr.Reason);
                        }
                        if (csi.PayEMail == 2)
                        {
                            cms = Comm_MsgSet.GetSingle(x => x.ParentSN == cr.ServiceInfoSN && x.MsgType == "PayEMail" && x.IsDelete == false);
                            mailmsg = Msg.GenMsg(cms.Content, cr.DocNo, cr.Title, cr.TransAccName, cr.CreateDate, cr.Name, cr.CorrectionDate, cr.PayLimitDate, sumamt, cr.Reason);
                            subject = Msg.GenMsg(cms.Title, cr.DocNo, cr.Title, cr.TransAccName, cr.CreateDate, cr.Name, cr.CorrectionDate, cr.PayLimitDate, sumamt, cr.Reason);
                        }
                        msgtype = "PayMsg";
                        msgtypename = "繳費";
                        break;
                    case 97: //核准通知
                        if (csi.ApproveMsg == 2 || (csi.CorMsg == 1 && cr.IsSendMsg.Value == true))
                        {
                            cms = Comm_MsgSet.GetSingle(x => x.ParentSN == cr.ServiceInfoSN && x.MsgType == "ApproveMsg" && x.IsDelete == false);
                            smsmsg = Msg.GenMsg(cms.Content, cr.DocNo, cr.Title, cr.TransAccName, cr.CreateDate, cr.Name, cr.CorrectionDate, cr.PayLimitDate, sumamt, cr.Reason);
                        }
                        if (csi.ApproveEMail == 2)
                        {
                            cms = Comm_MsgSet.GetSingle(x => x.ParentSN == cr.ServiceInfoSN && x.MsgType == "ApproveEMail" && x.IsDelete == false);
                            mailmsg = Msg.GenMsg(cms.Content, cr.DocNo, cr.Title, cr.TransAccName, cr.CreateDate, cr.Name, cr.CorrectionDate, cr.PayLimitDate, sumamt, cr.Reason);
                            subject = Msg.GenMsg(cms.Title, cr.DocNo, cr.Title, cr.TransAccName, cr.CreateDate, cr.Name, cr.CorrectionDate, cr.PayLimitDate, sumamt, cr.Reason);
                        }
                        msgtype = "ApproveMsg";
                        msgtypename = "核准";

                        break;
                    case 98: //駁回通知
                        if (csi.RejectMsg == 2 || (csi.CorMsg == 1 && cr.IsSendMsg.Value == true))
                        {
                            cms = Comm_MsgSet.GetSingle(x => x.ParentSN == cr.ServiceInfoSN && x.MsgType == "RejectMsg" && x.IsDelete == false);
                            smsmsg = Msg.GenMsg(cms.Content, cr.DocNo, cr.Title, cr.TransAccName, cr.CreateDate, cr.Name, cr.CorrectionDate, cr.PayLimitDate, sumamt, cr.Reason);
                        }
                        if (csi.RejectEMail == 2)
                        {
                            cms = Comm_MsgSet.GetSingle(x => x.ParentSN == cr.ServiceInfoSN && x.MsgType == "RejectEMail" && x.IsDelete == false);
                            mailmsg = Msg.GenMsg(cms.Content, cr.DocNo, cr.Title, cr.TransAccName, cr.CreateDate, cr.Name, cr.CorrectionDate, cr.PayLimitDate, sumamt, cr.Reason);
                            subject = Msg.GenMsg(cms.Title, cr.DocNo, cr.Title, cr.TransAccName, cr.CreateDate, cr.Name, cr.CorrectionDate, cr.PayLimitDate, sumamt, cr.Reason);
                        }
                        msgtype = "RejectMsg";
                        msgtypename = "駁回";
                        break;
                }

                //寄送email
                if (!string.IsNullOrWhiteSpace(mailmsg) && !string.IsNullOrWhiteSpace(cr.EMail))
                {
                    bool sendflag = Tools.Send_Mail(true, cr.EMail, cr.Name, subject, mailmsg, null);
                    if (sendflag == true)
                    {
                        //寫到maillog
                        Comm_MailLog cm = new Comm_MailLog();
                        cm.Title = cr.Title;
                        cm.ServiceInfoSN = cr.ServiceInfoSN;
                        cm.MsgType = msgtypename;
                        cm.DataType = DataType;
                        cm.SendDate = DateTime.Now;
                        cm.EMail = cr.EMail;
                        cm.MailTitle = subject;
                        cm.Content = mailmsg;
                        Comm_MailLog.Insert(cm);
                    }
                }
                //寄送簡訊
                if (!string.IsNullOrWhiteSpace(smsmsg) && !string.IsNullOrWhiteSpace(cr.Phone))
                {
                    //寫到單頭
                    Comm_MsgList cml = new Comm_MsgList();
                    cml.Title = cr.Title ;
                    cml.MsgType = msgtypename ;
                    cml.Phone = cr.Phone;
                    cml.Content = smsmsg;
                    cml.SendDate = DateTime.Now;
                    int listsn = Comm_MsgList.Insert(cml).FirstOrDefault().SN;
                    //寫到單身
                    string MsgID = Tools.SendTestMessage(cr.Phone, smsmsg);
                    string packageStatus = "";
                    string receiveTime = "";
                    bool isSendFail = false;
                    Tools.GetMessageStatus(MsgID, cr.Phone, out packageStatus, out receiveTime, out isSendFail);
                    Comm_MsgLog cm = new Comm_MsgLog();
                    cm.ServiceInfoSN =cr.ServiceInfoSN ;
                    cm.MsgType = msgtype ;
                    cm.MsgListSN = listsn;
                    cm.DataType = DataType;
                    cm.Phone = cr.Phone;
                    cm.SendDate = DateTime.Now;
                    cm.Content = smsmsg;
                    cm.IsSendFail = isSendFail;
                    cm.MsgID = MsgID;
                    cm.MsgResult = receiveTime;
                    Comm_MsgLog.Insert(cm);
                }

            }
            else//單一窗口
            {
                Comm_AcceptCase cac = Comm_AcceptCase.GetSingle(x => x.SN == iSN);
                Comm_AcceptSet cr = Comm_AcceptSet.GetSingle(x => x.SN == cac.AcceptSetSN);
                Comm_ServiceStep csi = Comm_ServiceStep.GetSingle(x => x.ServiceInfoSN == cr.ServiceInfoSN);
                switch (State)
                {
                    case 0://收件通知
                        if (csi.RecMsg == 2)
                            cms = Comm_MsgSet.GetSingle(x => x.ParentSN == cr.ServiceInfoSN && x.MsgType == "RecMsg" && x.IsDelete == false);
                        smsmsg = Msg.GenMsg(cms.Content, cac.DocNo, cr.Title, cac.TransAccName, cr.CreateDate, cac.ApplyName, cr.CreateDate, null, sumamt, null);
                        if (csi.RecEMail == 2)
                        {
                            cms = Comm_MsgSet.GetSingle(x => x.ParentSN == cr.ServiceInfoSN && x.MsgType == "RecEMail" && x.IsDelete == false);
                            mailmsg = Msg.GenMsg(cms.Content, cac.DocNo, cr.Title, cac.TransAccName, cr.CreateDate, cac.ApplyName, null, null, sumamt, "");
                            subject = Msg.GenMsg(cms.Title, cac.DocNo, cr.Title, cac.TransAccName, cr.CreateDate, cac.ApplyName, null, null, sumamt, "");
                        }
                        msgtype = "RecMsg";
                        msgtypename = "收件";
                        break;
                  
                    case 97: //核准通知
                        if (csi.ApproveMsg == 2 || (csi.CorMsg == 1))
                        {
                            cms = Comm_MsgSet.GetSingle(x => x.ParentSN == cr.ServiceInfoSN && x.MsgType == "ApproveMsg" && x.IsDelete == false);
                            smsmsg = Msg.GenMsg(cms.Content, cac.DocNo, cr.Title, cac.TransAccName, cr.CreateDate, cac.ApplyName, null , null, sumamt, null);
                        }
                        if (csi.ApproveEMail == 2)
                        {
                            cms = Comm_MsgSet.GetSingle(x => x.ParentSN == cr.ServiceInfoSN && x.MsgType == "ApproveEMail" && x.IsDelete == false);
                            mailmsg = Msg.GenMsg(cms.Content, cac.DocNo, cr.Title, cac.TransAccName, cr.CreateDate, cac.ApplyName, null, null, sumamt, null);
                            subject = Msg.GenMsg(cms.Title, cac.DocNo, cr.Title, cac.TransAccName, cr.CreateDate, cac.ApplyName, null, null, sumamt, null);
                        }
                        msgtype = "ApproveMsg";
                        msgtypename = "核准";

                        break;
                    case 98: //駁回通知
                        if (csi.RejectMsg == 2 || (csi.CorMsg == 1))
                        {
                            cms = Comm_MsgSet.GetSingle(x => x.ParentSN == cr.ServiceInfoSN && x.MsgType == "RejectMsg" && x.IsDelete == false);
                            smsmsg = Msg.GenMsg(cms.Content, cac.DocNo, cr.Title, cac.TransAccName, cr.CreateDate, cac.ApplyName, null, null, sumamt, null);
                        }
                        if (csi.RejectEMail == 2)
                        {
                            cms = Comm_MsgSet.GetSingle(x => x.ParentSN == cr.ServiceInfoSN && x.MsgType == "RejectEMail" && x.IsDelete == false);
                            mailmsg = Msg.GenMsg(cms.Content, cac.DocNo, cr.Title, cac.TransAccName, cr.CreateDate, cac.ApplyName, null, null, sumamt, null);
                            subject = Msg.GenMsg(cms.Title, cac.DocNo, cr.Title, cac.TransAccName, cr.CreateDate, cac.ApplyName, null, null, sumamt, null);
                        }
                        msgtype = "RejectMsg";
                        msgtypename = "駁回";
                        break;
                }

                //寄送email
                if (!string.IsNullOrWhiteSpace(mailmsg) && !string.IsNullOrWhiteSpace(cac.EMail))
                {
                    bool sendflag = Tools.Send_Mail(true, cac.EMail, cac.ApplyName, subject, mailmsg, null);
                    if (sendflag == true)
                    {
                        //寫到maillog
                        Comm_MailLog cm = new Comm_MailLog();
                        cm.Title = cr.Title;
                        cm.ServiceInfoSN = cr.ServiceInfoSN;
                        cm.MsgType = msgtypename;
                        cm.DataType = DataType;
                        cm.SendDate = DateTime.Now;
                        cm.EMail = cac.EMail;
                        cm.MailTitle = subject;
                        cm.Content = mailmsg;
                        Comm_MailLog.Insert(cm);
                    }
                }
                //寄送簡訊
                string phone = cac.ApplyTel.Trim() + "," + cac.TrusteeTel.Trim();
                string[] allphone = null;
                if (phone.Right(1) == ",")
                    allphone = phone.Substring(0, phone.Length - 1).Split(',');
                if (!string.IsNullOrWhiteSpace(smsmsg) && !string.IsNullOrWhiteSpace(phone))
                {
                    //寫到單頭
                    Comm_MsgList cml = new Comm_MsgList();
                    cml.Title ="";
                    cml.MsgType ="";
                    cml.Phone = phone;
                    cml.Content = smsmsg;
                    cml.SendDate = DateTime.Now;
                    int listsn = Comm_MsgList.Insert(cml).FirstOrDefault().SN;
                    //寫到單身
                    foreach (string item in allphone)
                    {
                        string MsgID = Tools.SendTestMessage(item, smsmsg);
                        string packageStatus = "";
                        string receiveTime = "";
                        bool isSendFail = false;
                        Tools.GetMessageStatus(MsgID, item, out packageStatus, out receiveTime, out isSendFail);
                        Comm_MsgLog cm = new Comm_MsgLog();
                        cm.ServiceInfoSN = 0;
                        cm.MsgType = "";
                        cm.MsgListSN = listsn;
                        cm.DataType = DataType;
                        cm.Phone = item;
                        cm.SendDate = DateTime.Now;
                        cm.Content = smsmsg;
                        cm.IsSendFail = isSendFail;
                        cm.MsgID = MsgID;
                        cm.MsgResult = receiveTime;
                        Comm_MsgLog.Insert(cm);
                    }
                }
            }

        }

    }

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
    /// 下載ods檔案
    /// </summary>
    /// <param name="fi">請傳xls的FileInfo 物件進來</param>
    public static void DownloadOdsOrPdf(MemoryStream ms, string type,string outname)
    {
        string filename =  Guid.NewGuid().ToString()+".xls";
        string filepath=string.Format("{0}\\{1}", System.Web.Hosting.HostingEnvironment.MapPath("/temp"), filename);
        FileStream file = new FileStream(filepath, FileMode.Create, FileAccess.Write);
        ms.WriteTo(file);
        file.Close();
        ms.Close();
        FileInfo fi = new FileInfo(filepath);
        DownloadOdsOrPdf2(type, fi,outname);
    }

    public static void DownloadOdsOrPdf2(string type, FileInfo fi,string outname)
    {

        string strPath = string.Format(@"\{0}\{1}\{2}", SessionCenter.SelectedSitesSN, "Temp", fi.Name);
        string filepath = Uploadkernel.Upload.SaveFile(fi, strPath);
        string ConvertFileName = "";
        if (type == "ods")
            ConvertFileName = Uploadkernel.Upload.WaitDocumentToODF(filepath, "");
        else if (type == "pdf")
            ConvertFileName = Uploadkernel.Upload.WaitDocumentToPDF(filepath, "");
        else
        ConvertFileName = Uploadkernel.Upload.WaitExcelToDoc(filepath, "");

        WebStorage.StorageFileInfo oStorageFileInfo = Uploadkernel.Upload.GetStorageFileInfo(ConvertFileName);
        string url = string.Format("{0}{1}", WebConfig.WsUrl, ConvertFileName);

        //將ws上2天前的暫存檔刪除
        bool flag = Uploadkernel.Upload.DeleteDeadlineFiles(Path.GetDirectoryName(oStorageFileInfo.FullName).Replace("\\", "/"), DateTime.Today.AddDays(-2));
        //將temp資料夾2天前的資料刪除
        string[] files = Directory.GetFiles(System.Web.Hosting.HostingEnvironment.MapPath("/temp"));
        DateTime specificTime = DateTime.Now.AddDays(-2);
        List<string> deleteFiles = new List<string>();
        // 抓出指定目錄下的指定的檔案：修改日期超過 2天以前
        foreach (var file in files)
        {
            // 如果檔案不存在，就跳過
            if (!File.Exists(file)) continue;
            flag = specificTime > File.GetLastWriteTime(file);
            if (flag)
            {
                deleteFiles.Add(file);
            }
        }
        //刪除上述的所有檔案
        deleteFiles.ForEach(f =>
        {
            File.Delete(f);
        });

        //Create a stream for the file
        Stream stream = null;

        //This controls how many bytes to read at a time and send to the client
        int bytesToRead = 10000;

        // Buffer to read bytes in chunk size specified above
        byte[] buffer = new Byte[bytesToRead];

        // The number of bytes read
        try
        {
            //Create a WebRequest to get the file
            HttpWebRequest fileReq = (HttpWebRequest)HttpWebRequest.Create(url);

            //Create a response for this request
            HttpWebResponse fileResp = (HttpWebResponse)fileReq.GetResponse();

            if (fileReq.ContentLength > 0)
                fileResp.ContentLength = fileReq.ContentLength;

            //Get the Stream returned from the response
            stream = fileResp.GetResponseStream();

            // prepare the response to the client. resp is the client Response
            var resp = HttpContext.Current.Response;

            //Indicate the type of data being sent
            resp.ContentType = "application/octet-stream";

            //Name the file 
            resp.AddHeader("Content-Disposition", "attachment; filename=\"" + outname + "\"");
            resp.AddHeader("Content-Length", fileResp.ContentLength.ToString());

            int length;
            do
            {
                // Verify that the client is connected.
                if (resp.IsClientConnected)
                {
                    // Read data into the buffer.
                    length = stream.Read(buffer, 0, bytesToRead);

                    // and write it out to the response's output stream
                    resp.OutputStream.Write(buffer, 0, length);

                    // Flush the data
                    resp.Flush();

                    //Clear the buffer
                    buffer = new Byte[bytesToRead];
                }
                else
                {
                    // cancel the download if client has disconnected
                    length = -1;
                }
            } while (length > 0); //Repeat until no data is read
        }
        finally
        {
            if (stream != null)
            {
                //Close the input stream
                stream.Close();
            }
        }

    }
    //public static string MimeType(string Filename)
    //{
    //    string mime = "application/octetstream";
    //    string ext = System.IO.Path.GetExtension(Filename).ToLower();
    //    Microsoft.Win32.RegistryKey rk = Microsoft.Win32.Registry.ClassesRoot.OpenSubKey(ext);
    //    if (rk != null && rk.GetValue("Content Type") != null)
    //        mime = rk.GetValue("Content Type").ToString();
    //    return mime;
    //}

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

    /// <summary>
    /// 資料加密
    /// </summary>
    /// <param name="Text"></param>
    public static string Encrypt(string Text)
    {
        return Utility.Encrypt(Text);
    }
    public static string Encrypt(int Text)
    {
        return Encrypt(Text.ToString());
    }


    /// <summary>
    /// 資料解密
    /// </summary>
    /// <param name="Text"></param>
    /// <returns></returns>
    public static string Decrypt(string Text)
    {
        try
        {
            return Utility.Decrypt(Text);
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



    /// <summary>
    /// 解壓縮檔案，傳入參數： 來源壓縮檔, 解壓縮後的目的路徑
    /// </summary>
    /// <param name="zipFileName"></param>
    /// <param name="targetPath"></param>
    public static void UnZip(string zipFileName, string targetPath)
    {
        if (!File.Exists(zipFileName)) return;
        DirectoryInfo di = new DirectoryInfo(targetPath);
        if (!di.Exists) di.Create();

        using (ZipFile zip = ZipFile.Read(zipFileName, Encoding.Default))
        {
            foreach (ZipEntry e in zip)
            {                
                e.Extract(targetPath, ExtractExistingFileAction.OverwriteSilently);  // overwrite == true
            }
        }
    }
    //public static bool UnZip(System.Data.Linq.Binary BinaryZip, string targetPath)
    //{
    //    try
    //    {
    //        byte[] Fils = BinaryZip.ToArray();
    //        Stream m = new MemoryStream(Fils);
    //        using (ZipFile zip = ZipFile.Read(m, Encoding.Default))
    //        {
    //            foreach (ZipEntry e in zip)
    //            {
    //                if (e.FileName.ToLower().IndexOf("thumbs.db") > -1) continue;
    //                e.Extract(targetPath, ExtractExistingFileAction.OverwriteSilently);  // overwrite == true
    //            }
    //        }
    //    }
    //    catch (Exception e)
    //    {
    //        return false;
    //    }
    //    return true;
    //}
    /// <summary>
    /// 壓縮整個目錄下的檔案，傳入參數： 目的壓縮檔, 來源路徑
    /// </summary>
    public static void ToZip(string zipFileName, string targetPath)
    {
        //判斷要產生的ZIP檔案是否存在
        if (File.Exists(zipFileName))
        {
            //原本的檔案存在，把他ReName
            //File.Copy(zipFileName, zipFileName + "-" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".bak");
            File.Delete(zipFileName);
        }

        ZipFile zip = new ZipFile();
        DirectoryInfo f = new DirectoryInfo(targetPath);
        FileInfo[] a = f.GetFiles();
        for (int i = 0; i < a.Length; i++)
        {
            //排除Zip檔
            string ext = Path.GetExtension(a[i].Name);
            if (ext.ToLower() != ".zip")
                zip.AddFile(targetPath + a[i].Name, "");
        }
        if (a.Length > 0) zip.Save(zipFileName);
    }

    ///// <summary>
    ///// 讀取Zip From Binary
    ///// </summary>
    ///// <param name="BinaryZip"></param>
    ///// <returns></returns>
    //public static ZipFile GetZip(System.Data.Linq.Binary BinaryZip)
    //{
    //    try
    //    {
    //        byte[] Fils = BinaryZip.ToArray();
    //        Stream m = new MemoryStream(Fils);
    //        return ZipFile.Read(m, Encoding.Default);
    //    }
    //    catch (Exception e)
    //    {
    //        return null;
    //    }
    //}

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
            try {    
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
        return Sanitizer.GetSafeHtmlFragment(Path).Replace("&amp;","&");
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

  

    
    #region 利用系統Mail發通知信
    public static bool Send_Mail( bool pIsHTML, string pToMail, string pToName, string pSubject, string pBody, string[] AttachmentPathList)
    {
        Hamastar.Common.Net.EasyEmail mail = new Hamastar.Common.Net.EasyEmail();

        //取得Web.config中Mail設定區段
        Configuration configurationFile = WebConfigurationManager.OpenWebConfiguration("~/web.config");
        MailSettingsSectionGroup mailSettings = configurationFile.GetSectionGroup("system.net/mailSettings") as MailSettingsSectionGroup;
 
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

    public static bool Send_Mail(Page objPage, bool pIsHTML, Hashtable pToMail, Hashtable pToCC, string pSubject, string pBody, Hashtable AttachmentPathList)
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

        mail.To = pToMail;
        if (pToCC.Count > 0)
        {
            mail.CC = pToCC;
        }
  

        if (AttachmentPathList != null)
        {            
            mail.StreamAttachments = AttachmentPathList;
        }
        try
        {
            mail.SendMail();
            return true;
        }
        catch(Exception ex)
        {
            Tools.WriteAutoLog("EasyEmail", ex.Message);
            return false;
        }
    }
    /// <summary>
    /// 寄信 Mail可用,分隔
    /// </summary>
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
                result = m_SendMail(pFromName, pFromEmail, host, account, password, toMail, pToName, pSubject, pBody, pAttach, objPage, pCC, pBCC, pIsHTML);
        }

        return result;
    }
    public bool m_SendMail(string MailFromTitle, string pFromMail, string MailSmtp, string MailName, string MailPassword, string pToMail, string pToName, string pSubject, string pBody, string pAttach, Page objPage, string pCC, string pBCC, bool pIsHTML)
    {
        Encoding strCode = Encoding.GetEncoding("UTF-8");
        //mail
        if (string.IsNullOrEmpty(MailFromTitle)) MailFromTitle = WebConfig.MailFromTitleExternal;
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
            catch(Exception ex)
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
    public enum UploadType{
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
            AllowExtention = WebConfig.UploadPicType.Replace("*","");
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
            return sType + "超過允許上傳限制 " +Size + "MB";            
        }

        if (!Tools.CheckExtention(Path.GetExtension(fu.PostedFile.FileName).ToLower(), AllowExtention.ToLower().Split(';')))
        {
           return "只允許上傳"+ sType +"  "+ AllowExtention;            
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
        using (StreamWriter w = File.AppendText(Path + "\\log.txt"))
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

    internal static string String2Base64(string p)
    {
        byte[] bytes = System.Text.Encoding.GetEncoding("utf-8").GetBytes(p);
        return HttpContext.Current.Server.UrlEncode(Convert.ToBase64String(bytes));
    }

    //寄送mail
    public static bool sendMail(string SiteName ,int LanguageSN, int titletype, string toMail, string subject, string body)
    {
        try
        {
            Configuration config = WebConfigurationManager.OpenWebConfiguration("/Manager");
            //取得Web.config中Mail設定區段
            MailSettingsSectionGroup mailSettings = (MailSettingsSectionGroup)config.GetSectionGroup("system.net/mailSettings");
            string host = "";
            string password = "";
            string account = "";
            string FromEmail = "";
            if (mailSettings != null)
            {
                host = mailSettings.Smtp.Network.Host;
                password = mailSettings.Smtp.Network.Password;
                account = mailSettings.Smtp.Network.UserName;
                FromEmail = WebConfig.EmailAccountExternal;
            }
            SmtpClient emailClient = new SmtpClient(host);
            emailClient.Credentials = new NetworkCredential(account, password);
            MailMessage msg = new MailMessage();
            msg.Body = body;
            msg.BodyEncoding = Encoding.UTF8;
            if (LanguageSN != -1)
            {
                switch (titletype)
                {
                    case 0:
                        if (LanguageSN == 1)
                            msg.From = new MailAddress(WebConfig.EmailAccountExternal, SiteName + "電子報");
                        else
                            msg.From = new MailAddress(WebConfig.EmailAccountExternal, SiteName + "E-Paper");
                        break;
                    case 1:
                        if (LanguageSN == 1)
                            msg.From = new MailAddress(WebConfig.EmailAccountExternal, SiteName + "系統通知");
                        else
                            msg.From = new MailAddress(WebConfig.EmailAccountExternal, SiteName + " Notification");
                        break;
                }
            }
            else
            {
                msg.From = new MailAddress(WebConfig.EmailAccountExternal, "");
            }
            msg.IsBodyHtml = true;
            //ref: http://blog.uni2.tw/Blog/Post/e69c89e9979ce4bfa1e4bbb6e6a899e9a18ce995b7e5baa6e5a4aae995b7e980a0e68890e4ba82e7a2bc-part-2
            //主旨長度最多 60 字，超過會變亂碼
            msg.Subject = subject.Substring(0, ((subject.Length > 60) ? 60 : subject.Length));
            msg.SubjectEncoding = Encoding.UTF8;
            if (toMail != "")
            {
                msg.To.Add(toMail);
                emailClient.Send(msg);
            }
            msg.Dispose();
            //Pages.Alert("郵件發送完成");  //寄信成功
            return true;
        }
        catch (Exception ex)
        {
            //Pages.Alert(ex.Message);
            //string logPath = System.Web.HttpContext.Current.Server.MapPath(WebConfig.ContentPath + "/App_Code");
            //WriteLog(logPath + "/EpaperError", "Address：" + toMail + "寄送錯誤；" + ex.Message);
            return false;
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
    public static Stream GenExcel(DataTable SourceTable, string[] columnname, bool IsAutoSize = true,string Caption="")
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
            captionrow=(HSSFRow)sheet.CreateRow(0);
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
            headerRow=(HSSFRow)sheet.CreateRow(0);
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
    #region 判斷密碼
    //public static string chackPassWord(string Password, string Account)
    //{

    //    //16	密碼長度
    //    //17	密碼中的非英數字符 >=
    //    //18	密碼中的英字符數 >=
    //    //19	密碼中的數字符數 >=
    //    int iPW_Length = 0;//密碼長度
    //    int.TryParse(JtoolConfig.GetFunctionValue(16), out iPW_Length);
    //    int iPW_nonNE_Config = 0;//密碼中的非英數字符
    //    int.TryParse(JtoolConfig.GetFunctionValue(17), out iPW_nonNE_Config);
    //    int iPW_English_Config = 0;//密碼中的英字符數
    //    int.TryParse(JtoolConfig.GetFunctionValue(18), out iPW_English_Config);
    //    int iPW_Number_Config = 0;//密碼中的數字符數
    //    int.TryParse(JtoolConfig.GetFunctionValue(19), out iPW_Number_Config);


    //    int iPW_nonNE = 0;//密碼中的非英數字符
    //    int iPW_English = 0;//密碼中的英字符數
    //    int iPW_Number = 0;//密碼中的數字符數
    //    //分析字串
    //    foreach (char tmp in Password)
    //    {
    //        if (Is_English(tmp.ToString()))
    //        {
    //            iPW_English++;
    //        }
    //        else if (Is_Number(tmp.ToString()))
    //        {
    //            iPW_Number++;
    //        }
    //        else
    //        {
    //            iPW_nonNE++;
    //        }
    //    }

    //    if (Password.IndexOf(Account) > -1)
    //    {
    //        return "帳號不可出現在密碼";
    //    }
    //    if (Password.Length < iPW_Length)
    //    {
    //        return "密碼長度 需大於:" + iPW_Length;
    //    }
    //    if (iPW_nonNE < iPW_nonNE_Config)
    //    {
    //        return "密碼中的非英數字符 需大於:" + iPW_nonNE_Config;
    //    }
    //    if (iPW_English < iPW_English_Config)
    //    {
    //        return "密碼中的英字符數 需大於:" + iPW_English_Config;
    //    }
    //    if (iPW_Number < iPW_Number_Config)
    //    {
    //        return "密碼中的數字符數 需大於:" + iPW_Number_Config;
    //    }



    //    return "";
    //}

    //private static bool Is_English(string str)
    //{
    //    System.Text.RegularExpressions.Regex reg1 = new System.Text.RegularExpressions.Regex(@"^[A-Za-z]+$");
    //    return reg1.IsMatch(str);
    //}
    ////判斷傳入的字符是否是數字
    //private static bool Is_Number(string str)
    //{
    //    System.Text.RegularExpressions.Regex reg1 = new System.Text.RegularExpressions.Regex(@"^[0-9]+$");
    //    return reg1.IsMatch(str);
    //}

    #endregion

    #region post File to HttpUploadHandler
    public static string PostFileToHttpUploadHandler(FileUpload fu, string UploadType, int SystemModuleListSN, int ParentSN)
    {
        return PostFileToHttpUploadHandler(new System.IO.MemoryStream(fu.FileBytes),jSecurity.Path_Traversal( fu.FileName), UploadType, SystemModuleListSN, ParentSN);
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
    public static string PostFileToHttpUploadHandler(Stream sFileStream,string FileName,string UploadType,int SystemModuleListSN,int ParentSN)
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
            return ret.Length == 0 ? chineseNumber[0] : ret.ToString ();
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

    //public static bool ZipToTargetPath(System.Data.Linq.Binary BinaryZip, string targetPath)
    //{
    //    try
    //    {
    //        byte[] Fils = BinaryZip.ToArray();
    //        Stream m = new MemoryStream(Fils);
    //        using (ZipFile zip = ZipFile.Read(m, Encoding.Default))
    //        {
    //            foreach (ZipEntry e in zip)
    //            {
    //                if (e.FileName.ToLower().IndexOf("thumbs.db") > -1) continue;
    //                e.Extract(targetPath, ExtractExistingFileAction.OverwriteSilently);  // overwrite == true
    //            }
    //        }
    //    }
    //    catch (Exception e)
    //    {
    //        return false;
    //    }
    //    return true;
    //}

    #region 呼叫簡訊服務    

    private void PokeService()
    {
        int Command = 130;
        string ServiceName = "SendMessage";
        try
        {
            System.ServiceProcess.ServiceController sc = new System.ServiceProcess.ServiceController(ServiceName);
            sc.ExecuteCommand(Command);
        }
        catch (Exception ex)
        {
            Pages.Alert(ex.Message);
        }
    }
    #endregion

    #region (NEW)呼叫公司WebService發送簡訊  https://cmw.hisales.hinet.net/MsgWebServiceAPI/Service.asmx


    [WebMethod]
    public static string SendTestMessage(string Phone, string Content)
    {

        string SMS_Username = WebConfig.SendMsg_ID;
        string SMS_Password = WebConfig.SendMsg_PW;
        AspSmsSocket smsSocket = new AspSmsSocket();
        smsSocket.InitHinetAccountInfo(SMS_Username, SMS_Password);
        string ReturnString = string.Empty;
        switch (smsSocket.ReqBind())
        {
            case 0:
                break;
            case 1:
                break;
            default:
                break;
        }

        if (smsSocket.IsBound().Equals(0))
        {
            switch (smsSocket.ReqSendTextMessage(Phone, Content, 1))
            {
                case 0:
                    ReturnString = smsSocket.RespSendMessageId();
                    break;
                case 1:
                    break;
                default:
                    break;
            }
            smsSocket.ReqUnbind();
        }

        return ReturnString;
    }


    //取得簡訊發送結果
    [WebMethod]
    public static string GetMessageStatus(string MsgID, string Phone, out string packageStatus, out string receiveTime,out bool isSendFail)
    {
        string SMS_Username = WebConfig.SendMsg_ID;
        string SMS_Password = WebConfig.SendMsg_PW;
        string messageStatus = string.Empty;
        packageStatus = string.Empty;
        receiveTime = string.Empty;
        ServicePointManager.DefaultConnectionLimit = 2000;
        AspSmsSocket smsSocket = new AspSmsSocket();
        smsSocket.InitHinetAccountInfo(SMS_Username, SMS_Password);
        string ReturnString = string.Empty;
        switch (smsSocket.ReqBind())
        {
            case 0:
                break;
            case 1:
                break;
            default:
                break;
        }
        isSendFail = true;
        if (smsSocket.IsBound().Equals(0))
        {
            int Query = smsSocket.ReqQuery(MsgID);
            int QueryCode = 0;
            
            switch (Query)
            {
                case 0:
                    QueryCode = smsSocket.RespQueryFinalState();
                    packageStatus = QueryCode.ToString();
                    switch (QueryCode)
                    {
                        case 1:
                            //成功傳送簡訊至受訊手機
                            receiveTime = (smsSocket.RespQueryDoneTime()).Substring(0,16)+" 發送成功";
                            isSendFail = false;
                            break;
                        case 2:
                            //無法傳送簡訊至受訊手機
                            receiveTime = (smsSocket.RespQueryDoneTime()).Substring(0, 16) + " 發送失敗";                                                     
                            break;
                        case 3:
                            //超過重送期限
                            receiveTime = (smsSocket.RespQueryDoneTime()).Substring(0, 16) + " 發送失敗";
                            break;
                        case 4:
                            //簡訊被拒絕
                            receiveTime = (smsSocket.RespQueryDoneTime()).Substring(0, 16) + " 拒絕簡訊";
                            break;
                        case 5:
                            //未申請簡訊相關服務
                            receiveTime = (smsSocket.RespQueryDoneTime()).Substring(0, 16) + " 拒絕簡訊";
                            break;
                        case 6:
                            //無法得知其簡訊最終狀態
                            receiveTime = "傳送中";
                            break;
                        case 7:
                            //簡訊傳送中
                            receiveTime = "傳送中";
                            break;
                        case 8:
                            //取消簡訊
                            receiveTime = (smsSocket.RespQueryDoneTime()).Substring(0, 16) + " 拒絕簡訊";
                            break;
                        case 9:
                            //簡訊閘道器有錯誤
                            receiveTime = (smsSocket.RespQueryDoneTime()).Substring(0, 16) + " 拒絕簡訊";
                            break;
                        case 10:
                            //拒收加值簡訊
                            receiveTime = (smsSocket.RespQueryDoneTime()).Substring(0, 16) + " 拒絕簡訊";
                            break;
                        default:
                            receiveTime = (smsSocket.RespQueryDoneTime()).Substring(0, 16) + " 發送失敗";
                            break;
                    }
                    break;
                case 1:
                    break;
                default:
                    break;
            }
            messageStatus = smsSocket.RespQueryDescChn();
            smsSocket.ReqUnbind();
            
        }
        return messageStatus;
    }



    #endregion
}
