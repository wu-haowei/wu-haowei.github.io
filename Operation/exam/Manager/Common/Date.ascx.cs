using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Hamastar.Common;
public partial class Common_Date : System.Web.UI.UserControl
{
    //是否可編輯
    bool _IsEditable = true;
    public bool IsEditable
    {
        get { return _IsEditable; }
        set { _IsEditable = value; }
    }

    private bool _DateTimeValidatorEnableFlag;
    public bool DateTimeValidatorEnableFlag
    {
        get
        {
            _DateTimeValidatorEnableFlag = this.RequiredFieldValidator1.Enabled;
            return _DateTimeValidatorEnableFlag;
        }
        set
        {
            _DateTimeValidatorEnableFlag = value;
            this.RequiredFieldValidator1.Enabled = _DateTimeValidatorEnableFlag;
        }
    }
    private string  _DateTimeValidatorGroup="";
    public string  DateTimeValidatorGroup
    {
        get
        {
            _DateTimeValidatorGroup = this.RequiredFieldValidator1.ValidationGroup;
            return _DateTimeValidatorGroup;
        }
        set
        {
            _DateTimeValidatorGroup = value;
            this.RequiredFieldValidator1.ValidationGroup = _DateTimeValidatorGroup;
        }
    }

    private string _Text = string.Empty;
    public string Text
    {
        get
        {
            _Text = Tools.AntiXss_URL(txtDateTime.Text.Trim());
            if (IsShowTime && _Text != "")
                _Text += " " + Tools.AntiXss_URL(ddlHour.SelectedValue) + ":" + Tools.AntiXss_URL(ddlMin.SelectedValue);
            return _Text;
        }
        set { _Text = value; }
    }

    private bool _IsShowTime = false ;
    public bool IsShowTime
    {
        get { return _IsShowTime; }
        set
        {
            _IsShowTime = value;
        }
    }
    private string _BtnCleanClass = "ButtonStyle_H1";
    public string BtnCleanClass
    {
        get { return _BtnCleanClass; }
        set
        {
            _BtnCleanClass = value;
            btnClean.Attributes.Add("class", _BtnCleanClass);
        }
    }

    private bool _IsUpdatePanel = false;
    public bool IsUpdatePanel
    {
        get { return _IsUpdatePanel; }
        set { _IsUpdatePanel = value; }
    }

    private bool _IsReSetTime = true ;
    public bool IsReSetTime
    {
        get { return _IsReSetTime; }
        set { _IsReSetTime = value; }
    }

    
    private string _Script = string.Empty;
    public string Script
    {
        get { return _Script; }
        set { _Script = value; }
    }
    private string _ReqErrorMessage = "請選擇日期";
    public string ReqErrorMessage
    {
        get { return _ReqErrorMessage; }
        set
        {
            _ReqErrorMessage = value;
            RequiredFieldValidator1.ErrorMessage= "請選擇日"+_ReqErrorMessage;
        }
    }

