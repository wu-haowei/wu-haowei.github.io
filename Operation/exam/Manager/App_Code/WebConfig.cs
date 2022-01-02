using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// WebConfig 的摘要描述
/// </summary>
public class WebConfig
{
    public WebConfig()
    {
        //
        // TODO: 在此加入建構函式的程式碼
        //
    }

    public static string AesKey
    {
        get
        {
            return System.Configuration.ConfigurationManager.AppSettings["AesKey"];
        }

    }
    #region 無障礙檢測
    private static string _Chk_Accessibility = string.Empty;
    public static string Chk_Accessibility
    {
        get
        {
            _Chk_Accessibility = System.Configuration.ConfigurationManager.AppSettings["Chk_Accessibility"];
            return WebConfig._Chk_Accessibility;
        }
        set { WebConfig._Chk_Accessibility = value; }
    }
    #endregion
    public static string SendMsg_ID
    {
        get
        {
            return System.Configuration.ConfigurationManager.AppSettings["SendMsg_ID"];
        }
    }

    public static string SendMsg_PW
    {
        get
        {
            return System.Configuration.ConfigurationManager.AppSettings["SendMsg_PW"];
        }
    }

    public static string FrontURL
    {
        get
        {
            return System.Configuration.ConfigurationManager.AppSettings["FrontURL"];
        }
    }

    public static string WsUrl
    {
        get
        {
            return System.Configuration.ConfigurationManager.AppSettings["WsUrl"];
        }
    }
    public static string WsServicesUrl
    {
        get
        {
            return System.Configuration.ConfigurationManager.AppSettings["WsServicesUrl"];
        }
    }
    public static string WsID
    {
        get
        {
            return System.Configuration.ConfigurationManager.AppSettings["WsID"];
        }
    }

    public static string WsPWD
    {
        get
        {
            return System.Configuration.ConfigurationManager.AppSettings["WsPWD"];
        }
    }

    private static string _ContentPath = string.Empty;
    public static string ContentPath
    {
        get
        {
            string ContentPath = HttpContext.Current.Request.ApplicationPath;
            if (ContentPath == "/")
                ContentPath = "";
            return ContentPath;
        }
    }
    private static string _SystemID = string.Empty;
    public static string SystemID
    {
        get
        {
            _SystemID = System.Configuration.ConfigurationManager.AppSettings["SystemID"];
            return _SystemID;
        }
        set { WebConfig._SystemID = value; }
    }


    #region DES 加解密用 長度必須為 8 個 ASCII 字元
    private static string _DesKey = string.Empty;
    public static string DesKey
    {
        get
        {
            _DesKey = System.Configuration.ConfigurationManager.AppSettings["key"];
            return _DesKey;
        }
        set { _DesKey = value; }
    }

    private static string _DesIv = string.Empty;
    public static string DesIv
    {
        get
        {
            _DesIv = System.Configuration.ConfigurationManager.AppSettings["iv"];
            return _DesIv;
        }
        set { _DesIv = value; }
    }
    #endregion
    private static string _NewWindowStyle = string.Empty;//開新視窗時的參數
    public static string NewWindowStyle
    {
        get
        {
            _NewWindowStyle = System.Configuration.ConfigurationManager.AppSettings["NewWindowStyle"];
            return _NewWindowStyle;
        }
        set { WebConfig._NewWindowStyle = value; }
    }
    private static int _CK_Hight = 400;//網頁編輯器編輯視窗高度
    public static int CK_Hight
    {
        get
        {
            int.TryParse(System.Configuration.ConfigurationManager.AppSettings["FCK_Hight"], out _CK_Hight);
            return _CK_Hight;
        }
        set { WebConfig._CK_Hight = value; }
    }
    private static string _UploadFolder = string.Empty;
    public static string UploadFolder
    {
        get
        {
            _UploadFolder = System.Configuration.ConfigurationManager.AppSettings["UploadFolder"];
            return WebConfig._UploadFolder;
        }
        set { WebConfig._UploadFolder = value; }
    }

    private static string _BackURL = string.Empty;
    public static string BackURL
    {
        get
        {
            _BackURL = System.Configuration.ConfigurationManager.AppSettings["BackURL"];
            return WebConfig._BackURL;
        }
        set { WebConfig._BackURL = value; }
    }

    #region 限制上傳影音檔
    private static long _UploadMediaSize = 0;
    public static long UploadMediaSize
    {
        get
        {
            long.TryParse(System.Configuration.ConfigurationManager.AppSettings["UploadMediaSize"], out _UploadMediaSize);
            return WebConfig._UploadMediaSize;
        }
        set { WebConfig._UploadMediaSize = value; }
    }
    private static string _UploadMediaType = string.Empty;
    public static string UploadMediaType
    {
        get
        {
            _UploadMediaType = System.Configuration.ConfigurationManager.AppSettings["UploadMediaType"];
            return WebConfig._UploadMediaType;
        }
        set { WebConfig._UploadMediaType = value; }
    }
    private static string _UploadMediaFileType = string.Empty;
    public static string UploadMediaFileType
    {
        get
        {
            _UploadMediaFileType = System.Configuration.ConfigurationManager.AppSettings["UploadMediaFileType"];
            return WebConfig._UploadMediaFileType;
        }
        set { WebConfig._UploadMediaFileType = value; }
    }
    #endregion

