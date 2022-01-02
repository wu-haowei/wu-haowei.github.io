<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage/DealData.master" AutoEventWireup="true" CodeFile="ForgotPasswd.aspx.cs" Inherits="ForgotPasswd" Async="true" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="content" runat="Server">
    <table border="0" cellpadding="0" cellspacing="0" width="100%" class="Detail-table">
        <tr>
            <td width="20%" class="td-title"><span class="red01">*</span>來源IP</td>
            <td>
                <asp:Label runat="server" ID="lblIP"></asp:Label>
            </td>
        </tr>
        <tr runat="server" id="trName">
            <td width="20%" class="td-title"><span class="red01">*</span>姓名</td>
            <td>
                <asp:TextBox ID="txtName" runat="server" Width="380px" data-errmsg="請輸入中文" required></asp:TextBox>
            </td>
        </tr>
        <tr>
            <td width="20%" class="td-title"><span class="red01">*</span>帳號</td>
            <td>
                <asp:TextBox ID="txtAccount" runat="server" Width="380px" data-errmsg="請輸入帳號" required></asp:TextBox>
            </td>
        </tr>
        <tr>
            <td class="td-title"><span class="red01">*</span>電子信箱</td>
            <td>
                <asp:TextBox ID="txtEmail" runat="server" Width="380px" data-errmsg="請輸入電子信箱" required></asp:TextBox>
            </td>
        </tr>
    </table>
    <asp:Button ID="btnOK" runat="server" Text="確定" OnClick="btnOK_Click" EnableViewState="False" CssClass="edit-btn" />
    <input id="btnCancel" type="button" value="取消" onclick="window.close();" Class="fix"  />
</asp:Content>

