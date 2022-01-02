using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Hamastar.BusinessObject;

public partial class MasterPage_MP : BasePageMaster
{
    public string ContentPath = WebConfig.ContentPath;
    vw_AccUser Account = null;

    protected void Page_Init(object sender, EventArgs e)
    {
        Account = SessionCenter.AccUser;
        if (Account == null)
        {
            Response.Redirect("/Login.aspx?x=" + (new Random(Guid.NewGuid().GetHashCode()).Next()));
            return;
        }
        else
            litLoginName.Text = Account.Name+" 你好！";
        //取得導覽資訊
        int nodeid = 0;
        if (!string.IsNullOrEmpty(jSecurity.GetQueryString("n")))
            int.TryParse(jSecurity.GetQueryString("n"), out nodeid);
        if (System.IO.Path.GetFileName(Request.PhysicalPath).ToLower() != "default.aspx" && System.IO.Path.GetFileName(Request.PhysicalPath).ToLower() != "news.aspx" && System.IO.Path.GetFileName(Request.PhysicalPath).ToLower() != "newscontent.aspx")
        {
            //取得目前節點的parent name
            string parent_name = Comm_WebArchive.GetPartentName(nodeid);
            litNav.Text = "目前位置：" + (string.IsNullOrEmpty(parent_name)==true?"":( parent_name + " > ")) + SessionCenter.UserWebMenu.Where(x => x.NodeID == nodeid).Select(x => x.Name).FirstOrDefault();
            litNav_Title.Text = SessionCenter.UserWebMenu.Where(x => x.NodeID == nodeid).Select(x => x.Name).FirstOrDefault();
        }
        else
        {
            litNav.Text = "目前位置：案件維護";

        }
        if (IsPostBack) return;
        if (Request.Url.Host.Contains("eservicet-mgr"))
        {
            // Literal1.Text = "<span class='test' style='padding-bottom:50px;color:red;font-size:16px'>(測試平台)</span>";//正式上線時要註解掉
            Page.Title = WebConfig.MailFromTitleInternal + "(測試平台)";
        }
        else
        {
            Page.Title = WebConfig.MailFromTitleInternal;
            Literal1.Text = "";
        }

        GenMenu();
    }

    private void GenMenu()
    {
        List<Comm_WebArchive> uwa = SessionCenter.UserWebMenu;
        //先群組化menu名稱
        var lv1 = (from x in uwa where x.ParentNodeID == 0 && x.IsEnable == true select x).OrderBy(x => x.Sort).ToList();
        foreach (var item in lv1)
        {

            List<Comm_WebArchive> menucotent = uwa.Where(x => x.ParentNodeID == item.NodeID && x.IsEnable == true).OrderBy(x => x.Sort).ToList();
            switch (item.NodeID)
            {
                case 3:
                    litMenu.Text += string.Format(@"<li><a href='/System/Report/Query.aspx?n=3'>{0}</a><ul class='sub_menu'>", item.Name);
                    break;
                default:
                    litMenu.Text += string.Format(@"<li><a href='#'>{0}</a><ul class='sub_menu'>", item.Name);
                    break;
            }
            //子選單
            foreach (Comm_WebArchive i in menucotent)
            {
                string url = i.Path.Contains("?") ? i.Path + "&n=" + i.NodeID.ToString() : i.Path + "?n=" + i.NodeID.ToString();
                litMenu.Text += string.Format("<li><a href='{0}'>{1}</a></li>", url, i.Name);
            }

            litMenu.Text += "</ul></li>";

        }
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        if (IsPostBack) return;

    }

    protected void lkbtnLogout_Click(object sender, EventArgs e)
    {
        Session.Clear();
        Response.Redirect(WebConfig.ContentPath + "/Login.aspx");
    }



}