    #region 限制上傳檔案
    private static long _UploadPdfSize = 0;
    public static long UploadPdfSize
    {
        get
        {
            long.TryParse(System.Configuration.ConfigurationManager.AppSettings["UploadPdfSize"], out _UploadPdfSize);
            return WebConfig._UploadPdfSize;
        }
        set { WebConfig._UploadPdfSize = value; }
    }

    private static long _UploadFileSize = 0;
    public static long UploadFileSize
    {
        get
        {
            long.TryParse(System.Configuration.ConfigurationManager.AppSettings["UploadFileSize"], out _UploadFileSize);
            return WebConfig._UploadFileSize;
        }
        set { WebConfig._UploadFileSize = value; }
    }
    private static string _UploadFileType = string.Empty;
    public static string UploadFileType
    {
        get
        {
            _UploadFileType = System.Configuration.ConfigurationManager.AppSettings["UploadFileType"];
            return WebConfig._UploadFileType;
        }
        set { WebConfig._UploadFileType = value; }
    }
    private static string _UploadPdfType = string.Empty;
    public static string UploadPdfType
    {
        get
        {
            _UploadPdfType = System.Configuration.ConfigurationManager.AppSettings["UploadPdfType"];
            return WebConfig._UploadPdfType;
        }
        set { WebConfig._UploadPdfType = value; }
    }

    private static string _UploadTxtType = string.Empty;
    public static string UploadTxtType
    {
        get
        {
            _UploadTxtType = System.Configuration.ConfigurationManager.AppSettings["UploadTxtType"];
            return WebConfig._UploadTxtType;
        }
        set { WebConfig._UploadTxtType = value; }
    }

    private static string _UploadDocFileType = string.Empty;
    public static string UploadDocFileType
    {
        get
        {
            _UploadDocFileType = System.Configuration.ConfigurationManager.AppSettings["UploadDocFileType"];
            return WebConfig._UploadDocFileType;
        }
        set { WebConfig._UploadDocFileType = value; }
    }
    #endregion

    #region 限制上傳圖片檔
    private static long _UploadPicSize = 0;
    public static long UploadPicSize
    {
        get
        {
            long.TryParse(System.Configuration.ConfigurationManager.AppSettings["UploadPicSize"], out _UploadPicSize);
            return WebConfig._UploadPicSize;
        }
        set { WebConfig._UploadPicSize = value; }
    }

    private static string _UploadPicType = string.Empty;
    public static string UploadPicType
    {
        get
        {
            _UploadPicType = System.Configuration.ConfigurationManager.AppSettings["UploadPicType"];
            return WebConfig._UploadPicType;
        }
        set { WebConfig._UploadPicType = value; }
    }
    private static string _UploadExcelFileType = string.Empty;
    public static string UploadExcelFileType
    {
        get
        {
            _UploadExcelFileType = System.Configuration.ConfigurationManager.AppSettings["UploadExcelFileType"];
            return WebConfig._UploadExcelFileType;
        }
        set { WebConfig._UploadExcelFileType = value; }
    }
    private static string _UploadPicFileType = string.Empty;
    public static string UploadPicFileType
    {
        get
        {
            _UploadPicFileType = System.Configuration.ConfigurationManager.AppSettings["UploadPicFileType"];
            return WebConfig._UploadPicFileType;
        }
        set { WebConfig._UploadPicFileType = value; }
    }
    private static string _UploadBannerType = string.Empty;
    public static string UploadBannerType
    {
        get
        {
            UploadBannerType = System.Configuration.ConfigurationManager.AppSettings["UploadBannerType"];
            return WebConfig._UploadBannerType;
        }
        set { WebConfig._UploadBannerType = value; }
    }
    #endregion

    #region 寄給同仁時,顯示主機資訊
    private static string _MailFromTitleInternal = string.Empty;
    public static string MailFromTitleInternal
    {
        get
        {
            _MailFromTitleInternal = System.Configuration.ConfigurationManager.AppSettings["MailFromTitleInternal"];
            return WebConfig._MailFromTitleInternal;
        }
        set { WebConfig._MailFromTitleInternal = value; }
    }
    private static string _EailAccountInternal = string.Empty;
    public static string EailAccountInternal
    {
        get
        {
            _EailAccountInternal = System.Configuration.ConfigurationManager.AppSettings["EailAccountInternal"];
            return WebConfig._EailAccountInternal;
        }
        set { WebConfig._EailAccountInternal = value; }
    }
    #endregion

    #region 寄給民眾時,顯示主機資訊
    private static string _MailFromTitleExternal = string.Empty;
    public static string MailFromTitleExternal
    {
        get
        {
            _MailFromTitleExternal = System.Configuration.ConfigurationManager.AppSettings["MailFromTitleExternal"];
            return WebConfig._MailFromTitleExternal;
        }
        set { WebConfig._MailFromTitleExternal = value; }
    }
    private static string _EmailAccountExternal = string.Empty;
    public static string EmailAccountExternal
    {
        get
        {
            _EmailAccountExternal = System.Configuration.ConfigurationManager.AppSettings["EmailAccountExternal"];
            return WebConfig._EmailAccountExternal;
        }
        set { WebConfig._EmailAccountExternal = value; }
    }
    #endregion

}