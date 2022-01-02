using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI.WebControls;
//using System.DirectoryServices;
using System.Text;

using Hamastar.BusinessObject;
using Hamastar.Common.Security;
using Hamastar.Common.Text;
using System.IO;
using System.Security.Principal;
using System.Web.UI;
using System.Net;

/// <summary>
/// BasePage 的摘要描述
/// </summary>
public class BasePage : System.Web.UI.Page
{
    public string ContentPath = WebConfig.ContentPath;
    public int SiteLanguageSN;

    //父層編號
    public int iKind = 0;
    //資料key
    public int iSN = 0;
    //父層編號
    public int iParentSN = 0;
    //CheckMarx漏洞修用
    public string ranValue = string.Empty;
    /// <summary>
    /// 所點選的節點代號
    /// </summary>
    public int iNodeID = 0;

    public BasePage()
    {
        //
        // TODO: 在此加入建構函式的程式碼
        //
    }

    protected Dictionary<string, object> CurrentConditions
    {
        get
        {
            Dictionary<string, object> m_Conditions = SessionCenter.CurrentConditions;
            if (m_Conditions == null)
            {
                m_Conditions = new Dictionary<string, object>();
                SessionCenter.CurrentConditions = m_Conditions;
            }
            return m_Conditions;
        }
    }



    protected override void Render(HtmlTextWriter writer)
    {
        StringBuilder sb = new StringBuilder();
        HtmlTextWriter myWriter = new HtmlTextWriter(new StringWriter(sb));
        base.Render(myWriter);
        string shtml = sb.ToString();
        writer.Write(shtml);
    }
    protected override void OnPreLoad(EventArgs e)
    {
        //CheckMarx漏洞修用給予Guid資料
        //Guid myGuid = Guid.NewGuid();
        //byte[] bArr = myGuid.ToByteArray();
        //ranValue = Math.Abs(BitConverter.ToInt32(bArr, 0));        

        ranValue = Hamastar.Common.chkcode.CreateRandomCode(8);

        if (SessionCenter.AccUser == null)
        {
            if (Request.LogonUserIdentity.IsAuthenticated)
            {
                string[] LoginInfo = Page.User.Identity.Name.Split('\\');

                //Request.LogonUserIdentity.Name.Split('\\');
                string ID = string.Empty;

                if (LoginInfo.Count() > 1)
                {
                    ID = LoginInfo[1];
                }
                else if (LoginInfo.Count() == 1)
                {
                    ID = LoginInfo[0];
                }

                vw_AccUser accuser = vw_AccUser.GetSingle(x => x.ID == ID);
                if (accuser != null)
                {
                    SessionCenter.AccUser = accuser;
                    Response.Redirect(WebConfig.ContentPath + "/System/CasePR/Query.aspx?n=13&x=" + ranValue);

                }
                else
                {
                    Response.Redirect(WebConfig.ContentPath + "/Login.aspx");

                }
            }
            else
            {
                Response.Redirect(WebConfig.ContentPath + "/Login.aspx");
            }
        }
        else
        {
            if (Request.Url.Query != "")
            {
                bool checkdata = true;
                if (Request.QueryString.Get("n") != null)
                {

                    string strNodeID = jSecurity.GetQueryString("n");
                    if (!int.TryParse(strNodeID, out iNodeID))
                        checkdata = false;
                    if (iNodeID == 0)
                        checkdata = false;
                    //紀錄目前節點,以利清除無效Session
                    if (Session["Old_NodeID"] == null) Session["Old_NodeID"] = iNodeID;
                    else if (int.Parse(Session["Old_NodeID"].ToString()) != iNodeID)
                    {
                        Session["Old_NodeID"] = iNodeID;
                        CurrentConditions.Clear();
                        //Session.Remove("goback");
                        //Session.Remove("gobacksyscode");
                    }
                }
                if (!checkdata)
                    Response.Redirect("/System/CasePR/Query.aspx?n=13&x=" + ranValue);


            }
            //站台語系:1=民國,2=西元
            SiteLanguageSN = 1;
            Session["SiteLanguageSN"] = SiteLanguageSN;

            // 設定台灣日曆 (民國年)
            if (SiteLanguageSN == 1)
            {
                System.Globalization.CultureInfo ci = new System.Globalization.CultureInfo("zh-TW");
                ci.DateTimeFormat.Calendar = new System.Globalization.TaiwanCalendar();
                System.Threading.Thread.CurrentThread.CurrentCulture = ci;
            }
        }
        //string[] NoAuthAspx = new string[] { "/Default.aspx", "/ModifyPwd.aspx" };



        //Label lblSystemTitle = Master.FindControl("lblSystemTitle") as Label;
        //if (lblSystemTitle != null && jSecurity.GetQueryString("listname") != null)
        //{
        //    lblSystemTitle.Text =Tools.Decrypt( jSecurity.GetQueryString("listname"));
        //}
    }


