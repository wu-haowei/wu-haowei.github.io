using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class Common_wucDateTime : System.Web.UI.UserControl
{
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

    private string _Text = string.Empty;
    public string Text
    {
        get
        {
            _Text = jSecurity.XssAndSqlFilter(txtDateTime.Text);
            return _Text;
        }
        set
        {
            _Text = value;
            txtDateTime.Text = _Text;
        }
    }

    private bool _IsShowClearButton = true;
    public bool IsShowClearButton
    {
        get
        {
            return _IsShowClearButton;
        }
        set
        {
            _IsShowClearButton = value;
        }
    }

    private bool _IsShowDefaultDate = false;
    public bool IsShowDefaultDate { get { return _IsShowDefaultDate; } set { _IsShowDefaultDate=value; } }
    private bool _IsShowTime = true;
    public bool IsShowTime
    {
        get { return _IsShowTime; }
        set
        {
            _IsShowTime = value;
           
        }
    }
    protected void Page_Init(object sender, EventArgs e)
    {
        txtDateTime.Attributes["data-ccms_datepicker"] = "{'IsShowClearButton':"+ _IsShowClearButton.ToString().ToLower() + ",'IsShowTime':" + _IsShowTime.ToString().ToLower() + "}";
       
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            if (!_IsShowTime)
            {
                txtDateTime.Width = 95;
                if (_IsShowDefaultDate && string.IsNullOrEmpty(txtDateTime.Text)) txtDateTime.Text = DateTime.Now.ToString("yyyy-MM-dd");

            }
            else
            {
                txtDateTime.Width = 125;
                if (_IsShowDefaultDate && string.IsNullOrEmpty(txtDateTime.Text)) txtDateTime.Text = DateTime.Now.ToString("yyyy-MM-dd HH:mm");
            }
        }
    }
}