using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Collections;
using System.Data;
using Hamastar.BusinessObject;
using System.Web.UI.WebControls;
/// <summary>
/// SessionCenter 的摘要描述
/// </summary>
public class SessionCenter
{
    public SessionCenter()
    {

    }

    /// <summary>
    /// 登出時間
    /// </summary>
    public static Int32 LoginOutTime
    {
        get
        {
            if (HttpContext.Current.Session["LoginOutTime"] == null)
            {
                HttpContext.Current.Session["LoginOutTime"] = 30 * 60;
                return Convert.ToInt32(HttpContext.Current.Session["LoginOutTime"]);
            }
            else
            {
                return Convert.ToInt32(HttpContext.Current.Session["LoginOutTime"]);
            }
        }

    }
    public static List<Comm_WebArchive> UserWebMenu
    {
        get
        {
            if (HttpContext.Current.Session["UserWebMenu"] == null)
            {
                return null;
            }
            else
            {
                return (List<Comm_WebArchive>)HttpContext.Current.Session["UserWebMenu"];
            }
        }
        set
        {
            HttpContext.Current.Session["UserWebMenu"] = value;
        }
    }

    /// <summary>
    /// 左邊樹狀節點  GetWebArchive.ashx 使用
    /// </summary>
    public static List<TreeNode> DataManagerTree
    {
        get
        {
            if (HttpContext.Current.Session["DataManagerTree"] == null)
            {
                return null;
            }
            else
            {
                return (List<TreeNode>)HttpContext.Current.Session["DataManagerTree"];
            }
        }
        set { HttpContext.Current.Session["DataManagerTree"] = value; }
    }

    ///// <summary>
    ///// 目前選擇的SelectedSitesSN
    ///// </summary>
    public static int SelectedSitesSN
    {
        get
        {
            return GetSession_Int("SelectedSitesSN");
        }
        set
        {
            HttpContext.Current.Session["SelectedSitesSN"] = value;
        }
    }
    /// <summary>
    /// 目前登入者帳號
    /// </summary>
    public static vw_AccUser AccUser
    {
        get
        {
            vw_AccUser _AccUser = null;
            if (HttpContext.Current.Session["AccUser"] != null)
            {
                _AccUser = (vw_AccUser)HttpContext.Current.Session["AccUser"];
            }
            return _AccUser;
        }
        set
        {
            HttpContext.Current.Session["AccUser"] = value;
        }
    }
    ///// <summary>
    ///// 實際登入者帳號
    ///// </summary>
    //public static vw_AccUser LoginAccUser
    //{
    //    get
    //    {
    //        vw_AccUser _AccUser = null;
    //        if (HttpContext.Current.Session["LoginAccUser"] != null)
    //        {
    //            _AccUser = (vw_AccUser)HttpContext.Current.Session["LoginAccUser"];
    //        }
    //        return _AccUser;
    //    }
    //    set
    //    {
    //        HttpContext.Current.Session["LoginAccUser"] = value;
    //    }
    //}

    #region Master 站台權限使用  
    /// <summary>
    /// 目前節點所用到的參數
    /// </summary>
    public static Dictionary<string, object> CurrentConditions
    {
        get
        {
            if (HttpContext.Current.Session["CurrentConditions"] == null)
            {
                return null;
            }
            else
            {
                return (Dictionary<string, object>)HttpContext.Current.Session["CurrentConditions"];
            }
        }
        set
        {
            HttpContext.Current.Session["CurrentConditions"] = value;
        }
    }
    #endregion

    private static int GetSession_Int(string SessinoName)
    {
        int iResule = 0;
        if (HttpContext.Current.Session[SessinoName] == null)
        {
            return 0;
        }
        else
        {
            iResule = int.Parse(HttpContext.Current.Session[SessinoName].ToString());
        }
        return iResule;
    }


}