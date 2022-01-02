<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Login.aspx.cs" Inherits="Login" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta charset="UTF-8" />
    <meta http-equiv="X-UA-Compatible" content="IE=edge" />
    <meta name="description" />
    <meta name="viewport" content="width=device-width, initial-scale=1" />
    <link rel="stylesheet" href="/css/Mylogin.css" />
    <link href="/css/sweetalert.css" rel="stylesheet" />
    <link href="/Scripts/jquery-impromptu.css" rel="stylesheet" />
    <link href="/css/jquery-impromptu.css" rel="stylesheet" />
    <link href="/css/introjs.min.css" rel="stylesheet" />
    <link href="/Scripts/fancybox/jquery.fancybox.css" rel="stylesheet" />
    <script src="Scripts/jquery-3.6.0.min.js"></script>
    <script src="/Scripts/jquery-impromptu.js"></script>
    <script src="Scripts/sweetalert.min.js"></script>
    <script src="/Scripts/intro.min.js"></script>
    <script src="/Scripts/blockUI/jquery.blockUI.js"></script>
    <script src="/Scripts/fancybox/jquery.fancybox.js"></script>
    <script type="text/javascript" src="/Scripts/JUtil.js"></script>

    <title>嘉義縣樂齡活動假牙個案管理系統
    </title>

</head>
<body>
    <form id="form1" runat="server">
        <asp:Panel ID="Panel1" runat="server" CssClass="root" DefaultButton="btnLogin">

            <div class="in">
                <div class="section">
                    <div class="in">
                        <div class="article">
                            <div class="in">
                                <asp:Literal ID="Literal1" runat="server"></asp:Literal>
                                <div class="fieldset">
                                    <span class="account">
                                        <asp:TextBox ID="txtAccount" runat="server" Text="" placeholder="帳號" MaxLength="30"></asp:TextBox>
                                    </span>
                                    <span class="password">
                                        <asp:TextBox ID="txtCode" type="password" runat="server" Text="" placeholder="密碼" MaxLength="20"></asp:TextBox>
                                    </span>
                                    <span class="check">
                                        <asp:ScriptManager ID="ScriptManager1" runat="server">
                                        </asp:ScriptManager>
                                        
                                                    <label for="<%=txtVali.ClientID%>"></label>
                                                    <asp:TextBox ID="txtVali" runat="server"></asp:TextBox>
                                                
                                                    <asp:UpdatePanel ID="UpdatePanel1" runat="server" class="img">
                                                        <ContentTemplate>
                                                            <asp:Image ID="Imgchkcode" runat="server" ImageUrl="/Common/CheckCode.aspx" />
                                                        </ContentTemplate>
                                                        <Triggers>
                                                            <asp:AsyncPostBackTrigger ControlID="btnRefreshValiCode" EventName="Click" />
                                                        </Triggers>
                                                    </asp:UpdatePanel>
                                                
                                                    <asp:Button ID="btnRefreshValiCode" runat="server" Text="重新產生" CssClass="reload"
                                                        CausesValidation="false" OnClick="btnRefresh_Click" onkeypress="this.click();" EnableViewState="False" />
                                                
                                    </span>
                                </div>
                                <div class="fieldset">
                                    <span class="login">
                                        <asp:Button ID="btnLogin" runat="server" Text="登入"
                                            OnClick="btnLogin_Click" EnableViewState="False" />
                                    </span>
                                    <span class="forget">
                                        <asp:Button ID="btnForgotPasswd" runat="server" Text="忘記密碼"
                                            OnClick="btnForgotPasswd_Click" EnableViewState="False" />
                                    </span>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
                <div class="footer">
                    <div class="in">
                        <%--<div class="logo"></div>--%>
                        <p>請確定您使用之瀏覽器為Chrome、FireFox、IE11.0以上版本，以獲得最佳瀏覽體驗。最佳瀏覽解析度為1280*1024。</p>
                    </div>
                </div>
            </div>
        </asp:Panel>

    </form>
</body>
</html>
