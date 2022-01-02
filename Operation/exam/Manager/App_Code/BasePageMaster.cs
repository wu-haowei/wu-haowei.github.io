using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text;

/// <summary>
/// BasePageMaster 的摘要描述
/// </summary>
public class BasePageMaster : System.Web.UI.MasterPage
{
    public BasePageMaster(){
        
    }
    protected override void OnPreRender(EventArgs e)
    {
        GenClinetJS();
    }

    private void GenClinetJS()
    {
        StringBuilder sb = new StringBuilder();

        //如果使用updatePanel 回來後解除LockUI
        sb.AppendLine("var CCMS_CanLock=1;");
        sb.AppendLine("if (typeof(Sys) != 'undefined')Sys.WebForms.PageRequestManager.getInstance().add_endRequest(CCMS_unLockUI);");
        sb.AppendLine("if (document.getElementById(\"form1\").addEventListener){document.getElementById(\"form1\").addEventListener(\"submit\", CCMS_LockUI);}else{document.getElementById(\"form1\").attachEvent(\"submit\", CCMS_LockUI);}");
     
        //LockUI
        sb.AppendLine("function CCMS_LockUI() {");
        sb.AppendLine("    if ((typeof(Page_IsValid) == 'undefined' || Page_IsValid) && CCMS_CanLock==1 ) {");
        sb.AppendLine("$.blockUI({ css: { border: 'none','background-color':''},message: '<img src=\"" + WebConfig.ContentPath + "/images/ajax-loader.gif\" />' });");
        sb.AppendLine("    CCMS_CanLock = 1 ;}");
        sb.AppendLine("}   ");
        //解除LockUI
        sb.AppendLine("function CCMS_unLockUI() {");
        sb.AppendLine("     $.unblockUI();");
        sb.AppendLine("}   ");
        //解除LockUI
        sb.AppendLine("function CCMS_DisableLockUI() {");
        sb.AppendLine("    CCMS_CanLock=0;");
        sb.AppendLine("}   ");

        //解除LockUI
        sb.AppendLine("function CCMS_DisableLockUI() {");
        sb.AppendLine("    CCMS_CanLock=0;");
        sb.AppendLine("}   ");

        sb.AppendLine("window.alert = function(Msg,Type,f) {");
        sb.AppendLine("Msg='<p style=\"font-size:16px;\">'+ Msg+'</p>';");

        sb.AppendLine("var stitle='<h4><img src=\"/images/popup_info.png\" />訊息</h4>';");
        sb.AppendLine("if(Type==0){");
        sb.AppendLine("var fCallBack  = function(e,v,m,f){window.location = window.location;};");
        sb.AppendLine("$.prompt(Msg,{title:stitle,submit: fCallBack,close:fCallBack,buttons: {'關閉':''}});");
        sb.AppendLine("}   ");
        sb.AppendLine("else if(Type==1){");
        sb.AppendLine("var fCallBack  = f;");
        sb.AppendLine("$.prompt(Msg,{title:stitle,submit: fCallBack,close:fCallBack,buttons: {'關閉':''}});");
        sb.AppendLine("}else{");
        sb.AppendLine("$.prompt(Msg,{title:stitle,buttons: {'關閉':''}});");
        sb.AppendLine("}   ");
        sb.AppendLine("}   ");
        Page.ClientScript.RegisterStartupScript(this.GetType(), "BasePageMaster", sb.ToString(), true);        
    }
}