    private int SiteLanguageSN = 1;
    protected void Page_Init(object sender, EventArgs e)
    {
        Session["SiteLanguageSN"] = 1;
        if (Session["SiteLanguageSN"] != null)
            int.TryParse(Session["SiteLanguageSN"].ToString(), out SiteLanguageSN);
        System.Text.StringBuilder sbScript = new System.Text.StringBuilder();

        if (SiteLanguageSN == 1)
        {
            sbScript.Append("<script type=\"text/javascript\">\n");
            sbScript.Append("$(document).ready(function() {\n");
            sbScript.Append(" $(\"#" + txtDateTime.ClientID + "\").dynDateTime({\n");
            sbScript.Append(" ifFormat:\"%Z-%m-%d\"\n");
            if (!string.IsNullOrEmpty(Script)) {
                {
                     sbScript.Append(" ,onSelect: function(dateText, date) {$(\"#" + txtDateTime.ClientID + "\").val(date);" + Script + "}");
                }
            }
            sbScript.Append(" }); \n");
            sbScript.Append("});\n");
            sbScript.Append("</script>\n");

            sbScript.Append("<script type=\"text/javascript\">//<![CDATA[\n");
          //  if (!IsEditable)
                sbScript.Append("document.getElementById(\"" + txtDateTime.ClientID + "\").readOnly  = true; \n");
            //if (!string.IsNullOrEmpty(Script))
            //{
            //    sbScript.Append("$(\"#" + txtDateTime.ClientID + "\").blur(function(){"+ Script + "}); \n");
            //}

            sbScript.Append("//]]></script>\n");
        }
        else
        {
            sbScript.Append("<script type=\"text/javascript\">//<![CDATA[\n");
            sbScript.Append("Calendar.setup({\n");
            sbScript.Append("  inputField   : '" + txtDateTime.ClientID + "', \n");
            sbScript.Append("  trigger      : '" + txtDateTime.ClientID + "', \n");
            sbScript.Append("  minuteStep   : 1,\n");
            sbScript.Append("  weekNumbers  : true,\n");
            //if (_IsShowTime)
            //{
            //    sbScript.Append("  showTime : true,\n");
            //    sbScript.Append("  onTimeChange : updateFields_" + txtDateTime.ClientID + ", \n");
            //}
            sbScript.Append("  onSelect     : function() {  updateFields_" + txtDateTime.ClientID + "(this); this.hide(); } \n");
            sbScript.Append("});\n");

            sbScript.Append("function updateFields_" + txtDateTime.ClientID + "(cal) { \n");
            sbScript.Append("   var date = cal.selection.get(); \n");
            sbScript.Append("   if (typeof(date) == 'undefined') { \n");
            sbScript.Append("       date = new Date(); \n");
            sbScript.Append("   } \n");
            sbScript.Append("   date = Calendar.intToDate(date); \n");
            sbScript.Append("   date = Calendar.printDate(date, '%Y-%m-%d'); \n");
            sbScript.Append("   var h = cal.getHours(), m = cal.getMinutes(); \n");
            sbScript.Append("   if (h < 10) h = '0' + h; \n");
            sbScript.Append("   if (m < 10) m = '0' + m; \n");
            sbScript.Append("   $('#" + txtDateTime.ClientID + "').val(date); \n");
            sbScript.Append("}; \n");
            if (!IsEditable)
                sbScript.Append("document.getElementById(\"" + txtDateTime.ClientID + "\").readOnly  = true; \n");
            sbScript.Append("//]]></script>\n");
        }
        if (IsUpdatePanel == true)
            ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), Guid.NewGuid().ToString(), sbScript.ToString(), false);
        else
            litScript.Text = sbScript.ToString();

        txtDateTime.Text = _Text;
        btnClean.Attributes.Add("onclick", "document.getElementById('" + txtDateTime.ClientID + "').value = ''");
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            DateTime vDate = DateTime.Now;
            if (_Text != "")
            {
                DateTime.TryParse(_Text, out vDate);
                txtDateTime.Text = vDate.ToYmdDateString();
            }
            if (!IsEditable)
            {
                if (_Text != "") litDate.Text = vDate.ToYmdDateString();
                txtDateTime.Attributes["style"] = "display:none";
                btnClean.Attributes["style"] = "display:none";
                ddlHour.Attributes["style"] = "display:none";
                ddlMin.Attributes["style"] = "display:none";
            }
            if (IsReSetTime)
            this.ShowDTime(vDate);
        }
    }

    
    private void ShowDTime(DateTime vDate)
    {
        if (!_IsShowTime)
        {
            ddlHour.Attributes["style"] = "display:none";
            ddlMin.Attributes["style"] = "display:none";
        }
        else
        {
            
            ddlHour.Attributes["style"] = "";
            ddlMin.Attributes["style"] = "";
            ddlHour.Items.Clear();
            for (int i = 0; i < 24; i++)
            {
                string str = i.ToString();
                ddlHour.Items.Add(new ListItem(str.PadLeft(2, '0'), str.PadLeft(2, '0')));
            }
            ddlMin.Items.Clear();
            for (int i = 0; i < 60; i+=5)
            {
                string str = i.ToString();
                 ddlMin.Items.Add(new ListItem(str.PadLeft(2, '0'), str.PadLeft(2,'0')));
            }
            if (txtDateTime.Text == "")
            {
                ddlHour.SelectedIndex = 0;
                ddlMin.SelectedIndex = 0;
            }
            else
            {
                ddlHour.SelectedValue = vDate.ToString("HH");
                try {
                    ddlMin.SelectedValue = vDate.ToString("mm");
                }
                catch
                {
                    ddlMin.SelectedIndex = 0;
                }
            }
        }
    }
    public  void  setDate(DateTime vDate){
        
        txtDateTime.Text = vDate.ToString("yyyy-MM-dd");
        this.ShowDTime(vDate);
    }

    public void setClear()
    {

        txtDateTime.Text ="";
    }
}