    /// <summary>
    /// 分頁處理
    /// </summary>
    /// <param name="objGridView">GridView Object</param>
    /// <param name="prCount">總筆數</param>
    public void GridViewPage(GridView objGridView, int prCount)
    {
        if (objGridView.BottomPagerRow != null)
        {
            //取得分頁區
            GridViewRow bottonPagerRow = objGridView.BottomPagerRow;

            // 從顯示筆數的列中，取得顯示筆數的 DropDownList 控制項。
            DropDownList pageList = (DropDownList)(bottonPagerRow.Cells[0].FindControl("ddlPager"));

            //節省資源型
            if (pageList != null)
            {
                //最大分頁
                int intMaxPage = 5;
                //起始頁
                int intStartPage = 1;

                TextBox txtPageSize = (TextBox)(bottonPagerRow.Cells[0].FindControl("txtPageSize"));
                txtPageSize.Text = objGridView.PageSize.ToString();

                //判斷最大頁數
                if (objGridView.PageCount < intMaxPage)
                {
                    intMaxPage = objGridView.PageCount;
                }
                else
                {
                    //判段起始頁
                    if ((objGridView.PageIndex + 1) > intMaxPage)
                    {
                        intStartPage = ((objGridView.PageIndex + 1) / (intMaxPage)) * intMaxPage;
                        //exp. 6,7,8,9 會得到 5 ,所以需要加1, 10 會得到 10 所以需要減4
                        if ((objGridView.PageIndex + 1) % (intMaxPage) != 0)
                        {
                            intStartPage++;
                        }
                        else
                        {
                            intStartPage = intStartPage - intMaxPage + 1;
                        }

                        intMaxPage = intStartPage + intMaxPage - 1;

                        //判斷最大頁會不會超過總頁數
                        if (objGridView.PageCount < intMaxPage)
                        {
                            intMaxPage = objGridView.PageCount;
                        }

                        //增加回前跑的Index
                        pageList.Items.Add(new ListItem("前五個分頁", (intStartPage - 1).ToString()));
                    }
                }

                //start from 1 to 5
                for (int intPage = intStartPage; intPage <= intMaxPage; intPage++)
                {
                    // 建立一個 ListItem 物件來存放筆數清單。
                    ListItem item = new ListItem(intPage.ToString());

                    // 如果 ListItem 物件的筆數與目前所選取的筆數相同，
                    // 將該 ListItem 物件的筆數標記成被選取（Selected）的狀態。
                    // 由於每當顯示筆數的列被建立時，都需重新建立 DropDownList 控制項，
                    // 此舉將會把目前已選取的筆數狀態保留在 DropDownList 控制項中。
                    if ((intPage - 1) == objGridView.PageIndex)
                    {
                        item.Selected = true;
                    }

                    // 把 ListItem 物件的筆數內容加入到 DropDownList 控制項的項目集合中。
                    pageList.Items.Add(item);
                }

                if (objGridView.PageCount > intMaxPage)
                {
                    //增加回後跑的Index
                    pageList.Items.Add(new ListItem("後五個分頁", (intMaxPage + 1).ToString()));
                }
            }


            //判斷上下頁等控制項是否啟用
            LinkButton lnkBtnFirst = (LinkButton)(bottonPagerRow.Cells[0].FindControl("lbtFirst"));
            LinkButton lnkBtnPrev = (LinkButton)(bottonPagerRow.Cells[0].FindControl("lbtPrev"));
            LinkButton lnkBtnNext = (LinkButton)(bottonPagerRow.Cells[0].FindControl("lbtNext"));
            LinkButton lnkBtnLast = (LinkButton)(bottonPagerRow.Cells[0].FindControl("lbtLast"));



            // 設定何時應該啟用或停用「第一筆」、「上一筆」、「下一筆」與「最後一筆」的超連結按鈕。
            if (objGridView.PageIndex == 0)
            {
                lnkBtnFirst.Enabled = false;
                lnkBtnPrev.Enabled = false;
            }
            else if (objGridView.PageIndex == objGridView.PageCount - 1)
            {
                lnkBtnLast.Enabled = false;
                lnkBtnNext.Enabled = false;
            }
            else if (objGridView.PageCount <= 0)
            {
                lnkBtnFirst.Enabled = false;
                lnkBtnPrev.Enabled = false;
                lnkBtnNext.Enabled = false;
                lnkBtnLast.Enabled = false;
            }

            //if (objGridView.PageIndex != 10000) //將javascript按鈕隱藏
            //{
            //    lnkBtnFirst.Enabled = false;
            //    lnkBtnPrev.Enabled = false;
            //    lnkBtnNext.Enabled = false;
            //    lnkBtnLast.Enabled = false;

            //}
            //加入總筆數,總頁數
            ////DataView DV = (DataView)objdsIndex.Select();

            //Label bottonPagerNo = new Label();
            //bottonPagerNo.Text = "總筆數" + DV.Count + "筆&nbsp;目前在第" + (gvIndex.PageIndex + 1) + "頁/共" + gvIndex.PageCount + "頁";
            //bottonPagerRow.Cells[0].Controls.Add(bottonPagerNo);

            //DV.Dispose();

            //Label bottonPagerNo = new Label();
            //bottonPagerNo.Text = "總筆數" + prCount + "筆&nbsp;目前在第" + (objGridView.PageIndex + 1) + "頁/共" + objGridView.PageCount + "頁";
            //bottonPagerRow.Cells[0].Controls.Add(bottonPagerNo);

            Label lblCount = (Label)(bottonPagerRow.Cells[0].FindControl("lblCount"));
            lblCount.Text = prCount.ToString();

            Label lblPageCount = (Label)(bottonPagerRow.Cells[0].FindControl("lblPageCount"));
            lblPageCount.Text = (objGridView.PageIndex + 1) + " / " + objGridView.PageCount;
        }
    }

    public void SelectedIndexChanged(GridView gvIndex)
    {
        // 取得顯示筆數的那一列。
        GridViewRow pagerRow = gvIndex.BottomPagerRow;
        // 從顯示筆數的列中，取得顯示筆數的 DropDownList 控制項。
        string spageList = jSecurity.XSS(((DropDownList)(pagerRow.Cells[0].FindControl("ddlPager"))).SelectedValue);
        // 將 GridView 移至使用者所選取的筆數。
        gvIndex.PageIndex = Convert.ToInt32(spageList) - 1;
    }
    public void TextChanged(GridView gvIndex)
    {
        // 取得顯示筆數的那一列。
        GridViewRow pagerRow = gvIndex.BottomPagerRow;
        // 從顯示筆數的列中，取得顯示筆數的 DropDownList 控制項。
        string sPageSize = jSecurity.XSS(((TextBox)(pagerRow.Cells[0].FindControl("txtPageSize"))).Text);
        // 將 GridView 移至使用者所選取的筆數。
        int iPageSize = 10;
        if (sPageSize == "0") sPageSize = "1";
        int.TryParse(sPageSize, out iPageSize);
        gvIndex.PageSize = iPageSize;
    }


}